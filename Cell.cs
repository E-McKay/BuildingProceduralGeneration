using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell{

    // MODIFY TO GET/SET
    private int id;
    bool containsStairs;
    //possible convert to tuples
    //location of top corenr
    public int XLoc; //Xtop
    public int YLoc; //Ybottom
    //size
    public int Xsize;
    public int Ysize;
    //children

    public int type;

    public List<Cell> children = new List<Cell>();

    public List<Side> Topsides = new List<Side>();
    public List<Side> Bottomsides = new List<Side>();
    public List<Side> Rightsides = new List<Side>();
    public List<Side> Leftsides = new List<Side>();
    public List<Edge> edges = new List<Edge>();

    public List<Cell> Children
    {
        get { return children; }
    }

    public int id_set
    {
        get { return id; }
        set { id = value; }
    }


    public Cell(int Xs, int Ys, int Xt, int Yb)
    {
        //containsStairs = stairs;
        Xsize = Xs;
        Ysize = Ys;
        XLoc = Xt;
        YLoc = Yb;
    }

    //method to split, if it can continue splitting return true, else return false
    public bool split(int chanceMod)
    {
        //ADD in cutting direction mandatory if any direction >4



        if (Xsize > 1 || Ysize > 1)
        {
            //insert if here to do % chance, so if size > 2 50% chance it stops, if size >3 10% chance, ect.


            //REDO LOGIC FOR THIS
            int cutchance = Xsize + Ysize; //*
            int cutrand = Random.Range(0, chanceMod); //make this something we can pass about and potentially modify? 
            int cut_direction = Random.Range(0,2);
            bool tocut = false;

            //Debug.Log(cutrand);

            if (cutchance - cutrand > 0) //chancing this will change min tile size
            {
                tocut = true;
            }

            if (Xsize == 1 && Ysize == 1)
            {
                cutchance = 0;
                tocut = false;
            }else if(Xsize > 4 && Ysize > 4){
                tocut = true;
            }
            else if (Xsize > 4)
            {
                cut_direction = 0; //might be wrong way around
                tocut = true;
            }
            else if (Ysize > 4)
            {
                cut_direction = 1; //might be wrong way around
                tocut = true;
            }
            //slightly broke here
            if (Xsize == 4)
            {
                cut_direction = 1;
                tocut = true;
            }
            else if (Ysize == 4)
            {
                cut_direction = 0;
                tocut = true;
            }

            if (Xsize == 1)
            {
                cut_direction = 1;
            }
            else if (Ysize == 1)
            {
                cut_direction = 0;
            }

            if (tocut == true)
            {
                //random cut direction (0 = X, 1 = Y)
                //Debug.Log(cut)
                if (cut_direction == 0)
                {
                    //random cut X locaiton
                    int cut_X = Random.Range(1, Xsize - 1);
                    Cell childA = new Cell(cut_X, Ysize, XLoc, YLoc);
                    Cell childB = new Cell(Xsize - cut_X, Ysize, (XLoc + cut_X), YLoc);
                    children.Add(childA);
                    children.Add(childB);
                }
                else
                {
                    //random cut Y location
                    int cut_Y = Random.Range(1, Ysize - 1);
                    Cell childA = new Cell(Xsize, cut_Y, XLoc, YLoc);
                    Cell childB = new Cell(Xsize, Ysize - cut_Y, XLoc, (YLoc + cut_Y));
                    children.Add(childA);
                    children.Add(childB);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }


    public void fillSides()
    {
        // loop twice to fill in order of top,left,bottom,right
        for (int i = 0; i < Xsize; i++)
        {
            int top = YLoc + Ysize;
            Side topS = new Side(i + XLoc , top);
            Side bottomS = new Side(i+ XLoc , YLoc);
            Topsides.Add(topS);
            Bottomsides.Add(bottomS);
        }
        for (int i = 0; i < Ysize; i++)
        {
            int right = XLoc + Xsize;
            Side leftS = new Side(XLoc, i+ YLoc); 
            Side rightS = new Side(right , i+ YLoc);
            Rightsides.Add(rightS);
            Leftsides.Add(leftS);
        }
    }

    

}
