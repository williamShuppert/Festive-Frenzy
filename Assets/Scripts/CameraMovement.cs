using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] Vector2 minPos;
    [SerializeField] Vector2 maxPos;

    private void Update()
    {
        var newPos = target.position;

        newPos = new Vector2(
            Mathf.Clamp(newPos.x, minPos.x, maxPos.x),
            Mathf.Clamp(newPos.y, minPos.y, maxPos.y)
        );

        newPos.z = -10;

        transform.position = newPos;
    }
}
