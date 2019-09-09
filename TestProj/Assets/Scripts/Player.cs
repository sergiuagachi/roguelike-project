using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MovingObject {

	[Serializable]
	public class ExtendedParameters : DefaultParameters {
		public int playerLevel;
		public int experience;

		public int armor;

		public int storedFood;
		
		public List<Item> items = new List<Item>{
			new Item("Sword", false),
			new Item("Key", false),
			new Item("Treasure", false),
			new Item("Shovel", false),
			new Item("Pickaxe", false),
		};

		public ExtendedParameters() : base(100, 50){
			armor = 0;
			playerLevel = 1;
			storedFood = 0;
		}
	}
	private class Quest {
		public readonly string Info;
		public bool Discovered;
		public bool Completed;

		public Quest(string info, bool discovered) {
			Info = info;
			Discovered = discovered;
		}
	}
	
	public static Player Instance;
	
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
	private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif

	private const int DefaultHealth = 100;
	private const int ArmorPerLevel = 10;
	private const int AttackPointsPerLevel = 25;
	private const float MoveDelay = 0.2f;
	private const int ConsumableHealValue = 40;

	public ExtendedParameters parameters;
	public List<ItemLocation> pickedItems = new List<ItemLocation>();
	private readonly List<Quest> _quests = new List<Quest> {
		new Quest("Find the treasure", false),
		new Quest("Find the key", false),
		new Quest("Find the shovel", false),
		new Quest("Find the pickaxe", false),
		new Quest("Find the sword", false),
	};
	
	private readonly Statistics _statistics = new Statistics();
	public Statistics Statistics => _statistics;
	
	private Text _levelText;
	private Text _healthText;
	private Text _experienceText;
	private Text _armorText;
	private Text _damageText;
	private Text _popUp;
	private Text _questLog;

	private Image _swordImage;
	private Image _pickaxeImage;
	private Image _shovelImage;
	private Image _keyImage;
	private Food _foodImage;
	private Text _amountText;
	
	private bool _onStairs;
	
	private Animator _animator;
	
	private static readonly int PlayerChop = Animator.StringToHash("playerChop");
	private static readonly int PlayerHit = Animator.StringToHash("playerHit");
	
	protected override void Start() {
		base.Start();
		
		if (Instance == null) {
			Instance = this;
		}
		else if (Instance != this) {
			Destroy(gameObject);
		}
		
		MoveTime = 0.1f;
		
		_animator = GetComponent<Animator>();
		parameters = new ExtendedParameters();

		GetUi();

		GameManager.Instance.ActivateCheckpoint(transform);
		
		DontDestroyOnLoad(gameObject);
	}

	public void GetUi() {
		_levelText = GameObject.Find("LevelText").GetComponent<Text>();
		_healthText = GameObject.Find("HealthText").GetComponent<Text>();
		_armorText = GameObject.Find("ArmorText").GetComponent<Text>();
		_damageText = GameObject.Find("DamageText").GetComponent<Text>();
		_experienceText = GameObject.Find("ExperienceText").GetComponent<Text>();
		_popUp = GameObject.Find("PopUp").GetComponent<Text>();
		_questLog = GameObject.Find("QuestInfo").GetComponent<Text>();

		_swordImage = GameObject.Find("SwordImage").GetComponent<Image>();
		_pickaxeImage = GameObject.Find("PickaxeImage").GetComponent<Image>();
		_shovelImage = GameObject.Find("ShovelImage").GetComponent<Image>();
		_keyImage = GameObject.Find("KeyImage").GetComponent<Image>();
		_foodImage = GameObject.Find("FoodImage").GetComponent<Food>();
		_amountText = GameObject.Find("Amount").GetComponent<Text>();
		
		UpdateUi();
	}

	private void UpdateUi() {
		_levelText.text = "Level: " + parameters.playerLevel;
		_healthText.text = "Health: " + parameters.Health;
		_armorText.text = "Armor: " + parameters.armor;
		_damageText.text = "Damage: " + parameters.AttackPoints;
		_experienceText.text = "Experience: " + parameters.experience;
		_popUp.text = "";

		_questLog.text = "";
		foreach (var quest in _quests) {
			if (quest.Discovered && !quest.Completed) {
				_questLog.text += quest.Info + "\n";
			}
			else {
				_questLog.text += "\n";
			}
		}

		_swordImage.enabled = HasItem("Sword");
		_pickaxeImage.enabled = HasItem("Pickaxe");
		_shovelImage.enabled = HasItem("Shovel");
		_keyImage.enabled = HasItem("Key");
		_amountText.text = "x" + parameters.storedFood;
	}

	private void Update () {
		GameManager.Instance.timeElapsed += Time.deltaTime;
		
		if(!GameManager.Instance.playerCanMove) return;

		else {
			var horizontal = (int) Input.GetAxisRaw("Horizontal");
			var vertical = (int) Input.GetAxisRaw("Vertical");

			// prevent diagonal movement
			if (horizontal != 0) {
				vertical = 0;
			}

#if UNITY_ANDROID
			if (Input.touchCount > 0) {
				var myTouch = Input.touches[0];
				if (myTouch.phase == TouchPhase.Began){
					touchOrigin = myTouch.position;
				}
				
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0){
					Vector2 touchEnd = myTouch.position;
					
					var x = touchEnd.x - touchOrigin.x;
					var y = touchEnd.y - touchOrigin.y;
					
					touchOrigin.x = -1;

					if (Mathf.Abs(x) > Mathf.Abs(y)) {
						horizontal = x > 0 ? 1 : -1;
					}
					else {
						vertical = y > 0 ? 1 : -1;
					}
				}
			}		
#endif
			
			if (horizontal != 0 || vertical != 0) {
				AttemptMove(horizontal, vertical);
			}
		}
	}

	private bool HasItem(string itemName) {	
		return parameters.items[parameters.items.FindIndex(x => x.name.Equals(itemName))].isActive;
	}

	private void AcquireItem(string itemName) {
		parameters.items[parameters.items.FindIndex(x => x.name.Equals(itemName))].isActive = true;
		_popUp.text = itemName + " obtained!";

		if (itemName == "Sword")
			_swordImage.enabled = true;
		else if (itemName == "Pickaxe")
			_pickaxeImage.enabled = true;
		else if (itemName == "Shovel")
			_shovelImage.enabled = true;
		else if (itemName == "Key") 
			_keyImage.enabled = true;
	}

	private void AssignExperience(int experienceGained) {

		_statistics.EnemiesKilled++;
		
		parameters.experience += experienceGained / parameters.playerLevel;
		
		if (parameters.experience < 100) return;

		parameters.experience -= 100;
		parameters.playerLevel++;

		parameters.AttackPoints += AttackPointsPerLevel;
		parameters.armor += ArmorPerLevel;
	}
	
	protected override void AttemptMove (int xDir, int yDir) {
		
		UpdateUi();
		
		GameManager.Instance.playerCanMove = false;

		_statistics.StepsTaken++;
		base.AttemptMove(xDir, yDir);

		if (HitOuterWall) {
			GameManager.Instance.playerCanMove = true;
			_statistics.StepsTaken--;
		}
	}
	
	protected override void OnCantMove(MonoBehaviour hitObj) {
		_statistics.StepsTaken--;
		
		switch (hitObj) {
			
			case Enemy enemy: {
				if (!HasItem("Sword")) {
					_popUp.text = "You can't attack enemies without a sword";
					_quests[4].Discovered = true;

					var hiddenWall = GameObject.Find("HiddenWall (1)");
					if (hiddenWall) {
						pickedItems.Add(hiddenWall.GetComponent<UniqueItem>().PickUp());
						hiddenWall.SetActive (false);
					}
						
				}
				else {
					_statistics.DamageDealt += enemy.TakeDamage(parameters.AttackPoints);
					
					if (enemy.isActiveAndEnabled == false) {
						AssignExperience(enemy.parameters.experienceGranted);
					}
					else {
						_statistics.DamageReceived += TakeDamage(enemy.parameters.AttackPoints);	
					}
					
					_animator.SetTrigger(PlayerChop);
				}
				break;
			}

			// todo: remove duplicate code
			case Wall wall: {
				switch (wall.type) {
					
					case Wall.Type.Dirt: {
					if (!HasItem("Shovel")) {
						_popUp.text = "You can't pass without a Shovel";
						_quests[2].Discovered = true;
					}
					else {
						_animator.SetTrigger(PlayerChop);
						wall.gameObject.SetActive(false);
						wall.enabled = false;
					}

					break;
					}

					case Wall.Type.Stone: {
					if (!HasItem("Pickaxe")) {
						_popUp.text = "You can't pass without a Pickaxe";
						_quests[3].Discovered = true;
					}
					else {
						_animator.SetTrigger(PlayerChop);
						wall.gameObject.SetActive(false);
						wall.enabled = false;
					}
					break;
					}

					default: {
					throw new ArgumentOutOfRangeException();
					}
				}
				break;
			}

			case Sign sign: {
				_popUp.text = sign.signText;

				if (sign.name == "Sign (1)") {
					_quests[0].Discovered = true;
					
					var hiddenWall = GameObject.Find("HiddenWall (2)");
					if (hiddenWall) {
						pickedItems.Add(hiddenWall.GetComponent<UniqueItem>().PickUp());
						hiddenWall.SetActive(false);
					}
				}
				
				break;
			}

			case Chest chest: {
				if (!HasItem("Key")) {
					_popUp.text = "Locked! You need a key to open the chest";
					_quests[1].Discovered = true;
				}
				else {
					if(chest.IsOpen){
						_popUp.text = "You obtained the treasure!";
						_quests[0].Completed = true;
					}
					else {
						chest.Open();
						AcquireItem("Treasure");
					}
				}
				break;
			}
		}

		StartCoroutine(WaitTillNextMove());
	}
	
	private void OnTriggerEnter2D (Collider2D other) {

		//// uncollectibles ////
		if(other.CompareTag("Exit")) {
			if (HasItem("Treasure")) {
				// end game
				GameManager.EndGame();
			}
			else {
				_popUp.text = "Without the treasure, the exit is closed";
			}
			return;
		}

		if (other.CompareTag("Stairs")) {
			if (_onStairs) {
				return;
			}

			_onStairs = true;
			
			var stairs = other.GetComponent<Stairs>();
			SpriteRenderer.flipX = !stairs.spriteRenderer.flipX;
			
			stairs.ChangeFloor();
			_statistics.FloorsChanged++;
			
			return;
		}

		if (other.CompareTag("Checkpoint")) {
			if (GameManager.Instance.GetLastCheckpointPosition() != other.transform.position) {
			   _popUp.text = "Checkpoint reached. Saved!";
			}
			
			GameManager.Instance.ActivateCheckpoint(other.transform);
			return;
		}

		//// collectibles ////
		if(other.CompareTag("Food")) {

			var food = other.GetComponent<Food>();

			if (food.storable) {
				parameters.storedFood++;
			}
			else {
				var healthPerFood = food.healthValue;
				Heal(healthPerFood);
			}
		}

		else if (other.CompareTag("Sword")) {
			_quests[4].Completed = true;
			AcquireItem("Sword");
		}
		
		else if (other.CompareTag("Pickaxe")) {
			_quests[3].Completed = true;
			AcquireItem("Pickaxe");
		}
		
		else if (other.CompareTag("Shovel")) {
			_quests[2].Completed = true;
			AcquireItem("Shovel");
		}
		
		else if (other.CompareTag("Key")) {
			_quests[1].Completed = true;
			AcquireItem("Key");
		}
		
		pickedItems.Add(other.GetComponent<UniqueItem>().PickUp());
		other.gameObject.SetActive (false);
	}

	private void OnTriggerExit2D(Collider2D other) {
		_onStairs = false;
	}

	public override int TakeDamage (int loss) {

		loss = Math.Max(0, loss - parameters.armor);
		
		_animator.SetTrigger (PlayerHit);

		parameters.Health -= loss;
		
		if(loss > 0)
			_healthText.text = "-" + loss + " Health: " + parameters.Health;

		var damageReceived = parameters.Health + loss;
		
		ChangeToDamagedSprite();
		CheckDeadPlayer ();

		return damageReceived;
	}

	private void CheckDeadPlayer () {
		if (parameters.Health <= 0) {
			RespawnAtCheckpoint();
		}
	}

	private void RespawnAtCheckpoint() {

		_statistics.DeathCounter++;
		parameters.Health = DefaultHealth;
		
		GameManager.Instance.ReturnToCheckpoint();
	}

	private IEnumerator WaitTillNextMove() {
		yield return new WaitForSeconds(MoveDelay);
		GameManager.Instance.playerCanMove = true;
	}

	public void Heal(int amount) {

		if (parameters.storedFood <= 0) {
			_popUp.text = "You have no food stored!";
			return;
		}
		
		parameters.storedFood--;
		_amountText.text = "x" + parameters.storedFood;
		
		var oldHealth = parameters.Health;
		parameters.Health = Math.Min(100, parameters.Health + amount);

		if (parameters.Health >= 100) {
			_statistics.TotalHeal += parameters.Health - oldHealth;
		}
		else {
			_statistics.TotalHeal += amount;
		}
		
		_healthText.text = "+" + amount + " Health: " + parameters.Health;
	}

	public override int GetHealth() {
		return parameters.Health;
	}
}

