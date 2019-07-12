using System;
using UnityEngine;

public abstract class Enemy : MovingObject{
    protected int PlayerDamage;

    private Animator _animator;
    private Transform _target;
	
	private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");
	private const float ChaseDistance = 4f;
	
	protected override void Start () {
		
		GameManager.Instance.AddEnemyToList (this);
		
		_animator = GetComponent<Animator> ();
		_target = GameObject.FindGameObjectWithTag ("Player").transform;
		
		base.Start ();
	}

	public virtual void Move () {
		var xDir = 0;
		var yDir = 0;

		var playerPosition = _target.position;
		var enemyPosition = transform.position;
		var distanceToTarget = new Vector2(Math.Abs(playerPosition.x - enemyPosition.x), Math.Abs(playerPosition.y - enemyPosition.y));
		var stepsToTarget = distanceToTarget.x + distanceToTarget.y;
		
		if(stepsToTarget > ChaseDistance)
			return;
		
		if (distanceToTarget.y > distanceToTarget.x) {
			yDir = _target.position.y > transform.position.y ? 1 : -1;
		}
		else {
			xDir = _target.position.x > transform.position.x ? 1 : -1;
		}

		AttemptMove(xDir, yDir);
	}
	
	protected override void OnCantMove(PhysicsObject component) {
		PhysicsObject hitObject = component;

		if (hitObject is Player) {
			hitObject.TakeDamage(PlayerDamage);
			
			if(((Player)hitObject).HasPowerup("BladeArmor"))
				TakeDamage(PlayerDamage);
			
			_animator.SetTrigger(EnemyAttack);
		}
	}
}