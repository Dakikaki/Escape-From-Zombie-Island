using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Line Pattern", menuName = "Weapons/Patterns/Line")]
public class LinePattern : AttackPattern
{
    public override List<Tile> GetValidTargets(GridManager gridManager, Tile startTile, int range)
    {
        List<Tile> validTiles = new List<Tile>();
        if (gridManager == null || startTile == null) return validTiles;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            for (int i = 1; i <= range; i++)
            {
                Tile tile = gridManager.GetTile(startTile.x + dir.x * i, startTile.z + dir.y * i);

                // If we hit an empty space, the line of sight is blocked.
                if (tile == null) break;

                validTiles.Add(tile);

                // Check if any unit is on this tile
                var units = FindObjectsByType<UnitController>(FindObjectsSortMode.None);
                bool isBlocked = false;
                foreach (var unit in units)
                {
                    if (unit.currentTile == tile)
                    {
                        isBlocked = true;
                        break;
                    }
                }

                // If the tile is blocked, stop casting further in this direction.
                if (isBlocked) break;
            }
        }
        return validTiles;
    }
}