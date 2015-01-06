using UnityEngine;
using System.Collections;

public class ItemEffect : MonoBehaviour {

	public AudioClip effectSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual string useItem(Entity self)
	{
		return null;
	}
}
