using System;
using UnityEngine;

public class Stairs : MonoBehaviour{
    
    public SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeFloor() {
        if (name.Contains("StairsUp")) {
            GameManager.Instance.LoadNextFloor();
        }

        if (name.Contains("StairsDown")) {
            GameManager.Instance.LoadPreviousFloor();
        }
    }
}
