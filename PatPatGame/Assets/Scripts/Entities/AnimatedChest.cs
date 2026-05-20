using System.Collections;
using UnityEngine;

public class AnimatedChest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    
    void Start()
    {
        InvokeRepeating(nameof(StartTestOpenClose), 1, 10);
    }

    private void StartTestOpenClose()
    {
        StartCoroutine(TestOpenClose());
    }

    private IEnumerator TestOpenClose()
    {
        Debug.Log("TestOpenClose called");
        OpenChest();
        yield return new WaitForSeconds(5);
        CloseChest();
    }

    public void OpenChest()
    {
        animator.SetTrigger("Open");
    }

    public void CloseChest()
    {
        animator.SetTrigger("Close");
    }
}
