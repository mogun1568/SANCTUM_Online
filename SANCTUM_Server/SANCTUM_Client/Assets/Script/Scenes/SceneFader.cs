using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;

    public Setting setting;

    public bool isFading;

    public void Init()
    {
        //img = GetComponentInChildren<Image>();
        img = Util.FindChild<Image>(gameObject, "Black", true);

        StopAllCoroutines();
        //setting.IntialSetting();
        StartCoroutine(FadeIn());
    }

    public void FadeTo(Define.Scene type)
    {
        isFading = true;
        StartCoroutine(FadeOut(type));
    }

    IEnumerator FadeIn()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        isFading = false;
    }

    IEnumerator FadeOut(Define.Scene type)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        //GameManager.instance.soundManager.Clear();
        //setting.ChangeScene();
        Cursor.lockState = CursorLockMode.None;
        Managers.Scene.LoadScene(type); 
    }
}
