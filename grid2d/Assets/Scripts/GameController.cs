using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public TilePrefabsHolder prefabsHolder;

	public GameObject tilePrefab;
	public GameObject boundariePrefab;
	public GameObject userPlayerPrefab;
	public GameObject AIPlayerPrefab;

	public static List<List<Tile>> map = new List<List<Tile>>();
	private List<Player> players = new List<Player>();

	public static Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	public int mapWidth = 60;
	public int mapHeight = 60;
	
	public int ROOM_MAX_SIZE = 7;
	public int ROOM_MIN_SIZE = 3;
	public int MAX_ROOMS = 20;

	// Use this for initialization
	void Start ()
	{
		MapManager.mapWidth = mapWidth;
		MapManager.mapHeight = mapHeight;
		MapManager.ROOM_MAX_SIZE = ROOM_MAX_SIZE;
		MapManager.ROOM_MIN_SIZE = ROOM_MIN_SIZE;
		MapManager.MAX_ROOMS = MAX_ROOMS;
		MapManager.generateMap();

		prefabsHolder = GameObject.Find("PrefabHolder").GetComponent<TilePrefabsHolder>();

		//MapManager.generateMapBSP();
		renderMap();
		generatePlayers();
		FOV ();
	}

	private void renderMap ()
	{
		MapManager.markTilesVisible();

		for (int r = 0; r < MapManager.mapWidth ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < MapManager.mapHeight ; c++){
				Tile t = row[c];
				if (t.isVisible)
				{
					if (t.isBoundary)
					{
						GameObject whichPrefabWall = determinePrefabWall(r, c);
						t.gamePrefab = (GameObject) Instantiate(whichPrefabWall, t.position, Quaternion.identity);
					}
					else
					{
						GameObject whichPrefabFloor = determinePrefabFloor(r, c);
						t.gamePrefab = (GameObject)	Instantiate(whichPrefabFloor, t.position, Quaternion.identity);
					}
					t.markTileAsUnexplored();
				}
//				else
//				{
//					Instantiate(prefabsHolder.DEFAULT_TILE, t.position, Quaternion.identity);
//				}
			}
		}
	}

	void FOV()
	{
		for (int i=0; i<360; i+=2)
		{
			float x = Mathf.Cos((float)i*0.01745f);
			float y = Mathf.Sin((float)i*0.01745f);
			DoFOV(x,y);
		}
	}

	void DoFOV(float x, float y)
	{
		float ox,oy;
		ox = (float)players[0].gridPosition.x+1f;
		oy = (float)players[0].gridPosition.y+1f;
		for(int i=0;i<5;i++)
		{
			map[(int)ox][(int)oy].markTileAsLit();
			//map[(int)ox][(int)oy].gamePrefab.GetComponent<SpriteRenderer>().color = Color.white;
			if(map[(int)ox][(int)oy].blocksLight==true)
				return;
			ox+=x;
			oy+=y;
		}
	}

	private GameObject determinePrefabWall (int x, int y)
	{
		int score = 0;

		try
		{
			if (map [x][y+1].isBoundary && map [x][y+1].isVisible)
				score += 8;
			if (map [x-1][y].isBoundary && map [x-1][y].isVisible)
				score += 4;
			if (map [x+1][y].isBoundary && map [x+1][y].isVisible)
				score += 2;
			if (map[x][y-1].isBoundary && map[x][y-1].isVisible)
				score += 1;
		}
		catch (ArgumentOutOfRangeException e)
		{
			Debug.LogError("Tile ("+x+","+y+")");
		}

		switch(score)
		{
		case 0:
			return prefabsHolder.COL_WALL;
		case 1:
			return prefabsHolder.S_WALL;
		case 2:
			return prefabsHolder.WE_WALL;
		case 3:
			return prefabsHolder.SE_WALL;
		case 4:
			return prefabsHolder.WE_WALL;
		case 5:
			return prefabsHolder.SW_WALL;
		case 6:
			return prefabsHolder.WE_WALL;
		case 7:
			if (!map[x-1][y-1].isBoundary && !map[x+1][y-1].isBoundary)
				return prefabsHolder.EWS_WALL;
			else if (!map[x-1][y-1].isBoundary)
				return prefabsHolder.SW_WALL;
			else
				return prefabsHolder.WE_WALL;
		case 8:
			return prefabsHolder.N_WALL;
		case 9:
			return prefabsHolder.NS_WALL;
		case 10:
			return prefabsHolder.NE_WALL;
		case 11:
			if (!map[x+1][y+1].isBoundary && !map[x+1][y-1].isBoundary)
				return prefabsHolder.NES_WALL;
			else
				return prefabsHolder.NS_WALL;
		case 12:
			return prefabsHolder.NW_WALL;
		case 13:
			if (!map[x-1][y+1].isBoundary && !map[x-1][y-1].isBoundary)
				return prefabsHolder.NWS_WALL;
			else
				return prefabsHolder.NS_WALL;
		case 14:
			if (!map[x-1][y+1].isBoundary && !map[x+1][y+1].isBoundary)
				return prefabsHolder.NEW_WALL;
			else
				return prefabsHolder.WE_WALL;
		case 15:
			return prefabsHolder.NESW_WALL;
		default:
			return prefabsHolder.DEFAULT_TILE;
		}
	}

	private GameObject determinePrefabFloor (int x, int y)
	{
		int score = 0;
		
		try
		{
			if (!map [x][y+1].isBoundary)
				score += 8;
			if (!map [x-1][y].isBoundary)
				score += 4;
			if (!map [x+1][y].isBoundary)
				score += 2;
			if (!map[x][y-1].isBoundary)
				score += 1;
		}
		catch (ArgumentOutOfRangeException e)
		{
			Debug.LogError("Tile ("+x+","+y+")");
		}

		switch(score)
		{
		case 0:
			return prefabsHolder.BLANK_FLOOR;
		case 1:
			return prefabsHolder.S_FLOOR;
		case 2:
			return prefabsHolder.E_FLOOR;
		case 3:
			return prefabsHolder.SE_FLOOR;
		case 4:
			return prefabsHolder.W_FLOOR;
		case 5:
			return prefabsHolder.SW_FLOOR;
		case 6:
			return prefabsHolder.WE_FLOOR;
		case 7:
			return prefabsHolder.EWS_FLOOR;
		case 8:
			return prefabsHolder.N_FLOOR;
		case 9:
			return prefabsHolder.NS_FLOOR;
		case 10:
			return prefabsHolder.NE_FLOOR;
		case 11:
			return prefabsHolder.NES_FLOOR;
		case 12:
			return prefabsHolder.NW_FLOOR;
		case 13:
			return prefabsHolder.NWS_FLOOR;
		case 14:
			return prefabsHolder.NEW_FLOOR;
		case 15:
			return prefabsHolder.NESW_FLOOR;
		default:
			return prefabsHolder.NESW_FLOOR;
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
			pos = new Vector2 (UnityEngine.Random.Range (room.x1, room.x2),
			                   UnityEngine.Random.Range (room.y1, room.y2));
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
