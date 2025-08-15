using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Adjacent Pattern", menuName = "Weapons/Patterns/Adjacent")]
public class AdjacentPattern : AttackPattern
{
    public override List<Tile> GetValidTargets(GridManager gridManager, Tile startTile, int range)
    {
        List<Tile> validTiles = new List<Tile>();
        if (gridManager == null || startTile == null) return validTiles;

        // Loop through a square grid centered on the start tile
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                // Skip the center tile itself
                if (x == 0 && z == 0) continue;

                Tile tile = gridManager.GetTile(startTile.x + x, startTile.z + z);
                if (tile != null)
                {
                    validTiles.Add(tile);
                }
            }
        }
        return validTiles;
    }
}