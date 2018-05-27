using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public int score;
	public int lifes;

	public static GameManager Instance { get; private set; }

	private void Awake() {
		if (Instance == null)
			Instance = this;
		else if(Instance != this)
			Instance = null;

//		DontDestroyOnLoad(gameObject);
	}

	public void AddScore(int value){
		score += value;
	}

	public void LoseLife()
	{
		lifes--;

		if (lifes == 0) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}
}