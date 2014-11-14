using UnityEngine;
using System.Collections;

public class TilePrefabsHolder : MonoBehaviour {

	public static TilePrefabsHolder instance;

	public GameObject BLANK_FLOOR;
	public GameObject N_WALL;
	public GameObject NW_WALL;
	public GameObject NE_WALL;
	public GameObject NS_WALL;
	public GameObject W_WALL;
	public GameObject WE_WALL;
	public GameObject SW_WALL;
	public GameObject E_WALL;
	public GameObject SE_WALL;
	public GameObject S_WALL;

	public GameObject DEFAULT_TILE;

	// Use this for initialization
	void Awake () 
	{
		instance = this;
	}
}
