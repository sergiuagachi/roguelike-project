using UnityEngine;

public abstract class PhysicsObject : MonoBehaviour
{
	public class DefaultParameters {
		public float Health;
	}
	protected DefaultParameters _defaultParameters = new DefaultParameters();
	
	protected BoxCollider2D BoxCollider;
	protected Rigidbody2D Rb2D;
	
	public Sprite damagedSprite;
	protected SpriteRenderer SpriteRenderer;
	
	protected virtual void Start () {
		BoxCollider = GetComponent <BoxCollider2D> ();
		Rb2D = GetComponent <Rigidbody2D> ();
		
		SpriteRenderer = GetComponent<SpriteRenderer> ();
	}

	protected void ChangeToDamagedSprite() {
		if(damagedSprite)
			SpriteRenderer.sprite = damagedSprite;
	}
	
	public virtual void TakeDamage(int loss) {
		_defaultParameters.Health -= loss;
		
		ChangeToDamagedSprite();

		if (_defaultParameters.Health <= 0) {
			gameObject.SetActive(false);
			enabled = false;
		}
	}
}
