using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecompositionObject : MonoBehaviour
{
    bool isCloneEnd = false;
    bool isLeft = false;
    int hitCount = 10;
    Vector3 scale;
    float mass;
    private void Awake()
    {
        mass = GetComponent<Rigidbody2D>().mass;
        if(this.gameObject.transform.localScale.x < 1)
            scale = this.gameObject.transform.lossyScale* 1.5f;
        else
            scale = this.gameObject.transform.lossyScale * 0.5f;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 26 && !isCloneEnd && gameObject.transform.localScale.x > 0.1f)
        {
            if (collision.transform.parent.position.x > transform.position.x)
                isLeft = false;
            else
                isLeft = true;

            CloneObject();
        }

        if (collision.gameObject.layer == 19)
        {
            if (hitCount <= 0)
            {
                CloneObject();
            }
            else
            {
                hitCount--;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, mass * 0.7f), ForceMode2D.Impulse);
                GameObject hitParticle = ObjectPool.Instance.PopFromPool("HitStoneParticleEffect");
                hitParticle.transform.position = transform.position;
                hitParticle.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer==26&&!isCloneEnd&&gameObject.transform.localScale.x > 0.1f)
        {
            if (collision.transform.parent.position.x > transform.position.x)
                isLeft = false;
            else
                isLeft = true;

            CloneObject();
        }

        if (collision.gameObject.layer == 19)
        {
            if (hitCount <= 0)
            {
                CloneObject();
            }
            else
            {
                hitCount--;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, mass * 0.7f), ForceMode2D.Impulse);
                GameObject hitParticle = ObjectPool.Instance.PopFromPool("HitStoneParticleEffect");
                hitParticle.transform.position = transform.position;
                hitParticle.SetActive(true);
            }
        }
    }
    public void CloneObject()
    {
        if(!isCloneEnd)
        {
            Sound_StoneCrack();
            StartCoroutine("CloningObject");
            isCloneEnd = true;
        }
    }
    public void Sound_StoneCrack()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.stoneCrack);
    }

    void CreateObject()
    {
        GameObject cloneObj = Instantiate(gameObject, gameObject.transform);
        Destroy(cloneObj.GetComponent<DecompositionObject>());
        cloneObj.transform.position = transform.position + new Vector3(Random.Range(-0.5f,0.5f), Random.Range(-0.5f, 0.5f),0);
        cloneObj.transform.localScale = scale;
        cloneObj.tag = "Untagged";
        cloneObj.gameObject.SetActive(true);
        Vector3 addForce = new Vector3(isLeft ? 200 : -200, 600, isLeft ? 200 : -200);
        cloneObj.GetComponent<Rigidbody2D>().AddForce(addForce, ForceMode2D.Impulse);
        StartCoroutine("DisappearObj");
    }

    public IEnumerator CloningObject()
    {
        gameObject.transform.localScale *= 0.5f;
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(isLeft ? 150 : -150, 600, isLeft ? 150 : -150), ForceMode2D.Impulse);
        int cnt = 0;
        while(cnt<2)
        {
            CreateObject();
            yield return new WaitForSeconds(0.05f);
            cnt++;
        }
        yield return null;
    }

    public IEnumerator DisappearObj()
    {
        Color thisColor = GetComponentInChildren<SpriteRenderer>().color;
        float cnt = 1;
        while(cnt>0.1f)
        {
            if(cnt<0.8f&&!this.transform.GetComponent<Collider2D>().isTrigger)
            {
                this.transform.GetComponent<Collider2D>().isTrigger = true;
            }

            foreach(var i in GetComponentsInChildren<SpriteRenderer>())
            {
                i.color = new Color(thisColor.r, thisColor.g, thisColor.b, cnt);
            }
            cnt -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var i in GetComponentsInChildren<SpriteRenderer>())
        {
            i.color = new Color(thisColor.r, thisColor.g, thisColor.b, 0);
        }
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
        yield return null;
    }
}
