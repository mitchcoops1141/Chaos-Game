using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float cameraFollowSpeed;

    public void SetCameraPosition(Vector3 playerPos)
    {
        Vector3 newPos;
        newPos.x = playerPos.x + cameraOffset.x;
        newPos.y = transform.position.y;
        newPos.z = playerPos.z + cameraOffset.z;

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * cameraFollowSpeed);
    }
}
