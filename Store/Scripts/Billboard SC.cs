using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSC : MonoBehaviour
{
    public Camera cameraToLookAt;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        v.y = 90;
        transform.LookAt(cameraToLookAt.transform.position - v);
    }
}
