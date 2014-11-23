using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public Camera cam;

	public MapManager mapManager;

	public int playerVisionRange = 4;
	public GameObject userPlayerPrefab;
	public GameObject AIPlayerPrefab;
	
	private List<Player> players = new List<Player>();

	public static List<Entity> objects = new List<Entity> ();

	public static Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	public bool takingTurn = false;

	//public static keyPressed = false;


	// Use this for initialization
	void Start ()
	{
		mapManager.generateMap();
		mapManager.renderMap();
		generatePlayers();
		mapManager.FOV (objects[0].gridPosition, playerVisionRange);
		cam.GetComponent<CameraController>().LookAtPlayer();
	}

	void OnGUI ()
	{
		GUI.Label (new Rect (20, 20, 200, 40), "Number of rooms : "+MapManager.rooms.Count);
	}
	
	private void generatePlayers ()
	{

		//Hero h = new Hero(playerStartPosition, userPlayerPrefab, "hero");
		Hero h;
		h = ((GameObject)Instantiate(userPlayerPrefab, playerStartPosition, Quaternion.identity)).GetComponent<Hero>();
		h.gridPosition = playerStartPosition;
		h.name = "hero";
		h.blocks = true;
		objects.Add(h);
		MapManager.map[(int)playerStartPosition.x][(int)playerStartPosition.y].addEntity(h);

		Enemy compPlayer;
		for (int nr = 0; nr < MapManager.rooms.Count ; nr++)
		{
			Rectangle room = MapManager.rooms[nr];
			Vector2 pos = new Vector2 (UnityEngine.Random.Range (room.x1, room.x2),
			                   UnityEngine.Random.Range (room.y1, room.y2));
			compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, pos, Quaternion.identity)).GetComponent<Enemy>();
			compPlayer.gridPosition = pos;
			compPlayer.name = "Troll #"+nr;
			compPlayer.blocks = true;
			objects.Add (compPlayer);
			MapManager.map[(int)pos.x][(int)pos.y].addEntity(compPlayer);
		}
	}


	// Update is called once per frame
	void Update ()
	{
		if (!objects[0].isMoving)
		{
			if (Input.GetButtonDown("up")){
				objects[0].move(Vector2.up);
				takingTurn = true;
			}
			else if (Input.GetButtonDown("down"))
			{
				objects[0].move(-Vector2.up);
				takingTurn = true;
			}
			else if(Input.GetButtonDown("left"))
			{
				objects[0].move(-Vector2.right);
				takingTurn = true;
			}
			else if (Input.GetButtonDown("right"))
			{
				objects[0].move(Vector2.right);
				takingTurn = true;
			}

			mapManager.FOV (objects[0].gridPosition, playerVisionRange);

//			if (takingTurn)
//			{
//				for (int i = 1; i < objects.Count ; i++)
//				{
//					objects[i].takeTurn();
//				}
//				takingTurn = false;
//			}
		}

	}

	DIRECTION checkForInput ()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
			return DIRECTION.UP;
		else if (Input.GetKeyDown(KeyCode.DownArrow))
			return DIRECTION.DOWN;
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
			return DIRECTION.LEFT;
		else if (Input.GetKeyDown(KeyCode.RightArrow))
			return DIRECTION.RIGHT;
		else
			return DIRECTION.NONE;
	}
}
