using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject tilePrefab;
	public GameObject boundariePrefab;
	public GameObject userPlayerPrefab;
	public GameObject AIPlayerPrefab;

	public int mapWidth = 20;
	public int mapHeight = 20;

	public int ROOM_MAX_SIZE = 7;
	public int ROOM_MIN_SIZE = 3;
	public int MAX_ROOMS = 4;

	public static List<List<Tile>> map = new List<List<Tile>>();
	private List<Player> players = new List<Player>();

	private Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	// Use this for initialization
	void Start ()
	{
		generateMap();
		renderMap();
		generatePlayers();
	}

	private class Rectangle {
		public int x1, y1, x2, y2;

		public Rectangle(int x, int y, int w, int h) {
			this.x1 = x;
			this.y1 = y;
			this.x2 = x + w;
			this.y2 = y + h;
		}

		public Vector2 getCenter()
		{
			Vector2 cent = new Vector2 ((x1 + x2) / 2, (y1 + y2) / 2);
			return cent;
		}

		public bool intersects(Rectangle other)
		{
			return (x1 <= other.x2 && x2 >= other.x1 && y1 <= other.y2 && y2 >= other.y1);
		}
	}

	private void createRoom (Rectangle room)
	{
		for (int r = room.x1; r < room.x2; r++) {
			for (int c = room.y1; c < room.y2 ; c++){
				map[r][c].isBoundary = false;
			}
		}
	}

	private void createHorizontalTunnel (int x1, int x2, int y)
	{
		for (int i = Math.Min(x1, x2); i < Math.Max(x1, x2); i++){
			map[i][y].isBoundary = false;
		}
	}

	private void createVerticalTunnel (int y1, int y2, int x)
	{
		for (int i = Math.Min(y1, y2); i < Math.Max(y1, y2); i++){
			map[x][i].isBoundary = false;
		}
	}

	private void generateMap ()
	{
		for (int r = 0; r < mapWidth; r++) {
			List<Tile> row = new List<Tile> ();
			for (int c = 0; c < mapHeight; c++)
			{
				Vector3 pos = new Vector3(r, c);
				Tile t = new Tile (pos, true);
				row.Add(t);
			}
			map.Add(row);
		}

		List<Rectangle> rooms = new List<Rectangle>();
		int num_rooms = 0;

		for (int r = 0; r < MAX_ROOMS; r++)
		{
			// random width and height
			int w = UnityEngine.Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE);
			int h = UnityEngine.Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE);
			int x = UnityEngine.Random.Range(0, mapWidth - w - 1);
			int y = UnityEngine.Random.Range(0, mapHeight - h - 1);

			Rectangle new_room = new Rectangle(x, y, w, h);

			// check intersections wiht other rooms
			bool failed = false;
			foreach(Rectangle otherRoom in rooms)
			{
				if (new_room.intersects(otherRoom)){
					failed = true;
					break;
				}
			}

			// if the room is valid (no intersections)
			if (!failed)
			{
				createRoom(new_room);
				Vector2 newRoomCenter = new_room.getCenter();

				if (num_rooms == 0) {
					// player starts at the center of the first room
					playerStartPosition = new Vector2(newRoomCenter.x, newRoomCenter.y);
				}
				else{
					// all rooms are connected with the previous room with a tunnel
					Vector2 prevRoomCenter = rooms[num_rooms-1].getCenter();

					if (UnityEngine.Random.Range(0, 1) == 1){
						// first move horizontally, then vertically
						createHorizontalTunnel ((int)prevRoomCenter.x, (int)newRoomCenter.x, (int)prevRoomCenter.y);
						createVerticalTunnel ((int)prevRoomCenter.y, (int)newRoomCenter.y, (int)newRoomCenter.x);
					}
					else{
						//first move vertically, then horizontally
						createVerticalTunnel ((int)prevRoomCenter.y, (int)newRoomCenter.y, (int)prevRoomCenter.x);
						createHorizontalTunnel ((int)prevRoomCenter.x, (int)newRoomCenter.x, (int)newRoomCenter.y);
					}
				}

				rooms.Add(new_room);
				num_rooms ++;
			}
		}

