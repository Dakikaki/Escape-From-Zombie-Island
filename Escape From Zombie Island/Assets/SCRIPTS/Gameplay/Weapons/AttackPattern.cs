using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// An abstract Scriptable Object that serves as a template for all weapon targeting behaviors.
/// </summary>
public abstract class AttackPattern : ScriptableObject
{
    /// <summary>
    /// The core method for every pattern. Given a starting point and a grid, 
    /// it returns all valid target tiles.
    /// </summary>
    /// <param name="gridManager">A reference to the GridManager to query for tiles.</param>
    /// <param name="startTile">The tile the unit is currently on.</param>
    /// <param name="range">The range of the attack, used differently by each pattern.</param>
    /// <returns>A list of all tiles that are valid targets.</returns>
    public abstract List<Tile> GetValidTargets(GridManager gridManager, Tile startTile, int range);
}