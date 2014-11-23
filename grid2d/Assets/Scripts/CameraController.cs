using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;
	public float z_offset = -10;

	private Vector3 offset;

	// Use this for initialization
	void Start ()
	{
		offset = new Vector3(0f, 0f, z_offset);
	}

	public void LookAtPlayer()
	{
		target = GameObject.FindWithTag("Player");
		if (target == null)
			Debug.Log("TARGET NULL");
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		transform.position = target.transform.position + offset;
	}
}
