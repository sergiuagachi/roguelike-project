using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	
	public static GameManager Instance;
	public bool playersCanMove = true;
	private int _playerDamage;
	public int armorPerLevel = 10;
	public int attackPointsPerLevel = 25;
	
	private int _floor = 1;

	private Player _playerInstace;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else if (Instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}
	
	public void LoadNextFloor() {
		
		//yield return new WaitForSeconds(levelStartDelay);
		_floor++;

		SceneManager.LoadScene("Floor" + _floor, LoadSceneMode.Single);
	}
	
	public IEnumerator LoadPreviousFloor() {
		
		yield return new WaitForSeconds(levelStartDelay);
		
		_floor--;
	}

	public void GameOver() {
		StartCoroutine(ResetGame());
	}

	private IEnumerator ResetGame() {
		
		// after game over, wait 3 seconds then restart the game
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene("Level1", LoadSceneMode.Single);
	}
}