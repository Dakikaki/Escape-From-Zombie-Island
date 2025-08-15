using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public AttackPattern attackPattern;
    public int range;

    [Header("Combat Stats")]
    public int apCost = 1;
    public int damage = 1;

    [Header("Visuals & Audio")]
    public PlayerAnimationController.Pose attackPose;
    public Texture2D cursor;
    public AudioClip drawSound;
    public AudioClip sheatheSound;
    public AudioClip attackSound;
}