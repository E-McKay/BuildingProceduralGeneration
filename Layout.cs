 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the main

public class Layout : MonoBehaviour {

    public int height;
    public int Xsize;
    public int Ysize;
    
    public int splitChance;
    public int typeChance;

    public bool FillStairSpace; //i.e. do we add an extra room in the space left by the stairs?
    public bool renderall;
    
    public int  Pathmode;
    public bool invertSearch;
    public bool randomweights;  //randomly add weights
    public int rndWeightScale;
    public bool Xweights;       //add weights to cells close to the 'x'
    public bool invertweights; //invert weights
    public bool DFirstWeights; //runs Bfirst to find a path to the goal, and then adds weights on to that path

    public float rndChancePath; //random % 


    public bool evaluate;
    public List<RoomController> tilesets = new List<RoomController>();

    public GameObject goal_Room;

    //PathFinder pathfinder = new PathFinder;

    //public AStarPath Astar;
    //public Dijkstras Dijk;
    
    List<Floor> buildingFloors = new List<Floor>();
    List<GameObject> PhysicalFloors = new List<GameObject>();
    List<GameObject> PhysicalRooms = new List<GameObject>();
    List<GameObject> PhysicalSides = new List<GameObject>();
    List<RoomController> RoomTypes = new List<RoomController>();

    public GameObject player;
    int currentfloor = 0;
    float floorheight = 10.3615f;
    List<int> renderedFloors;

    PathFinder pathfinder = new PathFinder();

    void buildRoomLists()
    {
        // taking in the potential additional floor types. a floor is comprised of corridor tiles and typed tiles
        //adding corridoor to the potential additional tiles.

        foreach (RoomController r in tilesets)
        {
            r.StartLists();
            RoomTypes.Add(r);
        }
    }

    // Use this for initialization
    void Start () {

        Vector3 playerLoc = new Vector3(4.0f, -15.96f, (Ysize * 15) -5.0f); //sets the players inital start location
        player.transform.position = playerLoc;                              
        buildRoomLists();                                                   //build the lists of parts which contain the same sizes
        if(Ysize == 1)                                                      // if the Y size is 1 set it to 2, to retain robustness
        {
            Debug.Log("one Wide buildings cant be produced; producing a 2 Wide building");
            Ysize = 2;
        }
        ResultGenerator results = new ResultGenerator(Xsize, Ysize);        //create new result generator object
        //loop to produce the data structures
        for (int i = 0; i < height; i++) //for every floor
        {
            results.startTime.Add(System.DateTime.Now);                     //add the start time to the results
            GameObject floor = new GameObject("floor" + i);                 //create the object which holds all the floors parts
            Floor cur_floor = new Floor(Xsize, Ysize, i,splitChance);       //create a new floor
            int rndFloorType = Random.Range(0, RoomTypes.Count);            //get the floor type 
            cur_floor.setType(rndFloorType);                                //set the floor type
            cur_floor.buildLayout(splitChance);                             //divide the floor into a layout
            cur_floor.setRoomTypes(typeChance);                             //set the type for every room
            cur_floor.SetEntrance(i,FillStairSpace,height);                 //add the entrance
            cur_floor.SetExit(i,FillStairSpace,height);                     //add the exit
            cur_floor.buildGraph();                                         //build the graph from the floor
            setRandomEdges(cur_floor);                                      //open up some new random doorways
            path(cur_floor);                                                //create doors via the path
            buildingFloors.Add(cur_floor);                                  //add the floor to the building
            results.endTime.Add(System.DateTime.Now);                       //add the end time to the results
            if (evaluate == true) { results.addResults(cur_floor); }                                  //add the results of the floor

        }
        var InitTime = System.DateTime.Now;

        addStart();                                                         //add the start room
        addGoal();                                                          //add the goal room
        for (int i = 0; i < height; i++)
        {
            Floor cur_floor = buildingFloors[i];                            //get the current floor
            int rndFloorType = cur_floor.getType();                         //get the current floors type
            populate(cur_floor, rndFloorType);                              //create a 3d model in relation to the current floor
            addStairs(cur_floor);                                           //add the stairs to the floor
            PhysicalFloors.Add(GameObject.Find("floor" + i));               //add the floor game object  to a list, used for culling
        }
        var endtime = System.DateTime.Now;
        results.calc();                                                     //calculate results
        System.TimeSpan timespan = endtime - InitTime;
        Debug.Log(timespan.TotalMilliseconds);
    }

