using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject that defines a pattern of tiles for an attack or ability.
/// </summary>
public abstract class AttackPattern : ScriptableObject
{
    /// <summary>
    /// Gets a list of tiles based on this pattern.
    /// </summary>
    /// <param name="startingTile">The tile the unit is on.</param>
    /// <param name="range">The range of the attack.</param>
    /// <returns>A list of valid target tiles.</returns>
    public abstract List<Tile> GetTilesInRange(Tile startingTile, int range);
}