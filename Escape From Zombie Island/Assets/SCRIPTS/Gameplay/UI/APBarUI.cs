using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages the visual representation of the player's Action Points.
/// </summary>
public class APBarUI : MonoBehaviour
{
    public GameObject apBlockPrefab;
    public Transform apBarContainer;
    public Color activeColor = new Color(1f, 0.8f, 0.2f); // Amber
    public Color spentColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Dark grey

    private List<Image> apBlocks = new List<Image>();

    void OnEnable()
    {
        PlayerController.OnAPChanged += UpdateAPBar;
    }

    void OnDisable()
    {
        PlayerController.OnAPChanged -= UpdateAPBar;
    }

    private void UpdateAPBar(int currentAP, int maxAP)
    {
        // Create or destroy blocks if the max AP has changed
        if (apBlocks.Count != maxAP)
        {
            // Clear existing blocks
            foreach (Transform child in apBarContainer)
            {
                Destroy(child.gameObject);
            }
            apBlocks.Clear();

            // Create new blocks
            for (int i = 0; i < maxAP; i++)
            {
                GameObject block = Instantiate(apBlockPrefab, apBarContainer);
                apBlocks.Add(block.GetComponent<Image>());
            }
        }

        // Update colors based on current AP
        for (int i = 0; i < apBlocks.Count; i++)
        {
            apBlocks[i].color = (i < currentAP) ? activeColor : spentColor;
        }
    }
}