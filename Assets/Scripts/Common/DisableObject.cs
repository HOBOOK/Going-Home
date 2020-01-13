using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour {

    // 오브젝트 제거
    public void SetDisable(GameObject obj)
    {
        if (obj == null)
            obj = this.gameObject;
        StartCoroutine(transparentSprite(obj));
    }

    public IEnumerator transparentSprite(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        var sprites = obj.GetComponentsInChildren<SpriteRenderer>();

        float alpha = 1.0f;
        while (alpha >= 0)
        {
            foreach (var a in sprites)
            {
                if (!a.name.Contains("hair"))
                    a.color = new Color(1, 1, 1, alpha);
            }

            alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var a in sprites)
            a.color = new Color(1, 1, 1, 0);
        obj.gameObject.SetActive(false);
        yield return null;

    }
}
