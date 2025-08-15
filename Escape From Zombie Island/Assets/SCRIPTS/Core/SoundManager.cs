using UnityEngine;

/// <summary>
/// A centralized manager for playing sound effects.
/// Implemented as a Singleton to be easily accessible.
/// </summary>
public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern: Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keeps it alive between scenes
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays a sound effect one time.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}