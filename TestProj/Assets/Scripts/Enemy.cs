using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : PhysicsObject{

    private Animator _animator;
	private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");

	[Serializable]
	public class ExtendedParameters : DefaultParameters {
		public int experienceGranted;

		public ExtendedParameters() : base(100, 20){
			experienceGranted = Health % 10; //100;//
		}
	}

	public ExtendedParameters parameters;

	private GameObject _monsterInfo;
	private GameObject _overlay;

	protected override void Start () {
		base.Start ();

		_monsterInfo = GameObject.Find("MonsterInfo");
		_overlay = GameObject.Find("Overlay");
		
		_animator = GetComponent<Animator> ();
	}

	public override void TakeDamage(int loss) {
		// todo: some enemies dont have animations
		if(_animator)
			_animator.SetTrigger(EnemyAttack);
		parameters.Health -= loss;
		
		ChangeToDamagedSprite();

		if (parameters.Health <= 0) {
			HideDetails();
			gameObject.SetActive(false);
			enabled = false;
		}
	}

	public override int GetHealth() {
		return parameters.Health;
	}

	private void ShowDetails() {

		var tempColor = _overlay.GetComponent<Image>().color;
		tempColor.a = 0;
		_overlay.GetComponent<Image>().color = tempColor;
		
		var text = _monsterInfo.gameObject.transform.GetChild(0).GetComponent<Text>();
		var image = _monsterInfo.gameObject.transform.GetChild(1).GetComponent<Image>();
		
		image.sprite = SpriteRenderer.sprite;
		text.text = "Name: " + transform.name.Substring(0, transform.name.Length - 4) + "\n" +
		                      "Health: " + parameters.Health + "\n" +
		                      "Damage: " + parameters.AttackPoints + "\n" +
		                      "Xp on kill: " + parameters.experienceGranted;
	}

	private void HideDetails() {
		var tempColor = _overlay.GetComponent<Image>().color;
		tempColor.a = 255;
		_overlay.GetComponent<Image>().color = tempColor;
	}

	private void OnMouseOver() {
		ShowDetails();
	}

	private void OnMouseExit() {
		HideDetails();
	}
}