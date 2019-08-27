using System;
using UnityEngine;

public class Enemy : PhysicsObject{

    private Animator _animator;
	private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");

	[Serializable]
	public class ExtendedParameters : DefaultParameters {
		public int experienceGranted;

		public ExtendedParameters() {
			Health = 100;
			AttackPoints = 20;
			experienceGranted = Health % 10; //100;//
		}
	}

	public ExtendedParameters parameters;

	protected override void Start () {
		_animator = GetComponent<Animator> ();
		
		base.Start ();
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
}