using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tirmikController : MonoBehaviour
{

    public float rotLerpFactor = 5;
    public float posLerpFactor = 2;
    public float speed = 0.5f;

    private Vector3 prevPoint;
    private bool shouldDeform = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButton(0))
        {

            Vector3 diff = Input.mousePosition - prevPoint;
            prevPoint = Input.mousePosition;

            diff = new Vector3(diff.x, 0, diff.y);

            
            Quaternion rot = Quaternion.LookRotation(diff, Vector3.up);


            Vector3 sensitivity = new Vector3(0, 0, 0);


            Vector3 diffNorm = diff.normalized;

          
            if (Vector3.Distance(diff,sensitivity) > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotLerpFactor);
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + diffNorm.x * speed, transform.position.y, transform.position.z + diffNorm.z * speed), Time.deltaTime * posLerpFactor);
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            if(shouldDeform)
            {
                transform.DOMoveY(-7.5f, 1f);
            }
            else
            {
                transform.DOMoveY(-7f, 1f);

            }
            shouldDeform = !shouldDeform;
        }

  

    }
}
