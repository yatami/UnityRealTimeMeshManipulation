using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsDeformer : MonoBehaviour
{

    public float collisionRadius = 0.1f;
    public DeformableMesh deformableMesh;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionStay(Collision collision)
    {
        
        foreach (var contact in collision.contacts)
        {
            deformableMesh.AddDepression(contact.point, collisionRadius);
        }
    }
}