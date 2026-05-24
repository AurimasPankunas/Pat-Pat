using System.Collections;
using UnityEngine;

public class AnimatedChest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform textSpawnPoint;
    [SerializeField] private Collider chestCollider;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float spawnForce;
    private AnimalManager animalManager;
    private EffectPlayer effectPlayer;
    private SoundManager soundManager;
    private string[] rarityColors = {"#000000", "#A15505", "#727D8E", "#BF9304", "#A42DCB" };
    private string[] rarities = {"Common", "Uncommon", "Rare", "Epic", "Legendary" };
    
    
    void Start()
    {
        animalManager = GameManager.Instance.animalManager;
        effectPlayer = GameManager.Instance.effectPlayer;
        soundManager = GameManager.Instance.soundManager;
    }

    private IEnumerator OpenClose(AnimalData animalData)
    {
        animator.SetTrigger("Open");
        soundManager.PlaySoundAtPosition(startSound, transform.position);

        yield return new WaitForSeconds(spawnDelay);

        MiniAnimal spawnedAnimal = animalManager.CreateMiniAnimal(animalData, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = spawnedAnimal.GetComponent<Rigidbody>();
        
        // effects
        ColorUtility.TryParseHtmlString(rarityColors[animalData.rarity - 1], out Color color);
        EffectOptions options = new EffectOptions { color = color };
        effectPlayer.PlayExisting(particles, options);
        soundManager.PlaySoundAtPosition(openSound, transform.position);
        
        if (rb != null)
            rb.AddForce(Vector3.up * spawnForce);

        animator.SetTrigger("Close");

        yield return new WaitForSeconds(0.5f);
        
        string size =  "Size: " + animalManager.GetType(animalData.typeID).size.ToString();
        string text = rarities[animalData.rarity - 1] + " " + animalData.animalName + "\n" + size;
        effectPlayer.SpawnText(textSpawnPoint, text, color, 3f);
    }

    public void OpenChest(AnimalData animalData)
    {
        StartCoroutine(OpenClose(animalData));
    }

    public void CloseChest()
    {
        animator.SetTrigger("Close");
    }
}
