using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Knight Pattern", menuName = "Weapons/Patterns/Knight")]
public class KnightPattern : AttackPattern
{
    private readonly Vector2Int[] knightMoves = new Vector2Int[]
    {
        new Vector2Int(1, 2), new Vector2Int(1, -2),
        new Vector2Int(-1, 2), new Vector2Int(-1, -2),
        new Vector2Int(2, 1), new Vector2Int(2, -1),
        new Vector2Int(-2, 1), new Vector2Int(-2, -1)
    };

    public override List<Tile> GetValidTargets(GridManager gridManager, Tile startTile, int range)
    {
        List<Tile> validTiles = new List<Tile>();
        if (gridManager == null || startTile == null) return validTiles;

        foreach (var move in knightMoves)
        {
            Tile tile = gridManager.GetTile(startTile.x + move.x, startTile.z + move.y);
            if (tile != null)
            {
                validTiles.Add(tile);
            }
        }
        return validTiles;
    }
}