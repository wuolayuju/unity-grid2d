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
			return "<color=red>No enemy is close enough to strike!</color>\n";
		}

		AudioSource.PlayClipAtPoint(effectSound, target.transform.position);

		Instantiate (visualEffect, target.gridPosition, Quaternion.identity);

		string info = target.fighterComponent.takeDamage(target, LIGHTNING_DAMAGE);

		//StartCoroutine(delay(target));

		h.inventory.Remove(self);

		Destroy(self.gameObject);

		return info + "<color=cyan>A lightning bolt strikes "+target.name+" for "+LIGHTNING_DAMAGE+" hit points!.</color>\n";
	}

	private IEnumerator delay(Entity target)
	{
		BasicEnemy ai = target.ai;
		target.ai = null;
		yield return new WaitForSeconds(0.5f);

		target.fighterComponent.takeDamage(target, LIGHTNING_DAMAGE);
		target.ai = ai;
	}
}
