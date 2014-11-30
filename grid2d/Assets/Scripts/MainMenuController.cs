using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public void StartGame()
	{
		Application.LoadLevel("MainScene");
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
