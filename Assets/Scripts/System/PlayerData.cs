using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int hp;
    public int hpFull;
    public int coin;
    public int air;
    public int bullet;
    public int currentWeaponId;
    public int minAttack;
    public int maxAttack;
    public int stageNumber;
    public int stageDetailNumber;

    public float[] checkPoint = new float[2];

    public string weapons;

    public PlayerData()
    {
        // Start integer //
        hp = status.hp;
        hpFull = status.hpFull;
        coin = status.coin;
        air = status.air;
        bullet = status.bullet;
        currentWeaponId = status.currentWeaponId;
        minAttack = status.minAttack;
        maxAttack = status.maxAttack;
        stageNumber = status.stageNumber;
        stageDetailNumber = status.stageDetailNumber;
        // End integer //

        // Start float //
        checkPoint[0] = status.checkPoint.x;
        checkPoint[1] = status.checkPoint.y;
        // End float //

        // Start List //
        weapons= "";
        for(var i = 0; i < status.weapons.Count; i++)
        {
            weapons += status.weapons[i].id + ",";
        }

        // End List //
    }
}
