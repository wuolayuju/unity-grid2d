using UnityEngine;
using System.Collections;

public class EntityAnimController : MonoBehaviour {

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

	void finishedMoving()
	{
		transform.parent.GetComponent<Entity> ().finishedMoving ();
	}
}
