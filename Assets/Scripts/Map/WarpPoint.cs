using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    bool isInSiding = false;
    float TempOrthSize;
    public Vector3 ToPosition = Vector3.zero;
    public GameObject ToObject;
    public bool isRestricCam;
    public float TargetOrthSize = 5;
    private void Start()
    {
        if (ToObject != null)
            ToPosition = ToObject.transform.position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Input.GetAxisRaw("Vertical") > 0 && !isInSiding)
            {
                if (!status.isInBuilding)
                {
                    Common.HERO().GetComponent<move>().SetFalseAllboolean();
                    StartCoroutine(InSide(collision));
                }
                else
                {
                    Common.HERO().GetComponent<move>().SetFalseAllboolean();
                    StartCoroutine(OutSide(collision));
                }
            }
        }
    }


    IEnumerator InSide(Collider2D collider2)
    {
        isInSiding = true;
        Common.GetCameraObject().GetComponent<CameraEffectHandler>().InOutCamera();
        status.isPlaying = true;
        TempOrthSize = Camera.main.orthographicSize;

        if(isRestricCam)
        {
            Camera.main.GetComponent<FollowCamera>().SetBoundX(ToObject.transform.position.x - 10, ToObject.transform.position.x + 10);
        }
        int cnt = 0;
        while (cnt < 2)
        {
            cnt++;
            yield return new WaitForSeconds(2f);
        }
        Common.GetCameraObject().GetComponent<CameraEffectHandler>().SetCameraSize(TargetOrthSize);
        status.isInBuilding = true;
        Debug.Log(collider2.transform.parent.name + " 이(가) " + name + " 에서 <" + ToPosition + "> 으로 워프됨.");
        collider2.transform.parent.position = ToPosition;
        Camera.main.transform.position = ToPosition;
        yield return new WaitForSeconds(0.5f);
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
}
