// Escape From Zombie Island
// PlayerController.cs
// by Dakikaki
// 2024

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : UnitController
{
    public PlayerAnimationController AnimationController;
    public WeaponData[] weapons;
    private WeaponData _currentWeapon;
    private int _currentWeaponIndex = -1; // -1 means no weapon is equipped

    public void OnWeapon1(InputValue value)
    {
        if (value.isPressed)
        {
            EquipWeapon(0);
        }
    }

    public void OnWeapon2(InputValue value)
    {
        if (value.isPressed)
        {
            EquipWeapon(1);
        }
    }

    public void OnWeapon3(InputValue value)
    {
        if (value.isPressed)
        {
            EquipWeapon(2);
        }
    }

    public void OnWeapon4(InputValue value)
    {
        if (value.isPressed)
        {
            EquipWeapon(3);
        }
    }

    public void OnWeapon5(InputValue value)
    {
        if (value.isPressed)
        {
            EquipWeapon(4);
        }
    }

    private void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            // If the same weapon is equipped, unequip it
            if (_currentWeaponIndex == weaponIndex)
            {
                _currentWeapon = null;
                _currentWeaponIndex = -1;
                AnimationController.ChangePose(PlayerAnimationController.Pose.Idle);
                GridManager.Instance.ClearHighlights();
            }
            else
            {
                _currentWeapon = weapons[weaponIndex];
                _currentWeaponIndex = weaponIndex;

                // MODIFIED: The pose is now changed based on the weapon's designated pose.
                AnimationController.ChangePose(_currentWeapon.pose);
                HighlightAttackPattern();
            }
        }
    }

    private void HighlightAttackPattern()
    {
        if (_currentWeapon != null)
        {
            GridManager.Instance.HighlightTiles(_currentWeapon.AttackPattern.GetPattern(currentTile), TileHighlight.Attack);
        }
    }

    // NEW: Added the attack functionality when the attack input is pressed.
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && _currentWeapon != null)
        {
            Attack();
        }
    }

    private void Attack()
    {
        var tilesToAttack = _currentWeapon.AttackPattern.GetPattern(currentTile);
        foreach (var tile in tilesToAttack)
        {
            if (tile.Unit != null && tile.Unit is ZombieController zombie)
            {
                zombie.TakeDamage(_currentWeapon.Damage);
            }
        }

        // After attacking, return to idle pose and clear highlights
        _currentWeapon = null;
        _currentWeaponIndex = -1;
        AnimationController.ChangePose(PlayerAnimationController.Pose.Idle);
        GridManager.Instance.ClearHighlights();
    }
}