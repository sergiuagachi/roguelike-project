using UnityEngine;

public class Loader : MonoBehaviour {
	public GameObject gameManager;
	public Player player;
	
	void Awake () {
		
		if (Player.Instance == null) {
			Instantiate(player, new Vector3(5, 9, 0), Quaternion.identity);
		}
		
		if (GameManager.Instance == null) {
			Instantiate(gameManager);
		}
	}
}