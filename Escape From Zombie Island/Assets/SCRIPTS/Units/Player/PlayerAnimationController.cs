using UnityEngine;

// This attribute allows the script to run in the editor, enabling the preview.
[ExecuteInEditMode]
public class PlayerAnimationController : MonoBehaviour
{
    // Updated enum for more generic attack types
    public enum Pose { Idle, Walk, MeleeAttack, RangedAttack }

    [Header("Pose Prefabs")]
    public GameObject idlePosePrefab;
    public GameObject walkPosePrefab;
    public GameObject meleeAttackPosePrefab;
    public GameObject rangedAttackPosePrefab;

    private GameObject currentPoseInstance; // Used for the pose during Play mode
    private GameObject editorPoseInstance;  // Used only for the editor preview
    private GameObject lastAssignedIdlePrefab; // Tracks inspector changes to update the preview

    void Start()
    {
        // This logic only runs in Play mode.
        if (Application.isPlaying)
        {
            // Clear the editor preview instance before starting the game.
            if (editorPoseInstance != null)
            {
                Destroy(editorPoseInstance);
            }

            if (idlePosePrefab == null)
            {
                Debug.LogError("Idle Pose Prefab is not assigned!", this.gameObject);
                return;
            }
            SwitchPose(Pose.Idle);
        }
    }

    void Update()
    {
        // This block runs only in the editor, not in play mode, to manage the preview safely.
        if (!Application.isPlaying && Application.isEditor)
        {
            UpdateEditorPose();
        }
    }

    // Clean up the preview when the component is disabled or the object is destroyed in the editor.
    void OnDisable()
    {
        if (!Application.isPlaying && Application.isEditor && editorPoseInstance != null)
        {
            DestroyImmediate(editorPoseInstance);
        }
    }

    /// <summary>
    /// Checks if the assigned idle prefab has changed and updates the editor preview accordingly.
    /// This is the safe alternative to using OnValidate for creating/destroying objects.
    /// </summary>
    private void UpdateEditorPose()
    {
        // If the prefab in the inspector has changed since our last update...
        if (lastAssignedIdlePrefab != idlePosePrefab)
        {
            // ...clear the existing preview...
            if (editorPoseInstance != null)
            {
                DestroyImmediate(editorPoseInstance);
            }

            // ...and if a new prefab is assigned, create a new preview.
            if (idlePosePrefab != null)
            {
                editorPoseInstance = Instantiate(idlePosePrefab, transform);
                // This is a CRUCIAL step. It prevents the preview object from being saved with the scene.
                editorPoseInstance.hideFlags = HideFlags.HideAndDontSave;
            }

            // Finally, update our tracking variable to match the inspector.
            lastAssignedIdlePrefab = idlePosePrefab;
        }
    }

    /// <summary>
    /// Switches the character's visual representation. This is the primary runtime logic.
    /// </summary>
    /// <param name="newPose">The new pose to display.</param>
    public void SwitchPose(Pose newPose)
    {
        if (!Application.isPlaying) return; // This should only run in play mode.

        if (currentPoseInstance != null)
        {
            Destroy(currentPoseInstance);
        }

        GameObject prefabToInstantiate = GetPrefabForPose(newPose);

        if (prefabToInstantiate != null)
        {
            currentPoseInstance = Instantiate(prefabToInstantiate, transform);
        }
    }

    /// <summary>
    /// Returns the prefab associated with a given pose.
    /// </summary>
    private GameObject GetPrefabForPose(Pose pose)
    {
        switch (pose)
        {
            case Pose.Idle: return idlePosePrefab;
            case Pose.Walk: return walkPosePrefab;
            case Pose.MeleeAttack: return meleeAttackPosePrefab;
            case Pose.RangedAttack: return rangedAttackPosePrefab;
            default: return null;
        }
    }
}