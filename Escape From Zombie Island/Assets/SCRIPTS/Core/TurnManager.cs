using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class TurnManager : MonoBehaviour
{
    public enum TurnState { PlayerTurn, ZombieTurn }
    public TurnState CurrentState { get; private set; }

    private Queue<ZombieController> zombieTurnQueue = new Queue<ZombieController>();
    private PlayerController player;

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
        OnPlayerTurnEnd?.Invoke();
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
        foreach (var zombie in zombieTurnQueue)
        {
            if (zombie != null)
            {
                yield return StartCoroutine(zombie.TakeTurn());
            }
        }

        StartPlayerTurn();
    }
}