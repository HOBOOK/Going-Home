using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_hp : MonoBehaviour {


    public GameObject Target = null;
    bool isOnPanelHP = false;
    float currentValue = 0;
    float panelHpTime = 0;
    GameObject canvasUI;

    private void OnEnable()
    {
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }
    private void Awake()
    {
        canvasUI = GameObject.Find("CanvasUI");
    }

    // Update is called once per frame
    void Update () {

        if (isOnPanelHP&&Target!=null)
        {
            this.transform.position = Target.transform.position+ new Vector3(0, Target.transform.localScale.y*1.5f) ;
            currentValue = DecrementSliderValue(transform.GetChild(0).GetComponent<Slider>().value, GetCurrentHp() / GetMaxHp());
            transform.GetChild(0).GetComponent<Slider>().value = currentValue;
            OffPanelHPTimer();
        }
    }
    public void OpenHpUI(GameObject target)
    {
        if (Target == null||Target != target)
            Target = target;

        GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(GetMaxHp(), 100, 300), 70);
        RectTransform rt = GetComponentInChildren<Slider>().gameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, Mathf.Clamp(GetMaxHp() * 0.1f, 20, 25));
        panelHpTime = 0.0f;
        currentValue = GetCurrentHp();
        transform.GetChild(0).GetComponent<Slider>().value = GetCurrentHp() / GetMaxHp();
        transform.GetChild(1).GetComponent<Text>().text = Common.hitTargetName;
        isOnPanelHP = true;
    }

    void OffPanelHPTimer(bool isDirect = false)
    {
        panelHpTime += Time.deltaTime;
        if (panelHpTime > 3.0 || GetCurrentHp()<=0)
        {
            DisablePanelHP();
        }
    }

    void DisablePanelHP()
    {
        Target = null;
        panelHpTime = 0.0f;
        isOnPanelHP = false;
        ObjectPool.Instance.PushToPool("hpEnemyUI", this.gameObject, canvasUI.transform);
    }


    float GetCurrentHp()
    {
        if (Target != null)
        {
            if (Target.GetComponent<npcMove>() != null)
                return Target.GetComponent<npcMove>().hp;
            else if (Target.GetComponent<bossMove>() != null)
                return Target.GetComponent<bossMove>().hp;
            else
                return 1;
        }
        else
            return 1;
    }
    float GetMaxHp()
    {
        if (Target != null)
        {
            if (Target.GetComponent<npcMove>() != null)
                return Target.GetComponent<npcMove>().maxHp;
            else if (Target.GetComponent<bossMove>() != null)
                return Target.GetComponent<bossMove>().maxHp;
            else
                return 1;
        }
        else
            return 1;
    }

    float DecrementSliderValue(float n, float target)
    {
        if (target < n)
            n -= Time.deltaTime * 0.5f;
        return n;
    }
}
