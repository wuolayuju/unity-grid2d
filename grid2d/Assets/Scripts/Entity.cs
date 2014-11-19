using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public Vector2 gridPosition;
	public GameObject prefab;
	public string name;
	public bool blocks;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void move(Tile t)
	{
		if (!t.isBlocked)
		{
			transform.position = t.position;
			gridPosition = t.position;
		}
	}

	public virtual void takeTurn();
}
