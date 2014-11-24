using UnityEngine;
using System.Collections;

public class Enemy : Entity {

	public Enemy (Vector2 gridPosition, string name)
		:base(gridPosition, name, true)
	{
		
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void takeTurn(Vector2 delta)
	{
		Debug.Log(name+" threatens you!");
	}
}
