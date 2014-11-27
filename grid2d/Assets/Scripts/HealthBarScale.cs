using UnityEngine;
using System.Collections;

public class HealthBarScale : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setScalePercent (float percentHp)
	{
		transform.localScale = new Vector3(percentHp,
		                                   transform.localScale.y);
	}
}
