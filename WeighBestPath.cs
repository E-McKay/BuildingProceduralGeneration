using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeighBestPath {
    private List<Cell> visitedCells = new List<Cell>();
    private List<Edge> visitedEdges = new List<Edge>();

    private float comp;

    public List<Edge> RunAlg(Floor cur_floor, bool invertSearch)
    {
        visitedCells = new List<Cell>();
        Cell Startcell = cur_floor.cells[0];
        Cell GoalCell = cur_floor.cells[0];
        if ((cur_floor.floorNumber % 2) == 0 && invertSearch == false || (cur_floor.floorNumber % 2) == 1 && invertSearch == true)
        {
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
        {
            int counter = 0;
            foreach (Cell c in cur_floor.cells)
            {
                if (c.YLoc + c.XLoc > counter)
                {
                    counter = c.YLoc + c.XLoc;
                    Startcell = c;
                }
            }
            foreach(Cell c in cur_floor.cells)
            {
                if(c.XLoc == 0 && c.YLoc == 0)
                {
                    GoalCell = c; 
                }
            }

        }

        List<Cell> curPath = new List<Cell>();
        visitedCells.Add(Startcell);
        List<Edge> newList = new List<Edge>();
        Dfirst(Startcell, GoalCell, newList, cur_floor);
        //Debug.Log("searchFin");
        //Debug.Log("Counts: Visited =" + visitedCells.Count + "total + " + cur_floor.cells.Count);
        return visitedEdges;
    }

    private void Dfirst(Cell cur_cell, Cell GoalCell, List<Edge> pathEdges, Floor cur_floor)
    {
        Edge nextEdge = new Edge(cur_cell, cur_cell, 0, 0, false);
        nextEdge.setWeight(1000);
        //visitedCells.Add(cur_cell);

        //Debug.Log("Moving to" + cur_cell.XLoc + "," + cur_cell.YLoc);

        if (cur_cell != GoalCell)
        {
            foreach (Edge e in cur_cell.edges)
            {
                if (cur_cell == e.getRoomA())
                {
                    if (!visitedCells.Contains(e.getRoomB()))
                    {
                        visitedCells.Add(e.getRoomB());
                        List<Edge> NpathEdges = new List<Edge>();
                        NpathEdges.Add(e);
                        NpathEdges.AddRange(pathEdges);
                        Dfirst(e.getRoomB(), GoalCell, NpathEdges, cur_floor);
                    }
                }
                else
                {
                    if (!visitedCells.Contains(e.getRoomA()))
                    {
                        List<Edge> NpathEdges = new List<Edge>();
                        NpathEdges.Add(e);
                        NpathEdges.AddRange(pathEdges);
                        visitedCells.Add(e.getRoomA());
                        Dfirst(e.getRoomA(), GoalCell, NpathEdges, cur_floor);
                    }
                }
            }

        }
        else
        {
            visitedEdges = pathEdges;
        }
    }
}
