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
        transform.localScale = new Vector3(0.7f, 0.25f, originalScale.z);
        yield return new WaitForSeconds(0.3f);

        transform.localScale = new Vector3(0.6f, 0.35f, originalScale.z);
        yield return new WaitForSeconds(0.2f);

        transform.localScale = new Vector3(0.4f, 0.6f, originalScale.z);
        yield return new WaitForSeconds(0.2f);

        transform.localScale = new Vector3(0.5f, 0.5f, originalScale.z);
        yield return new WaitForSeconds(0.2f);

        transform.localScale = originalScale;
        yield return new WaitForSeconds(cooldownTime);
        isBeingPatted = false;
    }
}
