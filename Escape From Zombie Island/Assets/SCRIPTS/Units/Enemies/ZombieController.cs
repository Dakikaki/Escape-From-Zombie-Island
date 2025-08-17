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

    public IEnumerator TakeTurn()
    {
        if (isMoving || player == null || player.currentTile == null) yield break;

        // Find the full path to the player
        currentPath = gridManager.FindPath(currentTile, player.currentTile);

        // If a path exists, determine how many steps to take
        if (currentPath != null && currentPath.Count > 1)
        {
            List<Tile> pathThisTurn = new List<Tile>();
            int stepsToTake = Mathf.Min(movementRange, currentPath.Count - 1);

            for (int i = 1; i <= stepsToTake; i++)
            {
                pathThisTurn.Add(currentPath[i]);
            }

            if (pathThisTurn.Count > 0)
            {
                yield return StartCoroutine(MoveAlongPath(pathThisTurn));
            }
        }
    }

    // This method is called by TakeDamage in the base UnitController class when health reaches 0.
    protected override void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");

        // You could add a death animation or particle effect here.

        // Remove the zombie from the game.
        base.Die();
    }
}