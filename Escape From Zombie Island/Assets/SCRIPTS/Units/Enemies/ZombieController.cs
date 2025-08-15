using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieController : UnitController
{
    private PlayerController player;

    protected override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerController>();
    }

    public IEnumerator TakeTurn()
    {
        // This logic is now simpler as it just uses the base class health
        if (isMoving) yield break;

        List<Tile> path = gridManager.FindPath(currentTile, player.currentTile);

        if (path != null && path.Count > 1)
        {
            if (path.Count == 2)
            {
                Debug.Log("Zombie attacks Player!");
                player.TakeDamage(1); // Assuming 1 damage for now
            }
            else
            {
                List<Tile> firstStep = new List<Tile> { path[1] };
                yield return StartCoroutine(MoveAlongPath(firstStep));
            }
        }
    }
}