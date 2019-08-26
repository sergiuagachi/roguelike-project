using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;
	public bool playersCanMove = true;
	private int _playerDamage;
	
	public int armorPerLevel = 10;
	public int attackPointsPerLevel = 25;
	
	private int _floor = 1;
	private int _lastCheckpointFloor = 1;

	public int Floor => _floor;

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
		LoadFloor(_floor++);
	}
	
	public void LoadPreviousFloor() {
		LoadFloor(_floor--);
	}

	private void LoadFloor(int floor) {
		SceneManager.LoadScene("Floor" + floor, LoadSceneMode.Single);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
		Player.Instance.UpdateTexts();
	}

	public void ReturnToCheckpoint() {
		LoadFloor(_lastCheckpointFloor);
	}
}