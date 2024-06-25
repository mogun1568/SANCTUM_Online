using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }		

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SGameStart, MakePacket<S_GameStart>);
		_handler.Add((ushort)MsgId.SGameStart, PacketHandler.S_GameStartHandler);		
		_onRecv.Add((ushort)MsgId.SCreateMap, MakePacket<S_CreateMap>);
		_handler.Add((ushort)MsgId.SCreateMap, PacketHandler.S_CreateMapHandler);		
		_onRecv.Add((ushort)MsgId.SLook, MakePacket<S_Look>);
		_handler.Add((ushort)MsgId.SLook, PacketHandler.S_LookHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SChangeStat, MakePacket<S_ChangeStat>);
		_handler.Add((ushort)MsgId.SChangeStat, PacketHandler.S_ChangeStatHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SInvenUpdate, MakePacket<S_InvenUpdate>);
		_handler.Add((ushort)MsgId.SInvenUpdate, PacketHandler.S_InvenUpdateHandler);		
		_onRecv.Add((ushort)MsgId.SExpUpdate, MakePacket<S_ExpUpdate>);
		_handler.Add((ushort)MsgId.SExpUpdate, PacketHandler.S_ExpUpdateHandler);		
		_onRecv.Add((ushort)MsgId.STurretUI, MakePacket<S_TurretUI>);
		_handler.Add((ushort)MsgId.STurretUI, PacketHandler.S_TurretUIHandler);		
		_onRecv.Add((ushort)MsgId.SFirstPersonMode, MakePacket<S_FirstPersonMode>);
		_handler.Add((ushort)MsgId.SFirstPersonMode, PacketHandler.S_FirstPersonModeHandler);		
		_onRecv.Add((ushort)MsgId.SRoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)MsgId.SRoomList, PacketHandler.S_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.SGameOver, MakePacket<S_GameOver>);
		_handler.Add((ushort)MsgId.SGameOver, PacketHandler.S_GameOverHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
		
		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}