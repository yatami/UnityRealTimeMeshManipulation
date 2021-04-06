using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDeformer : MonoBehaviour
{

    public DeformableMesh deformableMesh;
    public Transform initRaycastPoint;

    public float rayCastDensity = 0.005f;
    public float rayCastFieldRadius = 0.02f;
    //public float divisionNumber = 3;
    public  float deltaRate = 0.009f;

    List<Vector3> rayCastPos;
    Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        rayCastPos = new List<Vector3>();

        for (float x = initRaycastPoint.position.x; x < initRaycastPoint.position.x + rayCastFieldRadius; x += rayCastDensity)
        {
            for (float z = initRaycastPoint.position.z; z < initRaycastPoint.position.z + rayCastFieldRadius; z += rayCastDensity)
            {
                Vector3 newRayCastPos = new Vector3(x, initRaycastPoint.position.y, z);
                if (Vector3.Distance(initRaycastPoint.position, newRayCastPos) < rayCastFieldRadius)
                {
                    newRayCastPos = newRayCastPos - initRaycastPoint.position;
                    rayCastPos.Add(newRayCastPos);

                }
            }
        }

        for (float x = initRaycastPoint.position.x; x < initRaycastPoint.position.x + rayCastFieldRadius; x += rayCastDensity)
        {
            for (float z = initRaycastPoint.position.z - rayCastFieldRadius; z < initRaycastPoint.position.z; z += rayCastDensity)
            {
                Vector3 newRayCastPos = new Vector3(x, initRaycastPoint.position.y, z);
                if (Vector3.Distance(initRaycastPoint.position, newRayCastPos) < rayCastFieldRadius)
                {
                    newRayCastPos = newRayCastPos - initRaycastPoint.position;
                    rayCastPos.Add(newRayCastPos);

                }
            }
        }
        
        for (float x = initRaycastPoint.position.x - rayCastFieldRadius; x < initRaycastPoint.position.x ; x += rayCastDensity)
        {
            for (float z = initRaycastPoint.position.z - rayCastFieldRadius; z < initRaycastPoint.position.z ; z += rayCastDensity)
            {
                Vector3 newRayCastPos = new Vector3(x, initRaycastPoint.position.y, z);
                if (Vector3.Distance(initRaycastPoint.position, newRayCastPos) < rayCastFieldRadius)
                {
                    newRayCastPos = newRayCastPos - initRaycastPoint.position;
                    rayCastPos.Add(newRayCastPos);

                }
            }
        }
        for (float x = initRaycastPoint.position.x - rayCastFieldRadius; x < initRaycastPoint.position.x ; x += rayCastDensity)
        {
            for (float z = initRaycastPoint.position.z; z < initRaycastPoint.position.z + rayCastFieldRadius; z += rayCastDensity)
            {
                Vector3 newRayCastPos = new Vector3(x, initRaycastPoint.position.y, z);
                if (Vector3.Distance(initRaycastPoint.position, newRayCastPos) < rayCastFieldRadius)
                {
                    newRayCastPos = newRayCastPos - initRaycastPoint.position;
                    rayCastPos.Add(newRayCastPos);

                }
            }
        }
        /*
        for (float x = initRaycastPoint.position.x - rayCastFieldRadius; x < initRaycastPoint.position.x + rayCastFieldRadius; x += rayCastDensity)
        {
           
                Vector3 newRayCastPos = new Vector3(x, initRaycastPoint.position.y, initRaycastPoint.position.z);
                if (Vector3.Distance(initRaycastPoint.position, newRayCastPos) < rayCastFieldRadius)
                {
                    newRayCastPos = newRayCastPos - initRaycastPoint.position;
                    rayCastPos.Add(newRayCastPos);

                }
           
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(initRaycastPoint.transform.forward);

       // Debug.Log(Vector3.Distance(initRaycastPoint.position, prevPos));

        Vector3 diff = initRaycastPoint.position -  prevPos;
        prevPos = initRaycastPoint.position;

        /*for(int i =1; i < divisionNumber; i++)
        {

            foreach (Vector3 pos in rayCastPos)
            {
                RaycastHit hit;
                Ray ray = new Ray(initRaycastPoint.position + pos - (diff / divisionNumber * i), Vector3.down);
                //Debug.DrawRay(initRaycastPoint.position + pos, Vector3.down, Color.red, 1f);
                if (Physics.Raycast(ray, out hit, 0.3f))
                {
                    float deformRate = 1 - (Vector3.Distance(initRaycastPoint.position, initRaycastPoint.position + pos) / rayCastFieldRadius);
                    deformableMesh.AddDepression(hit.triangleIndex, deformRate);
                }
            }
        }*/

        Vector3 diffNorm = diff.normalized;
        int index = 0;
        

        while (diff.magnitude > (diffNorm * deltaRate * index).magnitude)
        {
            foreach (Vector3 pos in rayCastPos)
            {
                RaycastHit hit;
                Ray ray = new Ray(initRaycastPoint.position + pos - diff + diffNorm * deltaRate * index, Vector3.down);
                //Debug.DrawRay(initRaycastPoint.position + pos, Vector3.down, Color.red, 1f);
                if (Physics.Raycast(ray, out hit, 0.3f))
                {
                    float deformRate = 1 - (Vector3.Distance(initRaycastPoint.position, initRaycastPoint.position + pos) / rayCastFieldRadius);
                    deformableMesh.AddDepression(hit.triangleIndex, deformRate);
                }
            }

            index++;
        }

    }

 
}
