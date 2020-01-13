using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TriggerEvent
{
    public float startTimeAtTargetPositionX;
    [HideInInspector]
    public bool isTriggerEnd = false;
    public bool isEventClear = false;
    public bool isOnceTrigger = false;
    public Common.EventType eventType;
    public ClearEventCondition clearEventCondition;
    public Common.PlayingEventType playEventType;
    public string chatText;
    public bool isPlayingState = true;
    public GameObject TimelineDirector;
    public float eventTime;
    float initEventTime;
    public float cameraSize;
    [HideInInspector]
    public float tempCameraSize;

    public void PlayingEvent()
    {
        string e = "";
        switch(playEventType)
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
            case Common.PlayingEventType.Chat:
                Common.Chat(chatText, Common.HERO().transform);
                e = "Chat";
                break;
        }
        if(e!="")
            Common.HERO().GetComponent<move>().PlayingEventScene(e, isPlayingState);
    }

    public enum ClearEventCondition
    {
        None,
        NpcDie,
        TimeOut
    }

    public void CreateEvent()
    {
        if(!isTriggerEnd)
        {
            switch (eventType)
            {
                case Common.EventType.CameraShake:
                    SoundManager.instance.RandomizeSfx(AudioClipManager.instance.rumble);
                    Common.isShake = true;
                    break;

                case Common.EventType.CameraSlow:
                    break;

                case Common.EventType.CameraBlack:
                    Common.isBlackUpDown = true;
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
                case Common.EventType.TimeLineEvent:
                    Common.HERO().GetComponent<move>().SetFalseAllboolean();
                    Common.HERO().GetComponent<move>().moveAnimator.SetTrigger("idle");
                    Common.HERO().GetComponent<Animator>().applyRootMotion = true;
                    TimelineStart();
                    break;
                case Common.EventType.StageClear:
                    status.stageNumber += 1;
                    status.stageDetailNumber = 0;
                    TimeLineEventManager.instance.StartStageTimeLine();
                    break;
            }
            PlayingEvent();
            if (clearEventCondition == ClearEventCondition.TimeOut)
                initEventTime = eventTime;
            isTriggerEnd = true;
        }
    }
    public void TimelineStart()
    {
        if (TimelineDirector != null && TimelineDirector.GetComponent<PlayableDirector>() != null)
        {
            Common.isBlackUpDown = true;
            TimelineDirector.gameObject.SetActive(true);
            TimelineDirector.GetComponent<PlayableDirector>().Play();
        }
    }

    public void ClearEvent()
    {
        if(isTriggerEnd&&!isEventClear)
        {
            Common.HERO().GetComponent<move>().moveAnimator.SetBool("isPlayingEnd", true);

            switch (eventType)
            {
                case Common.EventType.CameraShake:
                    Common.isShake = false;
                    break;

                case Common.EventType.CameraSlow:
                    break;

                case Common.EventType.CameraBlack:
                    Common.isBlackUpDown = false;
                    break;

                case Common.EventType.CameraRestric:
                    Common.isCameraOut = false;
                    break;
            }

            if (clearEventCondition == ClearEventCondition.TimeOut)
                eventTime = initEventTime;
            status.isPlaying = false;
            isEventClear = true;
        }

    }

    public void ReCreateEvent()
    {
        if (isTriggerEnd)
        {
            switch (eventType)
            {
                case Common.EventType.CameraSizing:
                    Common.GetCameraObject().GetComponent<CameraEffectHandler>().SetCameraSize(tempCameraSize);
                    break;
            }
            isTriggerEnd = false;
        }
    }
}
