using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieController : UnitController
{
    [Header("Zombie Settings")]
    public float moveSpeed = 2f;
    private PlayerController player;
    private bool isMoving = false;
    private List<Tile> currentPath;

    protected override void Start()
    {
        base.Start();
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        // Zombie logic can go here if needed every frame
    }

    public void TakeTurn()
    {
        if (isMoving || player == null) return;

        // Find the path to the player
        currentPath = gridManager.FindPath(currentTile, player.currentTile);

        // If a path exists, start moving
        if (currentPath != null && currentPath.Count > 0)
        {
            StartCoroutine(MoveAlongPath(currentPath));
        }
    }

    private IEnumerator MoveAlongPath(List<Tile> path)
    {
        isMoving = true;

        foreach (Tile tile in path)
        {
            Vector3 targetPosition = tile.transform.position;
            // Keep the zombie's original y position
            targetPosition.y = transform.position.y;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition; // Snap to final position
            currentTile = tile;
        }

        isMoving = false;
    }
}