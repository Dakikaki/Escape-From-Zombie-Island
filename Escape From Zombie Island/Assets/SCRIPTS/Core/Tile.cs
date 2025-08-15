using UnityEngine;

/// <summary>
/// Holds data for a single tile and manages its visual state.
/// </summary>
public class Tile : MonoBehaviour
{
    // --- Grid Position ---
    public int x;
    public int z;

    [Header("Visuals")]
    [Tooltip("The material to use when this tile is highlighted as a valid target.")]
    public Material highlightMaterial;

    // --- Private State ---
    private Material originalMaterial;
    private MeshRenderer meshRenderer;

    // --- A* Pathfinding Data (Unchanged) ---
    [HideInInspector] public int gCost;
    [HideInInspector] public int hCost;
    [HideInInspector] public int fCost;
    [HideInInspector] public Tile parent;

    void Awake()
    {
        // Get a reference to the MeshRenderer to change materials
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // Store the original material so we can switch back to it
            originalMaterial = meshRenderer.material;
        }
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    /// <summary>
    /// Sets the tile's visual state to be highlighted or not.
    /// </summary>
    /// <param name="isHighlighted">True to apply the highlight material, false to revert to original.</param>
    public void SetHighlight(bool isHighlighted)
    {
        if (meshRenderer != null && highlightMaterial != null)
        {
            meshRenderer.material = isHighlighted ? highlightMaterial : originalMaterial;
        }
    }
}