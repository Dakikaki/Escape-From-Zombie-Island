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
        if (turnManager.CurrentState != TurnManager.TurnState.PlayerTurn || currentState == PlayerState.Moving)
        {
            return;
        }
        HandleInput();
    }

    private void HandleInput()
    {
        // Weapon Selection
        if (Keyboard.current.digit1Key.wasPressedThisFrame) { EquipWeapon(0); }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) { EquipWeapon(1); }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) { EquipWeapon(2); }

        // Trap Selection
        if (Keyboard.current.digit4Key.wasPressedThisFrame) { SelectTrap(0); }
        if (Keyboard.current.digit5Key.wasPressedThisFrame) { SelectTrap(1); }

        // Action Input (Clicking)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentState == PlayerState.Targeting) { HandleTargetingClick(); }
            else if (currentState == PlayerState.PlacingTrap) { HandleTrapPlacementClick(); }
            else { HandleMovementClick(); }
        }

        // Cancel Input (Right Click)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (currentState == PlayerState.Targeting) { ExitTargetingMode(); }
            if (currentState == PlayerState.PlacingTrap) { ExitTrapPlacementMode(); }
        }
    }

    private void EquipWeapon(int index)
    {
        if (currentState == PlayerState.Targeting && weapons.Count > index && weapons[index] == currentWeapon)
        {
            ExitTargetingMode();
            return;
        }
        if (index < weapons.Count && weapons[index] != null)
        {
            CancelCurrentAction();
            currentWeapon = weapons[index];
            EnterTargetingMode();
        }
    }

    private void SelectTrap(int index)
    {
        if (currentState == PlayerState.PlacingTrap && traps.Count > index && traps[index] == currentTrap)
        {
            ExitTrapPlacementMode();
            return;
        }
        if (index < traps.Count && traps[index] != null)
        {
            CancelCurrentAction();
            currentTrap = traps[index];
            EnterTrapPlacementMode();
        }
    }

    private void EnterTargetingMode()
    {
        currentState = PlayerState.Targeting;
        Cursor.SetCursor(currentWeapon.cursor, Vector2.zero, CursorMode.Auto);
        animationController.SwitchPose(currentWeapon.attackPose);
        SoundManager.Instance.PlaySound(currentWeapon.drawSound);
        if (currentWeapon.attackPattern != null)
        {
            highlightedTiles = currentWeapon.attackPattern.GetTilesInRange(currentTile, currentWeapon.range);
        }
        foreach (var tile in highlightedTiles) { tile.SetHighlight(true); }
    }

    private void ExitTargetingMode()
    {
        if (currentWeapon == null) return;
        currentState = PlayerState.Idle;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        animationController.SwitchPose(PlayerAnimationController.Pose.Idle);
        SoundManager.Instance.PlaySound(currentWeapon.sheatheSound);
        foreach (var tile in highlightedTiles) { tile.SetHighlight(false); }
        highlightedTiles.Clear();
        currentWeapon = null;
    }

    private void EnterTrapPlacementMode()
    {
        currentState = PlayerState.PlacingTrap;
        highlightedTiles.Clear();
        if (currentTile != null)
        {
            highlightedTiles.Add(currentTile);
            currentTile.SetHighlight(true);
        }
    }

    private void ExitTrapPlacementMode()
    {
        currentState = PlayerState.Idle;
        foreach (var tile in highlightedTiles) { tile.SetHighlight(false); }
        highlightedTiles.Clear();
        currentTrap = null;
    }

    private void HandleMovementClick()
    {
        Tile targetTile = gridManager.GetTileFromMousePosition();
        if (targetTile != null && targetTile != currentTile)
        {
            RequestMoveToTile(targetTile);
        }
    }

    private void HandleTargetingClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ZombieController targetZombie = hit.collider.GetComponent<ZombieController>();
            if (targetZombie != null && highlightedTiles.Contains(targetZombie.currentTile))
            {
                RequestAttack(targetZombie);
            }
            else
            {
                ExitTargetingMode();
            }
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
                ExitTrapPlacementMode();
                return;
            }
            if (SpendAP(currentTrap.apCost))
            {
                Instantiate(currentTrap.trapPrefab, targetTile.transform.position, Quaternion.identity);
                ExitTrapPlacementMode();
            }
        }
        else
        {
            ExitTrapPlacementMode();
        }
    }

    private void CancelCurrentAction()
    {
        if (currentState == PlayerState.Targeting) ExitTargetingMode();
        if (currentState == PlayerState.PlacingTrap) ExitTrapPlacementMode();
    }

    public override void ResetAP()
    {
        currentAP = this.maxAP;
        InvokeOnAPChanged();
    }

    public override bool SpendAP(int amount)
    {
        if (currentAP >= amount)
        {
            currentAP -= amount;
            InvokeOnAPChanged();
            return true;
        }
        return false;
    }

    private void RequestMoveToTile(Tile targetTile)
    {
        if (currentTile == null) { return; }
        List<Tile> path = gridManager.FindPath(currentTile, targetTile);
        if (path != null && path.Count > 1)
        {
            int apCost = path.Count - 1;
            if (SpendAP(apCost))
            {
                StartCoroutine(MoveAndAnimate(path));
            }
        }
    }

    private IEnumerator MoveAndAnimate(List<Tile> path)
    {
        currentState = PlayerState.Moving;
        animationController.SwitchPose(PlayerAnimationController.Pose.Walk);
        List<Tile> movementPath = path.GetRange(1, path.Count - 1);
        yield return StartCoroutine(MoveAlongPath(movementPath));
        animationController.SwitchPose(PlayerAnimationController.Pose.Idle);
        currentState = PlayerState.Idle;
    }

    private void RequestAttack(ZombieController target)
    {
        if (SpendAP(currentWeapon.apCost))
        {
            StartCoroutine(AttackSequence(target));
        }
    }

    private IEnumerator AttackSequence(ZombieController target)
    {
        currentState = PlayerState.Moving;
        transform.LookAt(target.transform);
        SoundManager.Instance.PlaySound(currentWeapon.attackSound);
        yield return new WaitForSeconds(0.3f);
        if (target != null)
        {
            target.TakeDamage(currentWeapon.damage);
        }
        ExitTargetingMode();
    }

    private List<Tile> GetAdjacentTiles(int range)
    {
        List<Tile> adjacentTiles = new List<Tile>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                if (x == 0 && z == 0) continue;
                Tile tile = gridManager.GetTile(currentTile.x + x, currentTile.z + z);
                if (tile != null)
                {
                    adjacentTiles.Add(tile);
                }
            }
        }
        return adjacentTiles;
    }
}