using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FadeInOut : MonoBehaviour {
	
	//function to be called on button click
	public void LoadNextLevel(string name){
		StartCoroutine(LevelLoad(name));
	}
	
	//load level after one sceond delay
	IEnumerator LevelLoad(string name){
		yield return new WaitForSeconds(1f);
		Application.LoadLevel(name);
	}
	
}
