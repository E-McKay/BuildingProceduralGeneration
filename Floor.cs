using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//covers each floor

public class Floor{

    private int type;
    private int Xsize;
    private int Ysize;
    public int floorNumber;
    private int splitChanceNum;

    public List<Cell> cells = new List<Cell>();
    private List<Edge> edges = new List<Edge>();

    WeighBestPath WbestPath = new WeighBestPath();

    public int getType()
    {
        return type;
    }

    public void setType(int t)
    {
        type = t;
    }

    public List<Edge> getEdge()
    {
        return edges;
    }

    public int getX()
    {
        return Xsize;
    }

    public int getY()
    {
        return Ysize;
    }

   public void setEdge(List<Edge> in_edge)
    {
        edges = in_edge;
    }

    public Floor(int X, int Y, int num, int splitChance) {
        Xsize = X;
        Ysize = Y;
        floorNumber = num;
        splitChanceNum = splitChance;
    }

    //build cells
    public void buildLayout(int chanceMod)
    {
        List<Cell> parents = new List<Cell>();
        List<Cell> leaves = new List<Cell>();
        Cell root = new Cell(Xsize, Ysize, 0, 0);
        if (root.split(chanceMod) == false)
        {
            leaves.Add(root);
        }; //initial split
        parents.AddRange(root.children);

        while (parents.Count != 0)
        {
            for (int i = 0; i < parents.Count; i++)
            {
                /////////////////////////////////////////////////
                //////////PLAY WITH THIS FOR THE ROOM SIZE FREQ
                ////////////////////////////////////////////////

                //UPDATE THIS ITS A BIT BROKE

                //having chancemod so as it gets smaller it occasionally bounces up so we get a bigger room

               //if (chanceMod < 15) { chanceMod = (chanceMod + 1) * 25; } else { chanceMod = (chanceMod / 10) + 10; }
               //chanceMod = Random.Range(-chanceMod /2 , chanceMod);
               //chanceMod = chanceMod * 5;

                if (parents[i].split(chanceMod) == false)
                {
                    //cant split any more, add to list of leaves
                    Cell leaf = parents[i];
                    leaves.Add(leaf);
                    parents.RemoveAt(i);
                }
                else
                {
                    //recurse
                    parents.AddRange(parents[i].children);
                    parents.RemoveAt(i);
                }
            }

        }
        cells = leaves;
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].id_set = i;
            cells[i].fillSides();
        }
    }

    //build graph from cells
    public void buildGraph()
    {
        List<Cell> ToScan = cells; //substitute this in later for performance reasons
        for(int i = 0; i < cells.Count; i++)
        {
            

            Cell curCell = cells[i];
            for(int j = 0; j < cells.Count; j++)
            {
                if(cells[i] != cells[j]){
                 //find next cell on the Ysize?
                CheckAdjacent(cells[i], cells[j], true);
                CheckAdjacent(cells[i], cells[j], false);
                }

            }
        }
    }

    public void setRoomTypes(int typechance)
    {
        foreach (Cell c in cells)
        {
            if(Random.Range(0,100) < typechance - 1)
            {
                c.type = 1;
            }
            else
            {
                c.type = 0;
            }
        }
    }

    public void CheckAdjacent(Cell origin, Cell target,bool sideways)
    {
        List<Side> originSides;
        List<Side> targetSides;

        if (sideways == true)
        {
            originSides = origin.Rightsides;
            targetSides = target.Leftsides;

        }
        else
        {
            originSides = origin.Topsides;
            targetSides = target.Bottomsides;
        }



        for (int i = 0; i < originSides.Count; i++)
        {


            Side current_side = originSides[i];
            for(int j = 0; j < targetSides.Count; j++)
            {
                Side target_side = targetSides[j];

                //Debug.Log("Source Side X = " + current_side.Xloc + "Source Side Y = " + current_side.Yloc + "Targ Side X = " + target_side.Xloc + "targ Side Y = " + target_side.Yloc);

                if (sideways == true)
                {
                    //Debug.Log("MatchingY" + current_side.Yloc + target_side.Yloc);
                    //Debug.Log("MatchingX" + current_side.Xloc + target_side.Xloc);

                    if ((current_side.Yloc == target_side.Yloc) && (current_side.Xloc == target_side.Xloc))
                    {
                       Edge newEdge = new Edge(origin, target, current_side.Xloc, current_side.Yloc, false);
                       origin.edges.Add(newEdge);
                       target.edges.Add(newEdge);
                       edges.Add(newEdge);
                    }
                }
                else
                {
                    if ((current_side.Yloc == target_side.Yloc) && (current_side.Xloc == target_side.Xloc))
                    {
                       Edge newEdge = (new Edge(origin, target, current_side.Xloc, current_side.Yloc, true));
                       origin.edges.Add(newEdge);
                       target.edges.Add(newEdge);
                       edges.Add(newEdge);
                    }
                    
                }

            }
        }
    }

    public void RandomWeights(int minNum, int maxNum)
    {
        foreach(Edge e in edges)
        {
            e.setWeight(e.getWeight() + Random.Range(minNum, maxNum + 1));
        }
    }

    public void RemoveWeights()
    {
        foreach(Edge e in edges)
        {
            e.setWeight(0);
        }
    }

    public void WeightPath(bool invert)
    {
        List<Edge> Nedges = WbestPath.RunAlg(this,invert);
        List<int> D_idx = new List<int>();
        for(int i = 0; i  < edges.Count; i++)
        {
            foreach(Edge j in Nedges)
            {
                if(edges[i].getX() == j.getX() && edges[i].getX() == j.getY())
                {
                    edges[i].setWeight(edges[i].getWeight() + 25);
                }
            }
        }
    }

    public void ShortestPathWeights() //as the shortest path is cells closest to the original diagonal we can base weights on how close to the diagonal centre the cell goes
    {
        foreach(Edge e in edges)
        {
            int weight = e.getX() - e.getY();
            if(weight < 0) { weight = weight * -1; }
            e.setWeight(e.getWeight() + weight);
        }
    }

    public void InvertWeights() //invert the shortest path
    {
        foreach (Edge e in edges)
        {
            e.setWeight(e.getWeight() * -1);
        }
    }

    public void SizedWeights() //weights based on cell size
    {
        foreach(Edge e in edges)
        {
            int weight = (e.getRoomA().Xsize + e.getRoomA().Ysize) + (e.getRoomB().Xsize + e.getRoomB().Ysize);
            e.setWeight(e.getWeight() + weight);
        }
    }


    //could merge these but its better to keep flexebility.

    public void SetExit(int floorNum,bool fillSpace, int height)
    {
        bool placeleft = (floorNum % 2 != 0);
        if (fillSpace == true && Ysize > 3 && floorNum != (height + 1))
        {
            Cell startcell = cells.Find(x => ((x.XLoc == 0) && (x.YLoc == 0)));
            startcell.Leftsides[0].wallNumber = 0;

            startcell = cells.Find(x => ((x.Xsize + x.XLoc == Xsize) && (x.YLoc == 0)));
            startcell.Rightsides[0].wallNumber = 0;

        }
        else if (placeleft == true)
        {
            Cell startcell = cells.Find(x => ((x.XLoc == 0) && (x.YLoc == 0)));
            startcell.Leftsides[0].wallNumber = 0;
        }
        else
        {
            Cell startcell = cells.Find(x => ((x.Xsize + x.XLoc == Xsize) && (x.YLoc == 0)));
            startcell.Rightsides[0].wallNumber = 0;
        }

    }

    public void SetEntrance(int floorNum, bool fillspace, int height )
    {
        //breaks if the  floors Ysize == 3

        bool placeleft = (floorNum % 2 != 0);

        if (fillspace == true && Ysize > 3)
        {
            Cell startcell = cells.Find(x => ((x.XLoc == 0) && (x.Ysize + x.YLoc == Ysize)));
            startcell.Leftsides[startcell.Ysize - 1].wallNumber = 0;

            startcell = cells.Find(x => ((x.Xsize + x.XLoc == Xsize) && (x.Ysize + x.YLoc == Ysize)));
            startcell.Rightsides[startcell.Ysize - 1].wallNumber = 0;
        }
        else if (placeleft != true)
        {
            Cell startcell = cells.Find(x => ((x.XLoc == 0) && (x.Ysize + x.YLoc == Ysize)));
            startcell.Leftsides[startcell.Ysize - 1].wallNumber = 0;
        }
        else
        {
            Cell startcell = cells.Find(x => ((x.Xsize + x.XLoc == Xsize) && (x.Ysize + x.YLoc == Ysize)));
            startcell.Rightsides[startcell.Ysize - 1].wallNumber = 0;
        }
    }


}
