using UnityEngine;

public class UniqueItem : MonoBehaviour {

    private void Awake() {
        GameManager.Instance.AddNonRespawnableItem(this);
    }
    
    public ItemLocation PickUp() {
        return new ItemLocation(GameManager.Instance.GetFloor(), transform.position);
    }
}
