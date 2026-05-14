using System.Collections;
using UnityEngine;

public class AnimalIdleRandomizer : MonoBehaviour
{
    private Animator animator;

    IEnumerator Start()
    {
        animator = GetComponent<Animator>();

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            animator.SetInteger("IdleIndex", Random.Range(0, 4));
        }
    }
}
