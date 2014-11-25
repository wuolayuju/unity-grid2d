using UnityEngine;
using System.Collections;

public class Hero : Entity {

	public Hero (Vector2 gridPosition, string name)
		:base(gridPosition, name, true, null, null)
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
		foreach (Entity e in GameController.objects)
		{
			if (e.gridPosition.x == dest.x && e.gridPosition.y == dest.y && e.blocks)
				target = e;
		}
		
		if (target != null)
		{
			return this.fighterComponent.attack(this, target);
			//Debug.Log ("You attack " + target.name + "!");
		}
		else
		{
			move (dx, dy);
		}

		return null;
	}
}
