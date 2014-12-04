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

	public static List<Entity> objects;

	public static Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	public bool turnTaken = false;

	string info = "";

	public bool isGameover = false;
	public bool inMenu = false;

	public GameObject UI_gameOverText;
	public GameObject UI_inventoryPanel;
	public GameObject UI_playerStatsPanel;
	public Text UI_combatLog;
	public Image UI_currentHpPlayerBar;
	public Text UI_hpPlayerValues;
	public Image UI_heroPortraitImage;
	public GameObject UI_descendPanel;
	public GameObject UI_gamePausedPanel;

	void Awake()
	{
		objects = new List<Entity>();
	}

	// Use this for initialization
	void Start ()
	{
		mapManager.generateMap();
		generatePlayers();
		mapManager.renderMap();
		MapManager.pathfinder = new Pathfinder(mapManager.mapWidth, mapManager.mapHeight);
		mapManager.FOV (objects[0].gridPosition, playerVisionRange);
		cam.GetComponent<CameraController>().LookAtPlayer();
//		UI_heroPortraitImage.sprite = userPlayerPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
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
		h.fighterComponent = new Fighter (15, 3, 6);
		objects.Add(h);

		Enemy compPlayer;
		for (int nr = 0; nr < MapManager.rooms.Count ; nr+=1)
		{
			Rectangle room = MapManager.rooms[nr];
			Vector2 pos;

			// repeat the randomisation of initial position until it does not matches the player's
			do
			{
				pos = new Vector2 (UnityEngine.Random.Range (room.x1, room.x2),
			    			               UnityEngine.Random.Range (room.y1, room.y2));
			}while(pos.x == h.gridPosition.x && pos.y == h.gridPosition.y);

			compPlayer = ((GameObject) Instantiate (AIPlayerPrefab, pos, Quaternion.identity)).GetComponent<Enemy>();
			compPlayer.gridPosition = pos;
			compPlayer.name = "Lizard";
			compPlayer.blocks = true;
			compPlayer.fighterComponent = new Fighter(3, 2, 5);
			compPlayer.ai = new BasicEnemy();
			objects.Add (compPlayer);

			//items
			if (UnityEngine.Random.Range(0,9) > 3)
			{
				pos = new Vector2(room.x1, room.y1);
				Entity it = ((GameObject) Instantiate (healingPotionPrefab,
				                                       pos,
				                                       Quaternion.identity)).GetComponent<Entity>();
				it.gridPosition = pos;
				it.name = "Healing potion";
				it.blocks = false;
				it.item = new Item("Heals up to 10 health points");
				objects.Add(it);
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		// RESTART GAME
		if (isGameover)
		{
			if (Input.GetKeyDown(KeyCode.R))
				Application.LoadLevel("MainScene");
			return;
		}

		// PAUSE THE GAME IF IN MENU
		if (inMenu)
		{
			return;
		}

		updateGUI(); 

		// PLAYER DEATH
		if (objects[0].fighterComponent.hp <= 0)
		{
			UI_gameOverText.gameObject.SetActive(true);
			isGameover = true;
			return;
		}

		// PAUSE MENU
		if (Input.GetKey (KeyCode.Escape))
		{
			UI_gamePausedPanel.gameObject.SetActive(true);
			inMenu = true;
			return;
		}

		// DESCEND TO NEXT LEVEL
		if (((Hero)objects[0]).onExit && inMenu == false)
		{
			UI_descendPanel.gameObject.SetActive(true);
			inMenu = true;
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
					if (objects[i].GetComponentInChildren<SpriteRenderer>().enabled)
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
				info = useItemInSlot(0) + info;
				updateGUIInventory();
			}
			else if (Input.GetButtonDown("useItem2"))
			{
				info = useItemInSlot(1) + info;
				updateGUIInventory();
			}
			else if (Input.GetButtonDown("useItem3"))
			{
				info = useItemInSlot(2) + info;
				updateGUIInventory();
			}
			else if (Input.GetButtonDown("useItem4"))
			{
				info = useItemInSlot(3) + info;
				updateGUIInventory();
			}
			else if (Input.GetButtonDown("useItem5"))
			{
				info = useItemInSlot(4) + info;
				updateGUIInventory();

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

	string useItemInSlot(int slot)
	{
		try
		{
			Hero h = (Hero)objects[0];
			if (h.inventory[slot] != null)
			{
				return h.inventory[slot].GetComponentInChildren<ItemEffect>().useItem(h.inventory[slot]);
			}
		}
		catch
		{
			return "<color=orange>No item in that slot.</color>\n";
		}

		return "";
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

		// stats
		UI_playerStatsPanel.transform.Find("StatsExpPanel/AttackText").GetComponent<Text>().text = objects[0].fighterComponent.power.ToString();
		UI_playerStatsPanel.transform.Find("StatsExpPanel/DefenceText").GetComponent<Text>().text = objects[0].fighterComponent.defense.ToString();
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

	public void descendToNextLevel()
	{
		Application.LoadLevel("MainScene");
	}

	public void getBackToPlay()
	{
		UI_descendPanel.SetActive(false);
		UI_gamePausedPanel.SetActive (false);
		inMenu = false;
		((Hero)objects[0]).onExit = false;
	}

	public void exitGame()
	{
		Application.Quit();
	}
}
