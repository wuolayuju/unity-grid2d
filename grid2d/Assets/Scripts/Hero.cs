using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : Entity {

	public List<Entity> inventory = new List<Entity>();
	public int MAX_ITEMS_INVENTORY = 20;

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
