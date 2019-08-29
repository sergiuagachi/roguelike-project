using UnityEngine;

public abstract class PhysicsObject : MonoBehaviour
{
	public class DefaultParameters {
		public int Health;
		public int AttackPoints;

		protected DefaultParameters(int health, int attackPoints) {
			Health = health;
			AttackPoints = attackPoints;
		}
	}

	protected Rigidbody2D Rb2D;
	
	public Sprite damagedSprite;
	protected SpriteRenderer SpriteRenderer;
	
	protected virtual void Start () {
		Rb2D = GetComponent <Rigidbody2D> ();
		SpriteRenderer = GetComponent<SpriteRenderer> ();
	}

	protected void ChangeToDamagedSprite() {
		if(damagedSprite)
			SpriteRenderer.sprite = damagedSprite;
	}
	
	public abstract void TakeDamage(int loss);

	public abstract int GetHealth();
}
