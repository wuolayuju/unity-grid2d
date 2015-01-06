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
	public GameObject lightningScrollPrefab;

	public static List<Entity> objects;

	public static Vector2 playerStartPosition;

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE };

	public bool turnTaken = false;

	string info = "";

	public AudioClip pickupSound;

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
	public GameObject UI_levelUpPanel;

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
		updateGUIInventory();
//		UI_heroPortraitImage.sprite = userPlayerPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
	}

	void OnGUI ()
	{
		UI_combatLog.text = info;
	}
	
	private void generatePlayers ()
	{
		Hero h;

		// Try if the user player already exists
		try
		{
			h = GameObject.FindWithTag("PlayerTransform").GetComponent<Hero>();
		}
		catch(NullReferenceException ex)
		{
			h = null;
		}

		// If it doesn't, create a new user player with level 1
		if (h == null)
			h = ((GameObject)Instantiate(userPlayerPrefab, playerStartPosition, Quaternion.identity)).GetComponent<Hero>();

		DontDestroyOnLoad(h);

		h.gridPosition = playerStartPosition;
		h.transform.position = playerStartPosition;
		h.blocks = true;
		objects.Add(h);

		/*
		 * NPC Players
		 */
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
			compPlayer.blocks = true;
			objects.Add (compPlayer);

			/*
			 * ITEMS
			 */ 
			Entity it;
			if (UnityEngine.Random.Range(0,9) > 3)
			{
				pos = new Vector2(room.x1, room.y1);
				it = ((GameObject) Instantiate (healingPotionPrefab,
				                                       pos,
				                                       Quaternion.identity)).GetComponent<Entity>();
				it.gridPosition = pos;
				it.blocks = false;
				it.item = new Item("Heals up to 10 health points");
				objects.Add(it);
			}

			pos = new Vector2(room.x2-1, room.y2-1);
			it = ((GameObject) Instantiate (lightningScrollPrefab,
			                                       pos,
			                                       Quaternion.identity)).GetComponent<Entity>();
			it.gridPosition = pos;
			it.blocks = false;
			int damageLightning = lightningScrollPrefab.GetComponent<LightningEffect>().LIGHTNING_DAMAGE;
			it.item = new Item("Strikes closest enemy for "+/*10*/ damageLightning+" HP");
			objects.Add(it);

		}
	}

	// Update is called once per frame
	void Update ()
	{
		// RESTART GAME
		if (isGameover)
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				Destroy(objects[0]);
				Application.LoadLevel("MainScene");
			}
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

		if (((Hero)objects[0]).hasLeveledUp)
		{
			UI_levelUpPanel.gameObject.SetActive(true);
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
		if (!objects [0].isMoving && turnTaken && turnFinished)
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
							AudioSource.PlayClipAtPoint(pickupSound, e.gridPosition);
							DontDestroyOnLoad(e);
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
//		try
//		{
			Hero h = (Hero)objects[0];
			if (h.inventory[slot] != null)
			{
				ItemEffect ie = h.inventory[slot].GetComponentInChildren<ItemEffect>();
				string info = ie.useItem(h.inventory[slot]);
				return info;
				//return h.inventory[slot].GetComponentInChildren<ItemEffect>().useItem(h.inventory[slot]);
			}
//		}
//		catch
//		{
//			return "<color=orange>No item in that slot.</color>\n";
//		}

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
		Hero h = (Hero) objects[0];
		UI_playerStatsPanel.transform.Find("StatsExpPanel/ExpPanel/ExpText").GetComponent<Text>().text =
			"EXP\n"+
			h.experience_points.ToString() + "/" +
			(h.LEVEL_UP_BASE + h.level * h.LEVEL_UP_FACTOR).ToString() +
			"\nXP";
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
		Hero h = (Hero)objects[0];
		// Heal the player for half the max hp
		h.fighterComponent.hp = Mathf.Clamp(h.fighterComponent.hp + (h.fighterComponent.max_hp / 2), 
		                                     0, 
		                                     h.fighterComponent.max_hp);
		h.gameObject.GetComponentInChildren<HealthBarScale>().setScalePercent(Mathf.Clamp((float)h.fighterComponent.hp/(float)h.fighterComponent.max_hp, 0f, 1f));
		//h.inventory.Clear();

		h.onExit = false;
		Application.LoadLevel("MainScene");
	}

	public void getBackToPlay()
	{
		UI_descendPanel.SetActive(false);
		UI_gamePausedPanel.SetActive (false);
		UI_levelUpPanel.SetActive (false);
		inMenu = false;
		((Hero)objects[0]).onExit = false;
		((Hero)objects[0]).hasLeveledUp = false;
	}

	public void exitGame()
	{
		Application.Quit();
	}
}
