using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the visual representation of the player's Action Points (AP).
/// It listens for AP changes and updates the UI accordingly.
/// </summary>
public class APBarUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Assign the AP block Image components here in order.")]
    public Image[] apBlocks;

    [Header("Player Reference")]
    [Tooltip("Optional: Assign the Player here. If left empty, it will be found automatically.")]
    public PlayerController player;

    // Awake is called before any Start or OnEnable methods.
    // This ensures we have a reference to the player as early as possible.
    void Awake()
    {
        // If the player is not assigned in the Inspector, find them in the scene
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        if (player == null)
        {
            Debug.LogError("APBarUI could not find the PlayerController! The AP bar will not function.", this.gameObject);
        }
    }

    // OnEnable is called when the object becomes active.
    private void OnEnable()
    {
        // Subscribe to the central event from the base UnitController class.
        // This will catch any AP changes from any unit.
        UnitController.OnAPChanged += UpdateAPBar;

        // Perform an initial update when the UI becomes active to show the correct state.
        if (player != null)
        {
            UpdateAPBar(player.currentAP, player.maxAP);
        }
    }

    // OnDisable is called when the object becomes inactive.
    private void OnDisable()
    {
        // Unsubscribe from the event to prevent errors when this object is destroyed.
        UnitController.OnAPChanged -= UpdateAPBar;
    }

    /// <summary>
    /// Updates the visible AP blocks based on the player's current AP.
    /// </summary>
    /// <param name="currentAP">The current number of action points.</param>
    /// <param name="maxAP">The maximum number of action points.</param>
    private void UpdateAPBar(int currentAP, int maxAP)
    {
        // This loop updates the visible AP blocks.
        for (int i = 0; i < apBlocks.Length; i++)
        {
            // Safety check to make sure the array element is not empty.
            if (apBlocks[i] != null)
            {
                // Enable the image if its index is less than the current AP count, otherwise disable it.
                apBlocks[i].enabled = (i < currentAP);
            }
        }
    }
}