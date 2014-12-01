using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public List<TilePrefabsHolder> listPrefabsHolders;

	private TilePrefabsHolder prefabsHolder;

	public static List<List<Tile>> map;

	public  int mapWidth = 60;
	public  int mapHeight = 60;
	
	public  int ROOM_MAX_SIZE = 7;
	public  int ROOM_MIN_SIZE = 3;
	public  int MAX_ROOMS = 20;

	public static List<Rectangle> rooms = new List<Rectangle>();

	public static Pathfinder pathfinder;


	void Awake()
	{
		int prefabsHolderIndex = UnityEngine.Random.Range (0, listPrefabsHolders.Count);
		prefabsHolder = listPrefabsHolders [prefabsHolderIndex];

		map = new List<List<Tile>> ();
		rooms = new List<Rectangle>();
	}

	void Start()
	{
	}

	private void createRoom (Rectangle room)
	{
		for (int r = room.x1; r < room.x2; r++) {
			for (int c = room.y1; c < room.y2 ; c++){
				map[r][c].isBoundary = false;
				map[r][c].blocksLight = false;
			}
		}
	}
	
	private void createHorizontalTunnel (int x1, int x2, int y)
	{
		int start = Math.Min(x1, x2);
		int end = Math.Max(x1, x2);

		//Debug.Log ("HORIZONTAL FROM " + start + " TO " + end + " AT Y = " + y);

		for (int i = start; i <= end; i++){
			map[i][y].isBoundary = false;
			map[i][y].blocksLight = false;
		}
	}
	
	private void createVerticalTunnel (int y1, int y2, int x)
	{
		int start =  Math.Min(y1, y2);
		int end = Math.Max(y1, y2);

		//Debug.Log ("VERTICAL FROM " + start + " TO " + end + " AT X = " + x);

		for (int i = start; i <= end; i++){
			map[x][i].isBoundary = false;
			map[x][i].blocksLight = false;
		}
	}

	private void fillMap ()
	{
		for (int r = 0; r < mapWidth; r++)
		{
			List<Tile> row = new List<Tile> ();
			for (int c = 0; c < mapHeight; c++)
			{
				Vector3 pos = new Vector3(r, c);
				Tile t = new Tile (pos, true, false, true, false, false, false, false);
				row.Add(t);
			}
			map.Add(row);
		}
	}

	public void markTilesVisible ()
	{
		for (int r = 0; r < mapWidth ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < mapHeight ; c++){
				Tile t = row[c];
				if (t.isBoundary)
				{
					bool hasTileNeighbours = false;
					for (int br = Math.Max (0, r-1) ; br <= Math.Min(mapWidth - 1, r+1) ; br++)
					{
						for (int bc = Math.Max(0, c-1) ; bc <= Math.Min(mapHeight -1, c+1) ; bc++)
						{
							if (!map[br][bc].isBoundary)
								hasTileNeighbours = true;
						}
					}
					if (hasTileNeighbours)
						map[r][c].isVisible = true;
				}
				else
				{
					map[r][c].isVisible = true;
				}
			}
		}
	}

	public void generateMap ()
	{
		fillMap();

		int num_rooms = 0;
		
		for (int r = 0; r < MAX_ROOMS; r++)
		{
			// random width and height
			int w = UnityEngine.Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE);
			int h = UnityEngine.Random.Range(ROOM_MIN_SIZE, ROOM_MAX_SIZE);
			int x = UnityEngine.Random.Range(2, mapWidth - w - 2);
			int y = UnityEngine.Random.Range(2, mapHeight - h - 2);
			
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
					GameController.playerStartPosition = new Vector2(newRoomCenter.x, newRoomCenter.y);
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

		Vector2 posExit = rooms[rooms.Count-1].getCenter();
		map[(int)posExit.x][(int)posExit.y].isExit = true;
	}

	public void generateMap2 ()
	{
		fillMap();
		
		int max_cells = mapWidth*mapHeight;
		max_cells = Mathf.RoundToInt(max_cells*0.3f);
		//max_cells = 100;

		int x = UnityEngine.Random.Range (0, mapWidth);
		int y = UnityEngine.Random.Range (0, mapHeight);

		map [x] [y].isBoundary = false;

		GameController.playerStartPosition = new Vector2(x, y);

		Debug.Log("P = ("+x+","+y+")");

		int num_cells = 0;
		int dx = x, dy = y;
		while (num_cells < max_cells)
		{
			int dir = UnityEngine.Random.Range (1, 5);
			
			switch(dir)
			{
			case 1:
				dy = y + 1;
				break;
			case 2:
				dx = x + 1;
				break;
			case 3:
				dy = y - 1;
				break;
			case 4:
				dx = x - 1;
				break;
			default:
				break;
			}
			
			Debug.Log("D = ("+dx+","+dy+")");
			if ((dx < mapWidth && dx >= 0) && (dy < mapHeight && dy >= 0))
			{
				if (map[dx][dy].isBoundary != false)
				{
					map[dx][dy].isBoundary = false;
					num_cells++;
				}
				x = dx;
				y = dy;
			}
			else
			{
				Debug.Log("FUCK");
				dx = x;
				dy = y;
			}
		}
		
	}

	public void generateMapBSP ()
	{
		fillMap();
		Rectangle room = new Rectangle(1, 1, mapWidth-1, mapHeight-1);
		divide(room);
	}

	private void divide (Rectangle r)
	{
		int r_width = r.x2 - r.x1;
		int r_height = r.y2 - r.y1;

		// stop condition
		if ((r_width <= (ROOM_MIN_SIZE+2)) || (r_height <= (ROOM_MIN_SIZE+2)))
		{
			//createRandomRoomInside(r);
			return;
		}

		int dir = UnityEngine.Random.Range(0,2);
		// vertical division
		if (dir == 0)
		{
			int x = UnityEngine.Random.Range(r.x1+(ROOM_MIN_SIZE+2), (r.x1 + r_width) - (ROOM_MIN_SIZE+2));
			Debug.Log("**** VERTICAL DIVISION x="+x);

			Debug.Log("Rect 1: (x="+r.x1+", y="+r.y1+", w="+x+", h="+r_height+")");
			Rectangle r1 = new Rectangle(r.x1, r.y1, x, r_height);
			divide (r1);
			//createRandomRoomInside(r1);

			Debug.Log("Rect 2: (x="+(x+1)+", y="+r.y1+", w="+(r_width-x)+", h="+r_height+")");
			Rectangle r2 = new Rectangle(x+1, r.y1, r_width-x, r_height);
			divide (r2);
			//createRandomRoomInside(r2);
		}
		// horizontal division
		else if (dir == 1)
		{
			int y = UnityEngine.Random.Range(r.y1 + (ROOM_MIN_SIZE+2), (r.y1 + r_height) - (ROOM_MIN_SIZE+2));
			Debug.Log("**** HORIZONTAL DIVISION y="+y);

			Debug.Log("Rect 1: (x="+r.x1+", y="+r.y1+", w="+r_width+", h="+y+")");
			Rectangle r1 = new Rectangle(r.x1, r.y1, r_width, y);
			divide (r1);
			//createRandomRoomInside(r1);

			Debug.Log("Rect 2: (x="+r.x1+", y="+(y+1)+", w="+r_width+", h="+(r_height-y)+")");
			Rectangle r2 = new Rectangle(r.x1, y+1, r_width, r_height-y);
			divide (r2);
			//createRandomRoomInside(r2);
		}
	}

	private void createRandomRoomInside (Rectangle r)
	{
		int new_x = UnityEngine.Random.Range(r.x1 + 1, r.x2 -1);
		int new_y = UnityEngine.Random.Range(r.y1 + 1, r.y2 -1);
		int new_width = UnityEngine.Random.Range(1, (r.x2 - new_x) - 1);
		int new_height = UnityEngine.Random.Range(1, (r.y2 - new_y) - 1);

		Debug.Log("---- New room: (x="+new_x+", y="+new_y+", w="+new_width+", h="+new_width+")");

		createRoom(new Rectangle(new_x, new_y, new_width, new_height));
	}

	public void renderMap ()
	{
		markTilesVisible();
		
		for (int r = 0; r < mapWidth ; r++){
			List<Tile> row = map[r];
			for (int c = 0; c < mapHeight ; c++){
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
					toggleObjectsInTile(r, c, false);
				}
				//				else
				//				{
				//					Instantiate(prefabsHolder.DEFAULT_TILE, t.position, Quaternion.identity);
				//				}
			}
		}
	}

	public void FOV(Vector2 point, int visionRange)
	{
		for (int r = 0; r < mapWidth ; r++)
		{
			for (int c = 0; c < mapHeight ; c++)
			{
				Tile t = MapManager.map[r][c];
				if (t.isLit)
				{
					t.isLit = false;
					toggleObjectsInTile(r, c, false);

				}
				if (t.isExplored && t.isVisible)
				{
					t.markTileAsExplored();
					toggleObjectsInTile(r, c, false);
				}
			}
		}
		for (int i=0; i<360; i+=2)
		{
			float x = Mathf.Cos((float)i*0.01745f);
			float y = Mathf.Sin((float)i*0.01745f);
			DoFOV(x,y, point, visionRange);
		}
	}
	
	public void DoFOV(float x, float y, Vector2 point, int visionRange)
	{
		float ox,oy;
		ox = (float)point.x+0.5f;
		oy = (float)point.y+0.5f;
		for(int i=1;i<visionRange;i++)
		{
			MapManager.map[(int)ox][(int)oy].markTileAsLit((float)i/visionRange);

			toggleObjectsInTile((int)ox, (int)oy, true);

			if(MapManager.map[(int)ox][(int)oy].blocksLight==true)
				return;
			ox+=x;
			oy+=y;
		}
	}

	private void toggleObjectsInTile(int x, int y, bool isEnabled)
	{
		for(int i=1; i < GameController.objects.Count; i++)
		{
			Entity e = GameController.objects[i];
			if (e.gridPosition.x == x && e.gridPosition.y == y)
			{
				try { e.GetComponentInChildren<Canvas>().enabled = isEnabled; }catch{}
				e.GetComponent<SpriteRenderer>().enabled = isEnabled;
			}
		}
	}

	public static void openDoor(int x, int y)
	{
		if (map[x][y].isDoor)
		{
			map[x][y].blocksLight = false;
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
			Debug.LogError("Tile ("+x+","+y+") -> EXCEPTION"+e.ToString());
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
			else if (!map[x+1][y-1].isBoundary)
				return prefabsHolder.SE_WALL;
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
			else if (!map[x+1][y+1].isBoundary)
				return prefabsHolder.NE_WALL;
			else if (!map[x+1][y-1].isBoundary)
				return prefabsHolder.SE_WALL;
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
		if(map[x][y].isDoor) return prefabsHolder.Door_Closed;

		if(map[x][y].isExit) return prefabsHolder.Stairs_Down;

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
			Debug.LogError("Tile ("+x+","+y+") -> "+e.ToString());
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
			if(!map[x-1][y+1].isBoundary || !map[x+1][y+1].isBoundary || !map[x+1][y-1].isBoundary || !map[x-1][y-1].isBoundary)
			{
				map[x][y].isDoor = true;
				map[x][y].blocksLight = true;
				return prefabsHolder.Door_Closed;
			}
			return prefabsHolder.WE_FLOOR;
		case 7:
			return prefabsHolder.EWS_FLOOR;
		case 8:
			return prefabsHolder.N_FLOOR;
		case 9:
			if(!map[x-1][y+1].isBoundary || !map[x+1][y+1].isBoundary || !map[x+1][y-1].isBoundary || !map[x-1][y-1].isBoundary)
			{
				map[x][y].isDoor = true;
				map[x][y].blocksLight = true;
				return prefabsHolder.Door_Closed;
			}
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
}
