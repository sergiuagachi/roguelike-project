using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public Sprite activeSprite;
    public int floor;
    
    protected void Awake () {
        _spriteRenderer = GetComponent<SpriteRenderer> ();
    }
    
    public void Activate() {
        _spriteRenderer.sprite = activeSprite;
    }
}
