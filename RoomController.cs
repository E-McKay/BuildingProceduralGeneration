using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour{

	public GameObject oneXoneA;
    public GameObject oneXoneB;

    public GameObject oneXtwoA;
    public GameObject oneXtwoB;

    public GameObject oneXthree;
    public GameObject oneXfour;

    public GameObject twoXtwo;
    public GameObject twoXthree;
    public GameObject threeXthree;

    public GameObject sideOne;
    public GameObject sideTwo;
    public GameObject sideThree;
    public GameObject sideFour;
    public GameObject sideFive;
    public GameObject sideSix;
    public GameObject sideSeven;
    public GameObject sideEight;
    public GameObject sideNine;
    public GameObject sideTen;

    public GameObject openDoor;
    public GameObject openRoom;

    public GameObject stairOne;
    public GameObject stairTwo;

    public GameObject window;
    public GameObject BlockedDoor;

    public GameObject DoorMarker;

    public GameObject OuterWall;

    System.Random rnd = new System.Random();

    //make lists of sides
     List<GameObject> oneOne = new List<GameObject>();
     List<GameObject> oneTwo = new List<GameObject>();
     List<GameObject> sides = new List<GameObject>();
     List<GameObject> stairs = new List<GameObject>();

    void Start()
    {
       
    }

    public void StartLists()
    {
        //add objects to sides
        oneOne.Add(oneXoneA);
        oneOne.Add(oneXoneB);

        oneTwo.Add(oneXtwoA);
        oneTwo.Add(oneXtwoB);

        stairs.Add(stairOne);
        stairs.Add(stairTwo);

        sides.Add(openDoor);
        sides.Add(openRoom);
        sides.Add(window);
        sides.Add(BlockedDoor);
        sides.Add(sideOne);
        sides.Add(sideTwo);
        sides.Add(sideThree);
        sides.Add(sideFour);
        sides.Add(sideFive);
        sides.Add(sideSix);
        sides.Add(sideEight);
        sides.Add(sideNine);
        sides.Add(sideTen);
    }

    public GameObject getOneOne()
    {
        int selector = rnd.Next(oneOne.Count);
        //  selector = selector - 1;
        return oneOne[selector];
    }

    public GameObject getOneTwo()
    {
        int selector = rnd.Next(oneTwo.Count);
      //  selector = selector - 1;
        return oneTwo[selector];
    }

    public GameObject getRandSide()
    {
        int selector = Random.Range(2, sides.Count); //- all the possible sizes not including 0 and 1 which are doors.
        return sides[selector];
    }

    public GameObject getRandStairs()
    {
        int selector = Random.Range(0, stairs.Count);
        return stairs[selector];
    }


    public GameObject getSide(int sidenum)
    {
        
        return sides[sidenum];
    }


}
