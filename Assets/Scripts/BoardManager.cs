using System.Collections.Generic; //we use lists
using System;
using UnityEngine;
using Random = UnityEngine.Random;
//This script generates randomly generated levels for us each time we play. "procedural"
public class BoardManager : MonoBehaviour {

    [Serializable] //Serialization is the process where a given object is converted into a different format about how the object is represented I.e. store the contents of an object to a file
    public class Count //Serialization allows the developer to save the state of an object and recreate it as needed, providing storage of objects as well as data exchange. 
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8; // 8 by 8 GAME BOARD
    public Count wallCount = new Count(5, 9); //Between 5 and 9 walls per level
    public Count foodCount = new Count(1, 5); // Same here
    public GameObject exit;
    //We are going to fill each of these arrays with our different prefabs to choose between in the inspector
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder; //It's something that we are going to use to keep the hierarchy clean because we're going to spawn a lot of game objects
    private List <Vector3> gridPositions = new List<Vector3>(); //keep the track of whether an object has been spawned in that position or not

    void InitialiseList()
    {
        gridPositions.Clear();
        //Runs the for inside the game board like 1,2 or 2,6
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f)); //We are creating a list of possible positions to place Walls, Enemies or Pickups
            }  // 1,1 1,2 1,3 1,4 1,5 1,6 2,1 2,2 2,3...
        }
    }
    
    //this setups the outer walls and the floor of our game board
    void BoardSetup () 
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++) // The reason for -1 and +1 is because the visual game starts in -1,-1 and finishes in 8,8 with floor and outer walls
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; //   We are choosing randomly a floor prefab
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)]; // We are going to instantiate an outer wall where the game should put it (-1,-1  -1,0  -1,1 etc)

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //Quaternion.identity means that will have no rotations
                //Instantiate clones the object original and returns the clone.
                instance.transform.SetParent(boardHolder); //Set up the parent of our newly instantiated game object to boardHolder
            }
        }
    }
//place the random objects on the game board
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count); //the number of positions stored in our gridPositions list which we're accessing using gridPositions.Count // from 0,0 to 6,6 game board 
        Vector3 randomPosition = gridPositions[randomIndex]; //We pick up a random position from random Index
        gridPositions.RemoveAt(randomIndex); // We make sure that we don't spawn two objects in the same location
        return randomPosition; //returning randomPosition so we actually use this random position to spawn our object in a random location
    }
    //spawn our tiles at the random position that we've chosen
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1); //Controls how many of a given object we're going to spawn, for example the number of walls in a level

        for (int i = 0; i < objectCount; i++) //spawning the number specified by objectCount
        {
            Vector3 randomPosition = RandomPosition(); //gives random positions until the object Count is over which is random too
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)]; //We are going to choose a random tile from our array of game objects tileArray to spawn
            Instantiate(tileChoice, randomPosition, Quaternion.identity); 
        }
    }
    //Start the board
    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f); //Generate a number of enemies based on the level number( log2 1 = 0 log2 2 = 1 enemy log2 4 = 2 enemies)
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);//Generates a copy that puts the exit always on the top right

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
