using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventManager : Singleton<ObjectPool>
{
    [Header("Target")]
    public GameObject target;
    [Header("Npc")]
    public List<TriggerNpc> triggerNpcList = new List<TriggerNpc>();
    [Header("Object")]
    public List<TriggerObject> triggerObjectList = new List<TriggerObject>();
    [Header("Event")]
    public List<TriggerEvent> triggerEventList = new List<TriggerEvent>();

    private bool isEndFlag = false;

    public void TriggerEventInitialize()
    {
        if(Common.triggerObjectInitialize)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if(!transform.GetChild(i).name.Equals("Once"))
                    Destroy(transform.GetChild(i).gameObject);
            }
            for (int ix = 0; ix < triggerNpcList.Count; ++ix)
            {
                if (triggerNpcList[ix].isTriggerEnd && !triggerNpcList[ix].isOnceTrigger)
                {
                    triggerNpcList[ix].isTriggerEnd = false;
                }
            }
            for (int ix = 0; ix < triggerObjectList.Count; ++ix)
            {
                if (triggerObjectList[ix].isTriggerEnd&&!triggerObjectList[ix].isOnceTrigger)
                {
                    triggerObjectList[ix].isTriggerEnd = false;
                }
            }
            for (int ix = 0; ix < triggerEventList.Count; ++ix)
            {
                if (triggerEventList[ix].isTriggerEnd&&!triggerEventList[ix].isOnceTrigger)
                {
                    triggerEventList[ix].isTriggerEnd = false;
                    triggerEventList[ix].isEventClear = false;
                }
            }
            Common.triggerObjectInitialize = false;
        }
    }

    void Awake()
    {
        target = Common.HERO();
    }
	
	// Update is called once per frame
	void Update ()
    {
        for (int ix = 0; ix < triggerNpcList.Count; ++ix)
        {
            if (triggerNpcList[ix].startTimeAtTargetPositionX <= target.transform.position.x && triggerNpcList[ix].startTimeAtTargetPositionX+2 > target.transform.position.x&& !triggerNpcList[ix].isTriggerEnd && status.checkPoint.x < triggerNpcList[ix].startTimeAtTargetPositionX)
            {
                triggerNpcList[ix].Initialize(transform);
            }
        }
        for (int ix = 0; ix < triggerObjectList.Count; ++ix)
        {
            if(triggerObjectList[ix].startTimeAtTargetPositionX<= target.transform.position.x && triggerObjectList[ix].startTimeAtTargetPositionX + 2 > target.transform.position.x && !triggerObjectList[ix].isTriggerEnd && status.checkPoint.x < triggerObjectList[ix].startTimeAtTargetPositionX)
            {
                triggerObjectList[ix].Initialize(transform);
            }
        }
        for (int ix = 0; ix < triggerEventList.Count; ++ix)
        {
            if (triggerEventList[ix].startTimeAtTargetPositionX <= target.transform.position.x && triggerEventList[ix].startTimeAtTargetPositionX + 2 > target.transform.position.x && !triggerEventList[ix].isTriggerEnd && status.checkPoint.x < triggerEventList[ix].startTimeAtTargetPositionX)
            {
                triggerEventList[ix].CreateEvent();
            }
            if(triggerEventList[ix].isTriggerEnd)
            {
                // 카메라사이징
                if(triggerEventList[ix].eventType == Common.EventType.CameraSizing)
                {
                    if(triggerEventList[ix].startTimeAtTargetPositionX > target.transform.position.x&& triggerEventList[ix].startTimeAtTargetPositionX +2 < target.transform.position.x)
                    {
                        triggerEventList[ix].ReCreateEvent();
                    }
                }
                // 이벤트 종료
                if (triggerEventList[ix].clearEventCondition!=TriggerEvent.ClearEventCondition.None)
                {
                    switch (triggerEventList[ix].clearEventCondition)
                    {
                        case TriggerEvent.ClearEventCondition.NpcDie:
                            if (triggerEventList[ix].isTriggerEnd)
                            {
                                for (int i = 0; i < transform.childCount; i++)
                                {
                                    if (transform.GetChild(i).name.Contains("Npc"))
                                    {
                                        isEndFlag = transform.GetChild(i).gameObject.activeSelf ? false : true;
                                    }
                                }
                            }
                            if (isEndFlag)
                            {
                                triggerEventList[ix].ClearEvent();
                            }
                            break;
                        case TriggerEvent.ClearEventCondition.TimeOut:
                            if (triggerEventList[ix].eventTime <= 0)
                            {
                                triggerEventList[ix].ClearEvent();
                            }
                            else
                                triggerEventList[ix].eventTime -= Time.deltaTime;
                            break;
                    }
                }
            }
        }
        TriggerEventInitialize();
    }
}
