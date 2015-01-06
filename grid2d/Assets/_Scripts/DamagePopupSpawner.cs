using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamagePopupSpawner : MonoBehaviour {

	public GameObject popupPrefab;
	public GameObject canvas;
	public Transform positionSpawn;

	void Awake ()
	{
		canvas = GameObject.Find("BasicUI");
	}

	// Use this for initialization
	void Start ()
	{
		canvas = GameObject.Find("BasicUI");
		//popupPrefab.transform.parent = canvas.transform;
		
//		popupPrefab.transform.Find("DamagePopupPrefab/DamagePopupCanvas").GetComponent<Canvas>().worldCamera = cam;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void spawnDamagePopup(string damage, string color)
	{
		canvas = GameObject.Find("BasicUI");

		GameObject damageGameObject = (GameObject) Instantiate(popupPrefab, 
		                                                       positionSpawn.position, 
		                                                       Quaternion.identity);
	
		damageGameObject.transform.parent = canvas.transform;
		damageGameObject.transform.localScale = Vector3.one;
		
		damageGameObject.GetComponentInChildren<Text>().text = 
			"<color="+color+">"+damage+"</color>";
	}
}
