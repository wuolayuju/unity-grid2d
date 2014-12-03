using UnityEngine;
using System.Collections;

public class KillEntity : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void killEntity()
	{
		transform.parent.GetComponent<Entity> ().killEntity ();
	}
}
