using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public AttackPattern attackPattern;
    public int range = 5;

    [Header("Combat Stats")]
    public int apCost = 1;
    public int damage = 1;

    [Header("Visuals & Audio")]
    public Sprite icon;
    public GameObject attackPose;
    public Texture2D cursor; // Corrected from Sprite to Texture2D
    public AudioClip drawSound;
    public AudioClip sheatheSound;
    public AudioClip attackSound;
    public GameObject attackEffect; // e.g., muzzle flash, slash effect
}