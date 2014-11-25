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

	public static List<Entity> objects = new List<Entity> ();

	public static Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	public bool turnTaken = false;

	//public static keyPressed = false;


	// Use this for initialization
	void Start ()
	{
		mapManager.generateMap();
		generatePlayers();
		mapManager.renderMap();
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
		h.fighterComponent = new Fighter (hp:30, defense:2, power:5);
		objects.Add(h);
		//MapManager.map[(int)playerStartPosition.x][(int)playerStartPosition.y].addEntity(h);

		Enemy compPlayer;
		for (int nr = 0; nr < MapManager.rooms.Count ; nr+=2)
		{
			Rectangle room = MapManager.rooms[nr];
			Vector2 pos;

			do
			{
				pos = new Vector2 (UnityEngine.Random.Range (room.x1, room.x2),
			    			               UnityEngine.Random.Range (room.y1, room.y2));
			}while(pos.x == h.gridPosition.x && pos.y == h.gridPosition.y);

			compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, pos, Quaternion.identity)).GetComponent<Enemy>();
			compPlayer.gridPosition = pos;
			compPlayer.name = "Troll #"+nr;
			compPlayer.blocks = true;
			compPlayer.fighterComponent = new Fighter(hp:10, defense:0, power:3);
			compPlayer.ai = new BasicEnemy();
			objects.Add (compPlayer);
		}
	}


	// Update is called once per frame
	void Update ()
	{
		string info = null;

		// check if there are any player moving
		bool turnFinished = true;
		for (int i = 1; i < objects.Count ; i++)
		{
			if (objects[i].isMoving)
			{
				turnFinished = false;
				break;
			}
		}

		// the user player has taken his turn and has stopped moving
		// now the rest of the players take their turns
		if (!objects [0].isMoving && turnTaken)
		{
			for (int i = 1; i < objects.Count ; i++)
			{
				if (objects[i].GetComponent<SpriteRenderer>().enabled)
				{
					info = objects[i].ai.takeTurn(objects[i]);
					if (info != null)
						Debug.Log(info);
				}
			}
		}

		// if all the other player has taken their turns, the user player can move
		if (turnFinished && !objects[0].isMoving)
		{
			turnTaken = true;
			if (Input.GetButtonDown("up")){
				info = ((Hero)objects[0]).moveOrAttack(0, 1);
			}
			else if (Input.GetButtonDown("down"))
			{
				info = ((Hero)objects[0]).moveOrAttack(0, -1);
			}
			else if(Input.GetButtonDown("left"))
			{
				info = ((Hero)objects[0]).moveOrAttack(-1, 0);
			}
			else if (Input.GetButtonDown("right"))
			{
				info = ((Hero)objects[0]).moveOrAttack(1, 0);
			}
			else
			{
				turnTaken = false;
			}

			if (info != null)
				Debug.Log(info);

//			// the player has moved
//			if (turnTaken)
//			{
//				for (int i = 1; i < objects.Count ; i++)
//				{
//					if (objects[i].GetComponent<SpriteRenderer>().enabled)
//					{
//						info = objects[i].ai.takeTurn(objects[i]);
//						if (info != null)
//							Debug.Log(info);
//					}
//				}
//			}

			mapManager.FOV (objects[0].gridPosition, playerVisionRange);
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
