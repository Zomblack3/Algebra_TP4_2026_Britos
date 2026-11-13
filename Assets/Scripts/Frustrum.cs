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
    
    List<Vector3> vertexList = new List<Vector3>();
    
    void Start()
    {
        CalculateVertex();
    }

    private void Update()
    {
        CalculateVertex();
        DrawFrustrum();
    }

    private float CalculateAspectRatio(float fovAngle)
    {
        return 2f * Mathf.Atan(Mathf.Tan(fovAngle * 0.5f * Mathf.Deg2Rad) / aspectRatio) * Mathf.Rad2Deg;
    }

    private void CalculateVertex()
    {
        vertexList.Clear();
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
        
        vertexList.Add(origin + v1 * distanceNear);
        vertexList.Add(origin + v2 * distanceNear);
        vertexList.Add(origin + v3 * distanceNear);
        vertexList.Add(origin + v4 * distanceNear);
        
        vertexList.Add(origin + v1 * distanceFar);
        vertexList.Add(origin + v2 * distanceFar);
        vertexList.Add(origin + v3 * distanceFar);
        vertexList.Add(origin + v4 * distanceFar);
    
    }
    
    void DrawFrustrum()
    {
        Debug.DrawLine(vertexList[0], vertexList[1], Color.red);
        Debug.DrawLine(vertexList[0], vertexList[2], Color.red);
        Debug.DrawLine(vertexList[1], vertexList[3], Color.red);
        Debug.DrawLine(vertexList[2], vertexList[3], Color.red);
        
        Debug.DrawLine(vertexList[0], vertexList[4], Color.blue);
        Debug.DrawLine(vertexList[2], vertexList[6], Color.blue);
        Debug.DrawLine(vertexList[4], vertexList[6], Color.blue);
        
        Debug.DrawLine(vertexList[1], vertexList[5], Color.green);
        Debug.DrawLine(vertexList[3], vertexList[7], Color.green);
        Debug.DrawLine(vertexList[5], vertexList[7], Color.green);
        
        Debug.DrawLine(vertexList[4], vertexList[5], Color.yellow);
        Debug.DrawLine(vertexList[6], vertexList[7], Color.yellow);
    }
}
