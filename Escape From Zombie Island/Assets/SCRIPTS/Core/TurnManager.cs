using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerController player;
    public List<ZombieController> zombies;

    private enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        Processing
    }

    private TurnState currentState;

    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        // Find all active zombies in the scene using the new, recommended method
        zombies.AddRange(FindObjectsByType<ZombieController>(FindObjectsSortMode.None));

        // Start the game with the player's turn
        StartPlayerTurn();
    }

    void Update()
    {
        // For now, we'll just use a key press to end the player's turn.
        // This will be replaced by UI buttons later.
        if (currentState == TurnState.PlayerTurn && Input.GetKeyDown(KeyCode.Space))
        {
            EndPlayerTurn();
        }
    }

    void StartPlayerTurn()
    {
        Debug.Log("--- PLAYER'S TURN ---");
        currentState = TurnState.PlayerTurn;
        if (player != null)
        {
            player.ResetAP();
        }
    }

    void EndPlayerTurn()
    {
        currentState = TurnState.Processing;
        Debug.Log("Player ends their turn.");
        StartCoroutine(EnemyTurnRoutine());
    }

    IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("--- ZOMBIES' TURN ---");
        currentState = TurnState.EnemyTurn;

        // Make each zombie take their turn, one by one
        foreach (var zombie in zombies)
        {
            if (zombie != null)
            {
                // Call the method first, then log a message.
                zombie.TakeTurn();
                Debug.Log($"{zombie.name} takes its turn.");

                // Wait for a short moment before the next zombie moves
                yield return new WaitForSeconds(1.0f);
            }
        }

        // After all enemies have moved, start the player's turn again.
        yield return new WaitForSeconds(1.0f); // A brief pause before the next turn starts
        StartPlayerTurn();
    }
}