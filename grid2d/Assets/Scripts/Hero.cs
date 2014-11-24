using UnityEngine;
using System.Collections;

public class Hero : Entity {

	public Hero (Vector2 gridPosition, string name)
		:base(gridPosition, name, true)
	{

	}

	public string moveOrAttack(Vector2 delta)
	{
		Vector2 dest = gridPosition + delta;
		
		Entity target = null;
		foreach (Entity e in MapManager.map[(int)dest.x][(int)dest.y].getObjects())
		{
			if (e.blocks)
				target = e;
		}
		
		if (target != null)
		{
			return this.fighterComponent.attack(this, target);
			//Debug.Log ("You attack " + target.name + "!");
		}
		else
		{
			move (delta);
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
