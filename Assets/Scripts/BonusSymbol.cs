using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSymbol : MonoBehaviour 
{
	public string symbolName;
	public int points;

	private float minTime = 9;
	private float maxTime = 10;

	private IEnumerator Start() 
	{
		float time = Random.Range(minTime, maxTime);

		yield return new WaitForSeconds(time);

		Destroy(gameObject);	
	}

	public void Collect()
	{
		GameManager.Instance.AddScore(points);

		Destroy(gameObject);
	}
}
