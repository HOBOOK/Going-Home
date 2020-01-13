using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class TriggerNpc
{
    private GameObject prefabObject;
    public Common.NPC_APPERANCE npcApperance;
    public List<string> chats = new List<string>();
    public bool isFlip = false;
    public bool isHold = false;
    public bool isNeutrality = false;

    [Range(0,10)]
    public int count = 1;
    public float startTimeAtTargetPositionX;
    public Vector3 startPosition;
    [HideInInspector]
    public bool isTriggerEnd = false;
    [HideInInspector]
    public bool isOnceTrigger = false;

    [Range(5, 20)]
    public float detectionRange;
    public GameObject DropItem;
    public Common.WeaponType[] Weapons = new Common.WeaponType[2];

    private int randomZ = 0;

    public void Initialize(Transform parent = null)
    {
        for(int i = 0; i < count; i++)
        {
            randomZ = i;
            CreateItem(parent);
        }
            
    }

    private GameObject CreateItem(Transform parent = null)
    {
        SetPrefabObject(npcApperance);
        GameObject item = Object.Instantiate(prefabObject) as GameObject;
        item.name = isOnceTrigger ? "Once" : prefabObject.name;
        item.name += randomZ;
        item.transform.SetParent(parent);
        item.transform.position = startPosition;
        if(chats.Count>0)
            item.GetComponent<npcMove>().initChats = chats;
        item.GetComponent<npcMove>().isFlip = isFlip;
        item.GetComponent<npcMove>().isHold = isHold;
        item.GetComponent<npcMove>().isNeutrality = isNeutrality;
        item.GetComponent<npcMove>().detectionRange = detectionRange;
        item.GetComponent<npcMove>().dropItem = DropItem;
        item.GetComponent<npcMove>().Weapons = Weapons;
        item.SetActive(true);
        isTriggerEnd = true;
        return item;
    }

    private void SetPrefabObject(Common.NPC_APPERANCE npcApperance)
    {
        startPosition.z = randomZ;
        startPosition.x += (randomZ*0.5f);
        if (npcApperance==Common.NPC_APPERANCE.Random)
        {
            prefabObject = Common.GetPrefabDatabase().GetComponent<PrefabsDatabaseManager>().GetApperance(Random.Range(0, Common.GetPrefabDatabase().GetComponent<PrefabsDatabaseManager>().npcPrefabList.Count));
        }
        else
        {
            prefabObject = Common.GetPrefabDatabase().GetComponent<PrefabsDatabaseManager>().GetApperance((int)npcApperance);
        }
    }
}


