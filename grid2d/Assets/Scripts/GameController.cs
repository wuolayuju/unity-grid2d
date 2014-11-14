using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

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
		//MapManager.generateMapBSP();
		renderMap();
		generatePlayers();
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
						GameObject whichPrefab = determinePrefab(r, c);
						Instantiate(whichPrefab, t.position, Quaternion.identity);
					}
					else
					{
						Instantiate(TilePrefabsHolder.instance.BLANK_FLOOR, t.position, Quaternion.identity);
					}
				}
			}
		}
		/*for (int r = 0; r < MapManager.mapWidth ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < MapManager.mapHeight ; c++){
				Tile t = row[c];
				if (t.isBoundary)
				{
					bool hasTileNeighbours = false;
					for (int br = Math.Max (0, r-1) ; br <= Math.Min(MapManager.mapHeight - 1, r+1) ; br++)
					{
						for (int bc = Math.Max(0, c-1) ; bc <= Math.Min(MapManager.mapWidth -1, c+1) ; bc++)
						{
							if (!map[br][bc].isBoundary)
								hasTileNeighbours = true;
						}
					}
					if (hasTileNeighbours)
					{
						GameObject whichPrefab = determinePrefab(r, c);
						Instantiate(whichPrefab, t.position, Quaternion.identity);
						//Instantiate(TilePrefabsHolder.instance.DEFAULT_TILE, t.position, Quaternion.identity);
					}
				}
				else
				{
					Instantiate(TilePrefabsHolder.instance.BLANK_FLOOR, t.position, Quaternion.identity);
				}
			}
		}*/
	}

	private GameObject determinePrefab (int x, int y)
	{
		int score = 0;

		if (map [x][y+1].isBoundary && map [x][y+1].isVisible)
			score += 8;
		if (map [x-1][y].isBoundary && map [x-1][y].isVisible)
			score += 4;
		if (map [x+1][y].isBoundary && map [x+1][y].isVisible)
			score += 2;
		if (map[x][y-1].isBoundary && map[x][y-1].isVisible)
			score += 1;

		switch(score)
		{
		case 1:
			return TilePrefabsHolder.instance.S_WALL;
		case 2:
			return TilePrefabsHolder.instance.WE_WALL;
		case 3:
			return TilePrefabsHolder.instance.SE_WALL;
		case 4:
			return TilePrefabsHolder.instance.WE_WALL;
		case 5:
			return TilePrefabsHolder.instance.SW_WALL;
		case 8:
			return TilePrefabsHolder.instance.N_WALL;
		case 10:
			return TilePrefabsHolder.instance.NE_WALL;
		case 12:
			return TilePrefabsHolder.instance.NW_WALL;
		default:
			if (score == 9 || score == 11 || score == 13)
				return TilePrefabsHolder.instance.NS_WALL;
			else if (score == 6 || score == 7 || score == 14)
				return TilePrefabsHolder.instance.WE_WALL;
			break;
		}

		return TilePrefabsHolder.instance.DEFAULT_TILE;
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
