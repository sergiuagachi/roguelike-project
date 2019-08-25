using UnityEngine;

public class Stairs : MonoBehaviour{
    public void ChangeLevel() {
        if (name.Contains("StairsUp")) {
            GameManager.Instance.LoadNextFloor();
        }

        if (name.Contains("StairsDown")) {
            GameManager.Instance.LoadPreviousFloor();
        }
    }
}
