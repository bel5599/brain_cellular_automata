using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodVessel : MonoBehaviour
{
    public LineRenderer line;
    public Transform pos1;
    public Transform pos2;
    // Start is called before the first frame update
    public void Start()
    {
        line.positionCount = 2;
    }

    // Update is called once per frame
    public void Update()
    {
        line.SetPosition(0, pos1.position);
        line.SetPosition(1, pos2.position);
    }
}
