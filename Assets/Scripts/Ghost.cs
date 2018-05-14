﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed;
    [HideInInspector] public Vector3Int gridPosition;
	private Vector3 cellCenter;

    private Vector3Int direction = Vector3Int.left;
    private Vector2 moveTo;
	private bool turning;

    private bool canMoveUp;
    private bool canMoveDown;
    private bool canMoveLeft;
    private bool canMoveRight;
    private Vector3Int cellUp;
    private Vector3Int cellDown;
    private Vector3Int cellLeft;
    private Vector3Int cellRight;

    private Maze maze;

    private void Start()
    {
        maze = Maze.Instance;
    }

    private void Update()
    {
        Vector3Int pos = maze.grid.WorldToCell(transform.position);

        if (pos != gridPosition)
        {
            OnCellChange(pos);
        }

        Move();

		
    }

    private void OnCellChange(Vector3Int position)
    {
        Debug.Log("OnCellChange");
        gridPosition = position;
		cellCenter = maze.grid.GetCellCenterWorld(gridPosition);

        cellUp = GetNextCell(Vector3Int.up);
        cellDown = GetNextCell(Vector3Int.down);
        cellLeft = GetNextCell(Vector3Int.left);
        cellRight = GetNextCell(Vector3Int.right);

        canMoveUp = maze.CanMove(cellUp);
        canMoveDown = maze.CanMove(cellDown);
        canMoveLeft = maze.CanMove(cellLeft);
        canMoveRight = maze.CanMove(cellRight);

        direction = DecideDirection();
        Vector3Int nextCell = GetNextCell(direction);

        if (maze.CanMove(nextCell))
            moveTo = maze.grid.GetCellCenterWorld(nextCell);
    }

    private Vector3Int GetNextCell(Vector3Int direction)
    {
        return gridPosition + direction;
    }

    private Vector3Int DecideDirection()
    {
        Vector3Int result = Vector3Int.zero;

        int availableCells = 0;
        if (canMoveUp) availableCells++;
        if (canMoveDown) availableCells++;
        if (canMoveLeft) availableCells++;
        if (canMoveRight) availableCells++;

        if (availableCells == 2)
        {
            if ((canMoveUp && canMoveDown) || (canMoveLeft && canMoveRight))
            {
                // Caminho reto
                result = direction;
            }
            else
            {
                // Curva (vai para próxima direção disponível, menos para tras)
                Vector3Int backDirection = new Vector3Int(-direction.x, -direction.y, 0);
                if (canMoveUp && Vector3Int.up != backDirection) result = Vector3Int.up;
                else if (canMoveDown && Vector3Int.down != backDirection) result = Vector3Int.down;
                else if (canMoveLeft && Vector3Int.left != backDirection) result = Vector3Int.left;
                else if (canMoveRight && Vector3Int.right != backDirection) result = Vector3Int.right;
            }
        }
        else if (availableCells > 2)
        {
            // Intersecção (3 ou 4 direções)

            // Ignora a direção contrária à atual (pra ele não voltar pelo caminho que estava vindo)
            Vector3Int backDirection = new Vector3Int(-direction.x, -direction.y, 0);
            // Decide a direção aleatoriamente
            List<Vector3Int> directionList = new List<Vector3Int>();
            if (canMoveUp && Vector3Int.up != backDirection) directionList.Add(Vector3Int.up);
            if (canMoveDown && Vector3Int.down != backDirection) directionList.Add(Vector3Int.down);
            if (canMoveLeft && Vector3Int.left != backDirection) directionList.Add(Vector3Int.left);
            if (canMoveRight && Vector3Int.right != backDirection) directionList.Add(Vector3Int.right);

            result = directionList[Random.Range(0, directionList.Count)];
        }

		bool resultIsHorizontal = result.x != 0;
		bool directionIsHorizontal = direction.x != 0;
		if(resultIsHorizontal != directionIsHorizontal)
			turning = true;

        return result;
    }

    private void Move()
    {
		if(turning)
		{
			transform.position = Vector2.MoveTowards(transform.position, cellCenter, speed * Time.deltaTime);

			if(transform.position == cellCenter)
			{
				turning = false;
			}
		}
		else
		{
        	transform.position = Vector2.MoveTowards(transform.position, moveTo, speed * Time.deltaTime);
		}
    }
}
