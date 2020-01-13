using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineSceanario : MonoBehaviour {

    bool isStart = false;
    bool isEnd = false;
    public double totalSceanrioTime;
    public float currentSceanrioTime = 0.0f;
    int endNumber = 0;
    Animator heroAnimator;
    public GameObject cameraTarget;
    public Sceanrio[] sceanrios;

    private void Awake()
    {
        totalSceanrioTime = GetComponent<PlayableDirector>().duration;
    }
    private void Start()
    {
        heroAnimator = Common.HERO().GetComponent<Animator>();
        CinemaCameraTargetChange(cameraTarget);
    }
    void Update ()
    {
        if (GetComponent<PlayableDirector>().state == PlayState.Playing&&!isStart)
        {
            isStart = true;
        }
        if (isStart&&!isEnd)
        {
            if(sceanrios.Length>0)
            {
                currentSceanrioTime += Time.deltaTime;

                foreach (var i in sceanrios)
                {
                    if (currentSceanrioTime > i.time && !i.isEnd)
                    {
                        Common.Chat(i.script, i.transform, i.correctionY);
                        sceanrios[endNumber].isEnd = true;
                        if (endNumber < sceanrios.Length - 1)
                            endNumber++;
                    }
                }
            }
            if (GetComponent<PlayableDirector>().state != PlayState.Playing)
            {
                StartCoroutine("EndTimeLine");
            }
        }
    }
    IEnumerator EndTimeLine()
    {
        Common.GetCameraObject().GetComponent<CinemachineBrain>().enabled = false;
        currentSceanrioTime = 0.0f;
        isEnd = true;
        Vector3 tempPos = Common.HERO().transform.position;
        Common.CameraAllEffectOff();
        yield return new WaitForSeconds(1f);
        heroAnimator.applyRootMotion = false;
        heroAnimator.Rebind();
        Common.HERO().transform.position = tempPos;
        this.gameObject.SetActive(false);
    }


    void CinemaCameraTargetChange(GameObject obj)
    {
        if (obj == null)
            obj = Common.HERO();
        
        Common.GetCameraObject().GetComponentInChildren<CinemachineVirtualCamera>().Follow = obj.transform;
    }


    [Serializable]
    public struct Sceanrio
    {
        public Transform transform;
        public string script;
        public float time;
        public bool isEnd;
        public int correctionY;
    }
}

