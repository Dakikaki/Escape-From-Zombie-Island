using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // Defines all the possible poses the character can be in.
    public enum Pose { Idle, Walk, MeleeAttack, RangedAttack }

    [Header("Pose Prefabs")]
    public GameObject idlePosePrefab;
    public GameObject walkPosePrefab;
    public GameObject meleeAttackPosePrefab; // For the knife
    public GameObject rangedAttackPosePrefab; // For the pistol/hatchet

    // This holds a reference to the currently active pose so we can destroy it.
    private GameObject currentPoseInstance;

    void Start()
    {
        // This is a safety check. If the idle prefab isn't assigned, nothing will show up.
        if (idlePosePrefab == null)
        {
            Debug.LogError("Idle Pose Prefab is not assigned in the Inspector on the PlayerAnimationController!", this.gameObject);
            return;
        }
        // Start the game with the character in the Idle pose.
        SwitchPose(Pose.Idle);
    }

    /// <summary>
    /// Destroys the old pose prefab and instantiates the new one.
    /// </summary>
    public void SwitchPose(Pose newPose)
    {
        // Destroy the old pose prefab if one exists.
        if (currentPoseInstance != null)
        {
            Destroy(currentPoseInstance);
        }

        // Figure out which new prefab to create.
        GameObject prefabToInstantiate = GetPrefabForPose(newPose);

        // Create the new prefab as a child of this object.
        if (prefabToInstantiate != null)
        {
            currentPoseInstance = Instantiate(prefabToInstantiate, transform);
        }
    }

    /// <summary>
    /// A helper function to return the correct prefab for a given pose.
    /// </summary>
    private GameObject GetPrefabForPose(Pose pose)
    {
        switch (pose)
        {
            case Pose.Idle:
                return idlePosePrefab;
            case Pose.Walk:
                return walkPosePrefab;
            case Pose.MeleeAttack:
                return meleeAttackPosePrefab;
            case Pose.RangedAttack:
                return rangedAttackPosePrefab;
            default:
                return null;
        }
    }
}