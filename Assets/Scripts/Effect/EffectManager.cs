using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public void NightEffect()
    {
        if (!Common.isNight)
        {
            foreach (var i in GameObject.Find("Stage1").GetComponentsInChildren<SpriteRenderer>())
            {
                i.color = new Color(i.color.r * 0.2f, i.color.g * 0.2f, i.color.b * 0.2f);
            }
            Common.isNight = true;
        }
    }

    public void MorningEffect()
    {
        if (Common.isNight)
        {
            foreach (var i in GameObject.Find("Stage1").GetComponentsInChildren<SpriteRenderer>())
            {
                i.color = new Color(i.color.r * 5f, i.color.g * 5f, i.color.b * 5f);
            }
            Common.isNight = false;
        }
    }
}
