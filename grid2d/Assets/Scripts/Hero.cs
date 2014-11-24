using UnityEngine;
using System.Collections;

public class Hero : Entity {

	public Hero (Vector2 gridPosition, string name)
		:base(gridPosition, name, true)
	{

	}

	public string moveOrAttack(int dx, int dy)
	{
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

//	public override void takeTurn(Vector2 delta)
//	{
//		base.takeTurn(delta);
//		Vector2 dest = gridPosition + delta;
//
//		Entity target = null;
//		foreach (Entity e in MapManager.map[(int)dest.x][(int)dest.y].getObjects())
//		{
//			if (e.blocks)
//				target = e;
//		}
//
//		if (target != null)
//		{
//			Debug.Log ("You attack " + target.name + "!");
//		}
//		else
//		{
//			move (delta);
//		}
//	}
}
