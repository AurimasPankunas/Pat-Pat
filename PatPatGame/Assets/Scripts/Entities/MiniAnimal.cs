using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MiniAnimal : MonoBehaviour
{
    public AnimalData data;
    private SoundManager soundManager;
    [SerializeField] private AudioClip spawnSound;

    public void PlaySpawnSound()
    {
        if (soundManager == null)
            soundManager = GameManager.Instance.soundManager;
        if (spawnSound != null)
            soundManager.PlaySoundAtPosition(spawnSound, transform.position);
    }

    // This class is now empty but it can have its own unique behaviour, like animations, particles, etc.
}

