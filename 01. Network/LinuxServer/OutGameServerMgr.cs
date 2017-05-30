//#define LEFT
//#define LOCAL
#define CAPSTONE

using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class OutGameServerMgr : ServerMgr
{
    // 싱글톤 패턴을 위한 변수
    private static OutGameServerMgr m_instance = null;
    private OutGamePacketHandlingMgr m_packetManager;

    bool m_clientExit;

    void Awake()
    {
        m_instance = this;
        m_clientExit = false;
        m_packetManager = new OutGamePacketHandlingMgr();
    }

    public OutGameServerMgr()
    {
        init("192.168.0.10");
    }

    // 싱글톤 변수에 접근하기 위한 getter
    public static OutGameServerMgr getInstance()
    {
        return m_instance;
    }

    public void analysePacket()
    {
        m_packetManager.analysePacket();
    }

    public override bool connectToServer()
    {
        m_tcpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
#if LEFT
            m_tcpClientSocket.Connect(IP_ADDRESS, 11111);
#elif BACK
            m_tcpClientSocket.Connect(IP_ADDRESS, 55555);
#elif LOCAL
            IP_ADDRESS = IPAddress.Parse("127.0.0.1");
            m_tcpClientSocket.Connect(IP_ADDRESS, 11111);
#elif CAPSTONE
            IP_ADDRESS = IPAddress.Parse("192.168.0.10");
            m_tcpClientSocket.Connect(IP_ADDRESS, 11111);
#endif
            m_isConnected = true;
            
            Debug.Log("Connect Success");
        }
        catch
        {
            m_isConnected = false;
            // 연결 실패
            Debug.Log("Connect Fail");
            return false;
            // 함수 종료
        }

        // 서버 연결 성공/실패가 정해지고 나서 이벤트를 signaled로 바꿔준다.
        // 이 이벤트가 없다면 서버에서 성공, 실패 여부가 오기 전에 스레드가 진행 해버리기 때문에
        // 이벤트로 스레드를 잡아둬야 한다.

        AsyncObject ao = new AsyncObject(4096);
        // 4096 크기를 가진 바이트 배열을 가진 AsyncObject 생성

        ao.WorkingSocket = m_tcpClientSocket;
        // 작업 중인 소켓을 저장하기 위해 소켓 할당

        m_tcpClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
        // 서버에서 들어오는 메시지를 받기 위해 BeginRecieve 함수 호출.

        return true;
    }

    public override void disConnectServer()
    {
        m_clientExit = true;
        if (m_isConnected == true)
        {
            m_isConnected = false;
            m_tcpClientSocket.Disconnect(false);
            m_tcpClientSocket.Close();
        }
    }
    protected override void insertPacketInQueue(byte[] msgByte)
    {
        m_packetManager.InsertPacketInQueue(msgByte);
    }

    public void SendPacket(NET_OUTGAME.SEND.PACKET_TYPE sendPacketType)
    {
        byte[] packet = m_packetManager.CreatePacket(sendPacketType);

        if (packet == null)
        {
            Debug.Log("NULL Packet");
            return;
        }
        SendMessage(packet);
    }

    public void SendPacket(NET_OUTGAME.SEND.PACKET_TYPE sendPacketType, params object[] _param)
    {
        m_packetManager.setPacketParameter(_param);
        byte[] packet = m_packetManager.CreatePacket(sendPacketType);

        if (packet == null)
        {
            Debug.Log("NULL Packet : params");
            return;
        }

        SendMessage(packet);
    }

    public void OnDestroy()
    {
        m_clientExit = true;
        if(m_isConnected == true)
            m_tcpClientSocket.Disconnect(false);
    }
}
