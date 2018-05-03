using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour 
{
	public Grid grid;
	public Tilemap mazeTilemap;
	public Tilemap dotTilemap;

	[Header("Fruits Per Level")]
	public GameObject[] bonusSymbolPrefabs;


	public bool CanMove(Vector3Int cell)
	{
		return !mazeTilemap.HasTile(cell);
	}

	public bool HasDot(Vector3Int cell)
	{
		return dotTilemap.HasTile(cell);
	}

	public void RemoveDot(Vector3Int cell)
	{
		Tile tile = dotTilemap.GetTile(cell) as Tile;

		if(tile != null)
			Debug.Log(tile.name);

		dotTilemap.SetTile(cell, null);

		GameManager.Instance.AddScore (10);
	}
}