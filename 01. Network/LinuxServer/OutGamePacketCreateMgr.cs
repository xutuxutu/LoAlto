using UnityEngine;

public class OutGamePacketCreateMgr : PacketCreateMgr
{
    public OutGamePacketCreateMgr()
    {
        init();
    }

    protected override void setSendPacketDictionary()
    {
        //connect
        SendPacketDictionary.Add((int)NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_ID_EM, SEND_PLAYER_ID_INFO_FC);
        SendPacketDictionary.Add((int)NET_OUTGAME.SEND.PACKET_TYPE.SEND_CHARACTER_SELECT_INFO_EM, SEND_CHARACTER_SELECT_INFO_FC);
        SendPacketDictionary.Add((int)NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_READY_EM, SEND_PLAYER_READY_FC);
        SendPacketDictionary.Add((int)NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_EXIT_ROOM_EM, SEND_PLAYER_ROOM_EXIT_FC);
    }
    
    public byte[] SEND_PLAYER_ID_INFO_FC()
    {
        
        byte[] data = new byte[NET_OUTGAME.SEND.PACKET_SIZE.SEND_PLAYER_ID];
        int curToken = 0;

        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_ID_EM, ref curToken);
        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_SIZE.SEND_PLAYER_ID, ref curToken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curToken);

        return data;
    }

    public byte[] SEND_CHARACTER_SELECT_INFO_FC()
    {
        byte[] data = new byte[NET_OUTGAME.SEND.PACKET_SIZE.SEND_CHARACTER_SELECT_INFO];
        int curToken = 0;

        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_TYPE.SEND_CHARACTER_SELECT_INFO_EM, ref curToken);
        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_SIZE.SEND_CHARACTER_SELECT_INFO, ref curToken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curToken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curToken);

        return data;
    }

    public byte[] SEND_PLAYER_READY_FC()
    {
        byte[] data = new byte[NET_OUTGAME.SEND.PACKET_SIZE.SEND_PLAYER_READY];
        int curToken = 0;

        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_READY_EM, ref curToken);
        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_SIZE.SEND_PLAYER_READY, ref curToken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curToken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curToken);

        return data;
    }

    public byte[] SEND_PLAYER_ROOM_EXIT_FC()
    {
        byte[] data = new byte[NET_OUTGAME.SEND.PACKET_SIZE.SEND_PLAYER_EXIT_ROOM];
        int curToken = 0;

        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_TYPE.SEND_PLAYER_EXIT_ROOM_EM, ref curToken);
        MakeByteArray(ref data, NET_OUTGAME.SEND.PACKET_SIZE.SEND_PLAYER_EXIT_ROOM, ref curToken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curToken);

        return data;
    }
}
