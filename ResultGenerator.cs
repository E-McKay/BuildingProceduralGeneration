using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultGenerator{


    private int Xsize;
    private int Ysize;

    private List<int> Length = new List<int>();
    private List<int> Turns = new List<int>();
    private List<int> totalRooms = new List<int>();
    private List<int> visitedRooms = new List<int>();
    private List<int> LongestRoomDist = new List<int>(); //i.e. longest branching path
    public List<System.DateTime> startTime = new List<System.DateTime>();
    public List<System.DateTime> endTime = new List<System.DateTime>();
    public List<int> TimeDiff = new List<int>();


    public ResultGenerator(int X,int Y)
    {
        Xsize = X;
        Ysize = Y;
    }
    public bool addResults(Floor f)
    {
        
        ReturnBestPath bpath = new ReturnBestPath();
        ReturnLongestPath lpath = new ReturnLongestPath();
        Edge PrevEdge;
        Edge GoalEdge;
        bool turns = false;
        int turnsCount = 0;
        int totalLen = 0;

        //bestpath
        List<Edge> BestPathE = new List<Edge>();
        BestPathE.AddRange(bpath.RunAlg(f));
        List<Cell> BestPathC = new List<Cell>();
        BestPathC = bpath.BestPath;
        totalLen = bpath.bestPathLen ;
        //longestPath
        int LongestPath = 0;
        lpath.RunAlg(f);
        LongestPath = lpath.LongestPath;
        LongestRoomDist.Add(LongestPath);

        /*
        //reset path if it Is > size
        if((float)LongestPath >  (float)Xsize * (float)Ysize  * 0.75f)
        {
            //modify main so that if we return false we rebuild the floor 
           return false
        }
        */


        if ((f.floorNumber % 2) == 0) //POSSIBLE BROKE?
        {   //first floor, and odd floors
            //Debug.Log("first floor");
            PrevEdge = new Edge(null, null, 0, Ysize, false);
           // GoalEdge = new Edge(null, null, Xsize, 0, false);
        }
        else
        {
            //Debug.Log("odd floor");

            PrevEdge = new Edge(null,null, Xsize , Ysize, false);
            //GoalEdge = new Edge(null,null, 0, 0, false);
        }

        //check v first edge

        for (int c = 1; c < BestPathE.Count; c++) //was C
        {
            if (BestPathE[c].getUp() != turns)
            {
                turns = BestPathE[c].getUp();
                turnsCount = turnsCount + 1;
            }
            else
            {
                if(PrevEdge.getX() == BestPathE[c].getX() && BestPathE[c].getUp() == false)
                {
                    turnsCount = turnsCount + 2;
                }
                else if(PrevEdge.getY() == BestPathE[c].getY() && BestPathE[c].getUp() == true)
                {
                    turnsCount = turnsCount + 2;
                }

            }

            PrevEdge = BestPathE[c];
        }
        //Debug.Log(turnsCount);
        totalRooms.Add(f.cells.Count);
        visitedRooms.Add(BestPathC.Count);
        Turns.Add(turnsCount);
        Length.Add(totalLen); //for some reason the 2nd last length is alsways +1 and the last is always -1 
        //Debug.Log(totalLen);
        //add TIME!
        //Debug.Log("LEN:" + totalLen);
        return true;  
  }

    

    public void calc()
    {
        for (int i = 0; i < startTime.Count; i++)
        {
            System.TimeSpan duration = endTime[i] - startTime[i];
            double durationF = duration.TotalMilliseconds;
            TimeDiff.Add(Mathf.RoundToInt((float)durationF));
            
        }

        int maxBranch = 0;
        foreach(int i in LongestRoomDist)
        {
            if(i > maxBranch)
            {
                maxBranch = i;
            }
        }


        Debug.Log("Data Structure construction Time(ms): Avg:" + AverageCalc(TimeDiff) + "StdDev:" + stdDevCalc(TimeDiff) + "StdDev%:" + PercentWithinStdDev(TimeDiff));
        Debug.Log("Visited Room: Avg:" + AverageCalc(visitedRooms) + "StdDev:" + stdDevCalc(visitedRooms) + "StdDev%:" + PercentWithinStdDev(visitedRooms));
        Debug.Log("Total Room: Avg:" + AverageCalc(totalRooms) + "StdDev:" + stdDevCalc(totalRooms) + "StdDev%:" + PercentWithinStdDev(totalRooms));
        Debug.Log("Turn Room: Avg:" + AverageCalc(Turns) + "StdDev:" + stdDevCalc(Turns) + "StdDev%:" + PercentWithinStdDev(Turns));
        Debug.Log("Longest Branch: Avg:" + AverageCalc(LongestRoomDist) + "StdDev:" + stdDevCalc(LongestRoomDist) + "StdDev%:" + PercentWithinStdDev(LongestRoomDist) + "Max:" + maxBranch);
        Debug.Log("Path Length: Avg:" + AverageCalc(Length) + "StdDev:" + stdDevCalc(Length) + "StdDev%:" + PercentWithinStdDev(Length));

    }

    public float AverageCalc(List<int> input)
    {
        float avg = 0;
        foreach (int i in input)
        {
            avg = avg + i;
        }
        avg = avg / input.Count;
        return avg;
    }

    public float varianceCalc(List<int> input)
    {
        float avg = AverageCalc(input);
        float variance = 0;
        List<float> variances = new List<float>();
        foreach (int i in input)
        {
            float test = ((i - avg));
            variance = (test * test) + variance;
        }
        variance = variance / (input.Count - 1); //-1 as our data is only a sample
        return variance;

    }

    public float stdDevCalc(List<int> input)
    {
        float variance = varianceCalc(input);
        float stdDev = Mathf.Sqrt(variance);

        return stdDev;
    }

    public float PercentWithinStdDev(List<int> input)
    {
        float stdDev = stdDevCalc(input);
        float avg = AverageCalc(input);
        int count = 0;

        foreach (int i in input)
        {
            if (avg + stdDev > i && avg - stdDev < i)
            {
                count++;
            }
        }
        if (count != 0)
        {

            float x = (count / (float)input.Count) * 100;
            return x;

        }
        else
        {
            return count;
        }
    }
}
