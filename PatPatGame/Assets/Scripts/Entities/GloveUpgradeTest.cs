using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class GloveUpgradeTest : MonoBehaviour
{
    [SerializeField] private PlayerBalance playerBalance;
    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            playerBalance.IncrementGloveRarity();
    }
}
