using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;

public class AnimalVisualFeedback : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isBeingPatted = false;
    [SerializeField] private float cooldownTime = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
    }


    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Hand entered trigger");
        if(collider.CompareTag("Hand") && !isBeingPatted)
        {
            isBeingPatted = true;
            StartCoroutine(SquishEffect());
        }
    }

    private IEnumerator SquishEffect()
    {
        transform.localScale = new Vector3(originalScale.x * 1.7f, originalScale.y * 0.4f, originalScale.z);
        yield return new WaitForSeconds(0.15f);

        transform.localScale = new Vector3(originalScale.x * 1.6f, originalScale.y * 0.7f, originalScale.z);
        yield return new WaitForSeconds(0.075f);

        transform.localScale = new Vector3(originalScale.x * 0.6f, originalScale.y * 1.2f, originalScale.z);
        yield return new WaitForSeconds(0.09f);

        transform.localScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.1f, originalScale.z);
        yield return new WaitForSeconds(0.075f);

        transform.localScale = originalScale;
        yield return new WaitForSeconds(cooldownTime);
        isBeingPatted = false;
    }
}
