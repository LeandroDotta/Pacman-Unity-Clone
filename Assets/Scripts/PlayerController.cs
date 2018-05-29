﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float speed;

    public Transform sprite;
    public Maze maze;

    private Vector2 direction = Vector2.left;
    private Vector2 inputDirection = Vector2.left;
	private Vector3 startPosition;
    [HideInInspector] public Vector3Int gridPosition;
	private Animator anim;
	private bool moving;


	private void Start() 
	{
		startPosition = transform.position;
		anim = GetComponent<Animator>();
	}

    private void Reset()
    {
        speed = 10;
        sprite = transform.Find("Sprite");
    }

    private void Update()
    {
        float axisHorizontal = Input.GetAxisRaw("Horizontal");
        float axisVertical = Input.GetAxisRaw("Vertical");

        if (axisVertical == 1)
        {
            inputDirection = Vector2.up;
        }
        else if (axisVertical == -1)
        {
            inputDirection = Vector2.down;
        }
        else if (axisHorizontal == 1)
        {
            inputDirection = Vector2.right;
        }
        else if (axisHorizontal == -1)
        {
            inputDirection = Vector2.left;
        }

        Vector3Int inputNextCell = gridPosition + Vector3Int.CeilToInt((Vector3)inputDirection);
        bool canTurn = maze.CanMove(inputNextCell);
        if (canTurn)
        {
            direction = inputDirection;
        }

        Vector3Int nextCell = gridPosition + Vector3Int.CeilToInt((Vector3)direction);
        bool canMove = maze.CanMove(nextCell);
        if (canMove)
        {
            Move();
        }

        gridPosition = maze.grid.WorldToCell(transform.position);

        bool centering = Centering(canMove);

		moving = canMove || centering;
		anim.SetBool("moving", moving);

		DectectCollisions();
    }

    private void FaceDirection()
    {
        if (direction == Vector2.up)
            sprite.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        else if (direction == Vector2.down)
            sprite.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        else if (direction == Vector2.right)
            sprite.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else if (direction == Vector2.left)
            sprite.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
    }

    private bool Centering(bool canMove)
    {
        Vector3 cellCenter = maze.grid.GetCellCenterWorld(gridPosition);
		Vector3 playerPos = transform.position;

        if (canMove)
        {
            if (direction.x != 0)
            {
                // Movendo na horizontal
                cellCenter.x = playerPos.x;
            }
            else
            {
                // Movendo na vertical
                cellCenter.y = playerPos.y;
            }
        }

		if(playerPos != cellCenter)
		{
        	transform.position = Vector2.MoveTowards(playerPos, cellCenter, speed * Time.deltaTime);
			return true;
		}

		return false;
    }

    private void Move()
    {
        FaceDirection();

        Vector2 movement = direction * speed * Time.deltaTime;
        transform.Translate(movement);
    }

	private void DectectCollisions()
	{
		// Colisão com o dot
		if (maze.HasDot(gridPosition))
		{
			maze.CollectDot(gridPosition);
		}

		// Colisão com o bonus
		if (maze.currentBonus != null) 
		{
			if (gridPosition == maze.currentBonus.gridPosition) 
			{
				maze.currentBonus.Collect();
			}
		}

		// Colisão com o fantasma
		if (maze.HasGhost(gridPosition))
		{
			GameManager.Instance.LoseLife ();
			ResetPosition ();
		}
	}

	private void ResetPosition()
	{
		transform.position = startPosition;
	}
}
