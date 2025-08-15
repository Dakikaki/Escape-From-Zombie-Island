using UnityEngine;

public class NoiseMakerController : MonoBehaviour
{
    private const float PLACEMENT_Y_OFFSET = 0.51f;

    private Tile myTile; // The tile this trap is sitting on

    void Start()
    {
        // Adjust position
        Vector3 currentPosition = transform.position;
        currentPosition.y = PLACEMENT_Y_OFFSET;
        transform.position = currentPosition;

        // Find the GridManager and the tile we are on
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            myTile = gridManager.GetTileAtWorldPosition(transform.position);
            // Register this trap with the GridManager
            gridManager.AddTrapToTile(myTile);
        }

        Debug.Log("Noise-Maker trap has been placed and is active!");
    }

    // Called automatically when the object is destroyed
    void OnDestroy()
    {
        // Un-register this trap from the GridManager
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            gridManager.RemoveTrapFromTile(myTile);
        }
    }
}