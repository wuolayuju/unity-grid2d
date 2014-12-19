using UnityEngine;
using System.Collections;

public class LightningEffect : ItemEffect {

	public GameObject visualEffect;
	public int LIGHTNING_DAMAGE = 10;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override string useItem (Entity self)
	{
		Hero h = (Hero)GameController.objects[0];

		Entity target = h.closestEntity();
		if (target == null)
		{
			return "<color=red>No enemy is close enoguh to strike!</color>\n";
		}

		Instantiate (visualEffect, target.gridPosition, Quaternion.identity);

		Debug.Log("ENEMY LIGHTINING: "+target.name);

		target.fighterComponent.takeDamage(target, LIGHTNING_DAMAGE);

		h.inventory.Remove(self);

		return "<color=cyan>A lightning bolt strikes "+target.name+" for "+LIGHTNING_DAMAGE+" hit points!.</color>\n";
	}
}
