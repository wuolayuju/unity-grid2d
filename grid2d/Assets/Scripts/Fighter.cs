using UnityEngine;
using System.Collections;

public class Fighter
{
	public int max_hp;
	public int hp;
	public int defense;
	public int power;

	public Fighter (int hp, int defense, int power)
	{
		this.max_hp = hp;
		this.hp = hp;
		this.defense = defense;
		this.power = power;
	}

	public string attack(Entity self, Entity target)
	{
		//a simple formula for attack damage
		int damage = self.fighterComponent.power - target.fighterComponent.defense;
			
		string info = "";
		if (damage > 0)
		{
			info = 
				"<color=green>" +self.name + "</color> attacks " + 
					"<color=green>" +target.name + "</color> " + 
				"for <color=red>" + damage + "</color> hit points.\n";
			info += target.fighterComponent.takeDamage(target, damage);
		}
		else
		{
			info = 
				"<color=green>" +self.name + "</color> attacks " + 
					"<color=green>" +target.name + "</color>" + 
				" but it has no effect.\n";
		}

		return info;
	}

	public string takeDamage(Entity self, int damage)
	{
		if (damage > 0)
		{
			hp -= damage;
			self.GetComponent<Animator>().SetTrigger("takeDamage");
			if (hp <= 0)
			{
				self.GetComponent<Animator>().SetTrigger("die");
				//self.GetComponent<SpriteRenderer>().enabled = false;
				//GameController.objects.Remove(self);
				self.blocks = false;
				self.ai = null;
				return "<color=green>" +self.name + "</color> has been defeated.\n";
			}
		}
		return "";
	}
}
