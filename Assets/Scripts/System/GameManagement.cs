using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        this.transform.hideFlags = HideFlags.HideInInspector;
        Cursor.visible = false;
        Application.targetFrameRate = 60;
        ItemSystem.LoadItem();
        SaveSystem.LoadPlayer();
    }
    private void Update()
    {
        TestEventKey();
    }
    void TestEventKey()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debugging.Log("F1 >> " + "카메라효과를 끕니다.");
            Common.CameraAllEffectOff();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            float timeScale = 1;
            if (Time.timeScale == 1f)
                timeScale = 2f;
            else if (Time.timeScale == 2f)
                timeScale = 4f;
            else
                timeScale = 1f;
            Time.timeScale = timeScale;
            Debugging.Log("F2 >> " + "게임진행속도 증가 x " + timeScale);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            if (!Common.isNight)
            {
                foreach (var i in GameObject.Find("Stage1").GetComponentsInChildren<SpriteRenderer>())
                {
                    i.color = new Color(i.color.r * 0.2f, i.color.g * 0.2f, i.color.b * 0.2f);
                }
                Common.isNight = true;
            }
            else
            {
                foreach (var i in GameObject.Find("Stage1").GetComponentsInChildren<SpriteRenderer>())
                {
                    i.color = new Color(i.color.r * 5f, i.color.g * 5f, i.color.b * 5f);
                }
                Common.isNight = false;
            }

            Debugging.Log("F3 >> " + Common.isNight);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            Common.START_GAME();
            Debugging.Log("F4 >> " + "게임 스타트 온");
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            Common.HERO().GetComponent<move>().PlayingEventScene("PlayingNotebook");
            Debugging.Log("F5 >> " + "NoteBook");
        }
        else if(Input.GetKeyDown(KeyCode.F6))
        {
            Common.GetBottomPosition(Common.HERO().transform);
        }
    }

}
