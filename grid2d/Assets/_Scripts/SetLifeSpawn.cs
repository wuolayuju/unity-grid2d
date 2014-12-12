using UnityEngine;
using System.Collections;

public class SetLifeSpawn : MonoBehaviour {

	// Object can be a GameObject or a component
	public Object myGameObjectOrComponent;
	public float timer;
	
	void Start(){
		// Default is the gameObject
		if (myGameObjectOrComponent == null)
			myGameObjectOrComponent = gameObject;
		
		// Destroy works with GameObjects and Components
		Destroy(myGameObjectOrComponent, timer);
	}
}
