using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float tileSize = 1.0f;
    public GameObject tilePrefab;

    private Dictionary<Vector2, Tile> grid;

    void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Dictionary<Vector2, Tile>();
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is not assigned in GridManager!");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileObject.name = $"Tile_{x}_{z}";
                Tile tile = tileObject.GetComponent<Tile>();
                tile.x = x;
                tile.z = z;
                grid[new Vector2(x, z)] = tile;
            }
        }
    }

    public Tile GetTileAt(int x, int z)
    {
        grid.TryGetValue(new Vector2(x, z), out Tile tile);
        return tile;
    }

    public Tile GetTileAtWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / tileSize);
        int z = Mathf.RoundToInt(worldPosition.z / tileSize);
        return GetTileAt(x, z);
    }

    public void HighlightTiles(List<Tile> tilesToHighlight)
    {
        foreach (Tile tile in grid.Values)
        {
            tile.SetHighlight(false);
        }

        foreach (Tile tile in tilesToHighlight)
        {
            tile.SetHighlight(true);
        }
    }

    public void AddTrapToTile(Tile tile) { }
    public void RemoveTrapFromTile(Tile tile) { }

    #region A* Pathfinding

    public List<Tile> FindPath(Tile startTile, Tile endTile)
    {
        List<Tile> openList = new List<Tile> { startTile };
        HashSet<Tile> closedList = new HashSet<Tile>();

        foreach (var tile in grid.Values)
        {
            tile.gCost = int.MaxValue;
            tile.CalculateFCost();
            tile.parent = null;
        }

        startTile.gCost = 0;
        startTile.hCost = CalculateDistance(startTile, endTile);
        startTile.CalculateFCost();

        while (openList.Count > 0)
        {
            Tile currentTile = openList.OrderBy(t => t.fCost).First();

            if (currentTile == endTile)
            {
                return RetracePath(startTile, endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (Tile neighbour in GetNeighbours(currentTile))
            {
                if (closedList.Contains(neighbour)) continue;

                int tentativeGCost = currentTile.gCost + CalculateDistance(currentTile, neighbour);
                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.parent = currentTile;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = CalculateDistance(neighbour, endTile);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        return null; // Path not found
    }

    private List<Tile> RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
        return path;
    }

    private List<Tile> GetNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();
        int[] xOffsets = { 0, 0, 1, -1 };
        int[] zOffsets = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            Tile neighbour = GetTileAt(tile.x + xOffsets[i], tile.z + zOffsets[i]);
            if (neighbour != null)
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }

    private int CalculateDistance(Tile a, Tile b)
    {
        int distX = Mathf.Abs(a.x - b.x);
        int distZ = Mathf.Abs(a.z - b.z);
        return 10 * (distX + distZ); // Manhattan distance
    }

    #endregion
}