using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeLineInterface
{
    [SerializeField]
    public int Stage;
    [SerializeField]
    public List<GameObject> Timelines = new List<GameObject>();
}
