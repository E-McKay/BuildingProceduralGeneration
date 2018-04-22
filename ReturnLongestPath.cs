using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//for some reason we need to reset bestpathLen from the edge contents


public class ReturnLongestPath {

    public List<Edge> visitedEdges;
    public List<Cell> BestPath = new List<Cell>();
    public int LongestPath = 0;

    private List<Cell> tempCells;
    private List<List<Cell>> paths;
    private List<List<int>> visited;
    private Edge startedge;
    private Edge goaledge;
    

    public List<Edge> RunAlg(Floor cur_floor)
    {

        ///ISSUUE WITH THE ODD FLOORS AND THE STARTEDGE 


        Cell Startcell = cur_floor.cells[0];
        Cell GoalCell = cur_floor.cells[0];

        ReturnBestPath bpath = new ReturnBestPath();
        List<Cell> BestPath = new List<Cell>();
        bpath.RunAlg(cur_floor);
        BestPath = bpath.BestPath;

        if ((cur_floor.floorNumber % 2) == 0)
        {
            //Debug.Log("FIRST");
            //if its an even floor
            startedge = new Edge(Startcell, Startcell, 0, cur_floor.getY() -1, false);
            goaledge = new Edge(Startcell, Startcell, cur_floor.getX() , 0, false);
            foreach (Cell c in cur_floor.cells)
            {
                if (c.YLoc > Startcell.YLoc && c.XLoc == 0)
                {
                    Startcell = c;
                }
            }
            foreach (Cell c in cur_floor.cells)
            {
                if (c.XLoc + c.Xsize == cur_floor.getX() && c.YLoc == 0)
                {
                    GoalCell = c;
                }
            }
        }
        else
        {   //odd floor
            //Debug.Log("SECOND");

            int counter = 0;
            startedge = new Edge(Startcell, Startcell, cur_floor.getX(), cur_floor.getY() -1, false);
            goaledge = new Edge(Startcell, Startcell, 0, 0, false);
            foreach (Cell c in cur_floor.cells)
            {
                if (c.YLoc + c.XLoc > counter)
                {
                    counter = c.YLoc + c.XLoc;
                    Startcell = c;
                }
            }
            foreach (Cell c in cur_floor.cells)
            {
                if (c.XLoc == 0 && c.YLoc == 0)
                {
                    GoalCell = c;
                }
            }

        }

      //  Debug.Log("goal = " + GoalCell.XLoc + "," + GoalCell.YLoc);


        foreach(Cell c in cur_floor.cells)
        {
            if((c != GoalCell || c!= Startcell) && !BestPath.Contains(c))
            {
                List<Cell> curPath = new List<Cell>();
                List<Cell> DefaultPath = new List<Cell>();

                paths = new List<List<Cell>>();
                visited = new List<List<int>>();

                DefaultPath.Add(Startcell);
                paths.Add(DefaultPath);
                List<Edge> newList = new List<Edge>();
                newList.Add(startedge);
                //Debug.Log("start");
                Dfirst(Startcell, GoalCell,c, DefaultPath, newList, cur_floor, 0); 
                //Debug.Log("SEARCH FINISHED");
            }
        }

        LongestPath = LongestPath + 1; //so we step inside the last cell.

        

        return visitedEdges;
    }

