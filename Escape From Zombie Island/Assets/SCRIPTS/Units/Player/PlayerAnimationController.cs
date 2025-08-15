using UnityEngine;

<<<<<<< HEAD
<<<<<<< HEAD
/// <summary>
/// Manages the player's visual representation, switching between different pose prefabs.
/// </summary>
=======
>>>>>>> parent of fb0af9e (Fixed player idle pose in editor)
=======
>>>>>>> parent of fb0af9e (Fixed player idle pose in editor)
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Pose Prefabs")]
    public GameObject idlePose;
    public GameObject walkPose;
    [Space(10)] // Adds a little space in the inspector for organization
    public GameObject hatchetPose;
    public GameObject knifePose;
    public GameObject pistolPose;
    [Space(10)]
    public GameObject placeBearTrapPose;
    public GameObject placeNoiseMakerPose;

<<<<<<< HEAD
<<<<<<< HEAD

    private GameObject activePose;

    void Start()
    {
        // Start in the idle pose by default
        SwitchPose(idlePose);
    }

    /// <summary>
    /// Deactivates the current pose and activates the new one.
    /// </summary>
    /// <param name="newPosePrefab">The prefab of the pose to switch to.</param>
    public void SwitchPose(GameObject newPosePrefab)
    {
        if (newPosePrefab == null)
        {
            Debug.LogWarning("Tried to switch to a null pose prefab.");
            return;
=======
    private GameObject currentPoseInstance;

    void Start()
    {
=======
    private GameObject currentPoseInstance;

    void Start()
    {
>>>>>>> parent of fb0af9e (Fixed player idle pose in editor)
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
>>>>>>> parent of fb0af9e (Fixed player idle pose in editor)
        }

        // Deactivate the old pose if it exists
        if (activePose != null)
        {
            activePose.SetActive(false);
        }

<<<<<<< HEAD
<<<<<<< HEAD
        // Find or instantiate the new pose
        Transform poseTransform = transform.Find(newPosePrefab.name);
        if (poseTransform == null)
=======
=======
>>>>>>> parent of fb0af9e (Fixed player idle pose in editor)
    private GameObject GetPrefabForPose(Pose pose)
    {
        switch (pose)
>>>>>>> parent of fb0af9e (Fixed player idle pose in editor)
        {
            // Instantiate the new pose as a child of this controller
            activePose = Instantiate(newPosePrefab, transform);
            activePose.name = newPosePrefab.name; // Clean up the "(Clone)" from the name
        }
        else
        {
            // If the pose already exists (was instantiated before), just reactivate it
            activePose = poseTransform.gameObject;
            activePose.SetActive(true);
        }
    }
}