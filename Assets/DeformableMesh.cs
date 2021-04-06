using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GeneratePlaneMesh))]
public class DeformableMesh : MonoBehaviour
{

    public float maximumDepression;
    public List<Vector3> originalVertices;
    public List<Vector3> modifiedVertices;
    public List<int> originalTriangles;
    public static int updateThreshHold = 1000;
    public bool shouldRecalcucateNormals;

    private GeneratePlaneMesh plane;
    private MeshCollider meshColRef;
    private static int updateCount = 0;

    private bool startedDeform;
    public void MeshRegenerated()
    {
        meshColRef = gameObject.GetComponent<MeshCollider>();
        plane = GetComponent<GeneratePlaneMesh>();
        plane.mesh.MarkDynamic();
        originalVertices = plane.mesh.vertices.ToList();
        modifiedVertices = plane.mesh.vertices.ToList();
        originalTriangles = plane.mesh.triangles.ToList();

        meshColRef.sharedMesh = plane.mesh;
        updateCount = updateThreshHold;

    }

    IEnumerator doafterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        meshColRef.sharedMesh = plane.mesh;
        yield break;
    }

    public void AddDepression(Vector3 depressionPoint, float radius)
    {
        var worldPos4 = this.transform.worldToLocalMatrix * depressionPoint;
        var worldPos = new Vector3(worldPos4.x, worldPos4.y, worldPos4.z);
        for (int i = 0; i < modifiedVertices.Count; ++i)
        {
            var distance = (worldPos - (modifiedVertices[i] )).magnitude;
            if (distance < radius)
            {
                float smoothParam = 1 - (distance / radius);
                var newVert = originalVertices[i] + Vector3.down * maximumDepression * smoothParam;
                if(modifiedVertices[i].y > newVert.y)
                {
                    modifiedVertices.RemoveAt(i);
                    modifiedVertices.Insert(i, newVert);
                }
            }
        }

        plane.mesh.SetVertices(modifiedVertices);
        meshColRef.sharedMesh = plane.mesh;
    }

    public void AddDepression(int triangleIndex, float deformRate)
    {
        /*var worldPos4 = this.transform.worldToLocalMatrix * depressionPoint;
        var worldPos = new Vector3(worldPos4.x, worldPos4.y, worldPos4.z);
        for (int i = 0; i < modifiedVertices.Count; ++i)
        {
            var distance = (worldPos - (modifiedVertices[i])).magnitude;
            if (distance < radius)
            {
                float smoothParam = 1 - (distance / radius);
                var newVert = originalVertices[i] + Vector3.down * maximumDepression * smoothParam;
                if (modifiedVertices[i].y > newVert.y)
                {
                    modifiedVertices.RemoveAt(i);
                    modifiedVertices.Insert(i, newVert);
                }
            }
        }*/
       


        int startIndex = originalTriangles[triangleIndex * 3];

        for(int i = startIndex; i < startIndex +3; i++)
        {
            var newVert = originalVertices[i] + Vector3.down * maximumDepression * deformRate;
            if(newVert.y < modifiedVertices[i].y)
            {
                modifiedVertices.RemoveAt(i);
                modifiedVertices.Insert(i, newVert);
            }
        }
    


        updateCount++;

        plane.mesh.SetVertices(modifiedVertices);

        if(shouldRecalcucateNormals)
        {
            plane.mesh.RecalculateNormals();

        }

        /*if (updateCount >= updateThreshHold)
        {
            //meshColRef.sharedMesh = plane.mesh;
            updateCount = 0;
        }*/

        if (!startedDeform)
        {
            meshColRef.sharedMesh = plane.mesh;
            StartCoroutine(doafterDelay());
            startedDeform = true;
        }

    }
}