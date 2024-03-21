using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainButtonMove : UI_Scene
{
    private Vector3 normal_position;
    private Vector3 normal_size;

    bool RunningScaleChange = false;

    void Start()
    {
        BindEvent(gameObject, (PointerEventData data) => { OnPointerEnter(); }, Define.UIEvent.Enter);
        BindEvent(gameObject, (PointerEventData data) => { OnPointerExit(); }, Define.UIEvent.Exit);
    }

    IEnumerator ButtonSizeUp()
    {
        if (RunningScaleChange == false)
        {
            normal_position = transform.position;
            normal_size = transform.localScale;
            RunningScaleChange = true;
        }
        while (true)
        {
            if (normal_position.x * 1.1f < transform.position.x)
                StopCoroutine("ButtonSizeUp");
            transform.position += new Vector3(transform.position.x * 0.3f * Time.deltaTime, 0, 0);
            transform.localScale += new Vector3(transform.localScale.x * 0.3f * Time.deltaTime, transform.localScale.y * 0.1f * Time.deltaTime, 0);
            if (normal_position.x * 1.1f < transform.position.x)
            {
                StopCoroutine("ButtonSizeUp");
            }
            yield return null;
        }
    }

    IEnumerator ButtonSizeDown()
    {
        while (true)
        {
            if (transform.localScale.x <= normal_size.x)
            {
                StopCoroutine("ButtonSizeDown");
                RunningScaleChange = false;
            }
            transform.position -= new Vector3(transform.position.x * 0.3f * Time.deltaTime, 0, 0);
            transform.localScale -= new Vector3(transform.localScale.x * 0.3f * Time.deltaTime, transform.localScale.y * 0.1f * Time.deltaTime, 0);
            yield return null;
        }
    }

    public void OnPointerEnter()
    {
        StopCoroutine("ButtonSizeDown");
        StartCoroutine("ButtonSizeUp");
    }

    public void OnPointerExit()
    {
        StopCoroutine("ButtonSizeUp");
        StartCoroutine("ButtonSizeDown");
    }
}