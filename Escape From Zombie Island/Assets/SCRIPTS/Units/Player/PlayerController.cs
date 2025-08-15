using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController
{
    [Header("Weapons")]
    [Tooltip("The list of weapons this player has available.")]
    public List<WeaponData> availableWeapons;
    [Tooltip("The weapon the player starts the scene with.")]
    public WeaponData startingWeapon;

    [Header("Dependencies")]
    [Tooltip("Reference to the animation controller for this player.")]
    public PlayerAnimationController animationController;

    private WeaponData currentWeapon;

    // --- Unity Methods ---

    protected override void Start()
    {
        // Run the Start logic from the base UnitController class first
        base.Start();
        // Now run the player-specific start logic
        EquipWeapon(startingWeapon);
    }

    void Update()
    {
        // Simple input handling for now. This will be replaced by a more robust system.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(availableWeapons[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && availableWeapons.Count > 1)
        {
            EquipWeapon(availableWeapons[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && availableWeapons.Count > 2)
        {
            EquipWeapon(availableWeapons[2]);
        }
    }

    // --- Public Methods ---

    public void EquipWeapon(WeaponData weaponToEquip)
    {
        if (weaponToEquip == null)
        {
            Debug.LogError("Tried to equip a null weapon.");
            return;
        }

        if (currentWeapon != null && currentWeapon.sheatheSound != null)
        {
            // Play sound via SoundManager if it exists
        }

        currentWeapon = weaponToEquip;
        Debug.Log("Equipped: " + currentWeapon.weaponName);

        if (currentWeapon.cursor != null)
        {
            Cursor.SetCursor(currentWeapon.cursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // Revert to default
        }

        if (currentWeapon.drawSound != null)
        {
            // Play sound via SoundManager if it exists
        }

        // Show the valid attackable tiles for the newly equipped weapon
        List<Tile> attackableTiles = currentWeapon.attackPattern.GetTilesInRange(currentTile, currentWeapon.range);
        gridManager.HighlightTiles(attackableTiles);

        // Switch the player's pose
        SwitchToPose(currentWeapon.attackPose);
    }

    // --- Private Methods ---

    private void SwitchToPose(GameObject posePrefab, AudioClip sheatheSound = null, AudioClip drawSound = null)
    {
        if (sheatheSound != null) { }
        if (drawSound != null) { }

        if (animationController != null)
        {
            animationController.SwitchPose(posePrefab);
        }
    }

    private void SwitchToPose(GameObject posePrefab)
    {
        if (animationController != null)
        {
            animationController.SwitchPose(posePrefab);
        }
    }
}