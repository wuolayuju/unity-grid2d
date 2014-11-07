using UnityEngine;
using System.Collections;

public class UserPlayer : Player 
{
	public override void MoveToDestPosition ()
	{
		transform.position = destPosition;
		gridPosition = destPosition;
		base.MoveToDestPosition ();
	}
}
