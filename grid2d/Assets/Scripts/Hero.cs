using UnityEngine;
using System.Collections;

public class Hero : Entity {

	public Hero (Vector2 gridPosition, GameObject prefab, string name)
		:base(gridPosition, prefab, name, true)
	{

	}

	public override void takeTurn()
	{
		//Input.ge

//		if (Input.GetKeyDown(KeyCode.UpArrow))
//			move(gridPosition + Vector2.up);
//		else if (Input.GetKeyDown(KeyCode.DownArrow))
//			move(gridPosition - Vector2.up);
//		else if (Input.GetKeyDown(KeyCode.LeftArrow))
//			move(gridPosition - Vector2.right);
//		else if (Input.GetKeyDown(KeyCode.RightArrow))
//			move(gridPosition + Vector2.right);

		StartCoroutine (WaitForKeyPress ());

	}

	private IEnumerator WaitForKeyPress()
	{
		while(true)
		{
			if (Input.GetButtonDown("up")){
				move(gridPosition + Vector2.up);
				break;
			}
			else if (Input.GetButtonDown("down"))
			{
				move(gridPosition - Vector2.up);
				break;
			}
			else if(Input.GetButtonDown("left"))
			{
				move(gridPosition - Vector2.right);
				break;
			}
			else if (Input.GetButtonDown("right"))
			{
				move(gridPosition + Vector2.right);
				break;
			}
		}
		yield return 0;
	}
}
