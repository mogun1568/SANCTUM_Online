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
		_onRecv.Add((ushort)MsgId.CGameStart, MakePacket<C_GameStart>);
		_handler.Add((ushort)MsgId.CGameStart, PacketHandler.C_GameStartHandler);		
		_onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.CInvenUpdate, MakePacket<C_InvenUpdate>);
		_handler.Add((ushort)MsgId.CInvenUpdate, PacketHandler.C_InvenUpdateHandler);		
		_onRecv.Add((ushort)MsgId.CLevelUp, MakePacket<C_LevelUp>);
		_handler.Add((ushort)MsgId.CLevelUp, PacketHandler.C_LevelUpHandler);		
		_onRecv.Add((ushort)MsgId.CTurretUI, MakePacket<C_TurretUI>);
		_handler.Add((ushort)MsgId.CTurretUI, PacketHandler.C_TurretUIHandler);		
		_onRecv.Add((ushort)MsgId.CTurretDemolite, MakePacket<C_TurretDemolite>);
		_handler.Add((ushort)MsgId.CTurretDemolite, PacketHandler.C_TurretDemoliteHandler);		
		_onRecv.Add((ushort)MsgId.CFirstPersonMode, MakePacket<C_FirstPersonMode>);
		_handler.Add((ushort)MsgId.CFirstPersonMode, PacketHandler.C_FirstPersonModeHandler);		
		_onRecv.Add((ushort)MsgId.CShoot, MakePacket<C_Shoot>);
		_handler.Add((ushort)MsgId.CShoot, PacketHandler.C_ShootHandler);		
		_onRecv.Add((ushort)MsgId.CEnterRoom, MakePacket<C_EnterRoom>);
		_handler.Add((ushort)MsgId.CEnterRoom, PacketHandler.C_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.CLeaveRoom, MakePacket<C_LeaveRoom>);
		_handler.Add((ushort)MsgId.CLeaveRoom, PacketHandler.C_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.CRoomList, MakePacket<C_RoomList>);
		_handler.Add((ushort)MsgId.CRoomList, PacketHandler.C_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.CPause, MakePacket<C_Pause>);
		_handler.Add((ushort)MsgId.CPause, PacketHandler.C_PauseHandler);
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