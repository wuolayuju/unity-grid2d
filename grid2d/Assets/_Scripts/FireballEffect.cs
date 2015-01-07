using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FireballEffect : ItemEffect {
	
	public GameObject visualEffect;
	public int FIREBALL_DAMAGE = 5;

	public List<Entity> targets;
	public int indexTarget = 0;

	public GameObject targetingPinPointerPrefab;
	public GameObject pinPointer;
	public Vector2 offsetSprite;

	public bool casting;

	public Entity self;
	

	// Use this for initialization
	void Start ()
	{
		targets = new List<Entity>();
		indexTarget = 0;
		offsetSprite = new Vector2(-0.25f, 1f);
		casting = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (casting && GameController.targeting == true)
		{
			if (Input.GetKeyDown(KeyCode.Z))
		    {
				GameController.targeting = false;
				Destroy(pinPointer);
			}
			if (Input.GetButtonDown("right"))
			{
				indexTarget = (indexTarget + 1) % targets.Count;
				if (pinPointer == null)
					Debug.Log("WTF");
				pinPointer.transform.position = targets[indexTarget].gridPosition + offsetSprite;
			}
			else if (Input.GetButtonDown("left"))
	        {
				// kind of negative modulo (C# weirdness)
				indexTarget--;
				if (indexTarget < 0) 
					indexTarget = targets.Count-1;
				pinPointer.transform.position = targets[indexTarget].gridPosition + offsetSprite;
			}
			else if (Input.GetKeyDown(KeyCode.Return))
			{
				GameController.targeting = false;
				Destroy(pinPointer);

				GameObject fireball = (GameObject) Instantiate(visualEffect, GameController.objects[0].gridPosition, Quaternion.identity);
				AudioSource.PlayClipAtPoint(effectSound, targets[indexTarget].transform.position);
				TargetingProjectile tpr = fireball.GetComponent<TargetingProjectile>();
				tpr.throwProjectile(GameController.objects[0], targets[indexTarget], FIREBALL_DAMAGE, self);
				tpr.isMoving = true;

				//targets[indexTarget].fighterComponent.takeDamage(targets[indexTarget], FIREBALL_DAMAGE);
				((Hero)GameController.objects[0]).inventory.Remove(self);
			}
		}
	}

	public override string useItem (Entity self)
	{
		this.self = self;
		targets.Clear();
		getEnemiesInSight();

		if (targets.Count == 0)
		{
			return "<color=red>No enemies nearby!</color>\n";
		}

		Vector2 posPinPointer = targets[0].gridPosition + offsetSprite;
		pinPointer = (GameObject) Instantiate(targetingPinPointerPrefab, posPinPointer, Quaternion.identity);
		indexTarget = 0;

		GameController.targeting = true;
		GameController.infoActionText = "Choose a target, press Z to cancel";

		casting = true;

		return "";
	}

	private void getEnemiesInSight()
	{
		targets.Clear();
		for (int i = 1; i < GameController.objects.Count ; i++)
		{
			Entity e = GameController.objects[i];
			if (e.GetComponentInChildren<SpriteRenderer>().enabled && e.ai != null)
			{
				targets.Add(e);
			}
		}
	}

	public void destroyEffect()
	{
		Destroy(this);
	}
}
