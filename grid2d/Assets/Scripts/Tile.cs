using UnityEngine;
using System.Collections;

public class Tile{

	private Vector2 _position;
	private bool _isBoundary;
	private bool _isOccupied;

	public Tile (Vector2 position, bool isBoundary)
	{
		_position = position;
		_isBoundary = isBoundary;
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
}
