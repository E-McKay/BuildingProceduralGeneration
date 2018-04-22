using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side{

    // we have 8 possible sides
    // if wallNumber = 0 its a door
    public int wallNumber;
    private int Xlocation;
    private int Ylocation;

    public Side(int X, int Y)
    {
        //assign it a random wall pannel
        wallNumber = Random.Range(4, 13); //not random
        Xlocation = X;
        Ylocation = Y;
        //Debug.Log(wallNumber);
    }
    //to set door
    public void SetDoor()
    {
        //option to expand door to 8
        //doors are 0 or 1
        wallNumber = 0;
    }
    public int Xloc
    {
        set { Xlocation = value; }
        get { return Xlocation; }
    }
    public int Yloc
    {
        set { Ylocation = value; }
        get { return Ylocation; }
    }
}
