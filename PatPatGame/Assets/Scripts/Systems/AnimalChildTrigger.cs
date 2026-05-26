using UnityEngine;

public class AnimalChildTrigger : MonoBehaviour
{
    public Animal_Stat_Increase animal_Stat_Increase; 

    private void OnTriggerEnter(Collider other)
    {
        if (animal_Stat_Increase != null)
        {
            animal_Stat_Increase.OnChildTriggerEntered(other);
        }
    }
}
