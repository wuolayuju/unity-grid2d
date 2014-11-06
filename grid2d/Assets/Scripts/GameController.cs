using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject tilePrefab;
	public GameObject userPlayerPrefab;
	public GameObject AIPlayerPrefab;
	public int mapSize = 4;

	private List<List<Tile>> map = new List<List<Tile>>();
	private List<Player> players = new List<Player>();

	// Use this for initialization
	void Start ()
	{
		generateMap();
		renderMap();
		generatePlayers();
	}

	private void generateMap ()
	{
		for (int r = 0; r < mapSize ; r++){
			List<Tile> row = new List<Tile>();
			for (int c = 0; c < mapSize ; c++){
				Vector3 pos = new Vector3(r, c);
				Tile tile = new Tile();
				tile.position = pos;
				row.Add(tile);
			}
			map.Add(row);
		}
	}

	private void renderMap ()
	{
		for (int r = 0; r < mapSize ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < mapSize ; c++){
				Tile t = row[c];
				Instantiate(tilePrefab, t.position, Quaternion.identity);
			}
		}
	}

	private void generatePlayers ()
	{
		UserPlayer humanPlayer;
		humanPlayer = ((GameObject) Instantiate (userPlayerPrefab, new Vector2 (0f, 0f), Quaternion.identity)).GetComponent<UserPlayer>();
		players.Add (humanPlayer);

		AIPlayer compPlayer;
		compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, new Vector2 (2f, 2f), Quaternion.identity)).GetComponent<AIPlayer>();
		players.Add (compPlayer);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
