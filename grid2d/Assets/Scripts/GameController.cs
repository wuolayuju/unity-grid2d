using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public Camera cam;

	public MapManager mapManager;

	public int playerVisionRange = 4;
	public GameObject userPlayerPrefab;
	public GameObject AIPlayerPrefab;
	public GameObject healingPotionPrefab;

	public static List<Entity> objects = new List<Entity> ();

	public static Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	public bool turnTaken = false;

	string info = "";

	public bool isGameover = false;

	public GameObject UI_gameOverText;
	public GameObject UI_inventoryPanel;
	public Text UI_combatLog;
	public Image UI_currentHpPlayerBar;
	public Text UI_hpPlayerValues;
	public Image UI_heroPortraitImage;

	// Use this for initialization
	void Start ()
	{
		mapManager.generateMap();
		generatePlayers();
		mapManager.renderMap();
		MapManager.pathfinder = new Pathfinder(mapManager.mapWidth, mapManager.mapHeight);
		mapManager.FOV (objects[0].gridPosition, playerVisionRange);
		cam.GetComponent<CameraController>().LookAtPlayer();
		UI_heroPortraitImage.sprite = userPlayerPrefab.GetComponent<SpriteRenderer>().sprite;
	}

	void OnGUI ()
	{
		UI_combatLog.text = info;

		// Put something inside the ScrollView
		//GUI.Label (new Rect (Screen.width/4, Screen.height/4*3, Screen.width/2, Screen.height/4), info, guiStyle);

		// End the ScrollView
//		GUI.EndScrollView();

		//GUI.Label (new Rect (20, 20, 200, 40), info, labelStyle);
	}
	
	private void generatePlayers ()
	{
		Hero h;
		h = ((GameObject)Instantiate(userPlayerPrefab, playerStartPosition, Quaternion.identity)).GetComponent<Hero>();
		h.gridPosition = playerStartPosition;
		h.name = "Hero";
		h.blocks = true;
		h.fighterComponent = new Fighter (10, 2, 5);
		objects.Add(h);

		Enemy compPlayer;
		for (int nr = 0; nr < MapManager.rooms.Count ; nr+=1)
		{
			Rectangle room = MapManager.rooms[nr];
			Vector2 pos;

			// repeat the randomisation of initial position until it does not matches the ayer's
			do
			{
				pos = new Vector2 (UnityEngine.Random.Range (room.x1, room.x2),
			    			               UnityEngine.Random.Range (room.y1, room.y2));
			}while(pos.x == h.gridPosition.x && pos.y == h.gridPosition.y);

			compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, pos, Quaternion.identity)).GetComponent<Enemy>();
			compPlayer.gridPosition = pos;
			compPlayer.name = "Troll #"+nr;
			compPlayer.blocks = true;
			compPlayer.fighterComponent = new Fighter(10, 0, 3);
			compPlayer.ai = new BasicEnemy();
			objects.Add (compPlayer);

			//items
			pos = new Vector2(room.x1, room.y1);
			Entity it = ((GameObject) Instantiate (healingPotionPrefab,
			                                       pos,
			                                       Quaternion.identity)).GetComponent<Entity>();
			it.gridPosition = pos;
			it.name = "Healing potion";
			it.blocks = false;
			it.item = new Item("This is a healing potion");
			objects.Add(it);
		}
	}


	// Update is called once per frame
	void Update ()
	{
		if (isGameover)
			return;

		updateGUI(); 

		if (objects[0].fighterComponent.hp <= 0)
		{
			UI_gameOverText.gameObject.SetActive(true);
			isGameover = true;
			return;
		}

		// check if there are any player moving
		bool turnFinished = true;
		for (int i = 1; i < objects.Count ; i++)
		{
			if (objects[i].isMoving)
			{
				turnFinished = false;
				break;
			}
		}

		// the user player has taken his turn and has stopped moving
		// now the rest of the players take their turns
		if (!objects [0].isMoving && turnTaken)
		{
			for (int i = 1; i < objects.Count ; i++)
			{
				if (objects[i].ai != null)
				{
					bool patroling;
					if (objects[i].GetComponent<SpriteRenderer>().enabled)
						patroling = false;
					else
						patroling = true;

					info = objects[i].ai.takeTurn(objects[i], patroling) + info;

					//StartCoroutine(TurnDelay());
				}
			}
		}

		// if all the other player has taken their turns, the user player can move
		if (turnFinished && !objects[0].isMoving)
		{
			turnTaken = true;
			if (Input.GetButtonDown("up")){
				info = ((Hero)objects[0]).moveOrAttack(0, 1) + info;
			}
			else if (Input.GetButtonDown("down"))
			{
				info = ((Hero)objects[0]).moveOrAttack(0, -1) + info;
			}
			else if(Input.GetButtonDown("left"))
			{
				info = ((Hero)objects[0]).moveOrAttack(-1, 0) + info;
			}
			else if (Input.GetButtonDown("right"))
			{
				info = ((Hero)objects[0]).moveOrAttack(1, 0) + info;
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				info = "<color=yellow>Turn skipped.</color>\n" + info;
			}
			else if (Input.GetButtonDown("pickup"))
			{
				// check for items in the current tile
				bool areObjectsInTile = false;
				foreach(Entity e in objects)
				{

					// it has to be an item
					if (e.item != null)
					{
						// same position as the player
						if (e.gridPosition.x == objects[0].gridPosition.x && e.gridPosition.y == objects[0].gridPosition.y)
						{
							info = e.item.pickUp(e, (Hero)objects[0]) + info;
							areObjectsInTile = true;
							updateGUIInventory();
							break;
						}
					}
				}
				if (areObjectsInTile == false)
				{
					info = "<color=orange>Nothing to pick up.</color>\n" + info;
				}
			}
			else if (Input.GetButtonDown("useItem1"))
			{
				Hero h = (Hero)objects[0];
				if (h.inventory[0] != null)
				{
					info = h.inventory[0].GetComponentInChildren<ItemEffect>().useItem(h.inventory[0]) + info;
					updateGUIInventory();
				}
			}
			else if (Input.GetButtonDown("useItem2"))
			{
				Hero h = (Hero)objects[0];
				if (h.inventory[1] != null)
				{
					info = h.inventory[1].GetComponentInChildren<ItemEffect>().useItem(h.inventory[1]) + info;
					updateGUIInventory();
				}
			}
			else if (Input.GetButtonDown("useItem3"))
			{
				Hero h = (Hero)objects[0];
				if (h.inventory[2] != null)
				{
					info = h.inventory[2].GetComponentInChildren<ItemEffect>().useItem(h.inventory[2]) + info;
					updateGUIInventory();
				}
			}
			else if (Input.GetButtonDown("useItem4"))
			{
				Hero h = (Hero)objects[0];
				if (h.inventory[3] != null)
				{
					info = h.inventory[3].GetComponentInChildren<ItemEffect>().useItem(h.inventory[3]) + info;
					updateGUIInventory();
				}
			}
			else if (Input.GetButtonDown("useItem5"))
			{
				Hero h = (Hero)objects[0];
				if (h.inventory[4] != null)
				{
					info = h.inventory[4].GetComponentInChildren<ItemEffect>().useItem(h.inventory[4]) + info;
					updateGUIInventory();
				}
			}
			else
			{
				turnTaken = false;
			}

			mapManager.FOV (objects[0].gridPosition, playerVisionRange);
		}

	}

	IEnumerator TurnDelay()
	{
		yield return new WaitForSeconds(0.4f);
	}

	void updateGUI()
	{
		// Health Bar
		int curHP = objects[0].fighterComponent.hp;
		int maxHP = objects[0].fighterComponent.max_hp;

		float percentHpPlayer = (float) curHP / (float) maxHP;
		Vector3 scaleHpBar = UI_currentHpPlayerBar.transform.localScale;
		scaleHpBar.x = percentHpPlayer;
		UI_currentHpPlayerBar.transform.localScale = scaleHpBar;
		UI_hpPlayerValues.text = curHP + "/" + maxHP;	

		//Inventory
//		Image item1Image = UI_inventoryPanel.transform.Find("Item1_Border/Item1_Image").GetComponent<Image>();
//		if (((Hero)objects[0]).inventory.Count > 0)
//		{
//			Entity item = ((Hero)objects[0]).inventory[0];
//			item.transform.position = item1Image.transform.position;
//			item1Image.sprite = item.GetComponent<SpriteRenderer>().sprite;
//			item1Image.color = Color.white;
//		}
	}

	void updateGUIInventory()
	{
		Hero h = (Hero)objects[0];
		for(int i = 0; i < h.MAX_ITEMS_INVENTORY ; i++)
		{
			Image itemImage = UI_inventoryPanel.transform.Find("Item"+(i+1)+"_Border/Item"+(i+1)+"_Image").GetComponent<Image>();

			Entity item = null;

			try 
			{ 
				item = h.inventory[i]; 
			} 
			catch 
			{ 
				item = null; 
			}
			finally
			{
				if (item == null)
				{
					itemImage.color = Color.clear;
				}
				else
				{
					item.transform.position = itemImage.transform.position;
					itemImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
					itemImage.color = Color.white;
				}
			}
		}
	}
}
