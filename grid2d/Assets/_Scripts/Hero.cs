using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : Entity {

	public List<Entity> inventory = new List<Entity>();
	public int MAX_ITEMS_INVENTORY = 5;

	public int level;
	public int experience_points;

	public int LEVEL_UP_BASE;
	public int LEVEL_UP_FACTOR;

	public bool onExit;

	public bool hasLeveledUp;

	public AudioClip stepSound;

	void Start()
	{
		inventory.Capacity = MAX_ITEMS_INVENTORY;
		onExit = false;
		hasLeveledUp = false;
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

		// if we are about to enter or exit a door tile, toggle its blocks light property
		if (MapManager.map[(int)dest.x][(int)dest.y].isDoor)
			MapManager.openDoor((int)dest.x, (int)dest.y);
//		else if (MapManager.map[(int)gridPosition.x][(int)gridPosition.y].isDoor)
//			MapManager.toggleDoor((int)gridPosition.x, (int)gridPosition.y);

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
			AudioSource.PlayClipAtPoint(stepSound, this.transform.position);
			move (dx, dy);
			if(MapManager.map[(int)dest.x][(int)dest.y].isExit)
			{
				onExit = true;
				return "<color=lime>You have found the exit!</color>\n";
			}
			if (itemsInTile == true)
				return "<color=orange>There is something here...</color>\n";
		}

		return null;
	}

	public string checkLevelUp()
	{
		int level_up_xp = LEVEL_UP_BASE + level * LEVEL_UP_FACTOR;
		if (experience_points >= level_up_xp)
		{
			level += 1;
			experience_points -= level_up_xp;

			hasLeveledUp = true;
			return "Your battle skills grow stronger! You reached level " + level + "!\n";
		}
		return "";
	}
}
