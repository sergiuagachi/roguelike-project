using UnityEngine;

public class Food : MonoBehaviour {
    
    public int healthValue;
    public bool storable;

    private void Awake() {
        GameManager.Instance.AddFood(this);
    }

    public PickedItem PickedUp() {
        return new PickedItem(GameManager.Instance.Floor, transform.position);
    }
}
