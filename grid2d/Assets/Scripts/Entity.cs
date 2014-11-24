using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour{

	public Vector2 gridPosition;
	public new string name;
	public bool blocks;

	public float lerpTime = 1.0f;
	public float currentLerpTime;

	public bool isMoving = false;
	public Vector2 start; 
	public Vector2 destination;

	public bool facingLeft = true;

	public Entity (Vector2 gridPosition, string name, bool blocks)
	{
		this.gridPosition = gridPosition;
		this.name = name;
		this.blocks = blocks;
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

	public virtual void takeTurn(Vector2 delta)
	{
		if (delta.x > 0 && !facingLeft)
			Flip();
		else if (delta.x < 0 && facingLeft)
			Flip ();
	}
}