    void Update()
    {
        float PlayerHeight = player.transform.position.y;
        int floorNum = Mathf.RoundToInt((PlayerHeight / floorheight) + 1);



        if (renderall == true && GameObject.Find("floor1") == false)
        {
            for(int i = 0; i < height; i++){
                renderFloor(i,true);
            }
        }

        if (renderall == false){
            if (currentfloor != floorNum)
            {
                //Debug.Log("FLOOR:"  + currentfloor);

                if (floorNum < currentfloor)
                {
                    //Debug.Log("going down");
                    renderFloor(floorNum + 2, false);
                    renderFloor(floorNum - 1, true);
                }
                else
                {
                    //Debug.Log("going up");
                    renderFloor(floorNum - 2, false);
                    renderFloor(floorNum + 1, true);
                }
                currentfloor = floorNum;

            }
        }
       
    }

    void renderFloor(int floornum, bool rend)
    {
        if (floornum >= 0 && floornum < height)
        {
            PhysicalFloors[floornum].SetActive(rend);
        }
    }

    void path(Floor cur_floor)
    {
        //weights
        if(randomweights == true){
            cur_floor.RandomWeights(0, rndWeightScale);
        }
        if(Xweights == true){
            cur_floor.ShortestPathWeights();
        }
        if(DFirstWeights == true){
            cur_floor.WeightPath(invertSearch);
        }
        if(invertweights == true){
            cur_floor.InvertWeights();
        }

        pathfinder.path(cur_floor, Pathmode,invertSearch);

        //randChancePath
        foreach(Cell c in cur_floor.cells)
        {
            foreach(Edge e in c.edges)
            {
                int cut = Random.Range(0, 100);
                if (cut < rndChancePath)
                {
                    e.setSides(Random.Range(0,2));
                    //Debug.Log("new side from rndchance");
                }
            }  
        }
    }

    void setRandomEdges(Floor cur_floor)
    {
        if (cur_floor.cells.Count > 1)
        {
            //chance for a random obstacle = 10
            foreach (Cell c in cur_floor.cells)
             {
                int edgeChance = Random.Range(0, 10);
                if (edgeChance < 3)
                {
                    int edgenum = Random.Range(0, c.edges.Count);
                    Edge rndE = c.edges[edgenum];
                    rndE.setSides(Random.Range(2, 4));
                    //Debug.Log("edge set");
                }
            }
        }
    }

