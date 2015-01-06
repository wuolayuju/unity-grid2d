using UnityEngine;
using System.Collections;

public class HealEffect : ItemEffect {

	public int hpToHeal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override string useItem (Entity self)
	{
		Hero h = (Hero)GameController.objects[0];

		if (h.fighterComponent.hp == h.fighterComponent.max_hp)
			return "<color=red>You are already at full health.</color>\n";

		int hpHealed = h.fighterComponent.hp;
		h.fighterComponent.hp += hpToHeal;
		if (h.fighterComponent.hp > h.fighterComponent.max_hp)
			h.fighterComponent.hp = h.fighterComponent.max_hp;

		hpHealed = h.fighterComponent.hp - hpHealed;

		AudioSource.PlayClipAtPoint(effectSound, h.transform.position);
		h.gameObject.GetComponentInChildren<DamagePopupSpawner>().spawnDamagePopup(hpHealed.ToString(), "lime");
		h.gameObject.GetComponentInChildren<HealthBarScale>().setScalePercent(Mathf.Clamp((float)h.fighterComponent.hp/(float)h.fighterComponent.max_hp, 0f, 1f));

		h.inventory.Remove(self);

		return "<color=lime>Your wounds start to feel better.</color>\n";
	}
}
