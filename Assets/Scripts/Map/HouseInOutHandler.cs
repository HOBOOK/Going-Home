using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInOutHandler : MonoBehaviour {

    public bool isInSiding = false;
    Vector3 ToPosition = Vector3.zero;
    float TempOrthSize;

    IEnumerator InSide(Collider2D collider2)
    {
        isInSiding = true;
        Common.GetCameraObject().GetComponent<CameraEffectHandler>().InOutCamera();
        status.isPlaying = true;
        TempOrthSize = Camera.main.orthographicSize;
        int cnt = 0;
        while (cnt < 2)
        {
            cnt++;
            yield return new WaitForSeconds(1.5f);
        }
        ToPosition = transform.GetChild(0).transform.position;
        status.isInBuilding = true;
        Common.GetCameraObject().GetComponent<CameraEffectHandler>().SetCameraSize(4);
        Debug.Log(collider2.transform.parent.name + " 이(가) " + name + " 에서 <" + ToPosition + "> 으로 워프됨.");
        collider2.transform.parent.position = ToPosition;
        Camera.main.transform.position = ToPosition;
        yield return new WaitForSeconds(1.0f);
        status.isPlaying = false;
        isInSiding = false;
        yield return null;
    }
    IEnumerator OutSide(Collider2D collider2)
    {
        isInSiding = true;
        Common.GetCameraObject().GetComponent<CameraEffectHandler>().InOutCamera();
        status.isPlaying = true;
        int cnt = 0;
        while (cnt < 2)
        {
            cnt++;
            yield return new WaitForSeconds(1.5f);
        }
        ToPosition = transform.position;
        status.isInBuilding = false;
        Common.GetCameraObject().GetComponent<CameraEffectHandler>().SetCameraSize(TempOrthSize);
        Debug.Log(collider2.transform.parent.name + " 이(가) " + name + " 에서 <" + ToPosition + "> 으로 워프됨.");
        collider2.transform.parent.position = ToPosition;
        Camera.main.transform.position = ToPosition;
        yield return new WaitForSeconds(1.0f);
        status.isPlaying = false;
        isInSiding = false;
        yield return null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (Input.GetAxisRaw("Vertical") > 0&& !isInSiding)
            {
                if (!status.isInBuilding)
                {
                    StartCoroutine(InSide(collision));
                }
                else
                {
                    StartCoroutine(OutSide(collision));
                }
            }
        }
    }
}
