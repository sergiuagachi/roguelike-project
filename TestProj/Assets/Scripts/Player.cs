using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MovingObject {

	public static Player Instance;
	
	private const int Food1Health = 10;
	private const int Food2Health = 20;

	private const int Drink1Energy = 10;
	private const int Drink2Energy = 20;

	private bool _onStairs;

	private Vector3 _lastCheckpointPosition;
	
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
		
		public List<Item> items = new List<Item>{
			new Item("Sword", false),
			new Item("Key", false),
			new Item("Treasure", false),
			new Item("Shovel", false),
			new Item("Pickaxe", false),
		};

		public ExtendedParameters() {
			Health = 100;
			armor = 0;
			blockChance = 0;
			AttackPoints = 50;
			playerLevel = 1;
		}
	}
	
	public ExtendedParameters parameters;
	
	private Text _levelText;
	private Text _healthText;
	private Text _experienceText;
	private Text _armorText;
	private Text _popUp;

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

		_lastCheckpointPosition = transform.position;
		
		base.Start();
		DontDestroyOnLoad(gameObject);
	}

	public void UpdateTexts() {
		_levelText = GameObject.Find("LevelText").GetComponent<Text>();
		_healthText = GameObject.Find("HealthText").GetComponent<Text>();
		_armorText = GameObject.Find("ArmorText").GetComponent<Text>();
		_experienceText = GameObject.Find("ExperienceText").GetComponent<Text>();
		_popUp = GameObject.Find("PopUp").GetComponent<Text>();
		
		_levelText.text = "Level: " + parameters.playerLevel;
		_healthText.text = "Health: " + parameters.Health;
		_armorText.text = "Armor: " + parameters.armor;
		_experienceText.text = "Experience: " + parameters.experience;
		_popUp.text = "";
	}
	
	private void Update () {
		
		if(!GameManager.Instance.playersCanMove) return;

		var horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
		var vertical = (int) (Input.GetAxisRaw ("Vertical"));
		
		// prevent diagonal movement
		if(horizontal != 0) {
			vertical = 0;
		}
		
		if(horizontal != 0 || vertical != 0) {
			AttemptMove(horizontal, vertical);
		}
	}

	private bool HasItem(string powerupName) {	
		return parameters.items[parameters.items.FindIndex(x => x.name.Equals(powerupName))].isActive;
	}

	private void CheckLevel(int experienceGained) {

		parameters.experience += (experienceGained % parameters.playerLevel);
		
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

		if (hitObj) {
			if (hitObj is Enemy enemy) {
				
				if (!HasItem("Sword")) {
					_popUp.text = "You can't attack enemies without a sword";
				}
				else {
					enemy.TakeDamage(parameters.AttackPoints);
					TakeDamage(enemy.Parameters.AttackPoints);

					if (enemy.isActiveAndEnabled == false) {
						CheckLevel(enemy.Parameters.ExperienceGranted);
					}
					
					_animator.SetTrigger(PlayerChop);
				}
			}

			if (hitObj is Wall wall) {
				// todo: remove duplicate code

				switch (wall.type) {
					case Wall.Type.Dirt:
						if (!HasItem("Shovel")) {
							_popUp.text = "You can't pass without a Shovel";
						}
						else {
							_animator.SetTrigger(PlayerChop);
							wall.gameObject.SetActive(false);
							wall.enabled = false;
						}
						break;
					case Wall.Type.Stone:
						if (!HasItem("Pickaxe")) {
							_popUp.text = "You can't pass without a Pickaxe";	
						}
						else {
							_animator.SetTrigger(PlayerChop);
							wall.gameObject.SetActive(false);
							wall.enabled = false;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (hitObj is Sign sign) {
				_popUp.text = sign.signText;
			}

			if (hitObj is Chest chest) {

				if (!HasItem("Key")) {
					_popUp.text = "You need a key to open the chest";
				}
				else {
					if (chest.IsOpen) {
						// end the game, will delete this line
						_popUp.text = "You won!";
					}
					else {
						chest.Open();	
					}
				}
			}
			
			StartCoroutine(WaitTillNextMove());
		}
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
			stairs.ChangeLevel();

			return;
		}

		if (other.CompareTag("Checkpoint")) {

			if (_lastCheckpointPosition != other.transform.position) {
			   _popUp.text = "Checkpoint reached. Saved!";
			}
			
			_lastCheckpointPosition = other.transform.position;
			
			return;
		}

		// collectibles
		
		if(other.CompareTag("Food")) {
			var healthPerFood = other.name.Equals("Food1") ? Food1Health : Food2Health;

			parameters.Health = Math.Min(100, parameters.Health + healthPerFood);
			_healthText.text = "+" + healthPerFood + " Health: " + parameters.Health;
		}
		
		else if(other.CompareTag("Drink")) {
			var energyPerDrink = other.name.Equals("Drink1") ? Drink1Energy : Drink2Energy;
			
			parameters.experience = Math.Min(100, parameters.experience + energyPerDrink);
			_experienceText.text = "+" + energyPerDrink + " Experience: " + parameters.experience;
		}

		else if (other.CompareTag("Sword")) {
			parameters.items[parameters.items.FindIndex(x => x.name.Equals("Sword"))].isActive = true;
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
		transform.position = _lastCheckpointPosition;
		GameManager.Instance.ReturnToCheckpoint();
	}

	private IEnumerator WaitTillNextMove() {
		yield return new WaitForSeconds(0.2f);
		GameManager.Instance.playersCanMove = true;
	}
}

