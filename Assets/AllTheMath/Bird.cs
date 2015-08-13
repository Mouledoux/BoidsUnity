using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{
    [Header("Vectors")]
    public Vector3 pos;
    public Vector3 velo;
    public Vector4 color;

    [Header("Distance to Other Boid")]
    [Space(10)]
    public float dis; //minimum distance a boid can be nbext to one another

    [Header("Type of Bird")]
    public bool isPredator = false;

    void Update()
    {
        gameObject.transform.up = velo;
        gameObject.transform.position = pos;
    }
}
