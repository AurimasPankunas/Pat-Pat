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
            yield return new WaitForSeconds(Random.Range(10f, 20f));
            animator.SetTrigger("ChangeIdle");
            animator.SetInteger("IdleIndex", Random.Range(0, 3));
        }
    }
}
