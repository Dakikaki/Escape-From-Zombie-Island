using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// Discovers the grid, manages tile data, handles all A* pathfinding requests,
/// and tracks trap placements.
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("The spacing between each tile. Must match editor placement!")]
    public float tileSpacing = 1.0f;

    private Tile[,] grid;
    private Camera mainCamera;
    private HashSet<Tile> tilesWithTraps = new HashSet<Tile>();

    public int GridWidth { get; private set; }
    public int GridHeight { get; private set; }
    public Vector2Int GridOffset { get; private set; }

    void Awake()
    {
        mainCamera = Camera.main;
        DiscoverAndRegisterGrid();
    }

    // --- Grid Initialization ---
    private void DiscoverAndRegisterGrid()
    {
        if (transform.childCount == 0)
        {
            Debug.LogError("GridManager has no child Tile objects to register!", this.gameObject);
            return;
        }

        Vector2Int minBounds = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int maxBounds = new Vector2Int(int.MinValue, int.MinValue);

        foreach (Transform child in transform)
        {
            int x = Mathf.RoundToInt(child.position.x / tileSpacing);
            int z = Mathf.RoundToInt(child.position.z / tileSpacing);
            minBounds.x = Mathf.Min(minBounds.x, x);
            minBounds.y = Mathf.Min(minBounds.y, z);
            maxBounds.x = Mathf.Max(maxBounds.x, x);
            maxBounds.y = Mathf.Max(maxBounds.y, z);
        }

        GridOffset = minBounds;
        GridWidth = maxBounds.x - minBounds.x + 1;
        GridHeight = maxBounds.y - minBounds.y + 1;
        grid = new Tile[GridWidth, GridHeight];

        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile != null)
            {
                int worldX = Mathf.RoundToInt(child.position.x / tileSpacing);
                int worldZ = Mathf.RoundToInt(child.position.z / tileSpacing);
                int arrayX = worldX - GridOffset.x;
                int arrayZ = worldZ - GridOffset.y;

                tile.x = worldX;
                tile.z = worldZ;
                grid[arrayX, arrayZ] = tile;
            }
        }
    }

    // --- Public Helper Functions ---

    public Tile GetTileFromMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<Tile>();
        }
        return null;
    }

    public Tile GetTileAtWorldPosition(Vector3 worldPosition)
    {
        // Use a physics check for robustness, in case a unit isn't perfectly centered.
        Collider[] colliders = Physics.OverlapSphere(worldPosition, 0.2f);
        foreach (var collider in colliders)
        {
            Tile tile = collider.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
        }
        return null;
    }

    public Tile GetTile(int x, int z)
    {
        int arrayX = x - GridOffset.x;
        int arrayZ = z - GridOffset.y;
        if (arrayX >= 0 && arrayX < GridWidth && arrayZ >= 0 && arrayZ < GridHeight)
        {
            return grid[arrayX, arrayZ];
        }
        return null;
    }

    // --- A* Pathfinding Logic ---

    public List<Tile> FindPath(Tile startTile, Tile endTile)
    {
        if (startTile == null || endTile == null) return null;

        List<Tile> openList = new List<Tile> { startTile };
        HashSet<Tile> closedSet = new HashSet<Tile>();

        foreach (Tile tile in grid)
        {
            if (tile == null) continue;
            tile.gCost = int.MaxValue;
            tile.CalculateFCost();
            tile.parent = null;
        }

        startTile.gCost = 0;
        startTile.hCost = CalculateDistanceCost(startTile, endTile);
        startTile.CalculateFCost();

        while (openList.Count > 0)
        {
            Tile currentTile = GetLowestFCostTile(openList);
            if (currentTile == endTile) { return ReconstructPath(endTile); }

            openList.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (Tile neighbour in GetNeighbours(currentTile))
            {
                if (closedSet.Contains(neighbour)) continue;

                int tentativeGCost = currentTile.gCost + 10; // 10 is a standard cost for grid movement
                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.parent = currentTile;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = CalculateDistanceCost(neighbour, endTile);
                    neighbour.CalculateFCost();
                    if (!openList.Contains(neighbour)) { openList.Add(neighbour); }
                }
            }
        }
        return null; // No path found
    }

    private List<Tile> ReconstructPath(Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;
        while (currentTile != null)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
        return path;
    }

    private List<Tile> GetNeighbours(Tile currentTile)
    {
        List<Tile> neighbours = new List<Tile>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
        foreach (var dir in directions)
        {
            Tile neighbour = GetTile(currentTile.x + dir.x, currentTile.z + dir.y);
            if (neighbour != null) { neighbours.Add(neighbour); }
        }
        return neighbours;
    }

    private int CalculateDistanceCost(Tile a, Tile b)
    {
        int distX = Mathf.Abs(a.x - b.x);
        int distZ = Mathf.Abs(a.z - b.z);
        return 10 * (distX + distZ); // Using Manhattan distance
    }

    private Tile GetLowestFCostTile(List<Tile> tileList)
    {
        Tile lowestFCostTile = tileList[0];
        for (int i = 1; i < tileList.Count; i++)
        {
            if (tileList[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = tileList[i];
            }
        }
        return lowestFCostTile;
    }

    // --- Trap Management ---

    public bool IsTileOccupiedByTrap(Tile tile)
    {
        return tilesWithTraps.Contains(tile);
    }

    public void AddTrapToTile(Tile tile)
    {
        if (tile != null)
        {
            tilesWithTraps.Add(tile);
        }
    }

    public void RemoveTrapFromTile(Tile tile)
    {
        if (tile != null)
        {
            tilesWithTraps.Remove(tile);
        }
    }
}