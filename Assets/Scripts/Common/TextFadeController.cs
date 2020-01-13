using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFadeController : MonoBehaviour {

    public GameObject target;
    GameObject textMesh;
    bool isFadeIn = false;
    bool isOver = false;
    public float setDistance;
    public int fadeType;
    public bool isStepByStep;
    string worldUIText;
    int textCount;
    float distance;
    
    public enum FadeType
    {
        Distance,
        Push
    }
    private void Awake()
    {
        if (target == null)
            target = Common.HERO();
    }
    void Start ()
    {
        textMesh = this.gameObject.transform.GetChild(0).gameObject;
        textMesh.SetActive(false);
        if(isStepByStep)
        {
            worldUIText = textMesh.GetComponent<Text>().text;
            textCount = worldUIText.Length;
            textMesh.GetComponent<Text>().text = "";

        }
    }
    IEnumerator TypingChat()
    {
        int cnt = 0;
        while (cnt < textCount)
        {
            textMesh.GetComponent<Text>().text += worldUIText[cnt];
            yield return new WaitForSeconds(0.05f);
            if (cnt % 10 == 0 && cnt > 1)
                textMesh.GetComponent<Text>().text += "\r\n";
            cnt++;
        }

        yield return null;
    }


    void FadeInText()
    {
        if (isFadeIn || distance > setDistance)
            return;
        else
        {
            textMesh.SetActive(true);
            this.GetComponent<Animator>().SetTrigger("fadeIn");
            if(isStepByStep)
                StartCoroutine("TypingChat");
            if (textMesh != null && textMesh.transform.childCount > 0)
            {
                if (textMesh.GetComponentsInChildren<Image>() != null)
                {
                    StartCoroutine("TextMeshChildImagesFadeIn");
                }
            }

            isFadeIn = true;
        }
        
    }
    void FadeOutText(bool isOverX = false)
    {
        if (isFadeIn)
        {
            if(isOverX)
            {
                if (distance > setDistance && this.transform.position.x < target.transform.position.x)
                {
                    this.GetComponent<Animator>().SetTrigger("fadeOut");
                    if (textMesh != null && textMesh.transform.childCount > 0)
                    {
                        if (textMesh.GetComponentsInChildren<Image>() != null)
                        {
                            StartCoroutine("TextMeshChildImagesFadeOut");
                        }
                    }
                    isOver = true;
                }
            }
            else
            {
                if (distance > setDistance)
                {
                    this.GetComponent<Animator>().SetTrigger("fadeOut");
                    if (textMesh != null && textMesh.transform.childCount > 0)
                    {
                        if (textMesh.GetComponentsInChildren<Image>() != null)
                        {
                            StartCoroutine("TextMeshChildImagesFadeOut");
                        }
                    }
                    isOver = true;
                }
            }
            
        }
        else
            return;
    }

    void Update ()
    {
        if(this.transform.position.x > status.checkPoint.x)
        {
            switch (fadeType)
            {
                case (int)FadeType.Distance:
                    distance = Vector2.Distance(this.transform.position, target.transform.position);
                    if (!isOver)
                    {
                        FadeInText();
                        FadeOutText(true);
                    }
                    else
                    {
                        if (textMesh.GetComponent<Text>().color.a <= 0)
                        {
                            this.gameObject.SetActive(false);
                            textMesh.gameObject.SetActive(false);
                        }
                    }
                    break;
                case (int)FadeType.Push:
                    distance = Vector2.Distance(this.transform.position, target.transform.position);
                    if (!isOver)
                    {
                        FadeInText();
                        FadeOutText();
                    }
                    else
                    {
                        if (textMesh.GetComponent<Text>().color.a <= 0)
                        {
                            this.gameObject.SetActive(false);
                            isOver = false;
                        }
                    }
                    break;
            }
        }
	}

    IEnumerator TextMeshChildImagesFadeIn()
    {
        float cnt = 0;
        Color color = textMesh.GetComponentInChildren<Image>().color;
        while(cnt<1)
        {
            foreach (var i in textMesh.GetComponentsInChildren<Image>())
            {
                i.color = new Color(color.r, color.g, color.b, cnt);
                cnt += 0.01f;
            }
            yield return new WaitForEndOfFrame();
        }
        foreach (var i in textMesh.GetComponentsInChildren<Image>())
        {
            i.color = new Color(color.r, color.g, color.b, 1);
        }
        yield return null;
    }
    IEnumerator TextMeshChildImagesFadeOut()
    {
        float cnt = 1;
        Color color = textMesh.GetComponentInChildren<Image>().color;
        while (cnt > 0)
        {
            foreach (var i in textMesh.GetComponentsInChildren<Image>())
            {
                i.color = new Color(color.r, color.g, color.b, cnt);
                cnt -= 0.01f;
            }
            yield return new WaitForEndOfFrame();

        }
        foreach (var i in textMesh.GetComponentsInChildren<Image>())
        {
            i.color = new Color(color.r, color.g, color.b, 0);
        }
        yield return null;
    }
}
