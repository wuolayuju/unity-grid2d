using UnityEngine;
using System.Collections;

public class PopupSpriteSpawner : MonoBehaviour {

	public GameObject popupPrefab;
	
	public Transform positionSpawn;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void spawnTauntPopup()
	{
		Instantiate(popupPrefab, positionSpawn.position, Quaternion.identity);
	}
}
