using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamagePopupSpawner : MonoBehaviour {

	public GameObject popupPrefab;

	public Transform positionSpawn;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void spawnDamagePopup(int damage, string color)
	{
		GameObject damageGameObject = (GameObject) Instantiate(popupPrefab, 
		                                                       positionSpawn.position, 
		                                                       Quaternion.identity);
		
		damageGameObject.GetComponentInChildren<Text>().text = 
			"<color="+color+">"+damage.ToString()+"</color>";
	}
}
