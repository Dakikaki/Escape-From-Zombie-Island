using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieController : UnitController
{
    [Header("Zombie Settings")]
    public int movementRange = 2; // The maximum number of tiles the zombie can move in one turn.

    private PlayerController player;
    private List<Tile> currentPath;

    protected override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerController>();
        this.moveSpeed = 2f; // Set the inherited moveSpeed
    }

    void Update()
    {
        // Zombie logic can go here if needed every frame
    }

    public IEnumerator TakeTurn()
    {
        if (isMoving || player == null || player.currentTile == null) yield break;

        // Find the full path to the player
        currentPath = gridManager.FindPath(currentTile, player.currentTile);

        // If a path exists, determine how many steps to take
        if (currentPath != null && currentPath.Count > 1) // Path includes the start tile, so > 1 means there's somewhere to go
        {
            // Create a list for the path this zombie will actually walk this turn
            List<Tile> pathThisTurn = new List<Tile>();

            // The path includes the zombie's starting tile, so we look at the next tiles
            // We can move up to 'movementRange' steps.
            int stepsToTake = Mathf.Min(movementRange, currentPath.Count - 1);

            for (int i = 1; i <= stepsToTake; i++)
            {
                pathThisTurn.Add(currentPath[i]);
            }

            // If there's a valid path for this turn, move along it
            if (pathThisTurn.Count > 0)
            {
                yield return StartCoroutine(MoveAlongPath(pathThisTurn));
            }
        }
    }

    // This method is inherited from UnitController and moves the character along the provided path list
    // No changes are needed here, as we are now feeding it the correctly shortened path.
}