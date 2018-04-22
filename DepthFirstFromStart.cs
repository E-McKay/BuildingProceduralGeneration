using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirstFromStart {

    private List<Cell> visitedCells = new List<Cell>();
    private float comp;

    public void RunAlg(Floor cur_floor, bool invert)
    {
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
            visitedCells = new List<Cell>();
            visitedCells.Add(Startcell);

            Dfirst(Startcell);
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
            visitedCells = new List<Cell>();
            visitedCells.Add(Startcell);
            Dfirst(Startcell);
        }
       
    }

    private void Dfirst(Cell cur_cell)
    {
        foreach (Edge e in cur_cell.edges)
        {
            if (cur_cell == e.getRoomA())
            {
                if (!visitedCells.Contains(e.getRoomB()))
                {
                    //Debug.Log("From A" + cur_cell.XLoc + "," + cur_cell.YLoc );
                   // Debug.Log("addingB" + e.getRoomB().XLoc + "," + e.getRoomB().YLoc);
                    e.setSides(Random.Range(0, 2));
                    visitedCells.Add(e.getRoomB());
                    Dfirst(e.getRoomB());
                }
            }
            else
            {
                if (!visitedCells.Contains(e.getRoomA()))
                {
                   // Debug.Log("From B" + cur_cell.XLoc + "," + cur_cell.YLoc);
                   // Debug.Log("addingA" + e.getRoomA().XLoc + "," + e.getRoomA().YLoc);
                    e.setSides(Random.Range(0, 2));
                    visitedCells.Add(e.getRoomA());
                    Dfirst(e.getRoomA());
                }
            }
        }
    }
}
