using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNpcManager : MonoBehaviour {

    public List<string> scripts = new List<string>();
    bool isScriptOn = false;
    bool isChatPop = false;
    void DroppingItem(GameObject dropItem)
    {
        if (dropItem!=null)
        {
            GameObject item = Instantiate(dropItem, GameObject.Find("dropItems").transform) as GameObject;
            item.transform.position = transform.position;
            item.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 7), ForceMode2D.Impulse);
        }
    }
    private void OnEnable()
    {
        DisappearEffect();
    }

    private void OnDisable()
    {
        DisappearEffect();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && scripts.Count > 0)
        {
            if (!isScriptOn && !isChatPop)
            {
                isChatPop = true;
                StartCoroutine("ChatAblePop");
            }
            if (Input.GetAxisRaw("Vertical") > 0 && !isScriptOn)
            {
                isScriptOn = true;
                StartCoroutine("Chatting");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && scripts.Count > 0)
        {
            if (isChatPop)
                isChatPop = false;
        }
    }


    IEnumerator Chatting()
    {
        if(isScriptOn)
        {
            Common.Chat(scripts[Random.Range(0, scripts.Count)], this.transform);
            yield return new WaitForSeconds(3.0f);
            isScriptOn = false;
            isChatPop = false;
        }
        yield return null;
    }

    IEnumerator ChatAblePop()
    {
        Debugging.Log(string.Format("\'{0}\'와 대화가능 상태", name));
        yield return null;
    }


    public void DisappearEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Disappear_Smoke");
        effect.transform.position = transform.position;
        effect.SetActive(true);
    }
}
