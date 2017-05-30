using UnityEngine;
using System.Collections.Generic;
using System;

public struct RecvPacket
{
    public int m_recvPacketType;
    public byte[] m_recvPacket;

    public RecvPacket(NET_INGAME.RECV.PACKET_TYPE pakcetType, byte[] packet)
    {
        m_recvPacketType = (int)pakcetType;
        m_recvPacket = packet;
    }

    public RecvPacket(NET_OUTGAME.RECV.PACKET_TYPE pakcetType, byte[] packet)
    {
        m_recvPacketType = (int)pakcetType;
        m_recvPacket = packet;
    }

    public bool comparePacketType(NET_INGAME.RECV.PACKET_TYPE type)
    {
        if (m_recvPacketType == (int)type)
            return true;
        return false;
    }

    public bool comparePacketType(NET_OUTGAME.RECV.PACKET_TYPE type)
    {
        if (m_recvPacketType == (int)type)
            return true;
        return false;
    }

    public void reset()
    {
        m_recvPacket = null;
        m_recvPacketType = -1;
    }
}

public enum CreaturePacketIndexType
{
    CREATURE_PACKET_INDEX_TYPE_PLAYER_S = 0,
    CREATURE_PACKET_INDEX_TYPE_PLAYER_E = 1,
    CREATURE_PACKET_INDEX_TYPE_WAYPOINT_S = 100,
    CREATURE_PACKET_INDEX_TYPE_WAYPOINT_E = 500,
    CREATURE_PACKET_INDEX_TYPE_TROLL_S = 1000,
    CREATURE_PACKET_INDEX_TYPE_TROLL_E = 1100,
    CREATURE_PACKET_INDEX_TYPE_HIT_DAMAGE_S = 1000000,
    CREATURE_PACKET_INDEX_TYPE_HIT_DAMAGE_E = 1000000,
    CREATURE_PACKET_INDEX_TYPE_DIE = 2000000,
}
public abstract class PacketHandlingMgr
{
    private PacketCreateMgr m_createPacket;
    private PacketDivideMgr m_dividePacket;

    protected Queue<RecvPacket> m_recvPacketQue;

    protected delegate void DivideRecvPacket(byte[] pakcet);
    protected DivideRecvPacket divideRecvPacket;
    protected Dictionary<int, DivideRecvPacket> RecvPacketDictionary;

    public PacketHandlingMgr()
    {
        Debug.Log("PacketHandlingManager : create");
        m_recvPacketQue = new Queue<RecvPacket>();
        RecvPacketDictionary = new Dictionary<int, DivideRecvPacket>();
        SetRecvPacketDictionary();
    }

    protected void setPacketManager(PacketDivideMgr divideMgr, PacketCreateMgr createMgr)
    {
        m_dividePacket = divideMgr;
        m_createPacket = createMgr;
    }

    protected abstract void SetRecvPacketDictionary();
        

    public void setPacketParameter(object[] value)
    {
        m_createPacket.setParameter(value);
    }

    public void setPacketParameter_(object[] value)
    {
        m_createPacket.setParameter_(value);
    }

    public byte[] CreatePacket(NET_INGAME.SEND.PACKET_TYPE packetType)
    {
        return m_createPacket.getSendPacket((int)packetType);
    }

    public byte[] CreatePacket(NET_OUTGAME.SEND.PACKET_TYPE packetType)
    {
        return m_createPacket.getSendPacket((int)packetType);
    }

    public void DividePacket(byte[] _data)
    {
        try
        {
            int type = -1; //ref를 쓰기위한 초기화.
            if (_data == null)
            {
                Debug.Log("RecvPacket is Null Data");
                return;
            }
            m_dividePacket.DividePacketArray(ref type, _data);

            //패킷 타입에 따라 호출되는 함수를 지정.
            RecvPacketDictionary.TryGetValue((int)type, out divideRecvPacket);
            divideRecvPacket(_data);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log(e.Message);
            Debug.Log(e.ToString());
        }
    }

    public void DividePacket(RecvPacket packet)
    {

            RecvPacketDictionary.TryGetValue(packet.m_recvPacketType, out divideRecvPacket);
            //deligate
            if (divideRecvPacket != null)
            	divideRecvPacket(packet.m_recvPacket);
            else
            	Debug.Log("패킷 해제 에러 : None Regist Deligate Funcion");


	}
    public void resetPacketQueue() { m_recvPacketQue.Clear(); }

    public abstract void InsertPacketInQueue(byte[] packet);
    public abstract void analysePacket();
}
