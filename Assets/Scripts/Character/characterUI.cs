using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterUI : MonoBehaviour
{

    public GameObject pushUI;
    bool isPop = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isPop)
        {
            if(Common.PushedObject)
                pushUI.transform.position = Common.PushedObject.transform.position + new Vector3(0, 2, 100);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("pushed") && collision.transform.position.y + 0.75f > transform.position.y)
        {
            isPop = true;
            pushUI.transform.position = collision.transform.position + new Vector3(0, 2, 100);
            pushUI.gameObject.SetActive(true);
        }
    }
}
