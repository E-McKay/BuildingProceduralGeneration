using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPathSingle{

    private List<Cell> visitedCells = new List<Cell>();
    private List<Edge> visitedEdges = new List<Edge>();
    private List<Cell> visitedPath = new List<Cell>();
    private List<Cell> RecurseCells = new List<Cell>();

    public class CellTuple: System.IEquatable<CellTuple>
    {
        public Cell First { get; private set; }
        public Cell Second { get; private set; }
        public CellTuple(Cell a, Cell b)
        {
            First = a;
            Second = b;
        }
        public bool Equals(CellTuple a)
        {
            return(this == a);
        }

        public static bool operator ==(CellTuple a, CellTuple b)
        {
            return ((a.First == b.First) && (a.Second == b.Second) || (a.Second == b.First) && (a.First == b.Second));
        }
        public static bool operator !=(CellTuple a, CellTuple b)
        {
            return ((a.First != b.First) || (a.Second != b.Second) && (a.Second != b.First) || (a.First != b.Second));
        }
    }


    private List<CellTuple> blockedEdges = new List<CellTuple>();


    private float comp;
    private bool foundpath;

    public void RunAlg(Floor cur_floor)
    {
        foundpath = false;
        visitedCells = new List<Cell>();
        visitedEdges = new List<Edge>();
        blockedEdges = new List<CellTuple>();

        //while(testPath(cur_floor)!= true)
        while (foundpath != true)
        {
            //if (foundpath != true) { randomEdge(cur_floor); testPath(cur_floor); }
            randomExpandStart(cur_floor);

        }
        //path exists

        //trying to close up edges
        foreach (Edge e in visitedEdges)
        {
            RecurseCells = new List<Cell>();
            e.setSides(Random.Range(2, 13));
            if(recursePathStart(cur_floor) == false)
            {
                e.setSides(Random.Range(0, 2));
            }
        }
    }

    private void randomExpandStart(Floor cur_floor){
        Cell root = cur_floor.cells[0];
        visitedCells.Add(root);
        randomExpand(cur_floor);
        foundpath = true;

    }    
    private void randomExpand(Floor cur_floor){
        if (visitedCells.Count < cur_floor.cells.Count)
        {

            Cell rndCell = visitedCells[Random.Range(0, visitedCells.Count)];
            Edge rndEdge =  rndCell.edges[Random.Range(0, rndCell.edges.Count)];
            rndCell = rndEdge.getRoomA();
            Cell otherSide = rndEdge.getRoomB();
            CellTuple t1 = new CellTuple(otherSide, rndCell);
            if(!blockedEdges.Contains(t1))
            {
                
                rndEdge.setSides(Random.Range(0, 2));
                blockedEdges.Add(t1);
                visitedEdges.Add(rndEdge);
                if (!visitedCells.Contains(rndCell)) { visitedCells.Add(rndCell);  }
                if (!visitedCells.Contains(otherSide)) { visitedCells.Add(otherSide);  }

            }           
            randomExpand(cur_floor);
        }
        else
        {
//            Debug.Log("pathFound");
        }



    }


    private bool recursePathStart(Floor cur_floor)
    {
        Cell root = cur_floor.cells[0];
        recursePath(root, cur_floor);
        if(RecurseCells.Count != cur_floor.cells.Count)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void recursePath(Cell root, Floor cur_floor)
    {
        foreach (Edge e in root.edges)
        {
            if (e.getSide() == 0 || e.getSide() == 1)
            {
                if (!RecurseCells.Contains(e.getRoomA()))
                {
                    RecurseCells.Add(e.getRoomA());
                    recursePath(e.getRoomA(), cur_floor);
                }
                if (!RecurseCells.Contains(e.getRoomB()))
                {
                    RecurseCells.Add(e.getRoomB());
                    recursePath(e.getRoomB(), cur_floor);
                }
            }
           
        }
    }
}
