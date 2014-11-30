using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : Entity {

	public List<Entity> inventory = new List<Entity>();
	public int MAX_ITEMS_INVENTORY = 5;

	void Start()
	{
		inventory.Capacity = MAX_ITEMS_INVENTORY;
	}

	public Hero (Vector2 gridPosition, string name)
		:base(gridPosition, name, true, null, null, null)
	{

	}

	public string moveOrAttack(int dx, int dy)
	{
		if (dx > 0 && !facingLeft)
			Flip();
		else if (dx < 0 && facingLeft)
			Flip ();

		Vector2 dest = new Vector2(gridPosition.x + dx, gridPosition.y + dy);

		if (MapManager.map[(int)dest.x][(int)dest.y].isDoor)
		{
			MapManager.openDoor((int)dest.x, (int)dest.y);
		}

		Entity target = null;
		bool itemsInTile = false;
		foreach (Entity e in GameController.objects)
		{
			if (e.gridPosition.x == dest.x && e.gridPosition.y == dest.y)
			{
				if (e.blocks && e.ai != null)
					target = e;
				else if (e.item != null)
					itemsInTile = true;
			}
		}
		
		if (target != null)
		{
			return this.fighterComponent.attack(this, target);
			//Debug.Log ("You attack " + target.name + "!");
		}
		else
		{
			move (dx, dy);
			if (itemsInTile == true)
				return "<color=orange>There is something here...</color>\n";
		}

		return null;
	}
}
