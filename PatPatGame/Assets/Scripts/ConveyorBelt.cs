using System;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public Transform spawnPosition;
    public List<GameObject> onBelt;
    private GameObject Item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onBelt = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

        for(int i = 0; i < onBelt.Count; i++)
        {
            onBelt[i].GetComponent<Rigidbody>().linearVelocity = speed * direction * Time.deltaTime;
        }
    }

    // When something collides with the belt
    void OnCollisionEnter(Collision collision)
    {
        onBelt.Add(collision.gameObject);
    }

    // When something leaves the belt unfreeze the rotations
    void OnCollisionExit(Collision collision)
    {
        onBelt.Remove(collision.gameObject);
        Rigidbody item = collision.gameObject.GetComponent<Rigidbody>();
        item.constraints = RigidbodyConstraints.None;

    }

    // Spawn an item on the belt and freeze its rotations
    public void SpawnItemOnBelt(GameObject gameObject)
    {
        Item = gameObject;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.transform.position = spawnPosition.position;
    }
}
