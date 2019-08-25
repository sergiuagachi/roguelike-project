using UnityEngine;

public class Enemy : PhysicsObject{

    private Animator _animator;
	private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");

	public class ExtendedParameters : DefaultParameters {
		public readonly int ExperienceGranted;

		public ExtendedParameters() {
			Health = 100;
			AttackPoints = 20;
			ExperienceGranted = 100;//Health % 10;
		}
	}

	public ExtendedParameters Parameters { get; private set; }

	protected override void Start () {
		_animator = GetComponent<Animator> ();

		Parameters = new ExtendedParameters();
		
		
		base.Start ();
	}

	public override void TakeDamage(int loss) {
		_animator.SetTrigger(EnemyAttack);
		Parameters.Health -= loss;
		
		ChangeToDamagedSprite();

		if (Parameters.Health <= 0) {
			gameObject.SetActive(false);
			enabled = false;
		}
	}
}