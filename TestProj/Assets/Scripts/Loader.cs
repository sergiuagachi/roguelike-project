using UnityEngine;

public class Loader : MonoBehaviour {
	public GameObject gameManager;
	public Player player;

	public void Awake () {
		
		if (Player.Instance == null) {
			Instantiate(player, new Vector3(2, 1, 0), Quaternion.identity);
		}
		
		if (GameManager.Instance == null) {
			Instantiate(gameManager);
		}
	}
}