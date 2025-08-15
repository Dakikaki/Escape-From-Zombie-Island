using UnityEngine;

[CreateAssetMenu(fileName = "New Trap", menuName = "Game Data/Trap")]
public class TrapData : ScriptableObject
{
    [Header("Trap Info")]
    public string trapName;
    [Tooltip("A brief description of what the trap does.")]
    [TextArea] public string description;
    public GameObject trapPrefab;
    public int range = 1; // How far the trap's effect reaches, if applicable.

    [Header("Combat Stats")]
    public int apCost = 1; // How many Action Points it costs to place this trap.
    public int damage = 1; // How much damage the trap deals upon activation.
    public int uses = 1; // How many times this trap can be placed per encounter.

    [Header("Visuals & Audio")]
    public Sprite icon; // Icon used in the UI.
    public Sprite cursor; // The cursor to show when placing this trap.
    public AudioClip placementSound; // Sound played when the trap is placed.
    public AudioClip triggerSound; // Sound played when the trap is activated.
    public GameObject triggerEffect; // Visual effect played when the trap is triggered.
}