using UnityEngine;

/// <summary>
/// Contains the specific behavior for the Bear Trap.
/// </summary>
public class BearTrapController : MonoBehaviour
{
    [Header("Trap Stats")]
    public int damage = 1; // As per GDD, this will root and do minor damage

    private bool isArmed = true;

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
}