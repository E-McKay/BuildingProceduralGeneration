using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimsSearch {

    private List<Cell> visitedCells = new List<Cell>();
    private List<Edge> visitedEdges = new List<Edge>();
    int startCellLocX = 0;
    int startCellLocY = 0;


    private float comp;

    public void RunAlg(Floor cur_floor, bool invert)
    {
        visitedCells = new List<Cell>();
        List<Cell> leaves = new List<Cell>();
        Cell Startcell = cur_floor.cells[0];
        if ((cur_floor.floorNumber % 2) == 0 && invert == false || (cur_floor.floorNumber % 2) == 1 && invert == true)
        {
            foreach (Cell c in cur_floor.cells)
            {
                if (c.YLoc > Startcell.YLoc && c.XLoc == 0)
                {
                    Startcell = c;
                }
            }
        }
        else
        {
            int counter = 0;
            foreach (Cell c in cur_floor.cells)
            {
                if(c.YLoc + c.XLoc > counter)
                {
                    counter = c.YLoc + c.XLoc;
                    Startcell = c;
                }
                
            }

        }
        startCellLocX = Startcell.XLoc;
        startCellLocY = Startcell.YLoc;

        List<Cell> curPath = new List<Cell>();
        visitedCells.Add(Startcell);
        leaves.Add(Startcell);
        Pfirst(leaves, cur_floor);
        //Debug.Log("searchFin");
        //Debug.Log("Counts: Visited =" + visitedCells.Count + "total + " + cur_floor.cells.Count);      
    }

    //prims alg. slightly broken
    //modify so when a node has all its neighbours met remove the node

    private void Pfirst(List<Cell> leaves,Floor cur_floor)
    {
       // Debug.Log(leaves.Count);
        Cell nextRoom = new Cell(0,0,0,0);
        Cell curRoom = new Cell(0, 0, 0, 0);
        bool moveForward = false;
        int weight = 1000;
        Edge newEdge = new Edge(nextRoom,nextRoom,0,0,false);


        if (cur_floor.cells.Count > visitedCells.Count -1)
        {
            //Debug.Log("loop");
            //for every leaf
            for (int i = 0; i < leaves.Count; i++)
            {
                //Debug.Log("leaf");
                Cell c = leaves[i]; //i = i-1?
                //get every neighbour
                for( int j = 0;  j < c.edges.Count; j++)
                {
                    Cell neighbour = c.edges[j].getRoomB();
                    if(c.XLoc == c.edges[j].getRoomB().XLoc && c.YLoc == c.edges[j].getRoomB().YLoc)
                    {
                        neighbour = c.edges[j].getRoomA();
                    }
                    if (!visitedCells.Contains(neighbour))
                    {
                        //new neighbour
                        //Debug.Log("new neighbour");
                        //Debug.Log("new: " + c.edges[j].getWeight() + "cur =:" + weight);

                        if (c.edges[j].getWeight() < weight)
                        {
                            //Debug.Log("lowest weight so far");
                            moveForward = true;
                            newEdge = c.edges[j];
                            nextRoom = neighbour;
                            curRoom = leaves[i];
                            weight = c.edges[j].getWeight();
                        }
                    }
                }
            }
           

            if(moveForward == true)
            {

                //Debug.Log("next cell = " + nextRoom.XLoc + "," + nextRoom.YLoc + "w:" + newEdge.getWeight() );
                newEdge.setSides(Random.Range(0, 2));
                visitedCells.Add(nextRoom);
                visitedEdges.Add(newEdge);
                leaves.Add(nextRoom);
                Pfirst(leaves, cur_floor);
                //not removing the  root from the leaves.
            }
            moveForward = false;
        }
    }
}
