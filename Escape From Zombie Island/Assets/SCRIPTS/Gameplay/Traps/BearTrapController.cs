using UnityEngine;

/// <summary>
/// Contains the specific behavior for the Bear Trap.
/// </summary>
public class BearTrapController : MonoBehaviour
{
    private const float PLACEMENT_Y_OFFSET = 0.51f;
    private Tile myTile; // The tile this trap is sitting on

    [Header("Trap Stats")]
    public int damage = 1; // As per GDD, this will root and do minor damage

    private bool isArmed = true;

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
    }

    // This function is called automatically by Unity when another collider enters this one.
    void OnTriggerEnter(Collider other)
    {
        // Check if the trap is armed and if the object that entered is a zombie
        if (isArmed && other.GetComponent<ZombieController>() != null)
        {
            Debug.Log("BEAR TRAP TRIGGERED!");

            // Get the ZombieController and deal damage
            ZombieController zombie = other.GetComponent<ZombieController>();
            zombie.TakeDamage(damage);

            // Future logic: Apply a "Rooted" status effect to the zombie here.

            // The trap is now sprung and can be destroyed.
            isArmed = false;
            Destroy(gameObject, 0.5f); // Destroy the trap after a short delay
        }
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