using UnityEngine;
using System.Collections;

public class TargetingProjectile : MonoBehaviour {

	public Object self;
	public Entity selfItem;

	public Entity source;
	public Entity target;
	public int damage;

	public bool isMoving = false;

	public float lerpTime = 1.0f;
	public float currentLerpTime;
	public Vector2 start; 
	public Vector2 destination;

	// Use this for initialization
	void Start ()
	{
		isMoving = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isMoving)
		{
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > lerpTime) {
				currentLerpTime = lerpTime;
			}
			
			//lerp!
			float perc = currentLerpTime / lerpTime;
			destination = target.gridPosition;
			transform.position = Vector2.Lerp(start, destination, perc);
			
			if ((Vector2)transform.position == destination)
			{
				isMoving = false;
				target.fighterComponent.takeDamage(target, damage);
				Destroy(self);
				Destroy(selfItem.gameObject);
			}
		}
	}

	public void throwProjectile(Entity source, Entity target, int damage, Entity self)
	{
		this.source = source;
		this.target = target;

		this.start = source.gridPosition;
		this.destination = target.gridPosition;
		
		this.damage = 5;
		this.selfItem = self;
		currentLerpTime = 0f;
		isMoving = true;
	}
}
