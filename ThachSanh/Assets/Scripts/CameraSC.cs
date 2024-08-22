using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSC : MonoBehaviour
{
    public Transform target; // Đối tượng mà camera sẽ theo dõi
    public float smoothSpeed = 0.125f; // Độ mượt của chuyển động camera
    public Vector3 offset; // Khoảng cách giữa camera và đối tượng
    public float minDistance = 12.0f; // Khoảng cách tối thiểu giữa camera và đối tượng

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position - offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        if (Vector3.Distance(transform.position, target.position) < minDistance)
        {
            transform.position = target.position - transform.forward * minDistance;
        }

        transform.LookAt(target);
    }
}
