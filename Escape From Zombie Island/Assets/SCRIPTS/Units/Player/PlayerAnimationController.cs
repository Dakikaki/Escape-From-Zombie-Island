using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // Updated enum for more generic attack types
    public enum Pose { Idle, Walk, MeleeAttack, RangedAttack }

    [Header("Pose Prefabs")]
    public GameObject idlePosePrefab;
    public GameObject walkPosePrefab;
    public GameObject meleeAttackPosePrefab;
    public GameObject rangedAttackPosePrefab;

    private GameObject currentPoseInstance;

    void Start()
    {
        if (idlePosePrefab == null)
        {
            Debug.LogError("Idle Pose Prefab is not assigned!", this.gameObject);
            return;
        }
        SwitchPose(Pose.Idle);
    }

    public void SwitchPose(Pose newPose)
    {
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