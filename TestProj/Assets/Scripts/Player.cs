using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MovingObject {

	public static Player Instance;
	
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
	private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif

	private readonly int _defaultHealth = 100;
	//todo: to be used when reaching ending
	private int _deathCounter;

	private const float MoveDelay = 0.2f;

	private const int ConsumableHealValue = 40;

	private bool _onStairs;

	public class PickedFood {
		public readonly int Floor;
		public Vector3 Position;

		public PickedFood(int floor, Vector3 position) {
			Floor = floor;
			Position = position;
		}
	}
	
	[Serializable]
	public class Item {
		public string name;
		public bool isActive;

		public Item(string name, bool isActive) {
			this.name = name;
			this.isActive = isActive;
		}
	}

	[Serializable]
	public class ExtendedParameters : DefaultParameters {
		public int playerLevel;
		public int experience;

		public int armor;
		
		public int blockChance;

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
			blockChance = 0;
			playerLevel = 1;
			storedFood = 0;
		}
	}
	
	public ExtendedParameters parameters;
	public List<PickedFood> pickedFood = new List<PickedFood>();
	
	private Text _levelText;
	private Text _healthText;
	private Text _experienceText;
	private Text _armorText;
	private Text _popUp;
	private Text _storedFood;

	private Animator _animator;
	private static readonly int PlayerChop = Animator.StringToHash("playerChop");
	private static readonly int PlayerHit = Animator.StringToHash("playerHit");
	
	protected override void Start() {
		
		if (Instance == null) {
			Instance = this;
		}
		else if (Instance != this) {
			Destroy(gameObject);
		}
		
		MoveTime = 0.1f;
		
		_animator = GetComponent<Animator>();
		parameters = new ExtendedParameters();

		UpdateTexts();

		GameManager.Instance.ActivateCheckpoint(transform);
		
		base.Start();
		DontDestroyOnLoad(gameObject);
	}

	public void UpdateTexts() {
		_levelText = GameObject.Find("LevelText").GetComponent<Text>();
		_healthText = GameObject.Find("HealthText").GetComponent<Text>();
		_armorText = GameObject.Find("ArmorText").GetComponent<Text>();
		_experienceText = GameObject.Find("ExperienceText").GetComponent<Text>();
		_popUp = GameObject.Find("PopUp").GetComponent<Text>();
		_storedFood = GameObject.Find("StoredFoodText").GetComponent<Text>();
		
		_levelText.text = "Level: " + parameters.playerLevel;
		_healthText.text = "Health: " + parameters.Health;
		_armorText.text = "Armor: " + parameters.armor;
		_experienceText.text = "Experience: " + parameters.experience;
		_popUp.text = "";
		_storedFood.text = "Stored Food: " + parameters.storedFood;
	}
	
	private void Update () {
		
		if(!GameManager.Instance.playersCanMove) return;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (parameters.storedFood > 0) {
				Heal(ConsumableHealValue);
				parameters.storedFood--;
			
				_storedFood.text = "Stored Food: " + parameters.storedFood;
			}
			
			StartCoroutine(WaitTillNextMove());
		}

		else {

			var horizontal = (int) (Input.GetAxisRaw("Horizontal"));
			var vertical = (int) (Input.GetAxisRaw("Vertical"));

			// prevent diagonal movement
			if (horizontal != 0) {
				vertical = 0;
			}

			//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
			
			if (horizontal != 0 || vertical != 0) {
				AttemptMove(horizontal, vertical);
			}
		}
	}

	private bool HasItem(string powerupName) {	
		return parameters.items[parameters.items.FindIndex(x => x.name.Equals(powerupName))].isActive;
	}

	private void CheckLevel(int experienceGained) {

		parameters.experience += experienceGained / parameters.playerLevel;
		
		if (parameters.experience < 100) return;

		parameters.experience -= 100;
		parameters.playerLevel++;

		parameters.AttackPoints += GameManager.Instance.attackPointsPerLevel;
		parameters.armor += GameManager.Instance.armorPerLevel;
	}
	
	protected override void AttemptMove (int xDir, int yDir) {
		
		_popUp.text = "";
		GameManager.Instance.playersCanMove = false;
		
		base.AttemptMove(xDir, yDir);

		_levelText.text = "Level: " + parameters.playerLevel;
		_healthText.text = "Health: " + parameters.Health;
		_armorText.text = "Armor: " + parameters.armor;
		_experienceText.text = "Experience: " + parameters.experience;
		
		if (HitOuterWall) {
			GameManager.Instance.playersCanMove = true;
		}
	}
	
	protected override void OnCantMove(MonoBehaviour component) {
		var hitObj = component;
		if (!hitObj) return;
		
		switch (hitObj) {
			
			case Enemy enemy: {
				if (!HasItem("Sword")) {
					_popUp.text = "You can't attack enemies without a sword";
				}
				else {
					enemy.TakeDamage(parameters.AttackPoints);
					
					if (enemy.isActiveAndEnabled == false) {
						CheckLevel(enemy.parameters.experienceGranted);
					}
					else {
						TakeDamage(enemy.parameters.AttackPoints);	
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
				break;
			}

			case Chest chest: {
				if (!HasItem("Key")) {
					_popUp.text = "You need a key to open the chest";
				}
				else {
					if(chest.IsOpen){
						//todo: end the game, will delete this line
						_popUp.text = "You won!";
					}
					else {
						chest.Open();
					}
				}
				break;
			}
		}

		StartCoroutine(WaitTillNextMove());
	}
	
	private void OnTriggerEnter2D (Collider2D other) {

		// uncollectibles
		
		if(other.CompareTag("Exit")) {
			if (HasItem("Treasure")) {
				// end game
				enabled = false;
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
			SpriteRenderer.flipX = stairs.spriteRenderer.flipX;
			
			stairs.ChangeLevel();

			return;
		}

		if (other.CompareTag("Checkpoint")) {

			if (GameManager.Instance.GetLastCheckpointPosition() != other.transform.position) {
			   _popUp.text = "Checkpoint reached. Saved!";
			}
			
			GameManager.Instance.ActivateCheckpoint(other.transform);
			
			return;
		}

		// collectibles
		
		if(other.CompareTag("Food")) {

			var food = other.GetComponent<Food>();

			if (food.storable) {
				parameters.storedFood++;
				_storedFood.text = "Stored Food: " + parameters.storedFood;
			}
			else {
				var healthPerFood = food.healthValue;

				Heal(healthPerFood);
					
			}
			
				
			pickedFood.Add(food.PickedUp());
		}

		else if (other.CompareTag("Sword")) {
			parameters.items[parameters.items.FindIndex(x => x.name.Equals("Sword"))].isActive = true;
		}
		
		else if (other.CompareTag("Pickaxe")) {
			parameters.items[parameters.items.FindIndex(x => x.name.Equals("Pickaxe"))].isActive = true;
		}
		
		else if (other.CompareTag("Shovel")) {
			parameters.items[parameters.items.FindIndex(x => x.name.Equals("Shovel"))].isActive = true;
		}
		
		else if (other.CompareTag("Key")) {
			parameters.items[parameters.items.FindIndex(x => x.name.Equals("Key"))].isActive = true;
		}
		
		other.gameObject.SetActive (false);
	}

	private void OnTriggerExit2D(Collider2D other) {
		_onStairs = false;
	}

	public override void TakeDamage (int loss) {

		loss = Math.Max(0, loss - parameters.armor);
		
		_animator.SetTrigger (PlayerHit);

		// can block the damage
		var willBlock = Random.Range(0, 100);
		if (parameters.blockChance > willBlock) {
			return;
		}

		parameters.Health -= loss;
		
		if(loss > 0)
			_healthText.text = "-" + loss + " Health: " + parameters.Health;
		
		ChangeToDamagedSprite();
		CheckDeadPlayer ();
	}

	private void CheckDeadPlayer () {
		if (parameters.Health <= 0) {
			//enabled = false;
			//GameManager.Instance.GameOver ();
			RespawnAtCheckpoint();
		}
	}

	private void RespawnAtCheckpoint() {

		_deathCounter++;
		parameters.Health = _defaultHealth;
		
		GameManager.Instance.ReturnToCheckpoint();
	}

	private IEnumerator WaitTillNextMove() {
		yield return new WaitForSeconds(MoveDelay);
		GameManager.Instance.playersCanMove = true;
	}

	private void Heal(int amount) {
		parameters.Health = Math.Min(100, parameters.Health + amount);
		_healthText.text = "+" + amount + " Health: " + parameters.Health;
	}

	public override int GetHealth() {
		return parameters.Health;
	}
}

