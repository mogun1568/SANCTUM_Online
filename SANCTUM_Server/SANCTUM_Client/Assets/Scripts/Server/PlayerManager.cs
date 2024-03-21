using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static S_PlayerList;

public class PlayerManager
{
    MyPlayer _myPlayer;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        Object obj = Resources.Load("Prefabs/Environment/Map");
        if (obj == null)
        {
            Debug.Log(null);
        }

        foreach (S_PlayerList.Player p in packet.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;

            if (p.isSelf)
            {
                MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.PlayerId = p.playerId;
                _myPlayer = myPlayer;
            } else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                _players.Add(p.playerId, player);
            }
        }
    }

    public void Move(S_BroadcastMove packet)
    {
        if (_myPlayer.PlayerId == packet.playerId)
        {
            //_myPlayer.targetPosition = new Vector3(packet.posX, packet.posY, packet.posZ);
        } else
        {
            Player player = null;
            if (_players.TryGetValue(packet.playerId, out player))
            {
                //player.targetPosition = new Vector3(packet.posX, packet.posY, packet.posZ);
            }
        }
    }

    public void Map(S_BroadcastMap packet)
    {
        if (_myPlayer.PlayerId == packet.playerId)
        {
            
        }
        else
        {
            Player player = null;
            if (_players.TryGetValue(packet.playerId, out player))
            {
                
            }
        }
    }

    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _myPlayer.PlayerId)
        {
            return;
        }

        Object obj = Resources.Load("/Prefabs/Environment/Map");
        GameObject go = Object.Instantiate(obj) as GameObject;

        Player player = go.AddComponent<Player>();
        _players.Add(packet.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if (_myPlayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        } else
        {
            Player player = null;
            if (_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }
}
