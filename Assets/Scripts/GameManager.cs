using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}