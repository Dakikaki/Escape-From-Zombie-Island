using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class APBarUI : MonoBehaviour
{
    public PlayerController player; // Reference to the player
    public Image[] apBlocks;

    void Start()
    {
        // If the player is not assigned in the Inspector, find them in the scene
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }
    }

    private void OnEnable()
    {
        if (player != null)
        {
            player.OnAPChanged += UpdateAPBar;
            // Initial update
            UpdateAPBar(player.currentAP, player.maxAP);
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnAPChanged -= UpdateAPBar;
        }
    }

    private void UpdateAPBar(int currentAP, int maxAP)
    {
        for (int i = 0; i < apBlocks.Length; i++)
        {
            // Activate the block if its index is less than the current AP
            apBlocks[i].enabled = (i < currentAP);
        }
    }
}