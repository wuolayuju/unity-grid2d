using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{
	public int max_hp;
	public int hp;
	public int defense;
	public int power;
	
	public GameObject slashEffect;
	public AudioClip slashSound;
	public AudioClip parrySound;

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
		
		self.GetComponentInChildren<Animator>().Play("attackSide");
		if (isHit < 0.0f)
		{
			AudioSource.PlayClipAtPoint(parrySound, target.transform.position);
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
			Instantiate(slashEffect, target.transform.position, Quaternion.identity);
			AudioSource.PlayClipAtPoint(slashSound, target.transform.position);
			info = target.fighterComponent.takeDamage(target, damage) + info;
			info += self.name + " attacks " + target.name + " for " + damage + " hit points.\n";
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
				string info = "";
				if (!(self is Hero))
				{
					Hero h = (Hero) GameController.objects[0];
					h.experience_points += self.ai.xp;
					info += h.checkLevelUp();
				}

				self.GetComponentInChildren<SpriteRenderer>().sortingOrder -= 1;
				info += self.name + " has been defeated. You gain "+self.ai.xp+" experience points.\n";
				self.ai = null;
				return info;
			}
		}
		return "";
	}
}
