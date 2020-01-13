using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearEffect : MonoBehaviour
{
    public void DisapearEffect()
    {
        StartCoroutine("DisappearSprite");
    }
    IEnumerator DisappearSprite()
    {
        yield return new WaitForSeconds(1.0f);
        if (GetComponentsInChildren<ParticleSystem>() != null)
        {
            foreach(var i in GetComponentsInChildren<ParticleSystem>())
            {
                i.GetComponentInChildren<ParticleSystem>().Play();
            }
        }
        yield return new WaitForSeconds(2.0f);
        var mask = GetComponentInChildren<SpriteMask>().gameObject;
        Vector3 pos = Vector3.zero;
        while (pos.y <= 2)
        {
            pos.y += 0.05f;
            mask.transform.localPosition = pos;
            yield return new WaitForSeconds(0.05f);
        }
        pos.y = 2;
        mask.transform.localPosition = pos;
        yield return new WaitForSeconds(5f);
        this.gameObject.SetActive(false);
        yield return null;

    }
}
