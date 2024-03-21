using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;
    NewMap _map;

    void Start()
    {        
        _network = GameObject.Find("GameMaster").GetComponent<NetworkManager>();
        _map = GetComponent<NewMap>();
        //_map.playerId = PlayerId;
        _map.Init();
        //StartCoroutine("CoSendPacket");
    }

    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            C_Move movePacket = new C_Move();
           
            _network.Send(movePacket.Write());
        }
    }

    void SendPacket()
    {
        C_Move movePacket = new C_Move();
        
        _network.Send(movePacket.Write());
    }
}
