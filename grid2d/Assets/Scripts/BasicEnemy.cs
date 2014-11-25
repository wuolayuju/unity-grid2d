using UnityEngine;
using System.Collections;


public class BasicEnemy
{
	public string takeTurn(Entity self, PathFinder.Pathfinder pf)
	{
		if (self.distanceTo(GameController.objects[0]) >= 2)
			self.moveTowards(GameController.objects[0], pf);
		else
			return self.fighterComponent.attack(self, GameController.objects[0]);

		return "";
	}
}
