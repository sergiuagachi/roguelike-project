using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MovingObject {

	private const int Food1Health = 10;
	private const int Food2Health = 20;

	private const int Drink1Energy = 10;
	private const int Drink2Energy = 20;

	private const int HelmetArmor = 2;
	private const int ChestArmor = 5;
	private const int LegArmor = 1;

	// to change the implement or to change how this works 
	private const int SpookDamage = 20;
	
	[Serializable]
	public class Powerup {
		public string name;
		public bool isActive;

		public Powerup(string name, bool isActive) {
			this.name = name;
			this.isActive = isActive;
		}
	}

	[Serializable]
	public class Parameters : DefaultParameters{
		public float energy = 100f;
		public int armor;

		public int attackPoints = 25;
		public int breakPoints = 2;		
		public int blockChance;
		
		// 0 - helmet
		// 1 - blade armor
		// 2 - skates
		// 3 - ring
		// 4 - shield
		// 5 - sword
		public List<Powerup> powerups = new List<Powerup>{
			new Powerup("Helmet", false),
			new Powerup("BladeArmor", false),
			new Powerup("Skates", false),
			new Powerup("Ring", false),
			new Powerup("Shield", false),
			new Powerup("Sword", false)
		};

		public Parameters() {
			Health = 100f;
		}
	}

	private Parameters _parameters;

	public Text healthText;
	public Text energyText;
	public Text armorText;

	private Animator _animator;
	private static readonly int PlayerChop = Animator.StringToHash("playerChop");
	private static readonly int PlayerHit = Animator.StringToHash("playerHit");

	protected override void Start() {

		_animator = GetComponent<Animator>();

		_parameters = new Parameters();

		healthText.text = "Health: " + _parameters.Health;
		energyText.text = "Energy: " + _parameters.energy;
		armorText.text = "Armor: " + _parameters.armor;

		armorText.gameObject.SetActive(_parameters.armor > 0);

		base.Start();
	}

	private void Update () {
		if(!GameManager.Instance.playersTurn) return;

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

	public bool HasPowerup(string powerupName) {	
		return _parameters.powerups[_parameters.powerups.FindIndex(x => x.name.Equals(powerupName))].isActive;
	}
	
	protected override void AttemptMove (int xDir, int yDir) {
		base.AttemptMove(xDir, yDir);

		if (!HitOuterWall) {
			// -1 energy for each movement or -2 health if no energy is available
			if (_parameters.energy > 0) {
				_parameters.energy -= 1f;
			}
			else {
				_parameters.Health -= 2f;
			}

			if (HasPowerup("Ring")) {
				_parameters.Health = Math.Min(100f, _parameters.Health + 1f);
			}

			healthText.text = "Health: " + _parameters.Health;
			energyText.text = "Energy: " + _parameters.energy;
			armorText.text = "Armor: " + _parameters.armor;
			
			CheckIfGameOver ();
		}	
		
		GameManager.Instance.playersTurn = false;
	}
	
	protected override void OnCantMove(PhysicsObject component) {
		PhysicsObject hitObj = component;

		if (hitObj) {
			if (hitObj is Enemy) {
				hitObj.TakeDamage(_parameters.attackPoints);

				if (hitObj is GhostEnemy) {
					TakeDamage(SpookDamage);
				}
			}

			if (hitObj is Wall) {
				hitObj.TakeDamage(_parameters.breakPoints);
			}
		}

		_animator.SetTrigger (PlayerChop);
	}
	
	private void OnTriggerEnter2D (Collider2D other) {
		if(other.CompareTag("Exit")) {
			StartCoroutine(GameManager.Instance.LoadNextLevel());
			enabled = false;
			return;
		}

		if(other.CompareTag("Food")) {
			var healthPerFood = other.name.Equals("Food1") ? Food1Health : Food2Health;

			_parameters.Health = Mathf.Min(100f, _parameters.Health + healthPerFood);
			healthText.text = "+" + healthPerFood + " Health: " + _parameters.Health;
		}
		
		else if(other.CompareTag("Drink")) {
			var energyPerDrink = other.name.Equals("Drink1") ? Drink1Energy : Drink2Energy;
			
			_parameters.energy = Mathf.Min(100f, _parameters.energy + energyPerDrink);
			energyText.text = "+" + energyPerDrink + " Energy: " + _parameters.energy;
		}
		
		else if (other.CompareTag("Armor")) {

			var armorToGain = 0;
			
			if (other.name.Equals("Helmet")) {
				var helmetIndex = _parameters.powerups.FindIndex(x => x.name.Equals("Helmet"));
				if (!_parameters.powerups[helmetIndex].isActive) {
					_parameters.powerups[helmetIndex].isActive = true;
					
					armorToGain = HelmetArmor;			
				}
			}
			else if (other.name.Equals("BladeArmor")) {
				var armorIndex = _parameters.powerups.FindIndex(x => x.name.Equals("BladeArmor"));
				if (!_parameters.powerups[armorIndex].isActive) {
					_parameters.powerups[armorIndex].isActive = true;
					
					armorToGain = ChestArmor;			
				}
			}
			else if (other.name.Equals("Skates")) {
				var skatesIndex = _parameters.powerups.FindIndex(x => x.name.Equals("Skates"));
				if (!_parameters.powerups[skatesIndex].isActive) {
					_parameters.powerups[skatesIndex].isActive = true;
					
					armorToGain =  LegArmor;			
				}
			}
			
			if (other.name.Equals("Shield")) {
				_parameters.powerups[_parameters.powerups.FindIndex(x => x.name.Equals("Shield"))].isActive = true;
				_parameters.blockChance = 50;
			}

			_parameters.armor += armorToGain;
			if (armorToGain > 0) {
				armorText.text = "+" + armorToGain + " Armor: " + _parameters.armor;
				armorText.gameObject.SetActive(true);
			}
		}
		
		else if (other.CompareTag("Accessory")) {
			if (other.name.Equals("Ring")) {
				_parameters.powerups[_parameters.powerups.FindIndex(x => x.name.Equals("Ring"))].isActive = true;			
			}
		}
		
		else if (other.CompareTag("Weapon")) {
			if (other.name.Equals("Sword")) {
				_parameters.powerups[_parameters.powerups.FindIndex(x => x.name.Equals("Sword"))].isActive = true;
				_parameters.attackPoints *= 2;
			}
		}
		
		other.gameObject.SetActive (false);
	}
	
	public override void TakeDamage (int loss) {
		_animator.SetTrigger (PlayerHit);

		// can block the damage
		var willBlock = Random.Range(0, 100);
		if (_parameters.blockChance > willBlock) {
			return;
		}

		_parameters.Health -= loss + _parameters.armor;
		
		ChangeToDamagedSprite();
		
		healthText.text = "-" + loss + " Health: " + _parameters.Health;
		
		CheckIfGameOver ();
	}
	
	private void CheckIfGameOver () {
		if (_parameters.Health <= 0) {
			enabled = false;
			GameManager.Instance.GameOver ();
		}
	}
}

