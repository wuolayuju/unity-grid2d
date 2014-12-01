using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public void StartGame()
	{
		StartCoroutine(LevelLoad());
		//Application.LoadLevel("MainScene");
	}

	public void ExitGame()
	{
		Application.Quit();
	}
	
	//load level after one sceond delay
	IEnumerator LevelLoad(){
		yield return new WaitForSeconds(1f);
		Application.LoadLevel("MainScene");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
