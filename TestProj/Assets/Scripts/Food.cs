using UnityEngine;

public class Food : MonoBehaviour {
    
    
    public int healthValue;
    public bool storable;

    private void Awake() {
        GameManager.Instance.AddFood(this);
    }

    public Player.PickedFood PickedUp() {
        return new Player.PickedFood(GameManager.Instance.Floor, transform.position);
    }
}
