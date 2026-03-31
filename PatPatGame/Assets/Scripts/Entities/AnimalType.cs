using UnityEngine;

[CreateAssetMenu(menuName = "Animals/Animal Type Definition")]
public class AnimalType : ScriptableObject
{
    public string id;
    public AnimalSize size;
    public GameObject fullPrefab;
    public GameObject miniPrefab;
}

public enum AnimalSize
{
    Small, Medium, Large
}

public enum AnimalVersion
{
    Full, Mini
}
