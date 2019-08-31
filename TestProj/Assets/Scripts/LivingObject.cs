using UnityEngine;

public abstract class LivingObject : MonoBehaviour {
	
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
	
	public abstract int TakeDamage(int loss);

	public abstract int GetHealth();
}
