using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MovingObject {

	public static Player Instance;
	
	private const int Food1Health = 10;
	private const int Food2Health = 20;

	private const int Drink1Energy = 10;
	private const int Drink2Energy = 20;

	private bool _onStairs; 
	
	[Serializable]
	public class Item {
		public string name;
		public bool isActive;

		public Item(string name, bool isActive) {
			this.name = name;
			this.isActive = isActive;
		}
	}

	//[Serializable]
	private class ExtendedParameters : DefaultParameters {
		public int Experience;
		public int PlayerLevel;

		public int Armor;
		
		public int BlockChance;
		
		public List<Item> powerups = new List<Item>{
			new Item("Sword", false),
			new Item("Key", false)
		};

		public ExtendedParameters() {
			Health = 100;
			Armor = 0;
			BlockChance = 0;
			AttackPoints = 50;
			PlayerLevel = 1;
		}
	}

	private ExtendedParameters _parameters;
	
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
		_parameters = new ExtendedParameters();

		UpdateTexts();		

		base.Start();
		DontDestroyOnLoad(gameObject);
	}

	public void UpdateTexts() {
		_levelText = GameObject.Find("LevelText").GetComponent<Text>();
		_healthText = GameObject.Find("HealthText").GetComponent<Text>();
		_armorText = GameObject.Find("ArmorText").GetComponent<Text>();
		_experienceText = GameObject.Find("ExperienceText").GetComponent<Text>();
		_popUp = GameObject.Find("PopUp").GetComponent<Text>();
		
		_levelText.text = "Level: " + _parameters.PlayerLevel;
		_healthText.text = "Health: " + _parameters.Health;
		_armorText.text = "Armor: " + _parameters.Armor;
		_experienceText.text = "Experience: " + _parameters.Experience;
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
		return _parameters.powerups[_parameters.powerups.FindIndex(x => x.name.Equals(powerupName))].isActive;
	}

	private void CheckLevel(int experienceGained) {

		_parameters.Experience += (experienceGained % _parameters.PlayerLevel);
		
		if (_parameters.Experience < 100) return;

		_parameters.Experience -= 100;
		_parameters.PlayerLevel++;

		_parameters.AttackPoints += GameManager.Instance.attackPointsPerLevel;
		_parameters.Armor += GameManager.Instance.armorPerLevel;
	}
	
	protected override void AttemptMove (int xDir, int yDir) {
		
		_popUp.text = "";
		GameManager.Instance.playersCanMove = false;
		
		base.AttemptMove(xDir, yDir);

		_levelText.text = "Level: " + _parameters.PlayerLevel;
		_healthText.text = "Health: " + _parameters.Health;
		_armorText.text = "Armor: " + _parameters.Armor;
		_experienceText.text = "Experience: " + _parameters.Experience;
		
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
					enemy.TakeDamage(_parameters.AttackPoints);
					TakeDamage(enemy.Parameters.AttackPoints);

					if (enemy.isActiveAndEnabled == false) {
						CheckLevel(enemy.Parameters.ExperienceGranted);
					}
					
					_animator.SetTrigger(PlayerChop);
				}
			}

			if (hitObj is Wall) {
				// too: if i have shovel/pick axe i can/t destroy some walls
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
		}
		GameManager.Instance.playersCanMove = true;
	}
	
	private void OnTriggerEnter2D (Collider2D other) {

		// uncollectibles
		
		if(other.CompareTag("Exit")) {
			//StartCoroutine(GameManager.Instance.LoadNextFloor());
			enabled = false;
			return;
		}

		if (other.CompareTag("Stairs")) {
			//StartCoroutine(GameManager.Instance.LoadNextFloor());

			
			if (_onStairs) {
				return;
			}

			_onStairs = true;
			Stairs stairs = other.GetComponent<Stairs>();
			stairs.ChangeLevel();

			return;
		}

		// collectibles
		
		if(other.CompareTag("Food")) {
			var healthPerFood = other.name.Equals("Food1") ? Food1Health : Food2Health;

			_parameters.Health = Math.Min(100, _parameters.Health + healthPerFood);
			_healthText.text = "+" + healthPerFood + " Health: " + _parameters.Health;
		}
		
		else if(other.CompareTag("Drink")) {
			var energyPerDrink = other.name.Equals("Drink1") ? Drink1Energy : Drink2Energy;
			
			_parameters.Experience = Math.Min(100, _parameters.Experience + energyPerDrink);
			_experienceText.text = "+" + energyPerDrink + " Experience: " + _parameters.Experience;
		}

		else if (other.CompareTag("Sword")) {
			_parameters.powerups[_parameters.powerups.FindIndex(x => x.name.Equals("Sword"))].isActive = true;
		}
		
		else if (other.CompareTag("Key")) {
			_parameters.powerups[_parameters.powerups.FindIndex(x => x.name.Equals("Key"))].isActive = true;
		}
		
		other.gameObject.SetActive (false);
	}

	private void OnTriggerExit2D(Collider2D other) {
		_onStairs = false;
	}

	public override void TakeDamage (int loss) {

		loss = Math.Max(0, loss - _parameters.Armor);
		
		_animator.SetTrigger (PlayerHit);

		// can block the damage
		var willBlock = Random.Range(0, 100);
		if (_parameters.BlockChance > willBlock) {
			return;
		}

		_parameters.Health -= loss;
		
		if(loss > 0)
			_healthText.text = "-" + loss + " Health: " + _parameters.Health;
		
		ChangeToDamagedSprite();
		CheckIfGameOver ();
	}

	private void CheckIfGameOver () {
		if (_parameters.Health <= 0) {
			enabled = false;
			GameManager.Instance.GameOver ();
		}
	}
}

