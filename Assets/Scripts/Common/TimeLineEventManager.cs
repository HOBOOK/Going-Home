using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimeLineEventManager : MonoBehaviour
{
    bool isInit = false;
    bool isAwake = false;
    bool isStageLoad = false;
    public List<GameObject> mapList = new List<GameObject>();
    public List<TimeLineInterface> timelineInterfaces = new List<TimeLineInterface>();

    public enum STAGE {ONE, TWO}
    public static TimeLineEventManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        this.transform.hideFlags = HideFlags.HideInInspector;
    }

    private void Start()
    {
        Debugging.LogSystem("map list is loaded Succesfully.");
        isInit = true;
        StartStageTimeLine();
    }

    void Update ()
    {
        DetailStageTimeLineCheck();
    }
 
    /// <summary>
    /// Stage 변경 체크
    /// </summary>
    public void StartStageTimeLine()
    {
        isStageLoad = false;
        if (!isInit && isAwake)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            isAwake = false;
        }
        switch ((STAGE)status.stageNumber)
        {
            case STAGE.ONE:
                Common.RELOAD_GAME();
                LoadMap(0);
                break;
            case STAGE.TWO:
                Common.RELOAD_GAME();
                LoadMap(1);
                break;
        }
        Debugging.LogSystem("스테이지 단계 : " + status.stageNumber + "\r\n세부 스테이지 단계 : " + status.stageDetailNumber);
        isInit = false;
        isAwake = true;
        isStageLoad = true;
        NormalStart();
    }

    /// <summary>
    /// 같은 Stage 안에서의 CutScene 단계체크
    /// </summary>
    void DetailStageTimeLineCheck()
    {
        if(isStageLoad)
        {
            switch ((STAGE)status.stageNumber)
            {
                case STAGE.ONE:
                    switch (status.stageDetailNumber)
                    {
                        case 0: // # 1
                            if (!Common.isAwake && !Common.isStart)
                            {
                                TimeLineStart(0);
                                Hide(0, Common.HERO());
                                Common.HERO().transform.position = new Vector3(-18.3f, -0.3f, 0);
                                status.stageDetailNumber = 1;
                            }
                            break;
                        case 1: // # 2
                            if (Common.isAwake)
                            {
                                Common.isBlackUpDown = true;
                                Hide(1, Common.HERO());
                                status.stageDetailNumber = 1;
                                GUI_Manager.instance.ChapterGUI(0);
                                status.stageDetailNumber = 2;
                            }
                            break;
                        case 2: // # 3
                            status.stageDetailNumber = -1;
                            break;
                    }
                    break;
            }
        }
    }

    // 게임실행시 인트로단계가 아니면 기본시작
    void NormalStart()
    {
        if(status.stageDetailNumber==-1)
        {
            Camera.main.GetComponent<CameraEffectHandler>().StartCamera();
            Common.START_GAME();
            Common.HERO().transform.position = status.checkPoint;
        }
    }


    public void Hide(float isHide, GameObject obj)
    {
        var sprites = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var a in sprites)
            a.color = new Color(1, 1, 1, isHide);
    }

    public void LoadMap(int mapNumber)
    {
        for (int i = 0; i < mapList.Count; i++)
        {
            if (i == mapNumber)
                mapList[i].gameObject.SetActive(true);
            else
                mapList[i].gameObject.SetActive(false);
        }
    }

    public void TimeLineStart(int timelinenumber)
    {
        if(timelineInterfaces[status.stageNumber].Timelines[timelinenumber]!=null)
        {
            Common.GetCameraObject().GetComponent<CinemachineBrain>().enabled = true;
            timelineInterfaces[status.stageNumber].Timelines[timelinenumber].SetActive(true);
            timelineInterfaces[status.stageNumber].Timelines[timelinenumber].GetComponent<PlayableDirector>().Play();
            Debugging.Log(status.stageNumber + " 스테이지의 " + timelinenumber + " 타임라인을 실행합니다.");
        }
        else
        {
            Debugging.LogWarning(status.stageNumber + " 스테이지의 " + timelinenumber + " 타임라인이 존재하지 않습니다.");
        }
    }

}
