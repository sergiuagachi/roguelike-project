using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public Sprite openedSprite;

    public bool IsOpen { get; private set; }

    protected virtual void Start () {
        _spriteRenderer = GetComponent<SpriteRenderer> ();
    }
    
    public void Open() {
        _spriteRenderer.sprite = openedSprite;
        IsOpen = true;
    }
}
