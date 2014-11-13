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

	// Use this for initialization
	void Start ()
	{
		MapManager.generateMap();//generateMap();
		renderMap();
		generatePlayers();
	}

	private void renderMap ()
	{
		for (int r = 0; r < MapManager.mapWidth ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < MapManager.mapHeight ; c++){
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
		humanPlayer = GameObject.Find ("userPlayer").GetComponent<UserPlayer>();
		humanPlayer.transform.position = pos;
		//humanPlayer = ((GameObject) Instantiate (userPlayerPrefab, pos, Quaternion.identity)).GetComponent<UserPlayer>();
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
}
