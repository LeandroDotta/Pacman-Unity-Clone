using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour 
{
	public float speed;

	public Transform sprite;
	public Maze maze;

	private Vector2 direction = Vector2.right;
	private Vector2 inputDirection = Vector2.right;
	private Vector3Int gridPosition;

	private void Reset()
	{
		speed = 10;
		sprite = transform.Find ("Sprite");
	}

	private void Update()
	{
		float axisHorizontal = Input.GetAxisRaw("Horizontal");
		float axisVertical = Input.GetAxisRaw("Vertical");

		if (axisVertical == 1) {
			inputDirection = Vector2.up;
		} else if (axisVertical == -1) {
			inputDirection = Vector2.down;
		} else if (axisHorizontal == 1) {
			inputDirection = Vector2.right;
		} else if (axisHorizontal == -1) {
			inputDirection = Vector2.left;
		}
			
		Vector3Int inputNextCell = gridPosition + Vector3Int.CeilToInt((Vector3)inputDirection);
		bool canTurn = maze.CanMove(inputNextCell);
		if(canTurn) {
			direction = inputDirection;
		}

		Vector3Int nextCell = gridPosition + Vector3Int.CeilToInt((Vector3)direction);
		bool canMove = maze.CanMove (nextCell);
		if (canMove) {
			Move ();
		} 
			
		gridPosition = maze.grid.WorldToCell(transform.position);

		Centering(canMove);

		if (maze.HasDot(gridPosition)) {
			maze.RemoveDot (gridPosition);
		}
	}

	private void FaceDirection()
	{
		if (direction == Vector2.up)
			sprite.rotation = Quaternion.Euler(new Vector3 (0, 0, 90));
		else if (direction == Vector2.down)
			sprite.rotation = Quaternion.Euler(new Vector3 (0, 0, -90));
		else if (direction == Vector2.right)
			sprite.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
		else if (direction == Vector2.left)
			sprite.rotation = Quaternion.Euler(new Vector3 (0, 0, 180));
	}

	private void Centering(bool bothAxis)
	{
		Vector3 cellCenter = maze.grid.GetCellCenterWorld(gridPosition);
//		Vector2 centerDirection = Vector2.zero;
//		Vector3 position = transform.position;
//
//		if (position != cellCenter) {
//			centerDirection = cellCenter - position;
//
//			if (!bothAxis) {
//				if (direction.x != 0) {
//					centerDirection.x = 0;
//				} else {
//					centerDirection.y = 0;
//				}
//			}
//
//			Vector2 movement = centerDirection * speed * Time.deltaTime;
//			transform.Translate(movement);
//		}


		if (direction.x != 0) { 
			// Movendo na horizontal
			if (transform.position.y != cellCenter.y) {
				
				Vector3 pos = transform.position;
				pos.y = cellCenter.y;

				transform.position = pos;
			}
		} else {
			// Movendo na vertical
			if (transform.position.x != cellCenter.x) {
				Vector3 pos = transform.position;
				pos.x = cellCenter.x;

				transform.position = pos;
			}
		}
	}

	private void Move()
	{
		FaceDirection ();

		Vector2 movement = direction * speed * Time.deltaTime;
		transform.Translate (movement);
	}
}
