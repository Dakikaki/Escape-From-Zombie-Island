using UnityEngine;

/// <summary>
/// A Scriptable Object template that defines the properties of a trap.
/// </summary>
[CreateAssetMenu(fileName = "New Trap", menuName = "Traps/Trap Data")]
public class TrapData : ScriptableObject
{
    [Header("Trap Info")]
    public string trapName;
    [Tooltip("The prefab that contains the trap's visual model and behavior script (e.g., NoiseMakerController).")]
    public GameObject trapPrefab;

    [Header("Gameplay Stats")]
    [Tooltip("How many Action Points it costs to place this trap.")]
    public int apCost = 2;
}