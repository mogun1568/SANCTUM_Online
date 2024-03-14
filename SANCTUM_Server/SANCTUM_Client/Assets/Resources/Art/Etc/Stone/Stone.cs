using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public float rotSpeed;

    void Update()
    {
        float weight = 3.0f*Time.deltaTime;
        float rotateFrame = rotSpeed * Time.deltaTime;
        transform.Rotate(rotateFrame, rotateFrame* weight, rotateFrame*0.5f);
    }
}
