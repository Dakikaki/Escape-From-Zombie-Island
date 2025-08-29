// Escape From Zombie Island
// PlayerAnimationController.cs
// by Dakikaki
// 2024

using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // MODIFIED: Changed from a single GameObject to a List to manage all player poses.
    public List<GameObject> Poses;

    // NEW: Added an enum to make pose names more readable and manageable.
    public enum Pose
    {
        Idle,
        Knife,
        Pistol,
        Hatchet,
        BearTrap,
        NoiseMaker
    }

    private void Start()
    {
        // MODIFIED: Set the default pose to Idle on start.
        ChangePose(Pose.Idle);
    }

    // MODIFIED: This method now deactivates all poses and then activates the one specified by the enum.
    public void ChangePose(Pose newPose)
    {
        // Deactivate all poses first
        foreach (var pose in Poses)
        {
            if (pose != null)
            {
                pose.SetActive(false);
            }
        }

        // Activate the selected pose
        if ((int)newPose < Poses.Count && Poses[(int)newPose] != null)
        {
            Poses[(int)newPose].SetActive(true);
        }
    }
}