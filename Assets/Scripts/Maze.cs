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

    [Header("Ghosts")]
    public Ghost blinky;
    public Ghost pinky;
    public Ghost inky;
    public Ghost clyde;

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

        Debug.Log("Dot Total: " + totalDots);
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