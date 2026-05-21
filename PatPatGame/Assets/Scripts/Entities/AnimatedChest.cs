using System.Collections;
using UnityEngine;

public class AnimatedChest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Collider chestCollider;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float spawnForce;
    private AnimalManager animalManager;
    private EffectPlayer effectPlayer;
    
    
    void Start()
    {
        animalManager = GameManager.Instance.animalManager;
        effectPlayer = GameManager.Instance.effectPlayer;
    }

    private IEnumerator OpenClose(AnimalData animalData)
    {
        animator.SetTrigger("Open");

        yield return new WaitForSeconds(spawnDelay);

        MiniAnimal spawnedAnimal = animalManager.CreateMiniAnimal(animalData, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = spawnedAnimal.GetComponent<Rigidbody>();
        chestCollider.enabled = false;
        EffectOptions options = new EffectOptions { color = Color.yellow };
        effectPlayer.PlayExisting(particles, options);
        if (rb != null)
            rb.AddForce(Vector3.up * spawnForce);

        animator.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
        chestCollider.enabled = true;
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
