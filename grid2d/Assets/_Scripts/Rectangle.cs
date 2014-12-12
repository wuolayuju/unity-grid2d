using UnityEngine;
using System.Collections;

public class Rectangle {
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
