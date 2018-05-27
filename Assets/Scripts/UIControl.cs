using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {


    public static UIControl Instance { get; private set; }
    public Text score;
    public Text lives;


    void Start () {

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Instance = null;

        score.GetComponent<Text>();
        lives.GetComponent<Text>();
    }
	

	void LateUpdate () {

        score.text = "Score:" + GameManager.Instance.score;
        lives.text = "Lives:" + GameManager.Instance.lifes;


    }
}
