using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapNode
{

    public bool walkable;
    public Vector3 WorldPosition;

    public int gCost;
    public int hCost;
    public int gridX;
    public int gridY;
    public MiniMapNode parent;


    public MiniMapNode(bool walk, Vector3 nodeLoc, int xPos, int yPos)
    {
        walkable = walk;
        WorldPosition = nodeLoc;
        gridX = xPos;
        gridY = yPos;

    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

}
