using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;
	
	public bool playerCanMove = true;
	private int _playerDamage;
	
	public int armorPerLevel = 10;
	public int attackPointsPerLevel = 25;
	
	private int _floor = 1;

	private Vector3 _lastCheckpointPosition;
	private int _lastCheckpointFloor = 1;

	public int Floor => _floor;
	private readonly List<Food> _foodOnFloor = new List<Food>();

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
		LoadFloor(++_floor);
	}
	
	public void LoadPreviousFloor() {
		LoadFloor(--_floor);
	}

	private void LoadFloor(int floor) {
		_foodOnFloor.Clear();
		SceneManager.LoadScene("Floor" + floor, LoadSceneMode.Single);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
		Player.Instance.GetTexts();
		
		foreach(var food in Player.Instance.pickedFood) {
			if (food.Floor == _floor) {
				foreach (var instace in _foodOnFloor) {
					if (instace.transform.position == food.Position) {
						instace.gameObject.SetActive(false);
						enabled = false;
					}
				}
			}
		}
	}

	public void ActivateCheckpoint(Transform checkpoint) {
		_lastCheckpointPosition = checkpoint.position;
		_lastCheckpointFloor = _floor;
	}

	public Vector3 GetLastCheckpointPosition() {
		return _lastCheckpointPosition;
	}
	
	public void ReturnToCheckpoint() {
		Player.Instance.transform.position = _lastCheckpointPosition;
		LoadFloor(_floor = _lastCheckpointFloor);
	}

	public void AddFood(Food food) {
		_foodOnFloor.Add(food);
	}
}