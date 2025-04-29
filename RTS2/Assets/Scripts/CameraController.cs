using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<CameraController>(true);
            }
            return instance;
        }
    }
    public float HorizontalMoveSpeed = 5f, VerticalMoveSpeed = 5f, ZoomSpeed = 5f,AutoMoveSpeed=20f;

    const float ZoomInLimit = 5f, ZoomOutLimit = 15f;
    Camera GameCamera;

    private void Awake()
    {
        GameCamera = GetComponent<Camera>();
    }

    void Update()
    {

        if (isAutoMoving)
        {
            Vector3 dir = (autoMoveTarget - this.transform.position).normalized * AutoMoveSpeed * DeltaTimeWrapper.CameraDelta;
            dir.z = 0;

            if (Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(autoMoveTarget.x, autoMoveTarget.y)) < 1f)
            {
                isAutoMoving = false;
            }

           this.transform.position+=dir;
        }
        else
        {
            this.transform.position += new Vector3(GetHorizontalMovement() * HorizontalMoveSpeed * DeltaTimeWrapper.CameraDelta, GetVerticalMovement() * VerticalMoveSpeed * DeltaTimeWrapper.CameraDelta);
        }

        GameCamera.orthographicSize += GetScrollAdjustment();

        GameCamera.orthographicSize = Mathf.Clamp(GameCamera.orthographicSize, ZoomInLimit, ZoomOutLimit);

        
    }

    float GetScrollAdjustment()
    {
        return Input.mouseScrollDelta.y * -ZoomSpeed * DeltaTimeWrapper.CameraDelta;
    }

    bool isAutoMoving = false;
    public Vector3 autoMoveTarget= Vector3.zero;
    public void SetToAutoMove(Vector3 target)
    {
        autoMoveTarget = target;
        isAutoMoving = true;
    }

    float GetVerticalMovement()
    {
        float retVal = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            retVal += 1f;
            isAutoMoving = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            retVal -= 1f;
            isAutoMoving = false;

        }

        

        return retVal;
    }

    float GetHorizontalMovement()
    {
        float retVal = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            retVal += 1f;
            isAutoMoving = false;

        }

        if (Input.GetKey(KeyCode.A))
        {
            retVal -= 1f;
            isAutoMoving = false;

        }
       
        return retVal;
    }
}
