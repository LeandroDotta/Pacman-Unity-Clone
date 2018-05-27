using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Pacman/Dot Tile", fileName = "New Dot Tile")]
public class DotTile : Tile
{
	public bool isEnergizer;
	public int score;
}