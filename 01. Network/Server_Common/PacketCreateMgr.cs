using System.Text;
using System;
using System.Collections.Generic;

public abstract class PacketCreateMgr
{
    protected delegate byte[] MakeSendPacket();
    protected Dictionary<int, MakeSendPacket> SendPacketDictionary;
    protected object[] m_parameter;
    private MakeSendPacket makeSendPacket;

    protected object[] m_parameter_;

    protected void init()
    {
        SendPacketDictionary = new Dictionary<int, MakeSendPacket>();
        setSendPacketDictionary();
    }

    //Dictionary에 enum값에 따른 호출 함수를 저장.
    protected abstract void setSendPacketDictionary();

    //보낼 패킷에 새로운 매개변수가 필요할 경우 미리 설정함.
    public void setParameter(object[] val)
    {
        m_parameter = val;
    }

    public void setParameter_(object[] val)
    {
        m_parameter_ = val;
    }

    //매개변수로 넘어온 enum값에 따라 보낼 패킷을 만들어서 반환.
    public byte[] getSendPacket(int packetType)
    {
        if (SendPacketDictionary.TryGetValue(packetType, out makeSendPacket))
        {
            byte[] packet = makeSendPacket();

            return packet;
        }
        return null;
    }

    protected void MakeByteArray(ref byte[] _packet, NET_INGAME.SEND.PACKET_TYPE _data, ref int _curToken)
    {
        byte[] temp = BitConverter.GetBytes((int)_data);
        Array.Copy(temp, 0, _packet, _curToken, sizeof(int));
        _curToken += sizeof(int);
    }

    protected void MakeByteArray(ref byte[] _packet, NET_OUTGAME.SEND.PACKET_TYPE _data, ref int _curToken)
    {
        byte[] temp = BitConverter.GetBytes((int)_data);
        Array.Copy(temp, 0, _packet, _curToken, sizeof(int));
        _curToken += sizeof(int);
    }

    protected void MakeByteArray(ref byte[] _packet, int _data, ref int _curToken)
    {
        byte[] temp = BitConverter.GetBytes(_data);
        Array.Copy(temp, 0, _packet, _curToken, sizeof(int));
        _curToken += sizeof(int);
    }
    protected void MakeByteArray(ref byte[] _packet, float _data, ref int _curToken)
    {
        byte[] temp = BitConverter.GetBytes(_data);
        Array.Copy(temp, 0, _packet, _curToken, sizeof(float));
        _curToken += sizeof(float);
    }
    protected void MakeByteArray(ref byte[] _packet, bool _data, ref int _curToken)
    {
        byte[] temp = BitConverter.GetBytes(_data);
        Array.Copy(temp, 0, _packet, _curToken, sizeof(bool));
        _curToken += sizeof(bool);
    }
    protected void MakeByteArray(ref byte[] _packet, string _data, ref int _curToken)
    {
        byte[] temp = BitConverter.GetBytes(_data.Length);
        Array.Copy(temp, 0, _packet, _curToken, sizeof(int));
        _curToken += sizeof(int);
        temp = Encoding.ASCII.GetBytes(_data);
        Array.Copy(temp, 0, _packet, _curToken, _data.Length);
        _curToken += _data.Length;
    }

    /*
    protected byte[] MakeEmptyPacket(SendPacketType _type)
    {
        byte[] _data;
        _data = BitConverter.GetBytes((int)_type);
        return _data;
    }*/
}
