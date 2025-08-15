using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Adjacent Pattern", menuName = "Game Data/Attack Patterns/Adjacent")]
public class AdjacentPattern : AttackPattern
{
    public override List<Tile> GetTilesInRange(Tile startingTile, int range)
    {
        List<Tile> validTiles = new List<Tile>();
        GridManager gridManager = FindFirstObjectByType<GridManager>();

        if (startingTile == null || gridManager == null)
        {
            return validTiles;
        }

        // Check all 8 directions around the starting tile
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue; // Skip the center tile

                Tile tile = gridManager.GetTileAt(startingTile.x + x, startingTile.z + z);
                if (tile != null)
                {
                    validTiles.Add(tile);
                }
            }
        }
        return validTiles;
    }
}