using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;
	
	public bool playerCanMove = true;
	private int _playerDamage;
	
	public int armorPerLevel = 10;
	public int attackPointsPerLevel = 25;

	private Vector3 _lastCheckpointPosition;
	private int _lastCheckpointFloor = 1;

	public int Floor { get; private set; } = 1;

	private readonly List<Food> _foodOnFloor = new List<Food>();
	private Text _floorText;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else if (Instance != this) {
			Destroy(gameObject);
		}

		_floorText = GameObject.Find("FloorText").GetComponent<Text>();
		_floorText.text = "Floor 1";
		
		DontDestroyOnLoad(gameObject);
	}

	public void LoadNextFloor() {
		LoadFloor(++Floor);
	}
	
	public void LoadPreviousFloor() {
		LoadFloor(--Floor);
	}

	private void LoadFloor(int floor) {
		_foodOnFloor.Clear();
		
		SceneManager.LoadScene("Floor" + floor, LoadSceneMode.Single);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
		Player.Instance.GetTexts();
		
		_floorText = GameObject.Find("FloorText").GetComponent<Text>();
		_floorText.text = "Floor " + Floor;
		
		foreach(var food in Player.Instance.pickedFood) {
			if (food.Floor == Floor) {
				foreach (var instance in _foodOnFloor) {
					if (instance.transform.position == food.Position) {
						instance.gameObject.SetActive(false);
						enabled = false;
					}
				}
			}
		}
	}

	public void ActivateCheckpoint(Transform checkpoint) {
		_lastCheckpointPosition = checkpoint.position;
		_lastCheckpointFloor = Floor;
	}

	public Vector3 GetLastCheckpointPosition() {
		return _lastCheckpointPosition;
	}
	
	public void ReturnToCheckpoint() {
		Player.Instance.transform.position = _lastCheckpointPosition;
		LoadFloor(Floor = _lastCheckpointFloor);
	}

	public void AddFood(Food food) {
		_foodOnFloor.Add(food);
	}
}