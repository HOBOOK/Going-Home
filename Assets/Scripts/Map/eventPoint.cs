using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventPoint : MonoBehaviour
{

    public Common.EventType eventType;
    public Common.PlayingEventType playEventType;
    public int eventNumber = 0;
    bool isEventEnd = false;
    bool isEnd = false;
    public float eventTime = 5;
    public float cameraSize;
    float tempCameraSize;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&!isEventEnd&&eventNumber>0)
        {
            Debug.Log(eventNumber + " 이벤트 시작 >> ");
            StartEvent();
        }
    }

    private void Update()
    {
        EndingEvent();
    }
    public void StartEvent()
    {
        if (!isEventEnd)
        {
            Common.isBlackUpDown = true;
            switch (eventType)
            {
                case Common.EventType.CameraShake:
                    Common.isShake = true;
                    break;

                case Common.EventType.CameraSlow:
                    break;

                case Common.EventType.CameraRestric:
                    Common.isCameraOut = true;
                    break;
                case Common.EventType.CameraSizing:
                    tempCameraSize = Common.GetCameraObject().GetComponent<Camera>().orthographicSize;
                    Common.GetCameraObject().GetComponent<CameraEffectHandler>().SetCameraSize(cameraSize);
                    break;
                case Common.EventType.DisableObject:
                    GameObject.Find("sword").GetComponent<DisableObject>().SetDisable(null);
                    break;
                case Common.EventType.StageClear:
                    status.stageNumber += 1;
                    status.stageDetailNumber = 0;
                    TimeLineEventManager.instance.StartStageTimeLine();
                    break;
            }
            PlayingEvent();
            isEventEnd = true;
        }
    }

    public void PlayingEvent()
    {
        string e = "";
        switch (playEventType)
        {
            case Common.PlayingEventType.None:
                break;
            case Common.PlayingEventType.LookUp:
                e = "LookingUp";
                break;
            case Common.PlayingEventType.LookLetter:
                e = "LookingLetter";
                break;
            case Common.PlayingEventType.Notebook:
                e = "PlayingNotebook";
                break;
        }
        if (e != "")
            Common.HERO().GetComponent<move>().PlayingEventScene(e);
    }

    public void EndingEvent()
    {
        if(isEventEnd&&!isEnd)
        {
            eventTime -= Time.deltaTime;
        }

        if(eventTime<=0)
        {
            Common.CameraAllEffectOff();
            Common.GetCameraObject().GetComponent<CameraEffectHandler>().SetCameraSize(tempCameraSize);
            isEnd = true;
            eventTime = 0;
            this.gameObject.SetActive(false);
        }
    }
}
