using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MiniMapGenerator : MonoBehaviour {

    

    public Vector2 gridWorldSize;
    //public float nodeRadius;
    public LayerMask unwalkableMask;
    MiniMapNode[,] Grid;
    //public int height;
    //public Vector2 minimapResolution;
    public GameObject targetImage;
    public GameObject playerImg;

    public bool OutputToFile;
    public bool enableMinimap;

    public Transform Player;
    // public Transform target;
    public int height;
    float nodeSize = 0.5f;
    int gridSizeX;
    int gridSizeY;
    int currentfloor = 0;
    float floorheight = 10.3615f;
    int modifier;
    List<Sprite> minimaps = new List<Sprite>();
    Vector3 playerPos = new Vector3();

    private int cur_floor_num = 0;

    float gridXscale = 1.0f; //diffrence in X size between image and floor
    float gridYscale = 1.0f; //difrrence in Ysize between image and floor
    float Xoffset = 0.0f;
    float Yoffset = 0.0f;
    float Xmodifier = 1.0f;
    float Ymodifier = 1.0f;

    void Start()
    {

        if (enableMinimap == false)
        {
            targetImage.SetActive(false);
            playerImg.SetActive(false);

        }
        else
        {
            //set minimap size
            //targetImage.GetComponent<Image>().

            //set modifiers for multiple sizes of minimap
            if (gridWorldSize.y < 4) { modifier = 5; } else { modifier = 1; }
            gridSizeX = Mathf.RoundToInt(((gridWorldSize.x + modifier + 1) * 15) / nodeSize);
            gridSizeY = Mathf.RoundToInt(((gridWorldSize.y) * 15) / nodeSize);
            if (modifier == 1) { modifier = -5; }

            //empty start
            Grid = null;
            createGrid(-1 * 10.365f - 7.0f);
            GenerateMinimaps();
            //generate minimaps at launch
            for (int i = 0; i < height; i++)
            {
                Grid = null;
                createGrid(i * 10.365f - 7.0f);
                cur_floor_num++;
                GenerateMinimaps();
            }
            //Debug.Log("made:" + minimaps.Count + "minimaps");

            //unrender floors, this isnt in the layout as having it here Guarantees proper minimap creation
            for (int i = 1; i < height; i++)
            {
                //Debug.Log("FLOOR" + i);
                GameObject.Find("floor" + i).SetActive(false);
            }

            //Minimap (unscaled)
            gridXscale = gridSizeX / (((float)gridWorldSize.x + 2.0f) * 15.0f);
            if (modifier == 5)
            {
                gridXscale = gridSizeX / (((float)gridWorldSize.x + 6.0f) * 15.0f);
            }
            gridYscale = gridSizeY / (((float)gridWorldSize.y + 0.0f) * 15.0f); // not working as it expects a square, NOT a rectangle.
                                                                                //scale minimap container
                                                                                //container.transform.localScale = new Vector3(0.8f,1,1);

            if (gridSizeX > gridSizeY)
            {
                //set the modifier to the * we need to times it by to make it less far
                Ymodifier = (float)gridSizeY / (float)gridSizeX;

                //set the offset to the value up/right it goes
                float newYsize = (200.0f / (float)gridSizeX) * (float)gridSizeY; // get the value where size * ? = 200 (the minimap is 200*200) 
                Yoffset = (200 - newYsize) / 2.0f;

            }
            else if (gridSizeY > gridSizeX)
            {
                //set the modifier to the * we need to times it by to make it less far
                Xmodifier = (float)gridSizeX / (float)gridSizeY;

                //set the offset to the value up/right it goes
                float newXsize = (200.0f / (float)gridSizeY) * (float)gridSizeX; // get the value where size * ? = 200 (the minimap is 200*200) 
                Xoffset = (200 - newXsize) / 2.0f;

            }

        }

    }





    void Update()
    {
        if(enableMinimap == true)
        {
            //% of actual level
            float playerXpercent;
            if (gridWorldSize.y < 4)
            {
                playerXpercent = (((Player.transform.position.x + 15.0f) / ((gridWorldSize.x + 6) * 15.0f)) * 100);
            }
            else
            {
                playerXpercent = (((Player.transform.position.x - 15.0f) / ((gridWorldSize.x + 2) * 15.0f)) * 100);
            }

            //playerXpercent = playerXpercent - 15.0f; 
            float playerYpercent = (((Player.transform.position.z) / ((gridWorldSize.y) * 15.0f)) * 100);
            float playerLocX = (playerXpercent * gridXscale);
            float playerLocY = (playerYpercent * gridYscale);
            playerLocX = (playerLocX * Xmodifier) + Xoffset + 20.0f;
            playerLocY = (playerLocY * Ymodifier) + Yoffset + 20.0f;
            //if X + 2 != y move Y up by (x+2 / 2)

            playerPos = new Vector3(playerLocX, playerLocY, -100.0f);
            playerImg.transform.position = playerPos;
            float rot = Player.rotation.eulerAngles.y;
            playerImg.transform.eulerAngles = new Vector3(0, 0,-rot );
            //Debug.Log("PlayerPercent:" + +playerXpercent + "," + playerYpercent + "mapsize:" + gridSizeX + gridSizeY + "gridscale" + gridXscale + "," + gridYscale + "LOC:" + playerLocX + playerLocY);

            //Debug.Log("size" + gridSizeX + "," + gridSizeY + "Modifiers" + Xmodifier + "," + Ymodifier + "OFFSETS" + Xoffset + "." + Yoffset);

            //set player loc in grid.
            float PlayerHeight = Player.position.y;
            int floor = Mathf.RoundToInt((PlayerHeight / floorheight) + 1);

            if (currentfloor != floor)
            {
                currentfloor = floor;
                if(currentfloor > height)
                {
                    currentfloor = height;
                }
                //minimap
                targetImage.GetComponent<Image>().sprite = minimaps[currentfloor + 1];
            }
        }  
    }

   

    void GenerateMinimaps()
    {
        //Texture2D minimap = new Texture2D(Mathf.RoundToInt(minimapResolution.x),Mathf.RoundToInt(minimapResolution.y),TextureFormat.ARGB32,false);
        Texture2D minimap = new Texture2D(gridSizeX,gridSizeY,TextureFormat.ARGB32,false);
        foreach(MiniMapNode n in Grid)
        {
            if (!n.walkable)
            {
                minimap.SetPixel(n.gridX, n.gridY, Color.red);
            }
            
        }
        minimap.Apply();
        minimaps.Add(Sprite.Create(minimap, new Rect(0, 0, gridSizeX, gridSizeY), new Vector2(0.5f, 0.5f)));

        if(OutputToFile == true)
        {
            string filename = "LAYOUT_" + gridSizeX + "x" + gridSizeY + "_" + cur_floor_num + ".png";
            // Encode texture into PNG
            byte[] bytes = minimap.EncodeToPNG();
            //Object.Destroy(minimap);

            // For testing purposes, also write to a file in the project folder
            File.WriteAllBytes(Application.dataPath + "/../"+ filename + ".png", bytes);
        }
        
    }

    void createGrid(float height)
    {
        Grid = new MiniMapNode[gridSizeX, gridSizeY];
        //Vector3 WorldBottomLeft = new Vector3(0,height,0) - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y/2;
        //WorldBottomLeft = new Vector3(WorldBottomLeft.x - (modifier * 3), WorldBottomLeft.y, WorldBottomLeft.z);
        Vector3 WorldBottomLeft = new Vector3(15.0f, height, 0.0f);
        if(modifier == 5)
        {
            WorldBottomLeft = new Vector3(-15.0f, height, 0.0f);
        }
        for (int x = 0 ; x < gridSizeX; x++)
        {

            for (int y = 0; y < gridSizeY; y++)
            {   //get each point for a node
                Vector3 worldPoint = WorldBottomLeft + Vector3.right * (x * nodeSize + (nodeSize/2)) + Vector3.forward * (y * nodeSize + (nodeSize / 2));
                bool walkable = false;
                if (!Physics.CheckSphere(worldPoint, nodeSize, unwalkableMask))
                {
                    walkable = true;
                }
                Grid[x, y] = new MiniMapNode(walkable, worldPoint, x,y);
            }
        }
    }


    public List<MiniMapNode> getNeighbours(MiniMapNode cur)
    {
        //get every node around the current one.
        //no diagonals, so its only 4 ops.
        List<MiniMapNode> neighbours = new List<MiniMapNode>();
        int curY1 = cur.gridY + 1;
        int curX1 = cur.gridX + 1;
        int curXM1 = cur.gridX - 1;
        int curYM1 = cur.gridY - 1;

        neighbours.Add(Grid[curX1, cur.gridY]);
        neighbours.Add(Grid[cur.gridX, curY1]);
        neighbours.Add(Grid[cur.gridX, curYM1]);
        neighbours.Add(Grid[curXM1, cur.gridY]);
        return neighbours;
    }

    public MiniMapNode NodefromPoint(Vector3 worldpos)
    {
        float percentX = (worldpos.x + gridWorldSize.x / 2) / (gridWorldSize.x);
        float percentY = (worldpos.z + gridWorldSize.y / 2) / (gridWorldSize.y);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return Grid[x,y];

    }

}
