using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour{

	public Vector2 gridPosition;
	public new string name;
	public bool blocks;

	public Fighter fighterComponent = null;
	public BasicEnemy ai = null;

	public float lerpTime = 1.0f;
	public float currentLerpTime;
	public bool isMoving = false;
	public Vector2 start; 
	public Vector2 destination;
	public bool facingLeft = true;

	public Entity (Vector2 gridPosition, string name, bool blocks, Fighter fighter, BasicEnemy ai)
	{
		this.gridPosition = gridPosition;
		this.name = name;
		this.blocks = blocks;
		this.fighterComponent = fighter;
		this.ai = ai;
	}

	void Update()
	{
		if (isMoving)
		{
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > lerpTime) {
				currentLerpTime = lerpTime;
			}

			//lerp!
			float perc = currentLerpTime / lerpTime;
			transform.position = Vector2.Lerp(start, destination, perc);

			if ((Vector2)transform.position == destination)
				isMoving = false;
		}
	}

	public void move(int dx, int dy)
	{
		if (dx > 0 && !facingLeft)
			Flip();
		else if (dx < 0 && facingLeft)
			Flip ();

		destination = new Vector2(gridPosition.x + dx, gridPosition.y + dy);

		if (!MapManager.map[(int)destination.x][(int)destination.y].isBoundary)
		{
			start = transform.position;
			currentLerpTime = 0f;
			//transform.position = Vector2.Lerp(transform.position, destination, Time.deltaTime * 1.0f);
			gridPosition = destination;
			isMoving = true;

		}
	}

	protected void Flip()
	{
		facingLeft = !facingLeft;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public float distanceTo(Entity e)
	{
		return Mathf.Abs(gridPosition.x - e.gridPosition.x) +
			Mathf.Abs(gridPosition.y - e.gridPosition.y);
	}

	public void moveTowards(Entity e, PathFinder.Pathfinder pf)
	{
//		int dx = (int)(e.gridPosition.x - gridPosition.x);
//		int dy = (int)(e.gridPosition.y - gridPosition.y);
//
//		float dist = distanceTo (e);
//		//dist = Mathf.Sqrt(dx^2 + dy^2);
//		//dist = Mathf.Abs(gridPosition.x - e.gridPosition.x) + Mathf.Abs(gridPosition.y - e.gridPosition.y);
//		Debug.Log("Distance from "+name+" = "+dist);
//
//		dx = (int)(Mathf.RoundToInt (dx / dist));
//		dy = (int)(Mathf.RoundToInt (dy / dist));
		
//		Vector2 dest = new Vector2(transform.position.x + dx, transform.position.y + dy);
//		if (!MapManager.map[(int)dest.x][(int)dest.y].isBoundary)
//		{
//			transform.position = dest;
//			gridPosition = dest;
//		}

		List<Vector2> pathToPlayer = pf.FindPath(gridPosition, GameController.objects[0].gridPosition);

		int dx = (int)(pathToPlayer[0].x - gridPosition.x);
		int dy = (int)(pathToPlayer[0].y - gridPosition.y);

		move (dx, dy);
	}

//	public virtual void takeTurn(Vector2 delta)
//	{
//
//	}
}
