using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float HorizontalMoveSpeed = 5f, VerticalMoveSpeed = 5f, ZoomSpeed = 5f;

    const float ZoomInLimit = 5f, ZoomOutLimit = 15f;
    Camera GameCamera;

    private void Awake()
    {
        GameCamera = GetComponent<Camera>();
    }

    void Update()
    {
        this.transform.position += new Vector3(GetHorizontalMovement() * HorizontalMoveSpeed * Time.deltaTime, GetVerticalMovement() * VerticalMoveSpeed * Time.deltaTime);

        GameCamera.orthographicSize += GetScrollAdjustment();

        GameCamera.orthographicSize = Mathf.Clamp(GameCamera.orthographicSize, ZoomInLimit, ZoomOutLimit);
    }

    float GetScrollAdjustment()
    {
        return Input.mouseScrollDelta.y * -ZoomSpeed * Time.deltaTime;
    }

    float GetVerticalMovement()
    {
        float retVal = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            retVal += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            retVal -= 1f;
        }

        return retVal;
    }

    float GetHorizontalMovement()
    {
        float retVal = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            retVal += 1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            retVal -= 1f;
        }

        return retVal;
    }
}
