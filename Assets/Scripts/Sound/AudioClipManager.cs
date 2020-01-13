using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    public AudioClip coin;
    public AudioClip reloadPistol;
    public AudioClip shootPistol;
    public AudioClip swingSword;
    public AudioClip swingKnife;
    public AudioClip punchHit;
    public AudioClip jump;
    public AudioClip heartBeat;
    public AudioClip knife1;
    public AudioClip knife2;
    public AudioClip damage1;
    public AudioClip damage2;
    public AudioClip pickup;
    public AudioClip equip;
    public AudioClip stoneCrack;
    public AudioClip stoneRolling;
    public AudioClip rumble;
    public AudioClip grasscut;
    public AudioClip dead;


    public AudioClip Bgm1;
    public AudioClip Bgm2;

    public static AudioClipManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
