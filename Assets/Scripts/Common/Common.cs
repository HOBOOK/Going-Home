using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Common : MonoBehaviour
{
    public static Common instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    public static GameObject GetPrefabDatabase()
    {
        return GameObject.Find("PrefabDatabase").gameObject;
    }
    public static GameObject GetCameraObject()
    {
        return GameObject.Find("Main Camera").gameObject;
    }
    public static GameObject GetBackgroundObject()
    {
        return GameObject.Find("background").gameObject;
    }
    public static GameObject HERO()
    {
        return GameObject.Find("Hero").gameObject;
    }
    public static GameObject GIRL()
    {
        return GameObject.Find("Girl").gameObject;
    }
    public static GameObject CAR()
    {
        return GameObject.Find("Car").gameObject;
    }
    public static GameObject EffectSound()
    {
        return GameObject.FindGameObjectWithTag("EffectSound");
    }
    public static GameObject PushedObject = null;

    public static bool isDataLoadSuccess = false;

    public enum WeaponType
    {
        no = 0,
        gun = 1,
        sword = 2,
        knife = 3
    };

    public enum NPC_APPERANCE
    {
        Npc001,
        Npc002,
        Npc003,
        Npc004,
        Npc005,
        Npc006,
        Npc007,
        Npc008,
        Npc009,
        Npc010,
        Npc011,
        Npc012,
        Npc013,
        Monster002,
        Random
    };
    public enum PlayingEventType
    {
        None,
        LookUp,
        LookLetter,
        Notebook,
        Chat
    }

    public enum EventType
    {
        CameraShake,
        CameraSlow,
        CameraBlack,
        CameraRestric,
        CameraSizing,
        DisableObject,
        TimeLineEvent,
        StageClear,
        None
    }


    public static RestricCameraType restricCamera;
    public enum RestricCameraType
    {
        off = -1,
        restricLeft,
        restricRight
    }
    public static bool isCameraOut;

    public static float cameraOrthSize;


    public static void CameraAllEffectOff()
    {
        isCameraOut = false;
        isShake = false;
        isBlackUpDown = false;
        isHitShake = false;
        Camera.main.GetComponent<FollowCamera>().isBound = false;
    }

    public static float restricX;

    public static string hitTargetName;

    public static GameObject hitTargetObject()
    {
        return GameObject.Find(hitTargetName);
    }

    public static bool isAwake = false;

    public static bool isStart;

    public static bool isShake;

    public static bool isHitShake;

    public static bool isNight;

    public static bool isBlackUpDown;


    public static int looMinus(int hp, int amount)
    {
        if (hp - amount <= 0)
            return 0;
        else
            return hp - amount;
    }

    public static int looHpPlus(int hp, int hpFull, int amount)
    {
        if (hp + amount >= hpFull*0.9f && hp < hpFull*0.9f)
            return (int)(hpFull*0.9);
        else if (hp + amount >= hpFull)
            return hpFull;
        else
            return hp + amount;
    }
    public static void RELOAD_GAME()
    {
        Common.isStart = false;
        Common.isAwake = false;
    }

    public static void START_GAME()
    {
        if (!Common.isStart)
        {
            Common.isStart = true;
            Common.isAwake = false;
        }

    }

    public static void AWAKE_GAME()
    {
        if (!Common.isAwake)
        {
            Common.triggerObjectInitialize = true;
            Common.isAwake = true;
        }

    }

    public static void SetPlayingStatus(bool isSet)
    {
        status.isPlaying = isSet;
    }

    public static void DisableObject(GameObject obj)
    {
        if(obj.GetComponent<Animator>())
        {
            obj.GetComponent<Animator>().SetTrigger("Disable");
        }
    }

    public static bool triggerObjectInitialize;

    public static void SetLayerRecursively(GameObject obj, int newLayer, int defaultLayer)
    {
        if (null == obj)
        {
            return;
        }
        obj.layer = obj.layer == defaultLayer ? newLayer : obj.layer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer, defaultLayer);
        }
    }

    public static float IncrementOrDecrementTowards(float n, float target, float a, bool isDecrement = false)
    {
        if (n == target)
        {
            return n;
        }
        else
        {
            float dir = Mathf.Sign(target - n);
            n += a * Time.deltaTime * dir;
            if (isDecrement)
                return (dir == Mathf.Sign(target + n)) ? n : target;
            else
                return (dir == Mathf.Sign(target - n)) ? n : target;
        }
    }

    public static void Chat(string chat, Transform tran = null, int posY = 0)
    {
        if (tran == null)
            tran = HERO().transform;

        if(tran.GetComponentInChildren<faceOff>()!=null)
        {
            tran.GetComponentInChildren<faceOff>().Face_Mouse();
        }
        GameObject chatObj = ObjectPool.Instance.PopFromPool("chatBox");
        chatObj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        posY = posY == 0 ? 1 : posY;
        if (tran.rotation.y != 0)
        {
            chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

        }
        else
        {
            chatObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            chatObj.GetComponentInChildren<Text>().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        }
        chatObj.GetComponent<UI_chatBox>().correctionY = posY;
        chatObj.GetComponent<UI_chatBox>().Target = tran;
        chatObj.GetComponent<UI_chatBox>().chatText = chat;
        chatObj.SetActive(true);
    }

    //배열 중복제거
    public static T[] GetDistinctValues<T>(T[] array)
    {

        List<T> tmp = new List<T>();

        for (int i = 0; i < array.Length; i++)
        {
            if (tmp.Contains(array[i]))
                continue;
            tmp.Add(array[i]);
        }

        return tmp.ToArray();
    }

    //오브젝트 바닥위치
    public static Vector3 GetBottomPosition(Transform transform)
    {
        Vector3 v3Pos = new Vector3(0.0f, -0.5f, 0.0f) * transform.localScale.y;
        v3Pos = transform.TransformPoint(v3Pos);
        return v3Pos;
    }
}