    public void populate(Floor cur_floor, int roomIdx)
    {
        RoomController rooms;
        GameObject newRoom = tilesets[0].getOneOne();

        for (int j = 0; j < cur_floor.cells.Count; j++)
        {


            if (cur_floor.cells[j].type == 0)
            {
                rooms = tilesets[0];
            }
            else
            {
                rooms = tilesets[roomIdx];
            }


            Cell cur_cell = cur_floor.cells[j];

            float XLoc = (cur_cell.XLoc * 15.0f) + 30.0f;
            float ZLoc = cur_cell.YLoc * 15.0f;
            float YLoc = cur_floor.floorNumber * 10.365f;
            Vector3 pos = new Vector3(XLoc, YLoc, ZLoc); //multiply by room sizes , height * 10.365

            float Xmodifier = 0.0f;
            float Zmodifier = 0.0f;
            float rotModifier = 0.0f;

            int flip = Random.Range(0, 2);
            if (flip != 0) { rotModifier = 180.0f; }

            Xmodifier = cur_cell.Xsize * 7.5f;
            Zmodifier = cur_cell.Ysize * 7.5f;
            pos = new Vector3(pos.x + Xmodifier, pos.y, pos.z + Zmodifier);

            if (cur_cell.Xsize == 1 && cur_cell.Ysize == 1) { newRoom = rooms.getOneOne(); rotModifier = 90 * Random.Range(0, 5); }
            else if (cur_cell.Xsize == 1 && cur_cell.Ysize == 2) { newRoom = rooms.getOneTwo(); }
            else if (cur_cell.Xsize == 1 && cur_cell.Ysize == 3) { newRoom = rooms.oneXthree; }
            else if (cur_cell.Xsize == 1 && cur_cell.Ysize == 4) { newRoom = rooms.oneXfour; }
            else if (cur_cell.Xsize == 2 && cur_cell.Ysize == 2) { newRoom = rooms.twoXtwo; rotModifier = 90 * Random.Range(0, 5); }
            else if (cur_cell.Xsize == 2 && cur_cell.Ysize == 3) { newRoom = rooms.twoXthree; }
            else if (cur_cell.Xsize == 3 && cur_cell.Ysize == 3) { newRoom = rooms.threeXthree; rotModifier = 90 * Random.Range(0, 5); }

            else
            {
                if (cur_cell.Xsize == 2 && cur_cell.Ysize == 1) { newRoom = rooms.getOneTwo(); }
                else if (cur_cell.Xsize == 3 && cur_cell.Ysize == 1) { newRoom = rooms.oneXthree; }
                else if (cur_cell.Xsize == 4 && cur_cell.Ysize == 1) { newRoom = rooms.oneXfour; }
                else if (cur_cell.Xsize == 3 && cur_cell.Ysize == 2) { newRoom = rooms.twoXthree; }
                rotModifier = rotModifier + 90.0f;
            }
            PhysicalRooms.Add((GameObject)Instantiate(newRoom, pos, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, rotModifier, 0);

            //Debug.Log("Cell" + j + "Floor" + cur_floor.floorNumber);
            //Debug.Log("Loc" + cur_cell.XLoc + "," + cur_cell.YLoc + " Size" + cur_cell.Xsize + "," + cur_cell.Ysize);
            //Debug.Log("offset" + Xmodifier + "," + Zmodifier);
            populateSides(cur_cell, pos, cur_floor, rooms);
            //Debug.Log("\n");
        }
    }

    void populateSides(Cell cur_cell, Vector3 pos, Floor cur_floor, RoomController rooms)
    {
        float Zoffset = cur_cell.Ysize * 7.5f;
        float Xoffset = cur_cell.Xsize * 7.5f;

        Vector3 posLeft = new Vector3(pos.x - Xoffset + 2.5f, pos.y, pos.z);
        Vector3 posRight = new Vector3(pos.x + Xoffset - 2.5f, pos.y, pos.z);
        Vector3 posUp = new Vector3(pos.x, pos.y, pos.z + Zoffset - 2.5f);
        Vector3 posDown = new Vector3(pos.x, pos.y, pos.z - Zoffset + 2.5f);

        //cells initally face right
        PopulateSide(cur_cell.Leftsides, posLeft, 0, cur_floor, rooms, PhysicalSides);
        PopulateSide(cur_cell.Rightsides, posRight, 180, cur_floor, rooms, PhysicalSides);
        PopulateSide(cur_cell.Topsides, posUp, 90, cur_floor, rooms, PhysicalSides);
        PopulateSide(cur_cell.Bottomsides, posDown, 270, cur_floor, rooms, PhysicalSides);
    }

    void PopulateSide(List<Side> sides, Vector3 pos, int rot, Floor cur_floor, RoomController rooms, List<GameObject> PhysicalSides)
    {

        Vector3 loc = new Vector3();
        float offset = sides.Count * 7.5f * -1;
        offset = offset + 7.5f;
        for (int i = 0; i < sides.Count; i++)
        {
            GameObject newSide = rooms.getSide(sides[i].wallNumber);
            if (rot == 0 || rot == 180)
            {
                loc = new Vector3(pos.x, pos.y, pos.z + offset);
            }
            else
            {
                loc = new Vector3(pos.x + offset, pos.y, pos.z);
            }
            PhysicalSides.Add((GameObject)Instantiate(newSide, loc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalSides[PhysicalSides.Count - 1].transform.Rotate(-90, rot, 0);
            offset = offset + 15.0f;
            PhysicalSides[PhysicalSides.Count - 1].AddComponent<MeshCollider>();
        }
    }

    public void addStart()
    {
        RoomController rooms = tilesets[0];
        float Ypos = (Ysize * 15.0f) - 7.5f;

        Vector3 StartLoc = new Vector3(2.5f, -10.365f, Ypos);
        ///StartCell.Leftsides.wall
        GameObject startStair = rooms.getRandStairs();

        //place Stairs
        PhysicalRooms.Add((GameObject)GameObject.Instantiate(startStair, StartLoc, Quaternion.identity, GameObject.Find("floor" + "0").transform));
        PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 90, 0);
        PhysicalSides.Add((GameObject)GameObject.Instantiate(tilesets[0].BlockedDoor, StartLoc, Quaternion.identity, GameObject.Find("floor" + "0").transform));
        PhysicalSides[PhysicalSides.Count - 1].transform.Rotate(-90, 0, 0);
        PhysicalSides[PhysicalSides.Count - 1].AddComponent<MeshCollider>();



    }

    public void addGoal()
    {

        bool isLeft = (height % 2 != 0);
        Vector3 StartLoc = new Vector3();
        if (isLeft == true)
        {
            Cell Endcell = new Cell(Xsize, Ysize, 1, 1);
            float Xpos = ((Xsize + 3) * 15.0f) - 7.5f;
            StartLoc = new Vector3(Xpos, (height - 1) * 10.365f, 7.5f);
            //place Stairs
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(goal_Room, StartLoc, Quaternion.identity, GameObject.Find("floor" + (height - 1)).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 180, 0);
        }
        else
        {
            Cell StartCell = new Cell(-1, 0, 1, 1);
            StartLoc = new Vector3(22.5f, (height - 1) * 10.365f, 7.5f);
            ///StartCell.Leftsides.wall
            //place Stairs
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(goal_Room, StartLoc, Quaternion.identity, GameObject.Find("floor" + (height - 1)).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
        }



    }
    ///////STAIRS // 

    public void addStairs(Floor cur_floor)
    {
        //stairs are placed after exit @ 0,maxY and maxX,maxY
        bool placeleft = (cur_floor.floorNumber % 2 != 0);

        if (cur_floor.floorNumber == height - 1)
        {
            if (FillStairSpace == true && Ysize > 3)
            {
                addFillStairs(placeleft, cur_floor);
            }
            return;
        }
        else if (Ysize == 1)
        {
            addShortStairs(placeleft, cur_floor);
            //add additional spacer
        }
        else if (Ysize > 3)
        {
            addLongStairs(placeleft, cur_floor);
            if (FillStairSpace == true)
            {
                addFillStairs(placeleft, cur_floor);
            }
        }
        else
        {
            addShortStairs(placeleft, cur_floor);
        }
    }

    void addLongStairs(bool placeleft, Floor cur_floor)
    {
        RoomController rooms = tilesets[0];
        Vector3 StartLoc;
        if (placeleft == true) { StartLoc = new Vector3(22.5f, cur_floor.floorNumber * 10.365f, 7.5f); }
        else { StartLoc = new Vector3((Xsize * 15.0f) + 37.5f, cur_floor.floorNumber * 10.365f, 7.5f); }

        float Xpos = (Xsize * 15.0f);
        float Ypos = Ysize * 15.0f;


        int Spacerlen = Ysize - 4; //we cant get into this loop if the result of this is <0 so no need to error handle
        int VLongHall = 0; //if the building is > 7 long

        GameObject FirstCorner = rooms.getOneOne();
        GameObject startStair = rooms.getRandStairs();
        GameObject LastCorner = rooms.getOneOne();

        if (Spacerlen == 0) { LastCorner = rooms.getOneOne(); }
        else if (Spacerlen == 1) { LastCorner = rooms.getOneTwo(); }
        else if (Spacerlen == 2) { LastCorner = rooms.oneXthree; }
        else if (Spacerlen == 3) { LastCorner = rooms.oneXfour; }
        else { VLongHall = Spacerlen; Spacerlen = 3; LastCorner = rooms.oneXfour; }; //if its over 7 long place the last as a 4x1, and later on we will fill the gap.

        float ymod = Ypos - (Spacerlen * 7.5f) - 7.5f;



        //place first
        PhysicalRooms.Add((GameObject)GameObject.Instantiate(FirstCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
        PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);

        //place arrow
        Vector3 markerloc = StartLoc;
        int markerRot = 0;
        if (placeleft != true)
        {
            markerloc.x = markerloc.x - 10;
            markerRot = 180;
        }
        else
        {
            markerloc.x = markerloc.x + 10;
        }
        PhysicalRooms.Add((GameObject)GameObject.Instantiate(rooms.DoorMarker, markerloc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
        PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, markerRot, 0);


        //ABSTRACT, this level of code dupe is poor af.
        if (placeleft == true)
        {
            Cell FirstCornerCell = new Cell(1, 1, -3, 0);
            FirstCornerCell.fillSides();
            FirstCornerCell.Topsides[0].wallNumber = 0;
            FirstCornerCell.Rightsides[0].wallNumber = 0;
            populateSides(FirstCornerCell, StartLoc, cur_floor, rooms);

            //stairs
            StartLoc.z = 17.5f;
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(startStair, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);


            //spacers
            while (VLongHall > 3)
            {
                float VLongHallMod = ((VLongHall * 2) * 7.5f) - 7.5f;
                StartLoc = new Vector3(22.5f, (cur_floor.floorNumber + 1) * 10.365f, VLongHallMod);
                GameObject filler = rooms.getOneOne();
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(filler, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
                Cell FillerCell = new Cell(1, 1, -1, Ysize - 1);
                FillerCell.fillSides();
                FillerCell.Bottomsides[0].wallNumber = 0;
                FillerCell.Topsides[0].wallNumber = 0;
                populateSides(FillerCell, StartLoc, cur_floor, rooms);

                VLongHall--;
            }


            //endcap
            StartLoc = new Vector3(22.5f, (cur_floor.floorNumber + 1) * 10.365f, ymod);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(LastCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
            Cell lastCornerCell = new Cell(1, Spacerlen + 1, -1, Ysize - 1);
            lastCornerCell.fillSides();
            lastCornerCell.Bottomsides[0].wallNumber = 0;
            lastCornerCell.Rightsides[Spacerlen].wallNumber = 0;
            populateSides(lastCornerCell, StartLoc, cur_floor, rooms);



        }
        else
        {
            Cell FirstCornerCell = new Cell(1, 1, Xsize, 0);
            FirstCornerCell.fillSides();
            FirstCornerCell.Topsides[0].wallNumber = 0;
            FirstCornerCell.Leftsides[0].wallNumber = 0;
            populateSides(FirstCornerCell, StartLoc, cur_floor, rooms);

            //stairs
            StartLoc.z = 17.5f;
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(startStair, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);

            //spacers
            while (VLongHall > 3)
            {
                float VLongHallMod = ((VLongHall * 2) * 7.5f) - 7.5f;
                StartLoc = new Vector3(Xpos + 37.5f, (cur_floor.floorNumber + 1) * 10.365f, VLongHallMod);
                GameObject filler = rooms.getOneOne();
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(filler, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
                Cell FillerCell = new Cell(1, 1, -1, Ysize - 1);
                FillerCell.fillSides();
                FillerCell.Bottomsides[0].wallNumber = 0;
                FillerCell.Topsides[0].wallNumber = 0;
                populateSides(FillerCell, StartLoc, cur_floor, rooms);

                VLongHall--;
            }
            ////endcap

            StartLoc = new Vector3(Xpos + 37.5f, (cur_floor.floorNumber + 1) * 10.365f, ymod);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(LastCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
            Cell lastCornerCell = new Cell(1, Spacerlen + 1, -1, Ysize - 1);
            lastCornerCell.fillSides();
            lastCornerCell.Bottomsides[0].wallNumber = 0;
            lastCornerCell.Leftsides[Spacerlen].wallNumber = 0;
            populateSides(lastCornerCell, StartLoc, cur_floor, rooms);
        }
    }

    void addShortStairs(bool placeleft, Floor cur_floor)
    {
        RoomController rooms = tilesets[0];

        Vector3 StartLoc;
        if (placeleft == true) { StartLoc = new Vector3(27.5f, cur_floor.floorNumber * 10.365f, 7.5f); }
        else { StartLoc = new Vector3((Xsize * 15.0f) + 32.5f, cur_floor.floorNumber * 10.365f, 7.5f); }

        float Xpos = (Xsize * 15.0f);
        float Ypos = Ysize * 15.0f;

        GameObject startStair = rooms.getRandStairs();
        GameObject FirstCorner = rooms.getOneOne();
        GameObject LastStraight = rooms.oneXthree;

        //place arrow
        Vector3 markerloc = StartLoc;
        int markerRot = 0;
        if (placeleft != true)
        {
            markerloc.x = markerloc.x - 5;
            markerRot = 180;
        }
        else
        {
            markerloc.x = markerloc.x + 5;
        }
        PhysicalRooms.Add((GameObject)GameObject.Instantiate(rooms.DoorMarker, markerloc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
        PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, markerRot, 0);


        //place Stairs
        PhysicalRooms.Add((GameObject)GameObject.Instantiate(startStair, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));

        //ABSTRACT, this level of code dupe is poor af.
        if (placeleft == true)
        {

            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, -90, 0);

            Cell FirstCornerCell = new Cell(1, 1, -3, 0);
            Cell LastCell = new Cell(3, 1, -3, Ysize - 1);


            //place corner
            StartLoc = new Vector3(-7.5f, StartLoc.y + 10.365f, StartLoc.z);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(FirstCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 90, 0);
            FirstCornerCell.fillSides();
            FirstCornerCell.Topsides[0].wallNumber = 1;
            FirstCornerCell.Rightsides[0].wallNumber = 0;
            populateSides(FirstCornerCell, StartLoc, cur_floor, rooms);
            // add spacers
            if (Ysize == 3)
            {
                Cell SpacerCell = new Cell(1, 1, -3, 1);
                GameObject Spacer = rooms.getOneOne();

                StartLoc = new Vector3(-7.5f, StartLoc.y, 22.5f);
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(Spacer, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
                SpacerCell.fillSides();
                SpacerCell.Topsides[0].wallNumber = 1;
                SpacerCell.Bottomsides[0].wallNumber = 1;
                populateSides(SpacerCell, StartLoc, cur_floor, rooms);

            }
            //loop back
            StartLoc = new Vector3(7.5f, StartLoc.y, Ypos - 7.5f);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(LastStraight, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 90, 0);
            LastCell.fillSides();
            LastCell.Bottomsides[0].wallNumber = 1;
            LastCell.Rightsides[0].wallNumber = 0;
            populateSides(LastCell, StartLoc, cur_floor, rooms);
        }
        else
        {
            Cell FirstCornerCell = new Cell(1, 1, Xsize + 2, 0);
            Cell LastCell = new Cell(3, 1, -3, Ysize - 1);

            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 90, 0);
            //place corner
            StartLoc = new Vector3(Xpos + 67.5f, StartLoc.y + 10.365f, StartLoc.z);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(FirstCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
            FirstCornerCell.fillSides();
            FirstCornerCell.Topsides[0].wallNumber = 1;
            FirstCornerCell.Leftsides[0].wallNumber = 0;
            populateSides(FirstCornerCell, StartLoc, cur_floor, rooms);

            //add spacers
            if (Ysize == 3)
            {
                Cell SpacerCell = new Cell(1, 1, -3, 1);
                GameObject Spacer = rooms.getOneOne();

                StartLoc = new Vector3(Xpos + 67.5f, StartLoc.y, 22.5f);
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(Spacer, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
                SpacerCell.fillSides();
                SpacerCell.Topsides[0].wallNumber = 1;
                SpacerCell.Bottomsides[0].wallNumber = 1;
                populateSides(SpacerCell, StartLoc, cur_floor, rooms);

            }
            //loop back
            StartLoc = new Vector3(Xpos + 52.5f, StartLoc.y, Ypos - 7.5f);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(LastStraight, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 90, 0);
            LastCell.fillSides();
            LastCell.Bottomsides[LastCell.Bottomsides.Count - 1].wallNumber = 1;
            LastCell.Leftsides[0].wallNumber = 0;
            populateSides(LastCell, StartLoc, cur_floor, rooms);
        }
    }

    void addFillStairs(bool placeleft, Floor cur_floor)
    {
        RoomController rooms = tilesets[0];
        Vector3 StartLoc;
        if (placeleft != true) { StartLoc = new Vector3(22.5f, (cur_floor.floorNumber) * 10.365f, 7.5f); }
        else { StartLoc = new Vector3((Xsize * 15.0f) + 37.5f, (cur_floor.floorNumber) * 10.365f, 7.5f); }

        float Xpos = Xsize * 15.0f;
        float Ypos = Ysize * 15.0f;

        bool vlong = false;

        int Spacerlen = Ysize - 4; //we cant get into this loop if the result of this is <0 so no need to error handle
        int VLongHall = 0; //if the building is > 7 long

        GameObject FirstCorner = rooms.getOneOne();
        GameObject startStair = rooms.getRandStairs();
        GameObject LastCorner = rooms.getOneOne();

        if (Spacerlen == 0) { LastCorner = rooms.getOneOne(); }
        else if (Spacerlen == 1) { LastCorner = rooms.getOneTwo(); }
        else if (Spacerlen == 2) { LastCorner = rooms.oneXthree; }
        else if (Spacerlen == 3) { LastCorner = rooms.oneXfour; }
        else { VLongHall = Spacerlen; Spacerlen = 3; LastCorner = rooms.oneXfour; vlong = true; }; //if its over 7 long place the last as a 4x1, and later on we will fill the gap.

        float ymod = Ypos - (Spacerlen * 7.5f) - 7.5f;


        //place first
        PhysicalRooms.Add((GameObject)GameObject.Instantiate(FirstCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
        PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
        /*
        //place arrow
        Vector3 markerloc = StartLoc;
        int markerRot = 0;
        if (placeleft != true)
        {
            markerloc.x = markerloc.x - 10;
            markerRot = 180;
        }
        else
        {
            markerloc.x = markerloc.x + 10;
        }
        PhysicalRooms.Add((GameObject)GameObject.Instantiate(rooms.DoorMarker, markerloc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
        PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, markerRot, 0);
        */
        //ABSTRACT, this level of code dupe is poor af.
        if (placeleft == true)
        {
            Cell FirstCornerCell = new Cell(1, 1, -3, 0);
            FirstCornerCell.fillSides();
            FirstCornerCell.Leftsides[0].wallNumber = 0;
            populateSides(FirstCornerCell, StartLoc, cur_floor, rooms);

            //spacers
            while (VLongHall > 3)
            {
                float VLongHallMod = ((VLongHall * 2) * 7.5f) - 7.5f;
                StartLoc = new Vector3(22.5f, (cur_floor.floorNumber) * 10.365f, VLongHallMod);
                GameObject filler = rooms.getOneOne();
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(filler, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
                Cell FillerCell = new Cell(1, 1, -1, Ysize - 1);
                FillerCell.fillSides();
                if (VLongHall != 4) { FillerCell.Bottomsides[0].wallNumber = 1; }
                FillerCell.Topsides[0].wallNumber = 1;
                populateSides(FillerCell, StartLoc, cur_floor, rooms);

                VLongHall--;
            }


            //endcap
            StartLoc = new Vector3(22.5f, (cur_floor.floorNumber) * 10.365f, ymod);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(LastCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
            Cell lastCornerCell = new Cell(1, Spacerlen + 1, -1, Ysize - 1);
            lastCornerCell.fillSides();
            lastCornerCell.Rightsides[Spacerlen].wallNumber = 0;
            if (vlong == true) { lastCornerCell.Bottomsides[0].wallNumber = 1; }
            populateSides(lastCornerCell, StartLoc, cur_floor, rooms);



        }
        else
        {
            Cell FirstCornerCell = new Cell(1, 1, Xsize, 0);
            FirstCornerCell.fillSides();
            FirstCornerCell.Rightsides[0].wallNumber = 0;
            populateSides(FirstCornerCell, StartLoc, cur_floor, rooms);

            //spacers
            while (VLongHall > 3)
            {
                float VLongHallMod = ((VLongHall * 2) * 7.5f) - 7.5f;
                StartLoc = new Vector3(Xpos + 37.5f, (cur_floor.floorNumber) * 10.365f, VLongHallMod);
                GameObject filler = rooms.getOneOne();
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(filler, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
                Cell FillerCell = new Cell(1, 1, -1, Ysize - 1);
                FillerCell.fillSides();
                FillerCell.Topsides[0].wallNumber = 1;
                if (VLongHall != 4) { FillerCell.Bottomsides[0].wallNumber = 1; }
                populateSides(FillerCell, StartLoc, cur_floor, rooms);

                VLongHall--;
            }
            ////endcap

            StartLoc = new Vector3(Xpos + 37.5f, (cur_floor.floorNumber) * 10.365f, ymod);
            PhysicalRooms.Add((GameObject)GameObject.Instantiate(LastCorner, StartLoc, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
            PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(-90, 0, 0);
            Cell lastCornerCell = new Cell(1, Spacerlen + 1, -1, Ysize - 1);
            lastCornerCell.fillSides();
            lastCornerCell.Leftsides[Spacerlen].wallNumber = 0;
            if (vlong == true) { lastCornerCell.Bottomsides[0].wallNumber = 1; }
            populateSides(lastCornerCell, StartLoc, cur_floor, rooms);
        }
    }
}


    /*
    public void addWalls(Floor cur_floor)
    {
        bool InitFloor = false;

        if (Ysize > 3)
        {

            if (cur_floor.floorNumber == 0) { InitFloor = true; }
            float Ypos = (Ysize * 15.0f) - 7.5f;
            float mod = 2.0f;
            int Xmod = 2;
            float Xpos = (Xsize * 15.0f) - 7.5f;

            for (int i = 0; i < Xsize + Xmod; i++)
            {

                Vector3 startlocLow = new Vector3((i * 15) + (mod * 11.5f) - 0.5f, (cur_floor.floorNumber * 10.365f) - 10.365f, 0);
                Vector3 startlocHigh = new Vector3((i * 15) + (mod * 11.5f) - 0.5f, (cur_floor.floorNumber * 10.365f) - 10.365f, Ypos + 7.5f);
                GameObject wall = tilesets[0].OuterWall;
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocLow, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 0, 0);
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocHigh, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 180, 0);

            }
            for (int i = 0; i < Ysize; i++)
            {
                Vector3 startlocLow = new Vector3(mod * 7.5f, (cur_floor.floorNumber * 10.365f) - 10.365f, (i * 15) + 7.5f);
                Vector3 startlocHigh = new Vector3(Xpos + ((mod + 5) * 7.5f), (cur_floor.floorNumber * 10.365f) - 10.365f, (i * 15) + 7.5f);
                GameObject wall = tilesets[0].OuterWall;

                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocHigh, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 270, 0);
                if (InitFloor == true && i == Ysize - 1 && Ysize > 3) { continue; }
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocLow, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 90, 0);
            }
        }
        else
        {
            float Ypos = (Ysize * 15.0f) - 7.5f;
            float mod = 2.0f;
            int Xmod = 6;
            float Xpos = -15.0f;

            for (int i = 0; i < Xsize + Xmod; i++)
            {

                Vector3 startlocLow = new Vector3((i * 15) - 7.5f, (cur_floor.floorNumber * 10.365f) - 10.365f, 0);
                Vector3 startlocHigh = new Vector3((i * 15) - 7.5f, (cur_floor.floorNumber * 10.365f) - 10.365f, Ypos + 7.5f);
                GameObject wall = tilesets[0].OuterWall;
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocLow, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 0, 0);
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocHigh, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 180, 0);

            }
            for (int i = 0; i < Ysize; i++)
            {
                Vector3 startlocLow = new Vector3(Xpos , (cur_floor.floorNumber * 10.365f) - 10.365f, (i * 15) + 7.5f);
                Vector3 startlocHigh = new Vector3(Xpos + ((Xsize  + 6)* 15), (cur_floor.floorNumber * 10.365f) - 10.365f, (i * 15) + 7.5f);
                GameObject wall = tilesets[0].OuterWall;
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocHigh, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 270, 0);
                PhysicalRooms.Add((GameObject)GameObject.Instantiate(wall, startlocLow, Quaternion.identity, GameObject.Find("floor" + cur_floor.floorNumber).transform));
                PhysicalRooms[PhysicalRooms.Count - 1].transform.Rotate(90, 90, 0);
            }
        }
    }
    */


