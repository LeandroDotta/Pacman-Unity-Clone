using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour 
{
	public float speed;
	[HideInInspector] public Vector3Int gridPosition;

	private Vector3Int direction = Vector3Int.left;
	private Vector2 moveTo;

	private MazeCell cellUp;
	private MazeCell cellDown;
	private MazeCell cellLeft;
	private MazeCell cellRight;
	private MazeCell nextCell;

	private Maze maze;

	private void Start()
	{
		maze = Maze.Instance;
	}

	private void Update()
	{
		gridPosition = maze.grid.WorldToCell(transform.position);

		cellUp = maze.NextCell(gridPosition, Vector3Int.up);
		cellDown = maze.NextCell(gridPosition, Vector3Int.down);
		cellLeft = maze.NextCell(gridPosition, Vector3Int.left);
		cellRight = maze.NextCell(gridPosition, Vector3Int.right);

		if (direction == cellUp.direction)
			nextCell = cellUp;
		else if (direction == cellDown.direction)
			nextCell = cellDown;
		else if (direction == cellLeft.direction)
			nextCell = cellLeft;
		else if (direction == cellRight.direction)
			nextCell = cellRight;

		if (nextCell.canMove) {
			moveTo = maze.grid.GetCellCenterWorld(nextCell.position);	
		}

		Move();

		if (IsInCenter ()) {
			DecideDirection ();
		}

//		Debug.Log ("Ghost Position: " + gridPosition);
	}

	private void Move()
	{
		transform.position = Vector2.MoveTowards(transform.position, moveTo, speed * Time.deltaTime); 

//		Vector2 movement = direction * speed * Time.deltaTime;
//		transform.Translate(movement);
	}

	private bool IsCorner()
	{
		int cellCount = GetAvailableCellCount();

		return cellCount >= 3;
	}

	private bool IsInCenter()
	{
		return transform.position == maze.grid.GetCellCenterWorld (gridPosition);
	}

	private int GetAvailableCellCount()
	{
		int availableCells = 0;

		if (cellUp.canMove)
			availableCells++;

		if (cellDown.canMove)
			availableCells++;

		if (cellLeft.canMove)
			availableCells++;

		if (cellRight.canMove)
			availableCells++;

		return availableCells;
	}

	private MazeCell[] GetAvailableCells()
	{
		List<MazeCell> availableCells = new List<MazeCell>();

		if (cellUp.canMove)
			availableCells.Add (cellUp);

		if (cellDown.canMove)
			availableCells.Add (cellDown);

		if (cellLeft.canMove)
			availableCells.Add (cellLeft);

		if (cellRight.canMove)
			availableCells.Add (cellRight);

		return availableCells.ToArray();
	}

	private void DecideDirection()
	{
		MazeCell[] availableCells = GetAvailableCells ();

		if (IsCorner ())
		{
			Debug.Log ("Is Corner!");
			direction = availableCells [Random.Range (0, availableCells.Length)].direction;
		} 
		else if(!nextCell.canMove) 
		{
			Debug.Log ("Is Not Corner!");

			foreach (MazeCell cell in availableCells) 
			{
				if (cell.canMove) {
					direction = cell.direction;
					return;
				}
			}
		}
	}
}
