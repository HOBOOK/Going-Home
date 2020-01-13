using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponInterface : MonoBehaviour
{
    public int nowWeapon;
    private void Start()
    {
        nowWeapon = status.currentWeaponId;
        for(var i =0; i< status.weapons.Count; i++)
        {
            EquipWeaponPrefab(status.weapons[i].id);
        }
    }
    private void FixedUpdate()
    {
        if(nowWeapon!=(int)status.currentWeaponId)
        {
            var cnt = 0;
            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<ItemPickController>().itemNumber == status.currentWeaponId)
                {
                    nowWeapon = (int)status.currentWeaponId;
                    transform.GetChild(i).gameObject.SetActive(true);
                    cnt++;
                }
                else
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            if(cnt==0)
            {
                nowWeapon = 1000;
            }
        }
    }

    private void EquipWeaponPrefab(int weaponNumber)
    {
        GameObject weapon = Instantiate(Common.GetPrefabDatabase().GetComponent<PrefabsDatabaseManager>().GetWeapon(weaponNumber), GameObject.FindGameObjectWithTag("EquipWeaponPoint").transform) as GameObject;
        if(weapon!=null)
        {
            if(weapon.transform.GetComponent<Rigidbody2D>()!=null)
            {
                weapon.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                weapon.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
                weapon.transform.GetComponent<Rigidbody2D>().isKinematic = true;
            }
            if(weapon.transform.GetComponent<PolygonCollider2D>()!=null)
                weapon.transform.GetComponent<PolygonCollider2D>().enabled = false;

            if (weapon.transform.GetComponent<BoxCollider2D>())
                weapon.transform.GetComponent<BoxCollider2D>().enabled = false;

            weapon.transform.localPosition = new Vector3(0, 0, 0);
            weapon.transform.rotation = new Quaternion(0, 0, 0, 0);
            if (weaponNumber == status.currentWeaponId)
                weapon.SetActive(true);
            else
                weapon.SetActive(false);
            Debugging.Log(weaponNumber + " 무기 소지품에 등록완료.");
        }
        else
        {
            Debugging.Log(weaponNumber + " 무기 소지품에 등록실패.");
        }
    }
}
