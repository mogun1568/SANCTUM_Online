using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerId {  get; set; }
    public Vector3 targetPosition;
    public Vector3 direction;

    void Start()
    {
        
    }

    void Update()
    {
        direction = targetPosition - transform.position;
        transform.Translate(direction.normalized * Time.deltaTime * 10.0f, Space.World);
    }
}
