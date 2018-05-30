using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour
{
    public static Maze Instance { get; private set; }

    public Grid grid;
    public Tilemap mazeTilemap;
    public Tilemap dotTilemap;

    [Header("Player")]
    public PlayerController player;

    [Header("Ghosts")]
    public Ghost blinky;
    public Ghost pinky;
    public Ghost inky;
    public Ghost clyde;

    public float chaseTime;
    public float scatterTime;
    public float frightenedTime;
    
    private float ghostStateTimer;
    private GhostState currentGhostState;


    [Header("Bonus Config")]
    public int dotsForFirstBonus;
    public int dotsForSecondBonus;
    public Vector2 bonusPosition;
    public BonusSymbol currentBonus;

    public int currentLevel = 1;

    [Header("Bonus Per Level")]
    public GameObject[] bonusSymbolPrefabs;

    private int totalDots;
    private int dotCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Instance = null;
    }

    private void Start()
    {
        dotTilemap.CompressBounds(); // Garante que o "bounds" do tilemap não estará alem das areas desenhadas

        totalDots = GetTileCount(ref dotTilemap);

        currentGhostState = GhostState.Scatter;
    }

    private void Update() 
    {
        ghostStateTimer += Time.deltaTime;

        if(currentGhostState == GhostState.Chase && ghostStateTimer >= chaseTime)
        {
            currentGhostState = GhostState.Scatter;
            blinky.SetState(GhostState.Scatter);
            pinky.SetState(GhostState.Scatter);
            inky.SetState(GhostState.Scatter);
            clyde.SetState(GhostState.Scatter);

            ghostStateTimer = 0;

            Debug.Log("Ghost State: Scatter");
        }

        if(currentGhostState == GhostState.Scatter && ghostStateTimer >= scatterTime)
        {
            currentGhostState = GhostState.Chase;
            blinky.SetState(GhostState.Chase);
            pinky.SetState(GhostState.Chase);
            inky.SetState(GhostState.Chase);
            clyde.SetState(GhostState.Chase);

            ghostStateTimer = 0;

            Debug.Log("Ghost State: Chase");
        }
    }

    public bool CanMove(Vector3Int cell)
    {
        return !mazeTilemap.HasTile(cell);
    }

    public bool HasDot(Vector3Int cell)
    {
        return dotTilemap.HasTile(cell);
    }

    public void CollectDot(Vector3Int cell)
    {
        DotTile tile = dotTilemap.GetTile<DotTile>(cell);

        if (tile != null)
        {
            GameManager.Instance.AddScore(tile.score);

            if (tile.isEnergizer)
            {
                // TODO Aplicar estado de invulnerabilidade
            }

            dotTilemap.SetTile(cell, null);
            dotCount++;

            if (dotCount == totalDots)
            {
                // TODO Fim do jogo (Vitória)
                Debug.Log("Venceu!");
            }

            if (dotCount == dotsForFirstBonus || dotCount == dotsForSecondBonus)
            {
                // TODO Adicionar bonus da fase
                AddBonus();
            }

            // Contador para os fantasmas sairem
            if(blinky.moveState == GhostMovement.Waiting)
                blinky.SetDotCount(dotCount);
            else if(pinky.moveState == GhostMovement.Waiting)
                pinky.SetDotCount(dotCount);
            else if(inky.moveState == GhostMovement.Waiting)
                inky.SetDotCount(dotCount);
            else if(clyde.moveState == GhostMovement.Waiting)
                clyde.SetDotCount(dotCount);
        }
    }
    
    public int GetTileCount(ref Tilemap tilemap)
    {
        int count = 0;

        for (int y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (int x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                    count++;
            }
        }

        return count;
    }

    public void AddBonus()
    {
        GameObject bonus = Instantiate(bonusSymbolPrefabs[currentLevel - 1]);
        bonus.transform.position = bonusPosition;
        bonus.SetActive(true);

        currentBonus = bonus.GetComponent<BonusSymbol>();
    }

    public void RemoveBonus()
    {
        currentBonus = null;
    }

    public bool HasGhost(Vector3Int cell)
    {
        if (blinky != null && blinky.gridPosition == cell)
            return true;

        if (pinky != null && pinky.gridPosition == cell)
            return true;

        if (inky != null && inky.gridPosition == cell)
            return true;

        if (clyde != null && clyde.gridPosition == cell)
            return true;

        return false;
    }
}