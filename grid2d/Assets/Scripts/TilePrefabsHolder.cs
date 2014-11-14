using UnityEngine;
using System.Collections;

public class TilePrefabsHolder : MonoBehaviour {

	public static TilePrefabsHolder instance;

	public Material spriteMaterial;

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
		BLANK_FLOOR.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		N_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		NW_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		NE_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		NS_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		W_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		WE_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		SW_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		E_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		SE_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		S_WALL.GetComponent<SpriteRenderer> ().material = spriteMaterial;
		DEFAULT_TILE.GetComponent<SpriteRenderer> ().material = spriteMaterial;
	}
}
