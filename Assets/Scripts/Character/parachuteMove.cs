using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parachuteMove : MonoBehaviour {

    Vector3 moveVelocity;
    Vector3 pos;
    GameObject parachuteLimb;
    GameObject parachuteBone;
    public bool isCtrl = false;
    public float movePower = 2;
    float moveAccPower = 0.3f;
    float moveDefaultPower = 0.75f;
    float keyInputTime = 0.0f;

    void Start () {
        if(parachuteBone==null)
        {
            parachuteBone = transform.GetChild(0).gameObject;
        }
        if(parachuteLimb==null)
        {
            parachuteLimb = transform.GetChild(1).gameObject;
        }
	}

    void Update()
    {
        Move();
    }

    void Move()
    {
        if(isCtrl)
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                keyInputTime = keyInputTime < -1 ? -1 : keyInputTime - Time.deltaTime;
                moveVelocity = Vector3.left;
                if (Common.isCameraOut && Common.restricCamera == Common.RestricCameraType.restricLeft)
                    return;
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                keyInputTime = keyInputTime > 1 ? 1 : keyInputTime + Time.deltaTime;
                moveVelocity = Vector3.right;
                if (Common.isCameraOut && Common.restricCamera == Common.RestricCameraType.restricRight)
                    return;
            }
            else
            {
                keyInputTime = keyInputTime > 0 ? keyInputTime - Time.deltaTime : keyInputTime < 0 ? keyInputTime + Time.deltaTime : 0;
                parachuteBone.transform.localPosition = new Vector2(keyInputTime, 0);
                moveAccPower = Common.IncrementOrDecrementTowards(moveAccPower, moveDefaultPower, 1f, true);
                if (moveAccPower != moveDefaultPower)
                    transform.position += moveVelocity * moveAccPower * Time.deltaTime;
                return;
            }
            parachuteBone.transform.localPosition = new Vector2(keyInputTime, 0);

            moveAccPower = Common.IncrementOrDecrementTowards(moveAccPower, movePower, 2f);

            pos = this.transform.position;
            pos += (moveVelocity * moveAccPower) * Time.deltaTime;
            this.transform.position = pos;
        }
        else
        {
            this.transform.position += (Vector3.left * moveAccPower) * Time.deltaTime;
            parachuteBone.transform.localPosition = new Vector2(-1, 0);
        }

    }
}
