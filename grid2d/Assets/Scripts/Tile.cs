using UnityEngine;
using System.Collections;

public class Tile{

	public GameObject gamePrefab;

	private Vector2 _position;
	private bool _isBoundary;
	private bool _isOccupied;
	private bool _isVisible;
	private bool _blocksLight;
	private bool _isExplored;

	public Tile (Vector2 position, bool isBoundary, bool isVisible, bool blocksLight, bool isExplored)
	{
		_position = position;
		_isBoundary = isBoundary;
		_isVisible = isVisible;
		_blocksLight = blocksLight;
		_isExplored = isExplored;
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

	public void markTileAsLit()
	{
		gamePrefab.GetComponent<SpriteRenderer> ().color = Color.white;
		_isExplored = true;
	}

	public void markTileAsUnexplored()
	{
		gamePrefab.GetComponent<SpriteRenderer> ().color = Color.black;
		_isExplored = false;
	}

	public void markTileAsExplored()
	{
		gamePrefab.GetComponent<SpriteRenderer> ().color = Color.gray;
		_isExplored = true;
	}
}
