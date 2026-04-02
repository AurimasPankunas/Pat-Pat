using UnityEngine;

[System.Serializable]
public class MiniAnimal : MonoBehaviour
{
    public AnimalData data;

    public MiniAnimalData Save()
    {
        Transform transform = GetComponent<Transform>();
        return new MiniAnimalData(data, transform.position, transform.rotation);
    }

    public void Load(MiniAnimalData data)
    {
        
    }
}

