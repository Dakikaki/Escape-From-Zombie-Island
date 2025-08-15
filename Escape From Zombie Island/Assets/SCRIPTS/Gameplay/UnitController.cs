using UnityEngine;

/// <summary>
/// The base class for any unit in the game (Player, Zombies, etc.).
/// </summary>
public class UnitController : MonoBehaviour
{
    [Header("Unit Stats")]
    public int maxHP = 3;
    public int currentHP;
    public int maxAP = 2;
    public int currentAP;

    [Header("Unit State")]
    public Tile currentTile;

    [Header("Dependencies")]
    public GridManager gridManager;

    // Event for UI to subscribe to. Sends (currentAP, maxAP).
    public event System.Action<int, int> OnAPChanged;

    protected virtual void Start()
    {
        currentHP = maxHP;
        currentAP = maxAP;

        // Find GridManager if not assigned
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
        }

        // Find the tile we are starting on
        if (gridManager != null)
        {
            currentTile = gridManager.GetTileAtWorldPosition(transform.position);
        }
    }

    /// <summary>
    /// Reduces the unit's AP and notifies any listeners (like the UI).
    /// </summary>
    public void SpendAP(int amount)
    {
        currentAP -= amount;
        if (currentAP < 0)
        {
            currentAP = 0;
        }
        // Fire the event to update the UI
        OnAPChanged?.Invoke(currentAP, maxAP);
    }

    /// <summary>
    /// Resets the unit's AP to its maximum value.
    /// </summary>
    public void ResetAP()
    {
        currentAP = maxAP;
        // Fire the event to update the UI
        OnAPChanged?.Invoke(currentAP, maxAP);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }
}