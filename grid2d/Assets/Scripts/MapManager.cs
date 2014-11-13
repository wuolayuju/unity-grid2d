using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public static int mapWidth = 40;
	public static int mapHeight = 40;
	
	public static int ROOM_MAX_SIZE = 7;
	public static int ROOM_MIN_SIZE = 3;
	public static int MAX_ROOMS = 10;

	void Start()
	{

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

	private static void createRoom (Rectangle room)
	{
		for (int r = room.x1; r < room.x2; r++) {
			for (int c = room.y1; c < room.y2 ; c++){
				GameController.map[r][c].isBoundary = false;
			}
		}
	}
	
	private static void createHorizontalTunnel (int x1, int x2, int y)
	{
		for (int i = Math.Min(x1, x2); i < Math.Max(x1, x2); i++){
			GameController.map[i][y].isBoundary = false;
		}
	}
	
	private static void createVerticalTunnel (int y1, int y2, int x)
	{
		for (int i = Math.Min(y1, y2); i < Math.Max(y1, y2); i++){
			GameController.map[x][i].isBoundary = false;
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
				Tile t = new Tile (pos, true);
				row.Add(t);
			}
			GameController.map.Add(row);
		}
	}

	public static void generateMap ()
	{
		fillMap();
		
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
		
		int max_cells = 60;

		int x = UnityEngine.Random.Range (1, mapWidth);
		int y = UnityEngine.Random.Range (1, mapHeight);

		GameController.map [x] [y].isBoundary = false;

		Debug.Log("P = ("+x+","+y+")");

		int num_cells = 0;
		int dx = x, dy = y;
		while (num_cells < max_cells)
		{
			int dir = UnityEngine.Random.Range (1, 4);
			
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
			}
			
			Debug.Log("D = ("+dx+","+dy+")");
			
			if ((dx < mapWidth && dx >= 1) || (dy < mapHeight || dy >= 1)) {
				if (GameController.map[dx][dy].isBoundary != false)
				{
					GameController.map[dx][dy].isBoundary = false;
					num_cells++;
				}
			}
		}
		
	}

}
