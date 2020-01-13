using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_Manager : MonoBehaviour
{
    // Panel Prefab//
    public GameObject PanelHP;
    public GameObject PanelChapter;
    public GameObject PanelToolTip;
    public GameObject PanelHeart;
    public GameObject UiHp;
    // Panel Prefab//

    private GameObject targetGameObject;
    private bool isOnPanelHP = false;
    private float panelHpTime = 0.0f;
    private float currentValue;

    // About Heart//
    public GameObject heartImage;
    bool isHeartLoad = false;
    bool heartSizeFlag = false;
    int tempHeart = 0;
    float heartSize = 1;
    // About Heart//

    public static GUI_Manager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        PanelHP.SetActive(false);
        PanelChapter.SetActive(false);
        PanelToolTip.SetActive(false);
        PanelHeart.SetActive(false);
        UiHp.SetActive(false);
    }
    void Update ()
    {
        //UpdatePanelHP();
        UpdatePanelHeart();
    }
    void UpdatePanelHP()
    {
        if (status.isDead || !Common.isStart)
            OffPanelHP(true);
        if (!string.IsNullOrEmpty(Common.hitTargetName) && PanelHP != null && !isOnPanelHP)
        {
            panelHpTime = 0.0f;
            targetGameObject = Common.hitTargetObject();
            currentValue = GetCurrentHp();
            PanelHP.gameObject.SetActive(true);
            PanelHP.transform.GetChild(0).GetComponent<Slider>().value = GetCurrentHp() / GetMaxHp();
            PanelHP.transform.GetChild(1).GetComponent<Text>().text = Common.hitTargetName;
            isOnPanelHP = true;
        }
        else if (string.IsNullOrEmpty(Common.hitTargetName) && PanelHP.activeSelf)
        {
            OffPanelHP();
        }
        if (isOnPanelHP)
        {
            if (targetGameObject != null && targetGameObject.name != Common.hitTargetName && !string.IsNullOrEmpty(Common.hitTargetName))
            {
                targetGameObject = Common.hitTargetObject();
                currentValue = GetCurrentHp();
                PanelHP.transform.GetChild(1).GetComponent<Text>().text = Common.hitTargetName;
            }
            currentValue = DecrementSliderValue(PanelHP.transform.GetChild(0).GetComponent<Slider>().value, GetCurrentHp() / GetMaxHp());
            PanelHP.transform.GetChild(0).GetComponent<Slider>().value = currentValue;
        }
    }
    void OffPanelHP(bool isDirect=false)
    {
        panelHpTime += Time.deltaTime;
        if(panelHpTime>3.0||isDirect)
        {
            DisablePanelHP();
        }
    }

    void DisablePanelHP()
    {
        targetGameObject = null;
        PanelHP.gameObject.SetActive(false);
        panelHpTime = 0.0f;
        isOnPanelHP = false;
    }

    public void ChapterGUI(int chapterNumber)
    {
        PanelChapter.SetActive(true);
        PanelChapter.GetComponent<GUI_ChapterTextManager>().SetChapterText(chapterNumber);
    }

    float GetCurrentHp()
    {
        if (targetGameObject != null)
        {
            if (targetGameObject.GetComponent<npcMove>() != null)
                return targetGameObject.GetComponent<npcMove>().hp;
            else if (targetGameObject.GetComponent<bossMove>() != null)
                return targetGameObject.GetComponent<bossMove>().hp;
            else
                return 1;
        }
        else
            return 1;
    }
    float GetMaxHp()
    {
        if (targetGameObject != null)
        {
            if (targetGameObject.GetComponent<npcMove>() != null)
                return targetGameObject.GetComponent<npcMove>().maxHp;
            else if (targetGameObject.GetComponent<bossMove>() != null)
                return targetGameObject.GetComponent<bossMove>().maxHp;
            else
                return 1;
        }
        else
            return 1;
    }

    float DecrementSliderValue(float n, float target)
    {
        if (target < n)
            n -= Time.deltaTime*0.5f;
        return n;
    }

    public void ToolTipOn(string text)
    {
        PanelToolTip.gameObject.SetActive(false);
        PanelToolTip.GetComponent<GUI_ToolTip>().text = text;
        PanelToolTip.gameObject.SetActive(true);
    }

    IEnumerator AwakePanelHeart()
    {
        isHeartLoad = false;
        tempHeart = status.hp;
        for (int i = 0; i < status.hp; i++)
        {
            GameObject heart = Instantiate(heartImage, PanelHeart.transform) as GameObject;
            RectTransform rt = heart.GetComponent(typeof(RectTransform)) as RectTransform;
            rt.anchoredPosition = new Vector2(100 + (i * 80), 0);
            heart.SetActive(true);
            heart.transform.localScale = new Vector3(0, 0, 0);
        }
        yield return new WaitForSeconds(2.0f);
        float cnt = 0;
        while(cnt<1)
        {
            foreach(var h in PanelHeart.transform.GetComponentsInChildren<RectTransform>())
            {
                h.localScale = new Vector3(cnt, cnt, cnt);
            }
            cnt += 0.05f;
            yield return new WaitForEndOfFrame();
        }
        foreach (var h in PanelHeart.transform.GetComponentsInChildren<RectTransform>())
        {
            h.localScale = new Vector3(1, 1, 1);
        }
        isHeartLoad = true;
        yield return null;

    }
    public void UpdatePanelHeart()
    {
        if(!PanelHeart.activeSelf&&!isHeartLoad)
        {
            if (Common.isStart)
            {
                PanelHeart.SetActive(true);
                StartCoroutine("AwakePanelHeart");
            }
        }
        if(isHeartLoad)
        {
            if (tempHeart > status.hp)
            {
                for (int i = status.hp; i < tempHeart; i++)
                {
                    if (i < PanelHeart.transform.childCount)
                        PanelHeart.transform.GetChild(i).gameObject.SetActive(false);
                }
                tempHeart = status.hp;
            }
            else if (tempHeart < status.hp)
            {
                for (int i = tempHeart; i < status.hp; i++)
                {
                    if (i < PanelHeart.transform.childCount)
                        PanelHeart.transform.GetChild(i).gameObject.SetActive(true);
                }
                tempHeart = status.hp;
            }
            if (PanelHeart.gameObject.activeSelf && status.hp>= 1)
            {
                if (heartSize < 1f)
                    heartSizeFlag = true;
                else if (heartSize > 1.1f)
                    heartSizeFlag = false;

                if (heartSizeFlag)
                    heartSize += 0.1f * Time.deltaTime;
                else
                    heartSize -= 0.1f * Time.deltaTime;
                PanelHeart.transform.GetChild(status.hp - 1).localScale = new Vector3(heartSize, heartSize, heartSize);
            }
            if (status.isDead)
                PanelHeart.gameObject.SetActive(false);
            else
                PanelHeart.gameObject.SetActive(true);
        }
    }
}
