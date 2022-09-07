using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform minPos, playerPos;
    [SerializeField]
    private float boundsY;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float cameraSpeed;
    public void Update()
    {
        var curTarPos = playerPos.position + offset;
        transform.position = Vector3.Lerp(transform.position, new Vector3(0f, curTarPos.y,-10f), cameraSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y,minPos.position.y - boundsY/2f, minPos.position.y), -10f);
    }

    public void ChangeY(float yPos)
    {
        minPos.transform.position = new Vector3(0f, minPos.transform.position.y + yPos, -10f);
    }
}
