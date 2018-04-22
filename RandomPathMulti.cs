using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPathMulti{

    private List<Cell> visitedCells = new List<Cell>();
    private List<Edge> visitedEdges = new List<Edge>();
    private List<Cell> visitedPath = new List<Cell>();

    private float comp;
    private bool foundpath;

    public void RunAlg(Floor cur_floor)
    {
        foundpath = false;
        visitedCells = new List<Cell>();
        visitedEdges = new List<Edge>();
        //while(testPath(cur_floor)!= true)
        while (foundpath != true)
        {
            //if (foundpath != true) { randomEdge(cur_floor); testPath(cur_floor); }
            randomExpandStart(cur_floor);

        }

    }

    private void randomExpandStart(Floor cur_floor){
        Cell root = cur_floor.cells[0];
        visitedCells.Add(root);
        randomExpand(cur_floor);
        foundpath = true;

    }

    private void randomExpand(Floor cur_floor){
        if(visitedCells.Count < cur_floor.cells.Count)
        {
            Cell rndCell = visitedCells[Random.Range(0, visitedCells.Count)];
            Edge rndEdge =  rndCell.edges[Random.Range(0, rndCell.edges.Count)];
            if(!visitedEdges.Contains(rndEdge))
            {
                visitedEdges.Add(rndEdge);
                rndEdge.setSides(Random.Range(0, 2));
                if (rndCell == rndEdge.getRoomA())
                {
                    if (!visitedCells.Contains(rndEdge.getRoomB())) { visitedCells.Add(rndEdge.getRoomB()); }
                }
                else if (rndCell == rndEdge.getRoomB())
                {
                    if (!visitedCells.Contains(rndEdge.getRoomA())) { visitedCells.Add(rndEdge.getRoomA()); }
                }
                else
                {
                   // Debug.Log("error");
                }
            }
            randomExpand(cur_floor);
        }
        else
        {
            //Debug.Log("pathFound");
        }



    }

    private void testPath(Floor cur_floor)
    {
        visitedPath = new List<Cell>();
        Cell root = cur_floor.cells[0];

        visitedPath.Add(root);
        if(recursePath(root,cur_floor) == true)
        {
            foundpath = true;
        }
    }

    private bool recursePath(Cell root,Floor cur_floor)
    {
        bool hasEdge = false;
       // Debug.Log( " VisitedCells" + visitedCells.Count);
       // Debug.Log( " PathLen" + visitedPath.Count);


        foreach (Edge e in visitedEdges)
        {
            if(e.getRoomA() == root && !visitedPath.Contains(e.getRoomA()))
            {
                //Debug.Log(" found leaf in pathing");
                visitedPath.Add(e.getRoomA());
                hasEdge = true;
            }
            if (e.getRoomB() == root && !visitedPath.Contains(e.getRoomB()))
            {
               // Debug.Log(" found leaf in pathing");
                visitedPath.Add(e.getRoomB());
                hasEdge = true;
            }

        }
        if(hasEdge == false)
        {
          if(cur_floor.cells.Count == visitedPath.Count)
            {
               // Debug.Log("path found");
                return true;
            }
        }

        return false;
    }

    private void randomEdge(Floor cur_floor)
    {
       List<Edge> edges = cur_floor.getEdge();
       
       int selector = Random.Range(0, edges.Count);
       if (!visitedEdges.Contains(edges[selector]))
       {
            int edgeChoose = Random.Range(0, 2);
            edges[selector].setSides(edgeChoose);
            visitedEdges.Add(edges[selector]);

            if (!visitedCells.Contains(edges[selector].getRoomA()))
            {
                visitedCells.Add(edges[selector].getRoomA());
            };
            if (!visitedCells.Contains(edges[selector].getRoomB()))
            {
                visitedCells.Add(edges[selector].getRoomB());
            };

        }
       /*
        else
        {
            randomEdge(cur_floor);
        }
        */
    }
}
