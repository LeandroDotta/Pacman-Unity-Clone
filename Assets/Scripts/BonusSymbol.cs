using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSymbol : MonoBehaviour 
{
	public string symbolName;
	public int points;

	[HideInInspector] public Vector3Int gridPosition;

	private float minTime = 9;
	private float maxTime = 10;

	private IEnumerator Start() 
	{
		gridPosition = Maze.Instance.grid.WorldToCell (transform.position);
		float time = Random.Range(minTime, maxTime);

		yield return new WaitForSeconds(time);

		Remove();
	}

	public void Collect()
	{
		GameManager.Instance.AddScore(points);

		Remove();
	}

	private void Remove()
	{
		Maze.Instance.RemoveBonus();

		Destroy(gameObject);
	}
}
