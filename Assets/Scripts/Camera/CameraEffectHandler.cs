﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraEffectHandler : MonoBehaviour
{
    public GameObject blackupdowneffect;
    public GameObject cinemachineObj;

	void Update ()
    {
		if(Common.isBlackUpDown&&!isStartBlackUpDownEffect)
            StartCoroutine("StartBlackUpDownEffect");
        else if(blackupdowneffect.activeSelf&&isStartBlackUpDownEffect&&!Common.isBlackUpDown)
        {
            StartCoroutine("StopBlackUpDownEffect");
        }

        if (Common.isShake)
        {
            Vector2 shakePos = Random.insideUnitCircle * shakeAmount;
            transform.localPosition = new Vector3(transform.position.x + shakePos.x, transform.position.y + shakePos.y, transform.position.z);
        }
        if(Common.isHitShake)
        {
            Vector2 shakePos = Random.insideUnitCircle * shakeAmount;
            transform.localPosition = new Vector3(transform.position.x + shakePos.x, transform.position.y + shakePos.y, transform.position.z);
            Invoke("HitShakeOff", shakeTime);
        }
    }

    #region 재생성 효과
    public void AliveCamera()
    {
        if (!GetComponent<Animator>().enabled)
            GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().SetTrigger("Alive");
    }

    public void StartCamera()
    {
        if (!GetComponent<Animator>().enabled)
            GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().SetTrigger("Start");
    }

    public void InOutCamera()
    {
        GetComponent<Animator>().SetTrigger("InOut");
    }
#endregion

    #region 흔드는 효과
    private float shakeAmount = 0.035f;
    private float shakeTime = 0.1f;
    void HitShakeOff()
    {
        if(Common.isHitShake)
            Common.isHitShake = false;
        shakeAmount = 0.035f;
        shakeTime = 0.1f;
    }
    public void HitShake(float shakeTime = 0.1f, float shakeAmount = 0.035f)
    {
        this.shakeAmount = shakeAmount;
        this.shakeTime = shakeTime;
        Common.isHitShake = true;
    }
    #endregion

    #region 위아래 검은바 효과
    bool isStartBlackUpDownEffect = false;
    IEnumerator StartBlackUpDownEffect()
    {
        Debugging.Log("컷씬 스타트");
        status.isPlaying = true;

        isStartBlackUpDownEffect = true;
        int cnt = 0;
        blackupdowneffect.SetActive(true);
        var sr = blackupdowneffect.GetComponentsInChildren<Image>();
        var rt = blackupdowneffect.GetComponentsInChildren<RectTransform>();

        foreach (var item in sr)
        {
            item.color = new Color(0, 0, 0);
        }
        foreach(var item in rt)
        {
            if (item.gameObject.GetInstanceID() != blackupdowneffect.GetInstanceID())
                item.sizeDelta = new Vector2(item.sizeDelta.x, 0);
        }
        while (cnt<50)
        {
            cnt++;
            foreach (var item in sr)
            {
                item.color = new Color(0, 0, 0, cnt*0.02f);
            }
            foreach (var item in rt)
            {
                if (item.gameObject.GetInstanceID() != blackupdowneffect.GetInstanceID())
                    item.sizeDelta = new Vector2(item.sizeDelta.x, cnt*4);
            }
            yield return new WaitForSeconds(0.015f);
        }
        foreach (var item in sr)
        {
            item.color = new Color(0, 0, 0, 1);
        }
        foreach (var item in rt)
        {
            if (item.gameObject.GetInstanceID() != blackupdowneffect.GetInstanceID())
                item.sizeDelta = new Vector2(item.sizeDelta.x, 200);
        }
        yield return null;
    }
    IEnumerator StopBlackUpDownEffect()
    {
        Debugging.Log("컷씬 종료");
        isStartBlackUpDownEffect = false;
        int cnt = 0;
        var sr = blackupdowneffect.GetComponentsInChildren<Image>();
        var rt = blackupdowneffect.GetComponentsInChildren<RectTransform>();
        
        while (cnt<50)
        {
            cnt++;
            foreach (var item in sr)
            {
                item.color = new Color(0, 0, 0,1-(cnt * 0.02f));
            }
            foreach (var item in rt)
            {
                if(item.gameObject.GetInstanceID()!=blackupdowneffect.GetInstanceID())
                   item.sizeDelta = new Vector2(item.sizeDelta.x,(200- (cnt * 4)));
            }
            yield return new WaitForSeconds(0.015f);
        }
        foreach (var item in rt)
        {
            if (item.gameObject.GetInstanceID() != blackupdowneffect.GetInstanceID())
                item.sizeDelta = new Vector2(item.sizeDelta.x, 0);
        }
        status.isPlaying = false;
        blackupdowneffect.SetActive(false);
        yield return null;
    }
    #endregion

    #region 카메라 넓고 좁게 설정
    public void SetCameraSize(float width)
    {
        if(!status.isInBuilding)
            StartCoroutine(SizingCamera(width));
    }

    IEnumerator SizingCamera(float width)
    {
        float getSize = GetComponent<Camera>().orthographicSize;
        Debugging.Log("카메라사이즈 변경 : " + getSize + " ----> " + width);
        bool isReduction = true;
        float setSize = width - getSize > 0 ? 0.01f : -0.01f;
        while(isReduction)
        {
            GetComponent<Camera>().orthographicSize += setSize;
            yield return new WaitForEndOfFrame();

            if (GetComponent<Camera>().orthographicSize >= width-0.02f&& GetComponent<Camera>().orthographicSize <= width + 0.02f)
            {
                GetComponent<Camera>().orthographicSize = width;
                Common.cameraOrthSize = width;
                isReduction = false;
            }
        }
        yield return null;
    }
#endregion
}
