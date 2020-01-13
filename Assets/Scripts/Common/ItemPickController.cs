using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickController : MonoBehaviour
{
    GameObject playerEquipPoint;
    public GameObject pickUI;
    bool isHaveItem = false;
    GameObject pickobj;

    public int itemType; // 0:weapon, 1:backpack, 2:symbol, 3:hat
    public bool isWeapon = false;
    public int itemNumber;
    bool isPickable = false;
    
    string[] FIND_TAG_BY_TYPE_OF_ITEM = 
    {
        "EquipWeaponPoint",
        "EquipBackPoint",
        "EquipSymbolPoint",
        "EquipHatPoint"
    };

	void Awake ()
    {
        playerEquipPoint = GameObject.FindGameObjectWithTag(FIND_TAG_BY_TYPE_OF_ITEM[itemType]);
        pickobj = pickUI ? Instantiate(pickUI, GameObject.Find("CanvasUI").transform) as GameObject : null;

    }

	void Update ()
    {
		if(isPickable && Input.GetAxisRaw("Vertical") == 1)
        {
            SoundManager.instance.RandomizeSfx(AudioClipManager.instance.pickup);

            foreach (var i in status.weapons)
            {
                if (itemNumber == i.id)
                {
                    isHaveItem = true;
                    break;
                }
                else
                    isHaveItem = false;
            }
            if(!isHaveItem)
            {
                transform.SetParent(playerEquipPoint.transform);
                transform.GetComponent<Rigidbody2D>().isKinematic = true;
                transform.GetComponent<PolygonCollider2D>().enabled = false;
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
                if (transform.GetComponent<BoxCollider2D>())
                    transform.GetComponent<BoxCollider2D>().enabled = false;
                transform.localPosition = new Vector3(0, 0, 0);
                transform.rotation = new Quaternion(0, 0, 0, 0);

                Common.HERO().GetComponent<Animator>().SetTrigger("picking");

                if (isWeapon)
                {
                    AutoEquip(itemNumber);
                    status.weapons.Add(ItemSystem.GetItem(itemNumber));
                    SaveSystem.SavePlayer();
                }
                gameObject.GetComponent<ItemPickController>().enabled = false;
            }
            else
            {
                Destroy(this.gameObject);
            }
            GUI_Manager.instance.ToolTipOn(string.Format("I got a {0}", this.name));

            isPickable = false;

            SetPickUI(false);
        }
	}

    public void AutoEquip(int itemNumber)
    {
        Common.HERO().GetComponent<move>().EquipWeapon(itemNumber);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPickable = true;
            SetPickUI(true);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 0 || collision.gameObject.layer == 4 || collision.gameObject.layer == 22)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            transform.GetComponent<PolygonCollider2D>().enabled = false;
        }
        if (collision.CompareTag("Player"))
        {
            isPickable = true;
            SetPickUI(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPickable = false;
        }
    }

    void SetPickUI(bool isOn)
    {
        if(isOn)
        {
            pickobj.transform.position = transform.position + new Vector3(0, 1.5f, 0);
            pickobj.SetActive(true);
        }
        else
        {
            Destroy(pickobj);
        }
    }

}
