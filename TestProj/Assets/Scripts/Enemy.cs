using System;
using UnityEngine;

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

	protected override void Start () {
		base.Start ();
		
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
		}
	}

	public override int GetHealth() {
		return parameters.Health;
	}
}