using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlysManager : MonoBehaviour
{
    Dictionary<Vector3, FlyMove> tempPos = new Dictionary<Vector3, FlyMove>();
    bool isTracking = false;
    private void Awake()
    {
        for(var i=0; i < this.transform.childCount; i++)
        {
            tempPos.Add(this.transform.GetChild(i).GetComponent<FlyMove>().transform.position, this.transform.GetChild(i).GetComponent<FlyMove>());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&&!isTracking)
        {
            StartTrack();
        }
    }

    void StartTrack()
    {
        isTracking = true;
        if(tempPos!=null)
        {
            foreach (var i in tempPos)
            {
                i.Value.Target = Common.HERO();
            }
        }
    }

    void EndTrack()
    {
        isTracking = false;
        if (tempPos != null)
        {
            foreach (var i in tempPos)
            {
                i.Value.Target = null;
                i.Value.firstPos = i.Key;
            }
        }
    }
}
