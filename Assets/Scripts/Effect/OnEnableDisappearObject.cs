using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableDisappearObject : MonoBehaviour
{
    void OnEnable()
    {
        if(GetComponent<SpriteRenderer>()!=null)
            StartCoroutine("DisappearObj");
        else if(GetComponent<MeshRenderer>()!=null)
            StartCoroutine("DisappearRendererObj");
    }

    public IEnumerator DisappearObj()
    {
        Color thisColor = GetComponent<SpriteRenderer>().color;
        float cnt = 1;
        while (cnt > 0.1f)
        {
            GetComponent<SpriteRenderer>().color = new Color(thisColor.r, thisColor.g, thisColor.b, cnt);
            cnt -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        GetComponent<SpriteRenderer>().color = new Color(thisColor.r, thisColor.g, thisColor.b, 0);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
        yield return null;
    }


    public IEnumerator DisappearRendererObj()
    {
        Color thisColor = GetComponent<MeshRenderer>().material.color;
        float cnt = 1;
        while (cnt > 0.1f)
        {
            GetComponent<MeshRenderer>().material.color = new Color(thisColor.r, thisColor.g, thisColor.b, cnt);
            cnt -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        GetComponent<MeshRenderer>().material.color = new Color(thisColor.r, thisColor.g, thisColor.b, 0);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
        yield return null;
    }
}
