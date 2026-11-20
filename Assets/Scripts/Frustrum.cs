using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;

public class Frustrum : MonoBehaviour
{
    [Range(60f, 120f)]
    [SerializeField] private float fovAngle;

    [SerializeField] private float distanceNear;
    [SerializeField] private float distanceFar;
    private float verticalAngle;
    float aspectRatio = 16f / 9f;

    private Vector3 vector;

    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    List<Vector3> vertexListNear = new List<Vector3>();
    List<Vector3> vertexListFar = new List<Vector3>();
    List<string> objsInsideFrustrum = new List<string>();

    private struct newFrustrum
    {
        //public Vector3 v1;
        //public Vector3 v2;
        //public Vector3 v3;
        //public Vector3 v4;

        public List<Vector3> vertexListNear;
        public List<Vector3> vertexListFar;

        public newFrustrum(/*Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,*/ List<Vector3> vertexListNear, List<Vector3> vertexListFar)
        {
            //this.v1 = v1;
            //this.v2 = v2;
            //this.v3 = v3;
            //this.v4 = v4;

            this.vertexListNear = vertexListNear;
            this.vertexListFar = vertexListFar;
        }
    }

    private newFrustrum finalFrustrum;

    private void Update()
    {
        CalculateVertex();
        GetObjsInFrustrum();

        DrawFrustrum();
        LogObjsInsideFrustrumNames();
    }

    private float CalculateAspectRatio(float fovAngle)
    {
        return 2f * Mathf.Atan(Mathf.Tan(fovAngle * 0.5f * Mathf.Deg2Rad) / aspectRatio) * Mathf.Rad2Deg;
    }

    private void CalculateVertex()
    {
        vertexListNear.Clear();
        vertexListFar.Clear();

        float horizontalAngle = fovAngle / 2f * Mathf.Deg2Rad;
        float verticalAngle = CalculateAspectRatio(fovAngle) / 2f * Mathf.Deg2Rad;

        float x = Mathf.Sin(horizontalAngle);
        float z = Mathf.Cos(horizontalAngle);

        float yTop = Mathf.Sin(verticalAngle);
        float horizontalScale = Mathf.Cos(verticalAngle);

        Vector3 origin = transform.position;

        Vector3 v1 = new Vector3(-x * horizontalScale, -yTop, z * horizontalScale).normalized;
        Vector3 v2 = new Vector3( x * horizontalScale, -yTop, z * horizontalScale).normalized;
        Vector3 v3 = new Vector3(-x * horizontalScale,  yTop, z * horizontalScale).normalized;
        Vector3 v4 = new Vector3( x * horizontalScale,  yTop, z * horizontalScale).normalized;
        
        v1 = transform.TransformDirection(v1);
        v2 = transform.TransformDirection(v2);
        v3 = transform.TransformDirection(v3);
        v4 = transform.TransformDirection(v4);
        
        vertexListNear.Add(origin + v1 * distanceNear);
        vertexListNear.Add(origin + v2 * distanceNear);
        vertexListNear.Add(origin + v3 * distanceNear);
        vertexListNear.Add(origin + v4 * distanceNear);
        
        vertexListFar.Add(origin + v1 * distanceFar);
        vertexListFar.Add(origin + v2 * distanceFar);
        vertexListFar.Add(origin + v3 * distanceFar);
        vertexListFar.Add(origin + v4 * distanceFar);

        finalFrustrum = new newFrustrum(vertexListNear, vertexListFar);
    }

