using UnityEngine;
using System.Collections;

public class Item{

	public string description;
	public bool isInInventory;

	public Item (string description)
	{
		this.isInInventory= false;
		this.description = description;
	}

	public string pickUp(Entity self, Hero player)
	{
		if (player.inventory.Count < player.MAX_ITEMS_INVENTORY)
		{
			self.enabled = false;
			self.GetComponent<SpriteRenderer>().enabled = false;
			player.inventory.Add(self);
			isInInventory = true;
			GameController.objects.Remove(self);
			return "<color=lime>You picked up a " +self.name+".</color>\n";
		}
		else
		{
			return "<color=red>Your inventory is full!</color>\n";
		}
	}

}
