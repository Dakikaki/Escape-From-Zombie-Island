using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Line Pattern", menuName = "Game Data/Attack Patterns/Line")]
public class LinePattern : AttackPattern
{
    public override List<Tile> GetTilesInRange(Tile startingTile, int range)
    {
        List<Tile> validTiles = new List<Tile>();
        GridManager gridManager = FindFirstObjectByType<GridManager>();

        if (startingTile == null || gridManager == null)
        {
            return validTiles;
        }

        // Horizontal and Vertical Lines
        for (int i = 1; i <= range; i++)
        {
            // Right
            validTiles.Add(gridManager.GetTileAt(startingTile.x + i, startingTile.z));
            // Left
            validTiles.Add(gridManager.GetTileAt(startingTile.x - i, startingTile.z));
            // Up
            validTiles.Add(gridManager.GetTileAt(startingTile.x, startingTile.z + i));
            // Down
            validTiles.Add(gridManager.GetTileAt(startingTile.x, startingTile.z - i));
        }

        // Remove any nulls if tiles were out of bounds
        validTiles.RemoveAll(item => item == null);
        return validTiles;
    }
}