using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public Vector2 gridPosition;
	public Vector2 destPosition;

	public void MoveToDestPosition()
	{
		transform.position = destPosition;
		gridPosition = destPosition;
	}

	public bool isMovePossible (GameController.DIRECTION dir)
	{
		Vector2 pos = gridPosition;
		//Debug.Log("*** Player position is (" + pos.x + "," + pos.y + ")");
		Tile t = GameController.map[(int)pos.x][(int)pos.y];
		
		if (dir == GameController.DIRECTION.DOWN)
			t = GameController.map[(int)pos.x][(int)pos.y - 1];
		
		else if (dir == GameController.DIRECTION.UP)
			t = GameController.map[(int)pos.x][(int)pos.y + 1];
		
		else if (dir == GameController.DIRECTION.RIGHT)
			t = GameController.map[(int)pos.x + 1][(int)pos.y];
		
		else if (dir == GameController.DIRECTION.LEFT)
			t = GameController.map[(int)pos.x - 1][(int)pos.y];
		
		//Debug.Log("### Tile in destination position (" + t.position.x + "," + t.position.y + ")");
		
		if (t.isBoundary) {
			return false;
		}
		else {
			destPosition = new Vector2(t.position.x, t.position.y);
			return true;
		}
	}
}
