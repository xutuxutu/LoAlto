using System;
using System.Text;

public class PacketDivideMgr
{
    // 문자열을 패킷에서 분리 해 리턴하는 함수
    public void DividePacketArray(ref string data, byte[] packet, ref int curToken)
    {
        int size = BitConverter.ToInt32(packet, curToken);
        // 바이트에 같이 온 문자열의 길이를 분리 한다.
        curToken += sizeof(int);
        // 데이터의 경계를 바꾼다.
        byte[] temp = new byte[size];
        Array.Copy(packet, curToken, temp, 0, size);
        // 문자열 구간을 temp 변수에 넣는다.
        data = Encoding.ASCII.GetString(temp);
        // 바이트 배열을 문자열로 변환한다. 한글이 올 수도 있어서 유니코드로 변환한다.
        curToken += size;
        // 데이터의 경계를 바꾼다.
    }

    // 정수를 패킷에서 분리 해 리턴하는 함수
    public void DividePacketArray(ref int data, byte[] packet, ref int curToken)
    {
        data = BitConverter.ToInt32(packet, curToken);
        // 바이트 배열을 정수로 변환한다.
        curToken += sizeof(int);
        // 데이터의 경계를 바꾼다.
    }

    public void DividePacketArray(ref float data, byte[] packet, ref int curToken)
    {
        data = BitConverter.ToSingle(packet, curToken);
        // 바이트 배열을 정수로 변환한다.
        curToken += sizeof(float);
        // 데이터의 경계를 바꾼다.
    }

    // 불 값을 패킷에서 분리 해 리턴하는 함수
    public void DividePacketArray(ref bool data, byte[] packet, ref int curToken)
    {
        data = BitConverter.ToBoolean(packet, curToken);
        // 바이트 배열을 불 값으로 변환한다.
        curToken += sizeof(bool);
        // 데이터 경계를 바꾼다.
    }

    // 패킷의 종류를 구분하기 위한 함수.
    public void DividePacketArray(ref int data, byte[] packet)
    {
        data = BitConverter.ToInt32(packet, 0);
        // 바이트 배열을 패킷의 종류를 구분하기 위한 함수로 바꾼다.
    }

    public void DividePacketArray(ref int type, ref int size, byte[] packet)
    {
        type = BitConverter.ToInt32(packet, 0);
        size = BitConverter.ToInt32(packet, sizeof(int));
    }

    public void DividePacketArray(ref int type, ref int size, ref int id, byte[] packet)
    {
        type = BitConverter.ToInt32(packet, 0);
        size = BitConverter.ToInt32(packet, sizeof(int));
        id = BitConverter.ToInt32(packet, sizeof(int) * 2);
    }
}