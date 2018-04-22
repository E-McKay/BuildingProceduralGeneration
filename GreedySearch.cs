using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedySearch {

    private List<Cell> visitedCells = new List<Cell>();
    private List<Edge> visitedEdges = new List<Edge>();

    private float comp;

    public void RunAlg(Floor cur_floor, bool invert)
    {
        visitedCells = new List<Cell>();
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
        
        List<Cell> curPath = new List<Cell>();
        visitedCells.Add(Startcell);
        Gfirst(Startcell, curPath,cur_floor);
       // Debug.Log("searchFin");
       // Debug.Log("Counts: Visited =" + visitedCells.Count + "total + " + cur_floor.cells.Count);        
    }

    private void Gfirst(Cell cur_cell, List<Cell> cellList, Floor cur_floor)
    {
        Cell nextRoom;
        Edge nextEdge = new Edge(cur_cell,cur_cell,0,0,false);
        bool moveForward = false;
        nextEdge.setWeight(1000);
        //visitedCells.Add(cur_cell);

        //Debug.Log("Moving to" + cur_cell.XLoc + "," + cur_cell.YLoc);

        if (cur_floor.cells.Count > visitedCells.Count)
        {
            foreach (Edge e in cur_cell.edges)
            {
                if (e.getRoomA() == cur_cell) { nextRoom = e.getRoomB(); } else { nextRoom = e.getRoomA(); }

                if (e.getWeight() < nextEdge.getWeight() && !visitedCells.Contains(nextRoom))
                {
                    moveForward = true;
                    nextEdge = e;
                }
            }

            if (moveForward == true)
            {
                if (nextEdge.getRoomA() == cur_cell)
                {
                    nextRoom = nextEdge.getRoomB();
                }
                else
                {
                    nextRoom = nextEdge.getRoomA();
                }
                visitedCells.Add(nextRoom);
                nextEdge.setSides(Random.Range(0, 2));
                visitedEdges.Add(nextEdge);
                cellList.Add(cur_cell);
                Gfirst(nextRoom, cellList, cur_floor);
            }
            else
            {
                //Debug.Log("go_back");
                cellList.RemoveAt(cellList.Count - 1);
                Gfirst(cellList[cellList.Count - 1], cellList, cur_floor); //sometimes gets error here.

            }
        }
    }
}
