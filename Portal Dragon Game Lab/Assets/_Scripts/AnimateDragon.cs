using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateDragon : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] nonAnimatedPosition;
    [SerializeField]
    private float movementSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        vertices = mesh.vertices;
        SaveOldPosition();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateDragonBody();
    }

    void SaveOldPosition()
    {
        nonAnimatedPosition = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            nonAnimatedPosition[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
        }
    }

    void AnimateDragonBody()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 newVector = vertices[i];
            newVector.z = Mathf.Sin(Time.time * movementSpeed + vertices[i].y) + nonAnimatedPosition[i].z;
            vertices[i] = newVector;
        }
        mesh.vertices = vertices;
    }
}
