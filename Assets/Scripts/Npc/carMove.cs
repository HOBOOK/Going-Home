using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carMove : MonoBehaviour
{
    public GameObject boom;
    private bool isBoom = false;
    public int hitCount = 10;
    public Sprite boomAfterSprite;
    public Sprite CarDefaultSprite;
    public Sprite CarOpenSprite;
    public GameObject Door;
    public GameObject SmokeEffect;

    private void Awake()
    {
        CarDefaultSprite = this.GetComponent<SpriteRenderer>().sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer==19)
        {
            if (!isBoom)
            {
                hitCount--;
                if(hitCount <= 5 && !SmokeEffect.activeSelf)
                {
                    SmokeEffect.SetActive(true);
                }
                if (hitCount <= 0)
                {
                    boom.SetActive(true);
                    boom.transform.GetComponent<Boom>().isBoom = true;
                    GetComponent<Animator>().enabled = false;
                    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(-5, 80), ForceMode2D.Impulse);
                    GetComponent<SpriteRenderer>().sprite = boomAfterSprite;
                    isBoom = true;
                }
            }
        }
    }
    public void DoorOpen()
    {
        if(Door && CarOpenSprite)
        {
            this.GetComponent<SpriteRenderer>().sprite = CarOpenSprite;
            Door.gameObject.SetActive(true);
        }
    }

    public void DoorClose()
    {
        this.GetComponent<SpriteRenderer>().sprite = CarDefaultSprite;
        Door.gameObject.SetActive(false);
    }

    public void Chat(string chat)
    {
        Common.Chat(chat, transform);
    }
}
