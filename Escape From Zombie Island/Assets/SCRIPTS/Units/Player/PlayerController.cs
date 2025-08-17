using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerController : UnitController
{
    private enum PlayerState { Idle, Moving, Targeting, PlacingTrap }
    private PlayerState currentState = PlayerState.Idle;

    [Header("Weapons & Traps")]
    public List<WeaponData> weapons;
    public List<TrapData> traps;
    private WeaponData currentWeapon;
    private TrapData currentTrap;

    [Header("Component References")]
    public PlayerAnimationController animationController;

    private Camera mainCamera;
    private TurnManager turnManager;
    private List<Tile> highlightedTiles = new List<Tile>();

    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
        turnManager = FindFirstObjectByType<TurnManager>();
        ResetAP();
    }

    void Update()
    {
        if (turnManager.CurrentState != TurnManager.TurnState.PlayerTurn || isMoving)
        {
            return;
        }
        HandleInput();
    }

    private void HandleInput()
    {
        // --- WEAPON SELECTION ---
        if (Keyboard.current.digit1Key.wasPressedThisFrame && weapons.Count > 0) { SelectWeapon(0); }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && weapons.Count > 1) { SelectWeapon(1); }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && weapons.Count > 2) { SelectWeapon(2); }

        // --- TRAP SELECTION ---
        if (Keyboard.current.digit4Key.wasPressedThisFrame && traps.Count > 0) { SelectTrap(0); }
        if (Keyboard.current.digit5Key.wasPressedThisFrame && traps.Count > 1) { SelectTrap(1); }

        // --- ACTION/MOVEMENT INPUT ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            switch (currentState)
            {
                case PlayerState.Targeting:
                    HandleTargetingClick();
                    break;
                case PlayerState.PlacingTrap:
                    HandleTrapPlacementClick();
                    break;
                case PlayerState.Idle:
                    HandleMovementClick();
                    break;
            }
        }

        // --- CANCEL ACTION ---
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelCurrentAction();
        }
    }

    private void SelectWeapon(int index)
    {
        if (currentState == PlayerState.Targeting && currentWeapon == weapons[index])
        {
            CancelCurrentAction();
            return;
        }
        CancelCurrentAction();
        currentWeapon = weapons[index];
        EnterTargetingMode();
    }

    private void SelectTrap(int index)
    {
        if (currentState == PlayerState.PlacingTrap && currentTrap == traps[index])
        {
            CancelCurrentAction();
            return;
        }
        CancelCurrentAction();
        currentTrap = traps[index];
        EnterTrapPlacementMode();
    }

    private void EnterTargetingMode()
    {
        if (currentWeapon == null) return;
        currentState = PlayerState.Targeting;
        SoundManager.Instance.PlaySound(currentWeapon.drawSound);
        if (currentWeapon.attackPattern != null)
        {
            highlightedTiles = currentWeapon.attackPattern.GetTilesInRange(currentTile, currentWeapon.range);
            foreach (var tile in highlightedTiles)
            {
                if (tile != null) tile.SetHighlight(true);
            }
        }
    }

    private void EnterTrapPlacementMode()
    {
        if (currentTrap == null) return;
        currentState = PlayerState.PlacingTrap;
        // Logic to highlight valid trap placement tiles. For now, let's just highlight adjacent tiles.
        highlightedTiles = GetAdjacentTiles(currentTrap.range);
        foreach (var tile in highlightedTiles)
        {
            if (tile != null) tile.SetHighlight(true);
        }
    }

    private void HandleTargetingClick()
    {
        Tile targetTile = gridManager.GetTileFromMousePosition();
        if (targetTile == null || !highlightedTiles.Contains(targetTile))
        {
            CancelCurrentAction();
            return;
        }

        ZombieController targetZombie = GetZombieAtTile(targetTile);
        if (targetZombie != null)
        {
            RequestAttack(targetZombie);
        }
        else
        {
            CancelCurrentAction();
        }
    }

    private void HandleTrapPlacementClick()
    {
        Tile targetTile = gridManager.GetTileFromMousePosition();
        if (targetTile != null && highlightedTiles.Contains(targetTile))
        {
            if (gridManager.IsTileOccupiedByTrap(targetTile))
            {
                Debug.Log("A trap already exists on this tile!");
                CancelCurrentAction();
                return;
            }
            if (SpendAP(currentTrap.apCost))
            {
                Instantiate(currentTrap.trapPrefab, targetTile.transform.position, Quaternion.identity);
                SoundManager.Instance.PlaySound(currentTrap.placementSound);
            }
        }
        CancelCurrentAction();
    }

    private void RequestAttack(ZombieController target)
    {
        if (SpendAP(currentWeapon.apCost))
        {
            StartCoroutine(AttackSequence(target));
        }
        else
        {
            Debug.Log("Not enough AP to attack!");
        }
    }

    private IEnumerator AttackSequence(ZombieController target)
    {
        isMoving = true;
        transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        SoundManager.Instance.PlaySound(currentWeapon.attackSound);

        yield return new WaitForSeconds(0.5f);

        if (target != null)
        {
            target.TakeDamage(currentWeapon.damage);
        }

        CancelCurrentAction();
        isMoving = false;
    }

    private void HandleMovementClick()
    {
        Tile targetTile = gridManager.GetTileFromMousePosition();
        if (targetTile != null && targetTile != currentTile)
        {
            List<Tile> path = gridManager.FindPath(currentTile, targetTile);
            if (path != null && path.Count > 1)
            {
                int apCost = path.Count - 1;
                if (SpendAP(apCost))
                {
                    // The path from FindPath includes the start tile, so we move along the rest of it.
                    List<Tile> movementPath = path.GetRange(1, path.Count - 1);
                    StartCoroutine(MoveAlongPath(movementPath));
                }
                else
                {
                    Debug.Log("Not enough AP to move there!");
                }
            }
        }
    }

    private void CancelCurrentAction()
    {
        if (currentState == PlayerState.Targeting && currentWeapon != null)
        {
            SoundManager.Instance.PlaySound(currentWeapon.sheatheSound);
        }

        foreach (var tile in highlightedTiles)
        {
            if (tile != null) tile.SetHighlight(false);
        }
        highlightedTiles.Clear();

        currentWeapon = null;
        currentTrap = null;
        currentState = PlayerState.Idle;
    }

    private ZombieController GetZombieAtTile(Tile tile)
    {
        Collider[] colliders = Physics.OverlapSphere(tile.transform.position, 0.2f);
        foreach (var collider in colliders)
        {
            ZombieController zombie = collider.GetComponent<ZombieController>();
            if (zombie != null)
            {
                return zombie;
            }
        }
        return null;
    }

    private List<Tile> GetAdjacentTiles(int range)
    {
        List<Tile> adjacentTiles = new List<Tile>();
        if (currentTile == null) return adjacentTiles;

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                if (x == 0 && z == 0) continue;

                int checkX = currentTile.x + x;
                int checkZ = currentTile.z + z;

                Tile tile = gridManager.GetTileAt(checkX, checkZ);
                if (tile != null)
                {
                    adjacentTiles.Add(tile);
                }
            }
        }
        return adjacentTiles;
    }
}