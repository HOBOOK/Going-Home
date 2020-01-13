using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolItemEventManager : MonoBehaviour
{

    bool isStartEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartEvent();
        }
    }

    void StartEvent()
    {
        if(!isStartEvent)
            StartCoroutine("StartingEvent");
    }

    IEnumerator StartingEvent()
    {
        Common.HERO().GetComponent<move>().SetFalseAllboolean();
        isStartEvent = true;
        Common.isBlackUpDown = true;
        int cnt = 0;
        while(cnt<3)
        {
            cnt++;
            yield return new WaitForSeconds(1f);
        }
        Common.isBlackUpDown = false;
        yield return null;
    }
}