    private bool CheckIfObjInFrustrum(Vector3[] objVertexs)
    {
        for (int j = 0; j < objVertexs.Length; j++)
        {
            if (objVertexs[j].x >= vertexListFar[0].x && objVertexs[j].x <= vertexListFar[1].x)
            {
                if (objVertexs[j].y >= vertexListFar[0].y && objVertexs[j].y <= vertexListFar[2].y)
                {
                    if (objVertexs[j].z >= vertexListNear[0].z && objVertexs[j].z <= vertexListFar[0].z)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool CheckIfNameOnList(string objName)
    {
        for (int i = 0; i < objsInsideFrustrum.Capacity; i++)
        {
            if (objsInsideFrustrum[i] == objName)
                return true;
        }

        return false;
    }

    private void GetObjsInFrustrum()
    {
        foreach (GameObject obj in objects)
        {
            Mesh objMesh = null;
            Vector3[] objVertexs = null;

            if (obj != null)    
            {
                MeshFilter objMeshFilter = obj.GetComponent<MeshFilter>();

                objMesh = objMeshFilter.sharedMesh;
            }

            if (objMesh != null)
                objVertexs = objMesh.vertices;

            if (CheckIfObjInFrustrum(objVertexs))
            {
                if (objsInsideFrustrum.Capacity <= 0)
                    objsInsideFrustrum.Add(obj.name);
                else
                {
                    if (!CheckIfNameOnList(obj.name))
                        objsInsideFrustrum.Add(obj.name);
                }
            }
        }
    }
    
    private void DrawFrustrum()
    {
        //Debug.DrawLine(vertexListNear[0], vertexListNear[1], Color.red);
        //Debug.DrawLine(vertexListNear[0], vertexListNear[2], Color.red);
        //Debug.DrawLine(vertexListNear[1], vertexListNear[3], Color.red);
        //Debug.DrawLine(vertexListNear[2], vertexListNear[3], Color.red);
        
        //Debug.DrawLine(vertexListNear[0], vertexListNear[4], Color.blue);
        //Debug.DrawLine(vertexListNear[2], vertexListNear[6], Color.blue);
        //Debug.DrawLine(vertexListNear[4], vertexListNear[6], Color.blue);
        
        //Debug.DrawLine(vertexListNear[1], vertexListNear[5], Color.green);
        //Debug.DrawLine(vertexListNear[3], vertexListNear[7], Color.green);
        //Debug.DrawLine(vertexListNear[5], vertexListNear[7], Color.green);
        
        //Debug.DrawLine(vertexListNear[4], vertexListNear[5], Color.yellow);
        //Debug.DrawLine(vertexListNear[6], vertexListNear[7], Color.yellow);

        // ----------------------------------------------------------------

        /* 0 (Abajo izquierda), 1 (Abajo derecha), 2 (Arriba izquierda), 3 (Arriba derecha) */

        Debug.DrawLine(vertexListNear[0], vertexListNear[1], Color.red);
        Debug.DrawLine(vertexListNear[0], vertexListNear[2], Color.red);
        Debug.DrawLine(vertexListNear[2], vertexListNear[3], Color.red);
        Debug.DrawLine(vertexListNear[3], vertexListNear[1], Color.red);

        //Debug.DrawLine(vertexListFar[0], vertexListFar[1], Color.blue);
        //Debug.DrawLine(vertexListFar[1], vertexListFar[2], Color.blue);
        //Debug.DrawLine(vertexListFar[2], vertexListFar[3], Color.blue);
        //Debug.DrawLine(vertexListFar[3], vertexListFar[0], Color.blue);

        Debug.DrawLine(vertexListFar[0], vertexListFar[1], Color.blue);
        Debug.DrawLine(vertexListFar[0], vertexListFar[2], Color.blue);
        Debug.DrawLine(vertexListFar[2], vertexListFar[3], Color.blue);
        Debug.DrawLine(vertexListFar[3], vertexListFar[1], Color.blue);

        Debug.DrawLine(vertexListNear[0], vertexListFar[0], Color.green);
        Debug.DrawLine(vertexListNear[1], vertexListFar[1], Color.green);
        Debug.DrawLine(vertexListNear[2], vertexListFar[2], Color.green);
        Debug.DrawLine(vertexListNear[3], vertexListFar[3], Color.green);

        //Debug.DrawLine(vertexListNear[1], vertexListNear[5], Color.green);
        //Debug.DrawLine(vertexListNear[3], vertexListNear[7], Color.green);
        //Debug.DrawLine(vertexListNear[5], vertexListNear[7], Color.green);

        //Debug.DrawLine(vertexListNear[4], vertexListNear[5], Color.yellow);
        //Debug.DrawLine(vertexListNear[6], vertexListNear[7], Color.yellow);
    }

    private void LogObjsInsideFrustrumNames()
    {
        if (objsInsideFrustrum.Capacity != 0)
            for (int i = 0; i < objsInsideFrustrum.Capacity; i++)
                Debug.Log($"Objetos dentro del frustrum: {objsInsideFrustrum}");
    }
}
