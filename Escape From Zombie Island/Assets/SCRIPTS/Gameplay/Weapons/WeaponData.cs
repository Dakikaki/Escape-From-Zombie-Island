// Escape From Zombie Island
// WeaponData.cs
// by Dakikaki
// 2024

using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon")]
public class WeaponData : ScriptableObject
{
    public string Name;
    public int ApCost;

    // NEW: Added the Damage field.
    public int Damage;

    // NEW: Added the pose field to link this weapon to a specific player animation pose.
    public PlayerAnimationController.Pose pose;

    // NEW: Added the AttackPattern field to define the weapon's range and shape of attack.
    public AttackPattern AttackPattern;
}