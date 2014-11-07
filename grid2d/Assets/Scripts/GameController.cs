using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject tilePrefab;
	public GameObject boundariePrefab;
	public GameObject userPlayerPrefab;
	public GameObject AIPlayerPrefab;
	public int mapSize = 4;

	private List<List<Tile>> map = new List<List<Tile>>();
	private List<Player> players = new List<Player>();

	private enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	// Use this for initialization
	void Start ()
	{
		generateMap();
		renderMap();
		generatePlayers();
	}

	private void generateMap ()
	{
		for (int r = 0; r < mapSize; r++) {
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
		}

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
		for (int r = 0; r < mapSize ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < mapSize ; c++){
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
		Vector2 pos = new Vector2(4f,4f);

		UserPlayer humanPlayer;
		humanPlayer = ((GameObject) Instantiate (userPlayerPrefab, pos, Quaternion.identity)).GetComponent<UserPlayer>();
		humanPlayer.gridPosition = pos;
		players.Add (humanPlayer);

		AIPlayer compPlayer;
		compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, new Vector2 (2f, 2f), Quaternion.identity)).GetComponent<AIPlayer>();
		players.Add (compPlayer);
	}


	// Update is called once per frame
	void Update ()
	{
		DIRECTION dir =  checkForInput();
		if (dir != DIRECTION.NONE)
		{
			if (!isMovePossible(dir)){
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
	}
}
