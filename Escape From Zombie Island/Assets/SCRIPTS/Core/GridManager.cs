// Escape From Zombie Island
// GridManager.cs
// by Dakikaki
// 2024

using System.Collections.Generic;
using UnityEngine;

// NEW: Added an enum to define the types of highlights a tile can have.
public enum TileHighlight
{
    None,
    Movement,
    Attack
}

public class GridManager : MonoBehaviour
{
    // NEW: Implementation of the Singleton pattern.
    public static GridManager Instance { get; private set; }

    public GameObject TilePrefab;
    public int Width;
    public int Height;

    public Material DefaultMaterial;
    public Material MovementMaterial;
    public Material AttackMaterial;

    private readonly Dictionary<Vector2, Tile> _tiles = new();

    private void Awake()
    {
        // NEW: Singleton initialization.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var tileObject = Instantiate(TilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                tileObject.transform.SetParent(transform);
                var tile = tileObject.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(x, y);
                _tiles[new Vector2(x, y)] = tile;
            }
        }
    }

    public Tile GetTile(Vector2 position)
    {
        _tiles.TryGetValue(position, out var tile);
        return tile;
    }

    // MODIFIED: This method now uses the TileHighlight enum to determine which material to apply.
    public void HighlightTiles(IEnumerable<Tile> tiles, TileHighlight highlightType)
    {
        ClearHighlights();
        foreach (var tile in tiles)
        {
            var renderer = tile.GetComponent<Renderer>();
            switch (highlightType)
            {
                case TileHighlight.Movement:
                    renderer.material = MovementMaterial;
                    break;
                case TileHighlight.Attack:
                    renderer.material = AttackMaterial;
                    break;
                default:
                    renderer.material = DefaultMaterial;
                    break;
            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var tile in _tiles.Values)
        {
            tile.GetComponent<Renderer>().material = DefaultMaterial;
        }
    }
}