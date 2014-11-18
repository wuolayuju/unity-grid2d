using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	public MapManager mapManager;

	public int playerVisionRange = 4;
	public GameObject userPlayerPrefab;
	public GameObject AIPlayerPrefab;
	
	private List<Player> players = new List<Player>();

	public static Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	// Use this for initialization
	void Start ()
	{
		mapManager.generateMap();
		mapManager.renderMap();
		generatePlayers();
		FOV ();
	}

	void OnGUI ()
	{
		GUI.Label (new Rect (20, 20, 200, 40), "Number of rooms : "+MapManager.rooms.Count);
	}

	void FOV()
	{
		for (int r = 0; r < mapManager.mapWidth ; r++)
		{
			for (int c = 0; c < mapManager.mapHeight ; c++)
			{
				Tile t = MapManager.map[r][c];
				if (t.isLit)
					t.isLit = false;
				if (t.isExplored && t.isVisible){
					t.markTileAsExplored();
				}
			}
		}
		for (int i=0; i<360; i+=1)
		{
			float x = Mathf.Cos((float)i*0.01745f);
			float y = Mathf.Sin((float)i*0.01745f);
			DoFOV(x,y);
		}
	}

	void DoFOV(float x, float y)
	{
		float ox,oy;
		ox = (float)players[0].gridPosition.x+0.5f;
		oy = (float)players[0].gridPosition.y+0.5f;
		for(int i=0;i<playerVisionRange;i++)
		{
			MapManager.map[(int)ox][(int)oy].markTileAsLit();
			if(MapManager.map[(int)ox][(int)oy].blocksLight==true)
				return;
			ox+=x;
			oy+=y;
		}
	}
	
	private void generatePlayers ()
	{
		Vector2 pos = playerStartPosition;

		UserPlayer humanPlayer;
		humanPlayer = GameObject.Find ("userPlayer").GetComponent<UserPlayer>();
		humanPlayer.transform.position = pos;
		humanPlayer.gridPosition = pos;
		players.Add (humanPlayer);

		AIPlayer compPlayer;
		for (int nr = 0; nr < MapManager.rooms.Count ; nr++)
		{
			Rectangle room = MapManager.rooms[nr];
			pos = new Vector3 (UnityEngine.Random.Range (room.x1, room.x2),
			                   UnityEngine.Random.Range (room.y1, room.y2),
			                   -1f);
			compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, pos, Quaternion.identity)).GetComponent<AIPlayer>();
			players.Add (compPlayer);
		}
	}


	// Update is called once per frame
	void Update ()
	{
		DIRECTION dir =  checkForInput();
		if (dir != DIRECTION.NONE)
		{
			if (!players[0].isMovePossible(dir)){
				//Debug.Log("**** NOT A POSSIBLE MOVE ****");
			}
			else {
				//Debug.Log("**** POSSIBLE MOVE ****");
				players[0].MoveToDestPosition();
				FOV();
			}
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
