using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{

    public string poolItemName = "Bullet";
    public float lifeTime = 0.5f;
    public float _elapsedTime = 0f;
    public float speed;
    public Transform Target;
    private Vector3 targetPos;
    private Vector3 thisPos;
    private float angle;
    public float offset;
    bool isReset = false;
    private void Update()
    {
        if (Target&&!isReset)
        {
            isReset = true;
            targetPos = Target.position;
            thisPos = transform.position;
            targetPos.x = targetPos.x - thisPos.x;
            targetPos.y = targetPos.y - thisPos.y;
            angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + offset));
        }
        transform.position += transform.right * speed * Time.deltaTime;
        if(GetTimer() > lifeTime)
        {
            SetTimer();
            isReset = false;
            ObjectPool.Instance.PushToPool(poolItemName, gameObject);
        }
    }
    float GetTimer()
    {
        return (_elapsedTime += Time.deltaTime);
    }
    void SetTimer()
    {
        _elapsedTime = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectPool.Instance.PushToPool(poolItemName, gameObject);
        TriggerEffet(poolItemName);
        SetTimer();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ObjectPool.Instance.PushToPool(poolItemName, gameObject);
        TriggerEffet(poolItemName);
        SetTimer();
    }

    private void TriggerEffet(string strPoolItemName)
    {
        if(strPoolItemName.Equals("Bullet"))
        {

            GameObject bulletEffect = ObjectPool.Instance.PopFromPool("BulletEffect");
            bulletEffect.transform.position = this.transform.position;
            bulletEffect.SetActive(true);
            GameObject effect = ObjectPool.Instance.PopFromPool("Arrow_Hit");
            effect.transform.position = this.transform.position;
            effect.SetActive(true);
        }
        else if (strPoolItemName.Equals("Knife"))
        {

            GameObject effect = ObjectPool.Instance.PopFromPool("Arrow_Hit");
            effect.transform.position = this.transform.position;
            effect.SetActive(true);
        }
            
    }

}
