﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostState { Chase, Scatter, Frightened }
public enum GhostMovement { Waiting, Exiting, Moving, Dead }

public class Ghost : MonoBehaviour
{
    public float speed;
    public int dotsToExit;
    public Vector2 exitPosition;
    public Vector3Int target;
    public Vector3Int blindSpot;

    [Header("Waiting")]
    public float waitingMoveDistance = 0.5f;
    public Vector3Int waitingStartDirection = Vector3Int.up;
    private Vector2 waitingUp;
    private Vector2 waitingDown;

    [HideInInspector] public Vector3Int gridPosition;

    private Vector3 cellCenter;


    private Vector2 origin;
    private Vector3Int direction = Vector3Int.left;
    private Vector3Int backDirection;
    private Vector2 moveTo;
    private bool turning;
    private int dotCount;

    private bool canMoveUp;
    private bool canMoveDown;
    private bool canMoveLeft;
    private bool canMoveRight;
    private Vector3Int cellUp;
    private Vector3Int cellDown;
    private Vector3Int cellLeft;
    private Vector3Int cellRight;

    private Maze maze;

    [Header("States")]
    public GhostState state;
    public GhostMovement moveState;

    private void Start()
    {
        maze = Maze.Instance;

        origin = transform.position;

        SetMoveState(moveState);
    }

    private void Update()
    {
        switch (moveState)
        {
            case GhostMovement.Waiting:
                // Animação para cima e para baixo:
                WaitingMove();
                break;
            case GhostMovement.Exiting:
                // Move para o centro da casa e depois para o labirinto
                Exit();
                break;
            case GhostMovement.Moving:
                // Move-se pelo labirinto
                Vector3Int pos = maze.grid.WorldToCell(transform.position);
                if (pos != gridPosition)
                {
                    OnCellChange(pos);
                }

                Move();
                break;
            case GhostMovement.Dead:
                //Move de volta para a casa
                break;
        }
    }

    private void OnCellChange(Vector3Int position)
    {
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

        DecideTarget();

        backDirection = new Vector3Int(-direction.x, -direction.y, 0);
        direction = DecideDirection();
        
        Vector3Int nextCell = GetNextCell(direction);

        if (maze.CanMove(nextCell))
            moveTo = maze.grid.GetCellCenterWorld(nextCell);
    }

    private Vector3Int GetNextCell(Vector3Int direction)
    {
        return gridPosition + direction;
    }

    private void DecideTarget()
    {
        switch(state)
        {
            case GhostState.Scatter:
                target = blindSpot;
                break;
            case GhostState.Chase:
                target = Maze.Instance.player.gridPosition;
                break;       
        }
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
                if (canMoveUp && Vector3Int.up != backDirection) result = Vector3Int.up;
                else if (canMoveDown && Vector3Int.down != backDirection) result = Vector3Int.down;
                else if (canMoveLeft && Vector3Int.left != backDirection) result = Vector3Int.left;
                else if (canMoveRight && Vector3Int.right != backDirection) result = Vector3Int.right;
            }
        }
        else if (availableCells > 2)
        {
            // Intersecção (3 ou 4 direções)

            // Decide o caminho mais curto
            float shortest = 9999;
            if (canMoveRight && Vector3Int.right != backDirection)
            {
                float distRight = Vector3Int.Distance(cellRight, target);

                if(distRight <= shortest)
                {
                    shortest = distRight;
                    result = Vector3Int.right;
                }
            }
            if (canMoveDown && Vector3Int.down != backDirection)
            {
                float distDown = Vector3Int.Distance(cellDown, target);

                if(distDown <= shortest)
                {
                    shortest = distDown;
                    result = Vector3Int.down;
                }
            }
            if (canMoveLeft && Vector3Int.left != backDirection)
            {
                float distLeft = Vector3Int.Distance(cellLeft, target);

                if(distLeft <= shortest)
                {
                    shortest = distLeft;
                    result = Vector3Int.left;
                }
            }
            if (canMoveUp && Vector3Int.up != backDirection)
            {
                float distUp = Vector3Int.Distance(cellUp, target);

                if(distUp <= shortest)
                {
                    shortest = distUp;
                    result = Vector3Int.up;
                }
            }
        }

        bool resultIsHorizontal = result.x != 0;
        bool directionIsHorizontal = direction.x != 0;
        if (resultIsHorizontal != directionIsHorizontal)
            turning = true;

        return result;
    }

    private void Move()
    {
        if (turning)
        {
            transform.position = Vector2.MoveTowards(transform.position, cellCenter, speed * Time.deltaTime);

            if (transform.position == cellCenter)
            {
                turning = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, moveTo, speed * Time.deltaTime);
        }
    }

    private void Exit()
    {
        if (transform.position.x != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, transform.position.y), speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, exitPosition, speed * Time.deltaTime);

            if ((Vector2)transform.position == exitPosition)
            {
                SetMoveState(GhostMovement.Moving);
            }
        }
    }

    private void WaitingMove()
    {
        if (direction == Vector3Int.up)
        {
            transform.position = Vector2.MoveTowards(transform.position, waitingUp, speed * Time.deltaTime);

            if((Vector2)transform.position == waitingUp)
                direction = Vector3Int.down;
        }
        else if (direction == Vector3Int.down)
        {
            transform.position = Vector2.MoveTowards(transform.position, waitingDown, speed * Time.deltaTime);

            if((Vector2)transform.position == waitingDown)
                direction = Vector3Int.up;
        }
    }

    public void SetDotCount(int count)
    {
        dotCount = count;

        if (dotCount >= dotsToExit)
            SetMoveState(GhostMovement.Exiting);
    }

    private void SetMoveState(GhostMovement state)
    {
        Debug.Log("SetMoveState");
        moveState = state;

        switch (moveState)
        {
            case GhostMovement.Waiting:
                direction = waitingStartDirection;
                waitingUp = new Vector2(origin.x, origin.y + waitingMoveDistance);
                waitingDown = new Vector2(origin.x, origin.y - waitingMoveDistance);
                break;
            case GhostMovement.Moving:
                direction = Vector3Int.left;
                break;
        }
    }

    private void Invert()
    {
        Vector3Int tempDir = direction;
        direction = backDirection;
        backDirection = direction;

        OnCellChange(gridPosition);
    }

    private bool IsHorizontal(Vector3Int dir)
    {
        return dir.x != 0;
    }
}