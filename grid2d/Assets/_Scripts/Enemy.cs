using UnityEngine;
using System.Collections;

public class Enemy : Entity {

	public Enemy (Vector2 gridPosition, string name)
		:base(gridPosition, name, true, null, null, null)
	{
		
	}
}
