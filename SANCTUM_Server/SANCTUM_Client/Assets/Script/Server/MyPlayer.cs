using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;
    //public Vector3 targetPosition;
    //Vector3 direction;

    void Start()
    {        
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //StartCoroutine("CoSendPacket");
    }

    void Update()
    {
        if (direction.magnitude < 0.1f)
        {
            targetPosition = new Vector3(UnityEngine.Random.Range(-50, 50), 0, UnityEngine.Random.Range(-50, 50));
            SendPacket();
            Debug.Log(targetPosition);
        }

        direction = targetPosition - transform.position;
        transform.Translate(direction.normalized * Time.deltaTime * 10.0f, Space.World);
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            C_Move movePacket = new C_Move();
            movePacket.posX = targetPosition.x;
            movePacket.posY = targetPosition.x;
            movePacket.posZ = targetPosition.x;
            _network.Send(movePacket.Write());
        }
    }

    void SendPacket()
    {
        C_Move movePacket = new C_Move();
        movePacket.posX = targetPosition.x;
        movePacket.posY = targetPosition.y;
        movePacket.posZ = targetPosition.z;
        _network.Send(movePacket.Write());
    }
}
