using UnityEngine;
using System.Collections;

public class LevelUpController : MonoBehaviour {

	public Hero h;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void raiseConstitutionStat()
	{
		h = (Hero) GameController.objects[0];
		h.fighterComponent.max_hp += 20;
		h.gameObject.GetComponentInChildren<DamagePopupSpawner>().spawnDamagePopup("+20 HP", "cyan");
	}

	public void raiseStrengthStat()
	{
		h = (Hero) GameController.objects[0];
		h.fighterComponent.power += 1;
		h.gameObject.GetComponentInChildren<DamagePopupSpawner>().spawnDamagePopup("+1 POW", "cyan");
	}

	public void raiseAgilityStat()
	{
		h = (Hero) GameController.objects[0];
		h.fighterComponent.defense += 1;
		h.gameObject.GetComponentInChildren<DamagePopupSpawner>().spawnDamagePopup("+1 DEF", "cyan");
	}
}
