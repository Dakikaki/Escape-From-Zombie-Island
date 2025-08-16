using UnityEngine;

/// <summary>
/// Contains the specific behavior for the Bear Trap.
/// </summary>
public class BearTrapController : MonoBehaviour
{
    private const float PLACEMENT_Y_OFFSET = 0.51f;
    private Tile myTile;

    [Header("Trap Stats")]
    public int damage = 1;

    private bool isArmed = true;

    void Start()
    {
        // Adjust position to the correct offset
        Vector3 currentPosition = transform.position;
        currentPosition.y = PLACEMENT_Y_OFFSET;
        transform.position = currentPosition;

        // Register this trap with the GridManager
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            myTile = gridManager.GetTileAtWorldPosition(transform.position);
            gridManager.AddTrapToTile(myTile);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isArmed && other.GetComponent<ZombieController>() != null)
        {
            Debug.Log("BEAR TRAP TRIGGERED!");

            ZombieController zombie = other.GetComponent<ZombieController>();
            zombie.TakeDamage(damage);

            isArmed = false;
            Destroy(gameObject, 0.5f);
        }
    }

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