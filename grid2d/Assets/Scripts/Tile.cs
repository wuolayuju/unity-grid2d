using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile{

	public GameObject gamePrefab;

	private Vector2 _position;
	private bool _isBoundary;
	private bool _isOccupied;
	private bool _isVisible;
	private bool _blocksLight;
	private bool _isExplored;
	private bool _isLit;

	private List<Entity> _objects;

	private Color COLOR_LIT = new Color32(215, 215, 215, 255);
	private Color COLOR_EXPLORE = new Color32(80, 80, 80, 255);
	private Color COLOR_UNEXPLORED = new Color32(0, 0, 0, 255);

	public Tile (Vector2 position, bool isBoundary, bool isVisible, bool blocksLight, bool isExplored, bool isLit)
	{
		_position = position;
		_isBoundary = isBoundary;
		_isVisible = isVisible;
		_blocksLight = blocksLight;
		_isExplored = isExplored;
		_isLit = isLit;
		_objects = new List<Entity> ();
	}

	public Vector2 position
	{
		get { return _position; }
		set { _position = value; }
	}

	public bool isBoundary
	{
		get { return _isBoundary; }
		set { _isBoundary = value; }
	}

	public bool isOccupied
	{
		get { return _isOccupied; }
		set { _isOccupied = value; }
	}

	public bool isVisible
	{
		get { return _isVisible; }
		set { _isVisible = value; }
	}

	public bool blocksLight
	{
		get { return _blocksLight; }
		set { _blocksLight = value; }
	}

	public bool isExplored
	{
		get { return _isExplored; }
		set { _isExplored = value; }
	}

	public bool isLit
	{
		get { return _isLit; }
		set { _isLit = value; }
	}

	public void addEntity (Entity e)
	{
		_objects.Add (e);
	}

	public List<Entity> getObjects ()
	{
		return _objects;
	}

	public void markTileAsLit()
	{
		gamePrefab.GetComponent<SpriteRenderer> ().color = COLOR_LIT;

		_isLit = true;
		_isExplored = true;
	}

	public void markTileAsUnexplored()
	{
		gamePrefab.GetComponent<SpriteRenderer> ().color = COLOR_UNEXPLORED;
		_isLit = false;
		_isExplored = false;
	}

	public void markTileAsExplored()
	{
		gamePrefab.GetComponent<SpriteRenderer> ().color = COLOR_EXPLORE;
	}
}
