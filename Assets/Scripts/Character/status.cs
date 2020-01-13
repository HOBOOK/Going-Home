using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class status
{
    [SerializeField]
    public static bool isDead;
    public static bool isPlaying;
    public static bool isCtrl;
    public static bool isInBuilding;
    public static bool isLeftorRight;

    public static int hp;
    public static int hpFull;
    public static int coin;
    public static int air;
    public static int bullet;
    public static int currentWeaponId;
    public static int minAttack;
    public static int maxAttack;
    public static int stageNumber;
    public static int stageDetailNumber;

    public static List<Item> weapons = new List<Item>();
    public static Vector3 checkPoint;
    public static Common.WeaponType weaponType;
}
