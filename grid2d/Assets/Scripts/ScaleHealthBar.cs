using UnityEngine;
using System.Collections;

public class ScaleHealthBar : MonoBehaviour {

	public RectTransform healthBar;

	// Use this for initialization
	void Start () 
	{
		healthBar = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ScalePercent(float percent)
	{
		healthBar.transform.localScale = new Vector3 (percent,
		                                             healthBar.transform.localScale.y);
	}
}
