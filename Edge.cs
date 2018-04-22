using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge {

    //these may not be acessible?
    //private room room_A;
    //private room room_B;
    Cell room_A;
    Cell room_B;
    private int weight;
    private int edgeX;
    private int edgeY;
    private bool up; //horizontal/vertical


    //NOTE: X AND Y ARE INVERTED!

    public Edge(Cell r_a, Cell r_b, int edge_X, int edge_Y, bool edge_up)
    {
        room_A = r_a;
        room_B = r_b;
        edgeX = edge_X;
        edgeY = edge_Y;
        up = edge_up;
    }
    public void setWeight(int newWeight)
    {
        weight = newWeight;
    }
    public int  getWeight()
    {
        return weight;
    }





    public Cell getRoomA()
    {
        return room_A;
    }
    public Cell getRoomB()
    {
        return room_B;
    }
    public int getX()
    {
        return edgeX;
    }
    public int getY()
    {
        return edgeY;
    }
    public bool getUp()
    {
        return up;
    }

    public void setSides(int val)
    {
        int a_side_num = 0;
        int b_side_num = 0;

       // Debug.Log("EdgeX" + edgeX + "Xloc" + room_A.XLoc);

        if (up == true)
        {
            a_side_num = edgeX - room_A.XLoc;
            b_side_num = edgeX - room_B.XLoc;

            room_A.Topsides[a_side_num].wallNumber = val;
            room_B.Bottomsides[b_side_num].wallNumber = val;
        }
        else
        {
            a_side_num = edgeY - room_A.YLoc;
            b_side_num = edgeY - room_B.YLoc;

            room_A.Rightsides[a_side_num].wallNumber = val;
            room_B.Leftsides[b_side_num].wallNumber = val;
        }
    }
    public int getSide()
    {
        int a_side_num = 0;
        int b_side_num = 0;
        int num;

        // Debug.Log("EdgeX" + edgeX + "Xloc" + room_A.XLoc);

        if (up == true)
        {
            a_side_num = edgeX - room_A.XLoc;
            b_side_num = edgeX - room_B.XLoc;

            num = room_A.Topsides[a_side_num].wallNumber;
        }
        else
        {
            a_side_num = edgeY - room_A.YLoc;
            b_side_num = edgeY - room_B.YLoc;

            num = room_A.Rightsides[a_side_num].wallNumber;
        }

        return num;
    }

}
