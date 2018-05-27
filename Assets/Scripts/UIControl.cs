using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour 
{
    public Text score;
    public Text lives;

	void LateUpdate () {

        score.text = "Score:" + GameManager.Instance.score;
        lives.text = "Lives:" + GameManager.Instance.lifes;


    }
}
