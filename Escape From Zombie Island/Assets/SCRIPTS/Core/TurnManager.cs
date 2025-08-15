using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Manages the game turns using a flexible event-based system.
/// </summary>
public class TurnManager : MonoBehaviour
{
    public enum TurnState { PlayerTurn, ZombieTurn }
    public TurnState CurrentState { get; private set; }

    private Queue<ZombieController> zombieTurnQueue = new Queue<ZombieController>();
    private PlayerController player;

    // Event that units can subscribe to, to know when their turn is over.
    public static event Action OnPlayerTurnEnd;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        var zombies = FindObjectsByType<ZombieController>(FindObjectsSortMode.None);
        foreach (var zombie in zombies)
        {
            zombieTurnQueue.Enqueue(zombie);
        }
        StartPlayerTurn();
    }

    void Update()
    {
        if (CurrentState == TurnState.PlayerTurn && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            EndPlayerTurn();
        }
    }

    private void StartPlayerTurn()
    {
        CurrentState = TurnState.PlayerTurn;
        player.ResetAP();
        Debug.Log("--- PLAYER TURN ---");
    }

    private void EndPlayerTurn()
    {
        OnPlayerTurnEnd?.Invoke(); // Announce that the player's turn is over.
        StartZombieTurnPhase();
    }

    private void StartZombieTurnPhase()
    {
        CurrentState = TurnState.ZombieTurn;
        Debug.Log("--- ZOMBIE TURN ---");
        StartCoroutine(ProcessZombieQueue());
    }

    private IEnumerator ProcessZombieQueue()
    {
        // Process one zombie at a time from the queue.
        foreach (var zombie in zombieTurnQueue)
        {
            yield return StartCoroutine(zombie.TakeTurn());
        }

        // All zombies have acted, start the player's turn again.
        StartPlayerTurn();
    }
}