using System;

public class Food : UniqueItem{
    public int healthValue;
    public bool storable;
    public bool isUi;

    private void OnMouseDown() {
        if (isUi) {
            Player.Instance.Heal(healthValue);
        }
    }
}
