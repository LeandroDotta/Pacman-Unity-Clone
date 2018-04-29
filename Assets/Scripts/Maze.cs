using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour 
{
	public Grid grid;
	public Tilemap tilemap;

	public bool CanMove(Vector3Int cell)
	{
		return !tilemap.HasTile(cell);
	}
}
