using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder {

    // Use this for initialization
    RandomPathSingle RpathSingle = new RandomPathSingle(); //randomly add doors until we have a path from every room to every other room
    RandomPathMulti RpathMulti = new RandomPathMulti(); //randomly add doors until we have a path from every room to every other room, no more than 1 door to each room
    BreadthFirst Bfirst = new BreadthFirst();
    DepthFirstFromStart Dfirst = new DepthFirstFromStart();
    GreedySearch Gfirst = new GreedySearch();
    PrimsSearch Psearch = new PrimsSearch();

    public void path(Floor cur_floor,int Pathmode, bool invertSearch)
    {
        //path
        if (Pathmode == 0)
        {
            Debug.Log("running breadth first search from the start");
            Bfirst.RunAlg(cur_floor, invertSearch);
        }
        else if (Pathmode == 1)
        {
            Debug.Log("running Depth first from start room");
            Dfirst.RunAlg(cur_floor, invertSearch);
        }
        else if (Pathmode == 2)
        {
            Debug.Log("running random path single door");
            RpathSingle.RunAlg(cur_floor);
        }
        else if (Pathmode == 3)
        {
            Debug.Log("running random path with multiple doors");
            RpathMulti.RunAlg(cur_floor);
        }
        else if (Pathmode == 4)
        {
            Debug.Log("running Greedy search");
            Gfirst.RunAlg(cur_floor, invertSearch);
        }
        else if (Pathmode == 5)
        {
            Debug.Log("running Prims");
            Psearch.RunAlg(cur_floor, invertSearch);
        }/*
        else if (Pathmode == 6)
        {
            Debug.Log("running Greedy + random_single");
            RpathSingle.RunAlg(cur_floor);
            Gfirst.RunAlg(cur_floor, invertSearch);
        }
        else if (Pathmode == 7)
        {
            Debug.Log("running Prims + random single");
            RpathSingle.RunAlg(cur_floor);
            Psearch.RunAlg(cur_floor, invertSearch);
        }*/
        else
        {
            //pathmode == discarding of best path + prims
            Debug.Log("no path specified running random single");
            RpathSingle.RunAlg(cur_floor);
        }
    }

}
