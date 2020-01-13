using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

public static class SaveSystem
{
    public static void SavePlayer()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();

        formatter.Serialize(stream, data);
        stream.Close();

        Debugging.LogSystem("File is saved in Successfully.");
    }

    public static void LoadPlayer()
    {
        Common.isDataLoadSuccess = false;
        int loadTryCount = 0;
        string path = Application.persistentDataPath + "/player.fun";
        PlayerData data = null;
        while(loadTryCount<3)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();
                break;
            }
            else
            {
                loadTryCount++;
            }
        }
        if(data!=null)
        {
            status.isDead = false;
            status.isPlaying = false;
            status.isCtrl = false;
            status.isInBuilding = false;

            status.hp = data.hp;
            status.hpFull = data.hpFull;
            status.coin = data.coin;
            status.air = data.air;
            status.bullet = data.bullet;
            status.currentWeaponId = data.currentWeaponId;
            status.minAttack = data.minAttack;
            status.maxAttack = data.maxAttack;
            status.stageNumber = data.stageNumber;
            status.stageDetailNumber = data.stageDetailNumber;
            status.checkPoint = new Vector3(data.checkPoint[0], data.checkPoint[1]);
            string[] str = data.weapons.Split(',');
            str = Common.GetDistinctValues<string>(str);
            foreach(var weapon in str)
            {
                if(!string.IsNullOrEmpty(weapon))
                {
                    int weaponNumber = Convert.ToInt32(weapon);
                    Item item = ItemSystem.GetItem(weaponNumber);
                    if(item!=null)
                        status.weapons.Add(item);
                }
            }
            Debugging.LogSystem("File is loaded Successfully >> Try : " + loadTryCount + "\r\n" + JsonUtility.ToJson(data));
        }
        else
        {
            Debugging.LogSystemWarning("Save file not fount in " + path);
            InitPlayer();
        }
        Common.isDataLoadSuccess = true;
    }

    public static void InitPlayer()
    {
        status.hpFull = 5;
        status.hp = 5;
        status.air = 100;
        status.bullet = 7;
        status.currentWeaponId = 1000;
        status.weaponType = Common.WeaponType.no;
        status.minAttack = 5;
        status.maxAttack = 10;
        status.checkPoint = new Vector3(-18.5f, 0.58f);
        status.weapons = new List<Item>();
        status.weapons.Add(ItemSystem.GetItem(1000));
        Debugging.LogSystem("Init Player");
    }
}
