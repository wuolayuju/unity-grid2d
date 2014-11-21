using UnityEngine;
using System.Collections;

public abstract class Entity{

	public Vector2 gridPosition;
	public GameObject prefab;
	public string name;
	public bool blocks;

	public Entity (Vector2 gridPosition, GameObject prefab, string name, bool blocks)
	{
		this.gridPosition = gridPosition;
		this.prefab = prefab;
		this.name = name;
		this.blocks = blocks;
	}

	public void moveTo(Vector2 dPos)
	{
		//if (isMovePossible
	}
	
	public bool isMovePossible (GameController.DIRECTION dir)
	{
		return true;
	}

	public void move(Vector2 dest)
	{
		if (!MapManager.map[(int)dest.x][(int)dest.y].isBlocked())
		{
			prefab.transform.position = dest;
			gridPosition = dest;
		}
	}

	public abstract void takeTurn();
}
