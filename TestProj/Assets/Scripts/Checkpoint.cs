using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public Sprite activeSprite;

    protected virtual void Start () {
        _spriteRenderer = GetComponent<SpriteRenderer> ();
    }
    
    public void Activate() {
        _spriteRenderer.sprite = activeSprite;
    }
}
