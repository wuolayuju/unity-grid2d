using UnityEngine;
using System.Collections;


public class BasicEnemy
{
	public string takeTurn(Entity self, bool patrol)
	{
		// not patroling
		if (!patrol)
		{
			if (self.distanceTo(GameController.objects[0]) >= 2)
				self.moveTowards(GameController.objects[0]);
			else
				return self.fighterComponent.attack(self, GameController.objects[0]);
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