    public bool visitedCheck(Cell cell_a, int count )
    {
        bool inList = false;
        bool smaller = false;

        foreach(List<int> i in visited)
        {
            if(i[0] == cell_a.XLoc && i[1] == cell_a.YLoc)
            {
                inList = true;
                if(i[2] > count)
                {
                    i[2] = count;
                    smaller = true;
                }
            }

        }

        if(inList == false)
        {
            List<int> newList = new List<int>();
            newList.Add(cell_a.XLoc);
            newList.Add(cell_a.YLoc);
            newList.Add(count);
            visited.Add(newList);
        }

        if(inList == true && smaller == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Dfirst(Cell cur_cell, Cell GoalCell, Cell targetCell,List<Cell> curPath, List<Edge> pathEdges, Floor cur_floor,  int pthLen)
    {
        Edge nextEdge = new Edge(cur_cell, cur_cell, 0, 0, false);
        nextEdge.setWeight(1000);
        //visitedCells.Add(cur_cell);

        //Debug.Log("Moving to" + cur_cell.XLoc + "," + cur_cell.YLoc);

        if ((curPath.Count < cur_floor.cells.Count && cur_cell != targetCell) )
        {
                foreach (Edge e in cur_cell.edges)
                {
                    if (cur_cell == e.getRoomA() && e.getSide() < 2 && !curPath.Contains(e.getRoomB()))
                    {   
                            List<Edge> NpathEdges = new List<Edge>();
                            List<Cell> NpathCells = new List<Cell>();
                            NpathEdges.AddRange(pathEdges);
                            NpathEdges.Add(e);
                            NpathCells.AddRange(curPath);
                            NpathCells.Add(e.getRoomB());
                            int NpthLen = pthLen + getLenBetween(pathEdges[pathEdges.Count - 1], e);
                            if (visitedCheck(e.getRoomB(), NpthLen) && cur_cell != GoalCell)
                            {
                                Dfirst(e.getRoomB(), GoalCell,targetCell, NpathCells, NpathEdges, cur_floor,NpthLen);
                            }
                    }
                    else if (e.getSide() < 2 && !curPath.Contains(e.getRoomA()))
                    {
                            List<Edge> NpathEdges = new List<Edge>();
                            List<Cell> NpathCells = new List<Cell>();
                            NpathEdges.AddRange(pathEdges);
                            NpathEdges.Add(e);
                            NpathCells.AddRange(curPath);
                            NpathCells.Add(e.getRoomA());
                            int NpthLen = pthLen + getLenBetween(pathEdges[pathEdges.Count - 1],e);
                            if (visitedCheck(e.getRoomA(), NpthLen) && cur_cell != GoalCell)
                            {
                                Dfirst(e.getRoomA(), GoalCell, targetCell, NpathCells, NpathEdges, cur_floor, NpthLen);
                            }
                        }   
                }   
            

        }
        else
        {  
            if(cur_cell == targetCell && cur_cell != GoalCell)
            {
                //pthLen = pthLen + getLenBetween(pathEdges[pathEdges.Count - 1], goaledge);
                
                pathEdges.Add(goaledge);
                if (pthLen > LongestPath || LongestPath == 0)
                {
                    LongestPath = pthLen;
                    BestPath = new List<Cell>();
                    BestPath.AddRange(curPath);
                    visitedEdges = new List<Edge>();
                    //pathEdges.RemoveAt(0);
                    visitedEdges = pathEdges;
                    //Debug.Log("best path:" + curPath.Count + "Len:" + bestPathLen);


                }
            }
            
        }
    }


    public int getLenBetween(Edge prev_edge, Edge next_edge)
    {
        Edge edge_A = prev_edge;
        Edge edge_B = next_edge;
        string type = "";
        int len = 0;

        //Debug.Log("edgeA:" + edge_A.getUp());
        //Debug.Log("edgeB:" + edge_B.getUp());


        if (edge_A.getUp() == true && edge_B.getUp() == true)
        {
            type = "up";

            //calculate the diffrence in Y vals
            len = edge_A.getY() - edge_B.getY();
            if (len == 0)
            {
                len = edge_A.getX() - edge_B.getX();
                if (len < 1)
                {
                    len = len * -1;
                }
                len = len + 1;
                //Debug.Log("A:" + edge_A.getX() + "," + edge_A.getY() + " B:" + edge_B.getX() + edge_B.getY() + " t:" + type);
                //Debug.Log("L:" + len);
                return len;

            }
            else if (len < 1)
            {
                len = len * -1;
            }
            //add the diffrence in X vals
            int Xmod = edge_A.getX() - edge_B.getX();



            if (Xmod < 1)
            {
                Xmod = Xmod * -1;
            }
            if (Xmod > 1)
            {
                //len = len + Xmod - 1;
            }

            len = len + Xmod;



        }
        else if (edge_A.getUp() == false && edge_B.getUp() == false)
        {
            type = "side";
            //calculate the diffrence in X vals
            len = edge_A.getX() - edge_B.getX();

            if (len == 0)
            {
                len = edge_A.getY() - edge_B.getY();
                if (len < 1)
                {
                    len = len * -1;
                }
                len = len + 1;
                //Debug.Log("A:" + edge_A.getX() + "," + edge_A.getY() + " B:" + edge_B.getX() + edge_B.getY() + " t:" + type);
                //Debug.Log("L:" + len);
                return len;

            }
            else if (len < 1)
            {
                len = len * -1;
            }
            //add the diffrence in Y vals
            int Ymod = edge_A.getY() - edge_B.getY();
            if (Ymod < 1)
            {
                Ymod = Ymod * -1;
            }
            if (Ymod > 1)
            {
                //Debug.Log("L:" + len + "Y:" + Ymod);

                //len = len + Ymod - 1;
                //len = Ymod;
            }

            len = len + Ymod;

        }
        else
        {


            //litreally the only thing broken

            type = "corner";
            //get the dif between the X's
            //edge B is the prev edge
            //edge A is the next edge

            int Xmod = 0;
            int Ymod = 0;

            int Xdif = ((edge_B.getX()) - edge_A.getX());
            //get the dif between the Ys
            int Ydif = ((edge_B.getY()) - (edge_A.getY()));


           //make it so with the 'odd' floors it works

            //Coming from to
            if (edge_B.getUp() == true)
            {
                if (Xdif < 0)
                {
                    //going right 
                    //Debug.Log("from right");
                    //?????

                    if (Ydif >= 1)
                    {
                       // Debug.Log("R->U");
                    }
                    else
                    {
                       // Debug.Log("R->D");
                        Ymod = Ymod + 1;
                        //Xmod = Xmod + 1;
                    }

                }
                else
                {
                    //going left
                    //Debug.Log("from left");

                    if (Ydif >= 1)
                    {
                        //Debug.Log("L->U");
                        Xmod = 1;
                    }
                    else
                    {
                        //Debug.Log("L->D");
                        Xmod = Xmod + 1;
                        Ymod = Ymod + 1;
                    }

                }



            }
            if (edge_B.getUp() == false)
            {
                if (Ydif < 0) //Ydif > 1
                {
                    //going up
                    //Debug.Log("from up");
                    if (Xdif >= 1)
                    {
                        //Debug.Log("U->R");

                    }
                    else
                    {
                        //Debug.Log("U->L");
                        Xmod = 1;

                    }

                }
                else
                {
                    //going Down
                    //Debug.Log("from down");

                    if (Xdif >= 1)
                    {
                        //Debug.Log("D->R");
                        Ymod = 1; //??
                    }
                    else
                    {
                        //Debug.Log("D->L");
                        Xmod = 1;
                        Ymod = 1;

                    }
                }



            }

            if (Xdif < 0) { Xdif = Xdif * -1; }
            if (Ydif < 0) { Ydif = Ydif * -1; }

            Xdif = Xdif + Xmod;
            Ydif = Ydif + Ymod;



            //Xdif = Xdif + Xmod;
            //Ydif = Ydif + Ymod;

            //if going down + 1
            len = Xdif + Ydif - 1;
            //Debug.Log("sizes:" + Xdif + "," + Ydif);


        }


        //Debug.Log("A:" + edge_B.getX() + "," + edge_B.getY() + " B:" + edge_A.getX() + edge_A.getY() + " t:" + type +"L:" + len);
        return len;
    }
}
