using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPointSave : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(this.transform.position.x > status.checkPoint.x)
            {
                status.checkPoint = transform.position;
                Debugging.Log("체크포인트 저장! >> " + transform.position);
                SaveSystem.SavePlayer();
            }
            this.gameObject.SetActive(false);
        }
    }
}
