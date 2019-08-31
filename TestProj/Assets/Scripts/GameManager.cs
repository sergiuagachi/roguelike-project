using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;
	
	public bool playerCanMove = true;

	private Vector3 _lastCheckpointPosition;
	private int _lastCheckpointFloor = 1;

	private int _floor = 1;

	private readonly List<UniqueItem> _itemsOnFloor = new List<UniqueItem>();
	private Text _floorText;
	public float timeElapsed;

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
		LoadFloor(++_floor);
	}
	
	public void LoadPreviousFloor() {
		LoadFloor(--_floor);
	}

	private void LoadFloor(int floor) {
		_itemsOnFloor.Clear();
		
		SceneManager.LoadScene("Floor" + floor, LoadSceneMode.Single);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
		if (arg0.name == "End") return;
		
		Player.Instance.GetTexts();
		_floorText = GameObject.Find("FloorText").GetComponent<Text>();
		_floorText.text = "Floor " + _floor;
		
		foreach(var item in Player.Instance.pickedItems) {
			if (item.Floor != _floor) continue;
			foreach (var spawnedItem in _itemsOnFloor) {
				if (spawnedItem.transform.position != item.Position) continue;
				spawnedItem.gameObject.SetActive(false);
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

	public void AddNonRespawnableItem(UniqueItem item) {
		_itemsOnFloor.Add(item);
	}

	public static void EndGame() {
		SceneManager.LoadScene("End", LoadSceneMode.Single);
	}

	public int GetFloor() {
		return _floor;
	}
}