/*		Rectangle room1 = new Rectangle (3, 3, 5, 5);
		Rectangle room2 = new Rectangle (12, 3, 3, 4);
		createRoom (room1);
		createRoom (room2);
		createHorizontalTunnel (8, 12, 4);*/

/*		for (int r = 0; r < mapSize; r++) {
			List<Tile> row = new List<Tile> ();

			// First and last rows are boundaries
			if ((r == 0) || (r == mapSize - 1)) {
				for (int c = 0; c < mapSize; c++)
				{
					Vector3 pos = new Vector3(r, c);
					Tile t = new Tile (pos, true);
					row.Add(t);
				}
			}
			else {
				for (int c = 0; c < mapSize; c++) {
					Vector3 pos = new Vector3(r, c);
					bool isBoundary = false;

					// First and last columns are boundaries in each row
					if ((c == 0) || (c == mapSize - 1))
						isBoundary = true;

					Tile t = new Tile (pos, isBoundary);
					row.Add(t);
				}
			}

			map.Add(row);
		}*/

	/*	for (int r = 0; r < mapSize ; r++){
			List<Tile> row = new List<Tile>();
			for (int c = 0; c < mapSize ; c++){
				Vector3 pos = new Vector3(r, c);
				Tile tile = new Tile();
				tile.position = pos;
				row.Add(tile);
			}
			map.Add(row);
		}*/
	}

	private void renderMap ()
	{
		for (int r = 0; r < mapWidth ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < mapHeight ; c++){
				Tile t = row[c];
				if (t.isBoundary)
					Instantiate(boundariePrefab, t.position, Quaternion.identity);
				else
					Instantiate(tilePrefab, t.position, Quaternion.identity);
			}
		}
	}

	private void generatePlayers ()
	{
		Vector2 pos = playerStartPosition;

		UserPlayer humanPlayer;
		humanPlayer = ((GameObject) Instantiate (userPlayerPrefab, pos, Quaternion.identity)).GetComponent<UserPlayer>();
		humanPlayer.gridPosition = pos;
		players.Add (humanPlayer);

		/*AIPlayer compPlayer;
		compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, new Vector2 (2f, 2f), Quaternion.identity)).GetComponent<AIPlayer>();
		players.Add (compPlayer);*/
	}


	// Update is called once per frame
	void Update ()
	{
		DIRECTION dir =  checkForInput();
		if (dir != DIRECTION.NONE)
		{
			if (!players[0].isMovePossible(dir)){
				Debug.Log("**** NOT A POSSIBLE MOVE ****");
			}
			else {
				Debug.Log("**** POSSIBLE MOVE ****");
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
	/*
	bool isMovePossible(DIRECTION dir)
	{
		Vector2 pos = players[0].gridPosition;
		Debug.Log("*** Player position is (" + pos.x + "," + pos.y + ")");
		Tile t = map[(int)pos.x][(int)pos.y];

		if (dir == DIRECTION.DOWN)
			t = map[(int)pos.x][(int)pos.y - 1];

		else if (dir == DIRECTION.UP)
			t = map[(int)pos.x][(int)pos.y + 1];

		else if (dir == DIRECTION.RIGHT)
			t = map[(int)pos.x + 1][(int)pos.y];

		else if (dir == DIRECTION.LEFT)
			t = map[(int)pos.x - 1][(int)pos.y];

		Debug.Log("### Tile in destination position (" + t.position.x + "," + t.position.y + ")");

		if (t.isBoundary) {
			return false;
		}
		else {
			players[0].destPosition = new Vector2(t.position.x, t.position.y);
			return true;
		}
	}*/
}
