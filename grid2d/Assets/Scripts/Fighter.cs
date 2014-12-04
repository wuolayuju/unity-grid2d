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
		// Hit if rnd()*ATK > rnd()*DEF
		float isHit = 
			self.fighterComponent.power * UnityEngine.Random.Range(0f,1f) 
			-
			target.fighterComponent.defense * UnityEngine.Random.Range(0f,1f) ;

		//Debug.Log(self.name+"->"+target.name+" = "+isHit);

		if (isHit < 0.0f)
		{
			target.gameObject.GetComponentInChildren<DamagePopupSpawner>().spawnDamagePopup("miss", "red");
			return self.name + " attacks " + target.name + " but it misses.\n";
		}

		//a simple formula for attack damage
		int damage = UnityEngine.Random.Range(self.fighterComponent.power - target.fighterComponent.defense,
		                                      self.fighterComponent.power - target.fighterComponent.defense + Mathf.RoundToInt(isHit));

		//int damage = self.fighterComponent.power - target.fighterComponent.defense + Mathf.RoundToInt(isHit);
			
		string info = "";
		if (damage > 0)
		{
			info = target.fighterComponent.takeDamage(target, damage) + info;
			info = self.name + " attacks " + target.name + " for " + damage + " hit points.\n";
		}
		else
		{
			info = self.name + " attacks " + target.name + " but it has no effect.\n";
		}

		return info;
	}

	public string takeDamage(Entity self, int damage)
	{
		if (damage > 0)
		{
			hp -= damage;
			self.GetComponentInChildren<Animator>().Play("takeDamage");
			self.gameObject.GetComponentInChildren<HealthBarScale>().setScalePercent(Mathf.Clamp((float)hp/(float)max_hp, 0f, 1f));
			//if (self.ai != null)
				self.gameObject.GetComponentInChildren<DamagePopupSpawner>().spawnDamagePopup(damage.ToString(), "red");
			if (hp <= 0)
			{
				self.GetComponentInChildren<Animator>().SetTrigger("die");
				self.blocks = false;
				self.ai = null;
				self.GetComponentInChildren<SpriteRenderer>().sortingOrder -= 1;
				return self.name + " has been defeated.\n";
			}
		}
		return "";
	}
}
