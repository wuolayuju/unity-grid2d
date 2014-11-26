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

	void Start()
	{
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

		for (int i = 1; i < GameController.objects.Count; i++) 
		{
			// prevents collision with other enemies
			Entity e = GameController.objects[i];
			if (e.gridPosition.x == destination.x &&
			    e.gridPosition.y == destination.y &&
			    e.blocks == true)

				return;
		}

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

	public void moveTowards(Entity e)
	{

		List<Vector2> pathToPlayer = MapManager.pathfinder.FindPath(gridPosition, GameController.objects[0].gridPosition);

		int dx = (int)(pathToPlayer[0].x - gridPosition.x);
		int dy = (int)(pathToPlayer[0].y - gridPosition.y);

		move (dx, dy);
	}
}
