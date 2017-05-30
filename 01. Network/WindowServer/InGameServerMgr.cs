#define LEFT
//#define BACK
//#define LOCAL
//#define CAPSTONE

using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class InGameServerMgr : ServerMgr
{
    private InGamePacketHandlingMgr m_packetManager;
    // 패킷 관리 매니저

    private Socket m_udpClientSocket;
    // 서버와 통신하기 위한 소켓

    IPEndPoint m_udpIPEndPoint;
    EndPoint m_udpEndPoint;
    Thread m_udprecvThread;
    bool m_clientExit;

    // 싱글톤 패턴을 위한 변수
    private static InGameServerMgr m_instance = null;
    // 싱글톤 변수에 접근하기 위한 getter
    public static InGameServerMgr getInstance() { return m_instance; }

    void Awake()
    {
        m_instance = this;
        m_clientExit = false;
        init();
    }

    private void init()
    {
        // 패킷매니저 생성
        m_packetManager = new InGamePacketHandlingMgr();
        // IPv4, UDP로 소캣을 생성한다.
        m_udpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //Udp Recive Timeout. 제한시간이 지나면 강제종료.
        m_udpClientSocket.ReceiveTimeout = 1000;

        //init("192.168.30.49");
        init("192.168.0.10");
    }

    public PacketHandlingMgr getPacketHandlingManager() { return m_packetManager; }

    public void analysePacket()
    {
        m_packetManager.analysePacket();
    }

    // 서버에 연결하는 기능의 함수
    public override bool connectToServer()
    {
        try
        {
#if LEFT
            m_tcpClientSocket.Connect(IP_ADDRESS, 9999);
            m_udpClientSocket.Connect(IP_ADDRESS, 8888);
            m_udpIPEndPoint = new IPEndPoint(IP_ADDRESS, 8888); //SendTo에 사용할 IPEndPoint.
#elif BACK
            m_tcpClientSocket.Connect(IP_ADDRESS, 22222);
            m_udpClientSocket.Connect(IP_ADDRESS, 33333);
            m_udpIPEndPoint = new IPEndPoint(IP_ADDRESS, 33333); //SendTo에 사용할 IPEndPoint.
#elif LOCAL
            IP_ADDRESS = IPAddress.Parse("127.0.0.1");
            m_tcpClientSocket.Connect(IP_ADDRESS, 9999);
            m_udpClientSocket.Connect(IP_ADDRESS, 8888);
            m_udpIPEndPoint = new IPEndPoint(IP_ADDRESS, 8888); //SendTo에 사용할 IPEndPoint.
#elif CAPSTONE
            IP_ADDRESS = IPAddress.Parse("192.168.0.10");
            m_tcpClientSocket.Connect(IP_ADDRESS, 9999);
            m_udpClientSocket.Connect(IP_ADDRESS, 8888);
            m_udpIPEndPoint = new IPEndPoint(IP_ADDRESS, 8888); //SendTo에 사용할 IPEndPoint.
#endif
            m_udpEndPoint = (EndPoint)m_udpIPEndPoint;          //RecvFrom에 사용할 EndPoint
            // 루프백 IP와 9999포트 번호로 서버에 연결 요청을 한다.
            // 연결에 실패하면 Exception이 발생 해 catch 문으로 이동하게 된다.
            m_isConnected = true;
            // 연결 성공
            m_udprecvThread = new Thread(udpReceiver);
            m_udprecvThread.Start();
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

        AsyncObject ao = new AsyncObject(RECV_BUFFER_SIZE);
        // 3472 크기를 가진 바이트 배열을 가진 AsyncObject 생성

        ao.WorkingSocket = m_tcpClientSocket;
        // 작업 중인 소켓을 저장하기 위해 소켓 할당

        m_tcpClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
        // 서버에서 들어오는 메시지를 받기 위해 BeginRecieve 함수 호출.

        //로그인 패킷 전송.
        sendLoginPacket();
        return true;
    }

    public override void disConnectServer()
    {
        m_clientExit = true;
        m_udprecvThread.Interrupt();
        if (m_isConnected == true)
        {
            m_isConnected = false;
            m_tcpClientSocket.Disconnect(false);
        }
    }

    protected override void insertPacketInQueue(byte[] msgByte) { m_packetManager.InsertPacketInQueue(msgByte); }
    public void resetPacketQueue() { m_packetManager.resetPacketQueue(); }

    public void sendLoginPacket()
    {
        byte[] packet = m_packetManager.CreatePacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_LOGIN_EM);
        SendMessage(packet);
    }

    public void sendStartPacket()
    {
        Debug.Log("SceneChange : SendStartPacket");
        byte[] packet = m_packetManager.CreatePacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_READY_EM);
        SendMessage(packet);
    }

    public void SendPacketTest(NET_INGAME.SEND.PACKET_TYPE sendPacketType, params object[] _param)
    {

        m_packetManager.setPacketParameter(_param); //파라미터값 설정
        byte[] packet = m_packetManager.CreatePacket(sendPacketType); //패킷생성
        /*
        int size = 1900;
        byte[] data = new byte[packet.Length * size];
        for (int i = 0; i < size; ++i)
        {
            Array.Copy(packet, 0, data, packet.Length * i, packet.Length);

        }
        */
        Debug.Log("데이터 크기 : " + packet.Length);
        if (packet == null)
        {
            Debug.Log("NULL Packet : params");
            return;
        }
        SendMessage(packet);

    }

    public void SendPacket(NET_INGAME.SEND.PACKET_TYPE sendPacketType)
    {
        if (InGameMgr.getInstance().isStart() == true)
        {
            byte[] packet = m_packetManager.CreatePacket(sendPacketType);

            if (packet == null)
            {
                Debug.Log("NULL Packet");
                return;
            }
            SendMessage(packet);
        }
        else
        {
            Debug.Log("Not Set GameReady");
        }
    }

    public void SendPacket(NET_INGAME.SEND.PACKET_TYPE sendPacketType, params object[] _param)
    {
        if (InGameMgr.getInstance().isStart() == true)
        {
            m_packetManager.setPacketParameter(_param); //파라미터값 설정
            byte[] packet = m_packetManager.CreatePacket(sendPacketType); //패킷생성

            if (packet == null)
            {
                Debug.Log("NULL Packet : params");
                return;
            }
            SendMessage(packet);
        }
        else
        {
            Debug.Log("Not Set GameReady");
        }
    }

    //오브젝트 액티브 정보 송신
    public void sendObjectActiveMessage(string name, int buttonID)
    {
        if(ProjectMgr.getInstance().isHost() == true)
        {
            int[] _objID = ObjectMgr.getInstance().getObjectID(name);
            SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_ACTIVE_EM, _objID[0], _objID[1], buttonID);
        }
    }

    //자신의 트랜스폼 정보 전송
    public void SendTransformInfo()
    {
        byte[] packet = m_packetManager.CreatePacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_TRANSFORM_EM);
        if (packet == null)
        {
            Debug.Log("SendPacket is Null data : Character Transform info");
            return;
        }

        m_udpClientSocket.SendTo(packet, m_udpEndPoint);
    }

    // 크리쳐들의 Transform 정보 전송
    public void SendCreatureTransformInfo(params object[] _param)
    {
        if (InGameMgr.getInstance().isStart() == true)
        {
            m_packetManager.setPacketParameter_(_param);
            byte[] packet = m_packetManager.CreatePacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO);
            if (packet == null)
            {
                Debug.Log("SendPacket is Null data : Creature Transform info");
                return;
            }

            m_udpClientSocket.SendTo(packet, m_udpEndPoint);
        }
    }

    // 모든 크리쳐들의 Transform 정보 전송
    public void SendUDPCreaturesTransformInfo()
    {
        if (InGameMgr.getInstance().isStart())
        {
            // m_packetManager.CreatePacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO);
            byte[] packet = CreatureMgr.getInstance().GetSendUDPPacketData(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO, CreatureMgr.getInstance().GetPackIndexCount());

            if (packet == null)
            {
                Debug.Log("SendPacket is Null data : Creature Transform info");
                return;
            }

            m_udpClientSocket.SendTo(packet, m_udpEndPoint);
        }
    }

    //UDP 패킷전송
    public void SendPacket_UDP(NET_INGAME.SEND.PACKET_TYPE sendPacketType)
    {
        byte[] packet = m_packetManager.CreatePacket(sendPacketType);

        if (packet == null)
        {
            Debug.Log("NULL Packet : UDP");
            return;
        }

       m_udpClientSocket.SendTo(packet, m_udpEndPoint);
    }

    public void SendPacket_UDP(NET_INGAME.SEND.PACKET_TYPE sendPacketType, params object[] _param)
    {
        m_packetManager.setPacketParameter(_param);
        byte[] packet = m_packetManager.CreatePacket(sendPacketType);

        if (packet == null)
        {
            Debug.Log("NULL Packet : UDP params");
            return;
        }

        m_udpClientSocket.SendTo(packet, m_udpEndPoint);
    }

    //udp Receive
    public void udpReceiver()
    {
        byte[] packet = new byte[8192];
        int recvBytes = 0;
        try {
            while (true)
            {
                try
                {
                    recvBytes = m_udpClientSocket.ReceiveFrom(packet, ref m_udpEndPoint);
                }
                catch
                {
                    if(m_clientExit == true)
                        return;
                }

                if (recvBytes > 0)
                // 받은 데이터 길이가 0보다 크다면
                {
                    Byte[] msgByte = new Byte[recvBytes];
                    // 길이만큼 바이트 배열 동적 할당

                    Array.Copy(packet, msgByte, recvBytes);

                    //udp 패킷 처리
                    m_packetManager.udpPacketHandler(msgByte);
                }
            }
        }
        catch(ThreadInterruptedException e)
        {
            Debug.Log(e);
            return;
        }
    }

    public void OnDestroy()
    {
        m_clientExit = true;
        if (m_isConnected == true)
        {
            m_udprecvThread.Interrupt();
            m_tcpClientSocket.Disconnect(false);
        }
    }
}
