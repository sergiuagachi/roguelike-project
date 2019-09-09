using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Food : UniqueItem, IPointerEnterHandler {
    public int healthValue;
    public bool storable;
    public bool isUi;

    private void OnMouseEnter() {
        Debug.Log("enter");
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("enter");
    }
}
