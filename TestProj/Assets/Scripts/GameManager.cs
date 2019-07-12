using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float turnDelay = 0.3f; //Delay between each Player turn.
	
	public static GameManager Instance;
	public bool playersTurn = true;
	
	public int level;
	private List<Enemy> _enemies;
	public bool enemiesMoving;
	public bool doingSetup;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else if (Instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		
		_enemies = new List<Enemy>();
		InitLevel();
	}
	
	private void InitLevel() {
		
		// while the game is loading we want the player to be unable to move, hence the flag
		doingSetup = true;
		
		// at the start of each level the remaining enemies are cleared from our list in order for new ones to spawn
		_enemies.Clear();

		if(GameObject.Find("Exit"))
			SceneManager.LoadScene("Level" + level, LoadSceneMode.Single);
		
		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);
	}
	
	void Update() {
		if (playersTurn || enemiesMoving || doingSetup) 
			return;
		
		StartCoroutine (MoveEnemies ());
	}
	public IEnumerator LoadNextLevel() {
		
		yield return new WaitForSeconds(levelStartDelay);
		
		level++;
		InitLevel();
	}
	
	void HideLevelImage() {
		GameObject.Find("LevelImage").SetActive(false);
		doingSetup = false;
	}
	
	public void AddEnemyToList(Enemy script) {
		_enemies.Add(script);
	}
	
	public void GameOver() {
		StartCoroutine(ResetGame());
	}

	private IEnumerator ResetGame() {
		level = 1;
		
		GameObject.Find("LevelImage").SetActive(true);
		
		// after game over, wait 3 seconds then restart the game
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene("Level1", LoadSceneMode.Single);
	}

	IEnumerator MoveEnemies() {
		// set the flag in order to stop the player from moving
		enemiesMoving = true;
		
		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);
		
		//If there are no enemies spawned (IE in first level):
		if (_enemies.Count == 0) {
			//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
			yield return new WaitForSeconds(turnDelay);
		}
		
		foreach (var enemy in _enemies) {
			// only move active enemies, not the 'dead' ones
			if (enemy.isActiveAndEnabled)
				enemy.Move();
			
			//Wait for Enemy's moveTime before moving next Enemy, 

			// TODO: might cause trouble for slower moving enemies, check for bugs just in case
			if (enemy.HasStartedMoving()) {
				yield return new WaitForSeconds(0.1f);
				enemy.StopedMoving();
			}
		}
		
		playersTurn = true;
		enemiesMoving = false;
	}
}