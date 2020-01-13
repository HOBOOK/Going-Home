using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeController : MonoBehaviour {

    public GameObject target;
    public bool isActionButton;
    public bool isFadeIn = false;
    public bool isFadeOut = false;
    public bool isOver = false;
    public float setDistance;
    public int fadeType;
    public float distance;
    
    public enum FadeType
    {
        Distance,
        Push
    }
	void Start ()
    {
        if(target==null)
        {
            target = Common.HERO();
        }
        this.GetComponent<Animator>().SetBool("Action", isActionButton);
	}

    private void OnEnable()
    {
        //StartCoroutine("PopUI");
    }

    IEnumerator PopUI()
    {
        Vector3 tempScale = this.transform.localScale;
        this.transform.localScale = new Vector3(0, 0, 0);
        while(this.transform.localScale.x>=tempScale.x)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, tempScale, 0.05f);
            yield return new WaitForEndOfFrame();
        }
        this.transform.localScale = tempScale;
        yield return null;
    }


    void FadeInText()
    {
        if (isFadeIn || distance > setDistance)
            return;

        else
        {
            GetComponent<Image>().enabled = true;
            this.GetComponent<Animator>().SetTrigger("fadeIn");
            isFadeIn = true;
        }
        
    }
    void FadeOutText(bool isOverX = false)
    {
        if (isFadeIn)
        {
            if (isOverX)
            {
                if (distance > setDistance && this.transform.position.x < target.transform.position.x)
                {
                    this.GetComponent<Animator>().SetTrigger("fadeOut");
                    isFadeIn = false;
                    isOver = false;
                    isFadeOut = true;
                }
            }
            else
            {
                if (distance > setDistance)
                {
                    this.GetComponent<Animator>().SetTrigger("fadeOut");
                    isOver = true;
                    isFadeIn = false;
                    isFadeOut = true;
                }
            }
        }
        else
            return;
    }

    void Update ()
    {
        switch(fadeType)
        {
            case (int)FadeType.Distance:
                distance = Vector2.Distance(this.transform.position, target.transform.position);
                if (!isOver)
                {
                    FadeInText();
                    FadeOutText(true);
                    if (GetComponent<Image>().color.a <= 0 && isFadeOut)
                    {
                        GetComponent<Image>().enabled = false;
                        isFadeOut = false;
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
                    if (GetComponent<Image>().color.a <= 0&&isFadeOut)
                    {
                        GetComponent<Image>().enabled = false;
                        isOver = false;
                        isFadeOut = false;
                    }
                }
                break;
        }
	}
}
