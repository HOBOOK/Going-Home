using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabsDatabaseManager : MonoBehaviour
{
    public List<GameObject> npcPrefabList = new List<GameObject>();
    public List<GameObject> weaponPrefabList = new List<GameObject>();
    public List<GameObject> itemPrefabList = new List<GameObject>();
    public List<GameObject> objectPrefabList = new List<GameObject>();
    public List<GameObject> effectPrefabList = new List<GameObject>();
    public GameObject chatBox;

    public static PrefabsDatabaseManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    public GameObject GetWeapon(int i)
    {
        GameObject weapon = weapon = weaponPrefabList.Find(item => item.GetComponent<ItemPickController>().itemNumber == i);
        return weapon;
    }


    public GameObject GetApperance(int i)
    {
        GameObject apperance = npcPrefabList[i] as GameObject;
        return apperance;
    }

    public GameObject GetChatBox()
    {
        return chatBox;
    }

    public GameObject GetEffectPrefab(int i)
    {
        GameObject effect = effectPrefabList[i] as GameObject;
        return effect;
    }
}
