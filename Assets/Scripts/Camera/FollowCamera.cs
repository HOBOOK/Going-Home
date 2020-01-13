using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class FollowCamera : MonoBehaviour
{
    
    public GameObject targetObj;
    Transform target;
    Vector3 cameraPos = Vector3.zero;
    Vector3 ToUpperCamera;
    Vector3 ToNormalCamera;
    public bool isBound = false;
    float defaultX = -100f;
    float minX, maxX;
    float cameraX = 0;
    float cameraY = 0;

    void Awake ()
    {
        target = targetObj.transform;
        ToUpperCamera = new Vector3(0, 0,  0);
        ToNormalCamera = new Vector3(0, 2.5f, 0);
    }

    public void ChangeTarget(GameObject t)
    {
        target = t.transform;
    }

	void LateUpdate ()
    {
        if (target != null&&!GetComponent<CinemachineBrain>().enabled)
        {
            cameraPos = Camera.main.WorldToViewportPoint(target.position);
            SetX();
            SetY();
            MoveCamera();
        }
    }
    private void MoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(cameraX, cameraY), 3f * Time.smoothDeltaTime);
        transform.Translate(0, 0, -5);
    }

    private void SetY()
    {
        if(Common.isCameraOut)
        {
            cameraY = transform.position.y;
        }
        else if(isBound)
        {
            cameraY = target.position.y + 2.5f;
        }
        else
        {
            cameraY = target.position.y + 2.5f;
        }
        if (cameraY < 2.5f)
        {
            cameraY = 2.5f;
        }
    }

    private void SetX()
    {
        if(Common.isCameraOut)
        {
            cameraX = status.isLeftorRight ? target.position.x - 0.5f : target.position.x + 0.5f;
        }
        else if(status.isPlaying)
        {
            cameraX = target.position.x;
        }
        else if(isBound)
        {
            if (minX > target.position.x)
            {
                cameraX = minX;
            }
            else if (maxX < target.position.x)
            {
                cameraX = maxX;
            }
            else if (target.position.x >= minX && target.position.x <= maxX)
            {
                cameraX = status.isLeftorRight ? target.position.x - 0.5f : target.position.x + 0.5f;
            }
        }
        else
        {
            cameraX = status.isLeftorRight ? target.position.x- 0.5f : target.position.x+ 0.5f;
        }
    }

    public void SetBoundX(float min, float max)
    {
        minX = min;
        maxX = max;
        isBound = true;
    }
}
