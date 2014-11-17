using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public static int mapWidth = 60;
	public static int mapHeight = 60;
	
	public static int ROOM_MAX_SIZE = 7;
	public static int ROOM_MIN_SIZE = 3;
	public static int MAX_ROOMS = 20;

	public static List<Rectangle> rooms = new List<Rectangle>();

	void Start()
	{

	}

	private static void createRoom (Rectangle room)
	{
		for (int r = room.x1; r < room.x2; r++) {
			for (int c = room.y1; c < room.y2 ; c++){
				GameController.map[r][c].isBoundary = false;
				GameController.map[r][c].blocksLight = false;
			}
		}
	}
	
	private static void createHorizontalTunnel (int x1, int x2, int y)
	{
		for (int i = Math.Min(x1, x2); i < Math.Max(x1, x2); i++){
			GameController.map[i][y].isBoundary = false;
			GameController.map[i][y].blocksLight = false;
		}
	}
	
	private static void createVerticalTunnel (int y1, int y2, int x)
	{
		for (int i = Math.Min(y1, y2); i < Math.Max(y1, y2); i++){
			GameController.map[x][i].isBoundary = false;
			GameController.map[x][i].blocksLight = false;
		}
	}

	private static void fillMap ()
	{
		for (int r = 0; r < mapWidth; r++)
		{
			List<Tile> row = new List<Tile> ();
			for (int c = 0; c < mapHeight; c++)
			{
				Vector3 pos = new Vector3(r, c);
				Tile t = new Tile (pos, true, false, true, false);
				row.Add(t);
			}
			GameController.map.Add(row);
		}
	}

	public static void markTilesVisible ()
	{
		for (int r = 0; r < mapWidth ; r++){
			List<Tile> row = GameController.map[r];
			for (int c = 0; c < mapHeight ; c++){
				Tile t = row[c];
				if (t.isBoundary)
				{
					bool hasTileNeighbours = false;
					for (int br = Math.Max (0, r-1) ; br <= Math.Min(mapHeight - 1, r+1) ; br++)
					{
						for (int bc = Math.Max(0, c-1) ; bc <= Math.Min(mapWidth -1, c+1) ; bc++)
						{
							if (!GameController.map[br][bc].isBoundary)
								hasTileNeighbours = true;
						}
					}
					if (hasTileNeighbours)
						GameController.map[r][c].isVisible = true;
				}
				else
				{
					GameController.map[r][c].isVisible = true;
				}
			}
		}
	}

	public static void generateMap ()
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
	}

	public static void generateMap2 ()
	{
		fillMap();
		
		int max_cells = mapWidth*mapHeight;
		max_cells = Mathf.RoundToInt(max_cells*0.3f);
		//max_cells = 100;

		int x = UnityEngine.Random.Range (0, mapWidth);
		int y = UnityEngine.Random.Range (0, mapHeight);

		GameController.map [x] [y].isBoundary = false;

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
				if (GameController.map[dx][dy].isBoundary != false)
				{
					GameController.map[dx][dy].isBoundary = false;
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

	public static void generateMapBSP ()
	{
		fillMap();
		Rectangle room = new Rectangle(1, 1, mapWidth-1, mapHeight-1);
		divide(room);
	}

	private static void divide (Rectangle r)
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

	private static void createRandomRoomInside (Rectangle r)
	{
		int new_x = UnityEngine.Random.Range(r.x1 + 1, r.x2 -1);
		int new_y = UnityEngine.Random.Range(r.y1 + 1, r.y2 -1);
		int new_width = UnityEngine.Random.Range(1, (r.x2 - new_x) - 1);
		int new_height = UnityEngine.Random.Range(1, (r.y2 - new_y) - 1);

		Debug.Log("---- New room: (x="+new_x+", y="+new_y+", w="+new_width+", h="+new_width+")");

		createRoom(new Rectangle(new_x, new_y, new_width, new_height));
	}
}
