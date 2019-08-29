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

	//private Image _leftPanelImage;
	//private Text _leftPanelText;
	private GameObject _leftPanel;
	private GameObject _overlay;

	protected override void Start () {
		base.Start ();

		//_leftPanelImage = GameObject.Find("LeftPanelImage").GetComponent<Image>();
		//_leftPanelText = GameObject.Find("LeftPanelText").GetComponent<Text>();
		_leftPanel = GameObject.Find("LeftPanel");
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
			gameObject.SetActive(false);
			enabled = false;
			HideDetails();
		}
	}

	public override int GetHealth() {
		return parameters.Health;
	}

	private void ShowDetails() {

		var tempColor = _overlay.GetComponent<Image>().color;
		tempColor.a = 0;
		_overlay.GetComponent<Image>().color = tempColor;
		
		var text = _leftPanel.gameObject.transform.GetChild(0).GetComponent<Text>();
		var image = _leftPanel.gameObject.transform.GetChild(1).GetComponent<Image>();
		
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