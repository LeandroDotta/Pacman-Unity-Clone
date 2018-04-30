using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour 
{
	public Grid grid;
	public Tilemap mazeTilemap;
	public Tilemap dotTilemap;


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
		dotTilemap.SetTile(cell, null);

		GameManager.Instance.AddScore (10);
	}
}