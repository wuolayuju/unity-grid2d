using UnityEngine;
using System.Collections;


public class BasicEnemy
{
	public bool taunted = false;

	public string takeTurn(Entity self, bool patrol)
	{
		// not patroling
		if (!patrol)
		{
			if (self.ai.taunted == false)
			{
				self.gameObject.GetComponentInChildren<PopupSpriteSpawner>().spawnTauntPopup();
				self.ai.taunted = true;
				return "<color=red>"+self.name + " notices you!."+"</color>\n";
			}

			// probability to stall for a turn
			if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
				return "";

			if (self.distanceTo(GameController.objects[0]) >= 2)
			{
				self.moveTowards(GameController.objects[0]);
			}
			else
			{
				if (GameController.objects[0].gridPosition.x > self.gridPosition.x && !self.facingLeft)
					self.Flip();
				if (GameController.objects[0].gridPosition.x < self.gridPosition.x && self.facingLeft)
					self.Flip();

				return self.fighterComponent.attack(self, GameController.objects[0]);
			}
		}
		else
		{
			// patroling = random direction
			int randDir = UnityEngine.Random.Range(1, 5);
			switch(randDir)
			{
			case 1:
				self.move(0, 1); break; // up
			case 2:
				self.move(1, 0); break;// right
			case 3:
				self.move(0, -1); break;// down
			case 4:
				self.move(-1, 0); break;//left
			default:
				break;
			}
		}

		return "";
	}
}
