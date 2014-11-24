using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour{

	public Vector2 gridPosition;
	public new string name;
	public bool blocks;

	public Fighter fighterComponent;
	public BasicEnemy ai;

	public float lerpTime = 1.0f;
	private float currentLerpTime;
	public bool isMoving = false;
	private Vector2 start; 
	private Vector2 destination;
	private bool facingLeft = true;

	public Entity (Vector2 gridPosition, string name, bool blocks, Fighter fighter = null, BasicEnemy ai = null)
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

	public void move(Vector2 delta)
	{
		if (delta.x > 0 && !facingLeft)
			Flip();
		else if (delta.x < 0 && facingLeft)
			Flip ();

		destination = gridPosition + delta;

		if (!MapManager.map[(int)destination.x][(int)destination.y].isBlocked())
		{
			start = transform.position;
			currentLerpTime = 0f;
			isMoving = true;
			//transform.position = Vector2.Lerp(transform.position, destination, Time.deltaTime * 1.0f);
			gridPosition = destination;
		}
	}

	private void Flip()
	{
		facingLeft = !facingLeft;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public float distanceTo(Entity e)
	{
		return Vector2.Distance(gridPosition, e.gridPosition);
	}

	public void moveTowards(Entity e)
	{
		float dist = distanceTo (e);

		int dx = (int)(e.gridPosition.x - gridPosition.x);
		int dy = (int)(e.gridPosition.y - gridPosition.y);

		dx = (int)(Mathf.RoundToInt (dx / dist));
		dy = (int)(Mathf.RoundToInt (dy / dist));

		move (new Vector2 (dx, dy));
	}

//	public virtual void takeTurn(Vector2 delta)
//	{
//
//	}
}
