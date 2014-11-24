using UnityEngine;
using System.Collections;

public class BasicEnemy
{
	public string takeTurn(Entity self)
	{
		if (self.distanceTo(GameController.objects[0]) >= 2)
			self.moveTowards(GameController.objects[0]);
		else
			return self.fighterComponent.attack(self, GameController.objects[0]);

		return null;
	}
}
