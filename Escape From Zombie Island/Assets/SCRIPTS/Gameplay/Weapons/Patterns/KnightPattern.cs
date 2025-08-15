using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Knight Pattern", menuName = "Game Data/Attack Patterns/Knight")]
public class KnightPattern : AttackPattern
{
    public override List<Tile> GetTilesInRange(Tile startingTile, int range)
    {
        List<Tile> validTiles = new List<Tile>();
        GridManager gridManager = FindFirstObjectByType<GridManager>();

        if (startingTile == null || gridManager == null)
        {
            return validTiles;
        }

        // Possible L-shaped moves for a knight
        int[] xMoves = { 1, 1, 2, 2, -1, -1, -2, -2 };
        int[] zMoves = { 2, -2, 1, -1, 2, -2, 1, -1 };

        for (int i = 0; i < 8; i++)
        {
            Tile tile = gridManager.GetTileAt(startingTile.x + xMoves[i], startingTile.z + zMoves[i]);
            if (tile != null)
            {
                validTiles.Add(tile);
            }
        }
        return validTiles;
    }
}