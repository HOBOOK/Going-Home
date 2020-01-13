using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHeightManager : MonoBehaviour
{
    float scaleX;
    public bool isStart = false;
    public bool isWave = false;
    public int waveAmount = 0;
    public string LayerMaskName="";

    public float TargetHeight = 1;
    private void Start()
    {
        scaleX = this.transform.localScale.x;
    }

    private void FixedUpdate()
    {
        if(this.transform.localScale.y>0.15f)
        {
            GetComponentInChildren<BuoyancyEffector2D>().enabled = true;
            foreach (var i in GetComponentsInChildren<BoxCollider2D>())
            {
                i.enabled = true;
            }
        }
        else
        {
            GetComponentInChildren<BuoyancyEffector2D>().enabled = false;
            foreach(var i in GetComponentsInChildren<BoxCollider2D>())
            {
                if(i!=GetComponent<BoxCollider2D>())
                    i.enabled = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(LayerMaskName!="")
        {
            if (!isStart && LayerMask.LayerToName(collision.gameObject.layer).Equals(LayerMaskName))
            {
                Debug.Log("물채워넣기 시작");
                WaveWater(waveAmount);
                StartCoroutine("SwellingWater");
                isStart = true;
            }
        }
    }
    public IEnumerator SwellingWater()
    {
        float currentHeight = this.transform.localScale.y;
        bool isHigh = currentHeight > TargetHeight;

        if(isHigh)
        {
            while (currentHeight > TargetHeight)
            {
                currentHeight -= 0.001f;
                this.transform.localScale = new Vector3(scaleX, currentHeight, 1);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            while (currentHeight < TargetHeight)
            {
                currentHeight += 0.001f;
                this.transform.localScale = new Vector3(scaleX, currentHeight, 1);
                yield return new WaitForSeconds(0.05f);
            }
        }
        this.transform.localScale = new Vector3(scaleX, TargetHeight, 1);

        yield return null;
    }

    public void WaveWater(float amount)
    {
        if(isWave)
        {
            transform.GetComponentInChildren<BuoyancyEffector2D>().flowMagnitude = amount;
            isWave = false;
        }
    }
}
