using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFall : MonoBehaviour
{

    public GameObject[] objectsToFall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("tirmik"))
        {
            Debug.Log("they trig");

            foreach (GameObject g in objectsToFall)
            {
                g.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
}
