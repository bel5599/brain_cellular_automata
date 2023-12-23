using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderController : MonoBehaviour
{

    public GameObject Cylinder;
    public GameObject point1;
    public GameObject point2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ApplyPoints();
    }

    private Quaternion offset = Quaternion.Euler(-90, 0, 0);

    public void ApplyPoints()
    {
        Transform cylinder = Cylinder.transform;
        Vector3 p1 = point1.transform.position;
        Vector3 p2 = point2.transform.position;

        var delta = p2 - p1;

        // First apply the world space rotation so the forward vector points p1 to p2
        cylinder.rotation = Quaternion.LookRotation(delta);
        // Then add the local offset around the local X axis
        cylinder.localRotation *= offset;
        // Or also
        //cylinder.Rotate(-90, 0, 0);

        // Set the position to the center between p1 and p2
        cylinder.position = (p1 + p2) / 2f;

        // Set the Y scale to the half of the distance between p1 and p2    
        var scale = cylinder.localScale;
        scale.y = delta.magnitude / 2f;
        cylinder.localScale = scale;
    }
}
