using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// 비동기 프로그래밍을 위한 변수가 담긴 클래스
public class AsyncObject
{
    public Byte[] Buffer;
    // 패킷을 보낼 때나, 받을 때 쓸 변수
    public Socket WorkingSocket;
    // 작업 중인 소켓
    public AsyncObject(Int32 bufferSize)
    {
        this.Buffer = new Byte[bufferSize];
        // 버퍼 크기 동적 할당
    }
}

public abstract class ServerMgr : MonoBehaviour
{
    protected static int RECV_BUFFER_SIZE = 3472;
    //통신하기 위한 IP주소
    protected IPAddress IP_ADDRESS;
    //통신하기 위한 tcp클라이언트 소켓
    protected Socket m_tcpClientSocket;

    // 데이터를 받을 때의 콜백 함수를 지정해 주기 위한 변수
    protected AsyncCallback m_fnReceiveHandler;
    // 데이터를 보내고 나서의 콜백 함수를 지정해 주기 위한 변수
    protected AsyncCallback m_fnSendHandler;

    // 통신 상태를 구분하는 bool값
    protected bool m_isConnected;

    public abstract bool connectToServer();
    public abstract void disConnectServer();

    public void init(string IP)
    {
        Debug.Log("ServerManager : init");
        //IP등록
        IP_ADDRESS = IPAddress.Parse(IP);
        // IPv4, TCP로 소캣을 생성한다.
        m_tcpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        m_tcpClientSocket.ReceiveTimeout = 1000;
        // 연결 상태를 false로 초기화 한다.
        m_isConnected = false;
        // handleDataReceive 함수를 콜백 함수로 생성한다.
        m_fnReceiveHandler = new AsyncCallback(handleDataReceive);
        // handleDataSend 함수를 콜백 함수로 생성한다.
        m_fnSendHandler = new AsyncCallback(handleDataSend);
    }

    // 메시지를 보내는 함수
    public void SendMessage(byte[] _data)
    {
        AsyncObject ao = new AsyncObject(1);
        // _data의 크기만큼의 바이트 배열을 가진 AsyncObject 클래스를 생성한다.

        ao.Buffer = _data;
        // 버퍼에 보낼 데이터 복사

        ao.WorkingSocket = m_tcpClientSocket;
        // 작업 중인 소켓을 저장하기 위해 소켓 할당.

        try
        {
            Debug.Log("보낸 데이터 크기 : " + ao.Buffer.Length);
            m_tcpClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnSendHandler, ao);
            // 서버에 메시지를 보낸다.
            // 예외가 발생하면 catch문으로 이동한다.
        }
        catch
        {
            Debug.Log("is Null data : SendMessage");
            return;
            // 예외 발생
        }
    }

    // 메시지를 받는 함수
    public void handleDataReceive(IAsyncResult ar)
    {
        AsyncObject ao = (AsyncObject)ar.AsyncState;
        // 콜백 함수의 인자로 넘어온 서버에서 받은 패킷을 AsyncObject로 형변환 해서 저장한다.
        Int32 recvBytes;
        // 받은 데이터를 저장 할 변수 선언
        try
        {
            recvBytes = ao.WorkingSocket.EndReceive(ar);
            if (recvBytes > 4000)
                Debug.Log("데이터 크기 :" + recvBytes);
            // 받은 데이터 길이를 변수에 저장
            // 예외가 발생하면 catch문으로 이동한다.
        }
        catch
        {
            Debug.Log("RECV ERROR : TCP");
            return;
            // 예외 발생
        }
        if (recvBytes > 0)
        // 받은 데이터 길이가 0보다 크다면
        {
            int socketRemainBuffer = recvBytes;
            int totalBufSize = recvBytes;
            //최종 처리 버퍼 받은 크기만큼 할당.
            byte[] ResultBuffer = new byte[recvBytes];
            //다시 들어온 데이터를 받을 버퍼
            byte[] RecvBuffer;
            //받은 데이터를 최종 버퍼에 저장.
            Array.Copy(ao.Buffer, ResultBuffer, recvBytes);
            
             Debug.Log("패킷 크기 : " + socketRemainBuffer);
            //만약 받은 버퍼의 크기가 4096이면 반복
            while(socketRemainBuffer >= 4096)
            {
                //다시 데이터를 받을 버퍼
                RecvBuffer = new byte[4096];
                //현재까지 받은 최종버퍼만큼 크기 할당
                byte[] tempBuf = new byte[ResultBuffer.Length];
                //현재까지 받은 데이터를 템프버퍼에 저장
                Array.Copy(ResultBuffer, tempBuf, ResultBuffer.Length);
                //사이즈 및 데이터 재 수신
                socketRemainBuffer = ao.WorkingSocket.Receive(RecvBuffer);
                //최종 버퍼를 재 수신 받은 크기만큼 늘려서 재 할당
                ResultBuffer = new byte[tempBuf.Length + socketRemainBuffer];
                //템프 버퍼의 값을 최종 버퍼에 복사.
                Array.Copy(tempBuf, ResultBuffer, tempBuf.Length);
                //다시받은 버퍼의 처음부터 최종버퍼의 뒷 부분에 다시 받은 메세지의 크기만큼 저장.
                Array.Copy(RecvBuffer, 0, ResultBuffer, tempBuf.Length, socketRemainBuffer); 

                Debug.Log("패킷 크기 초과 : " + ResultBuffer.Length);
            }

            //byte[] msgByte = new Byte[recvBytes];
            // 길이만큼 바이트 배열 동적 할당
            //Array.Copy(ao.Buffer, msgByte, recvBytes);
            // 받은 버퍼를 msgByte변수에 저장
            insertPacketInQueue(ResultBuffer);
        }
        else
        {
            Debug.Log("ServerMgr : DataSize : " + recvBytes);
        }
        try
        {
            ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
            // 다시 서버에게 메시지 받을 준비를 한다.
            // 예외가 발생하면 catch문으로 이동한다.
        }
        catch
        {
            Debug.Log("RECV ERROR : TCP");
            return;
            // 예외 발생
        }
    }

    // 메시지를 보내고 호출되는 함수
    public void handleDataSend(IAsyncResult ar)
    {
        AsyncObject ao = (AsyncObject)ar.AsyncState;
        // 콜백 함수의 인자로 넘어온 서버로 보낸 패킷을 AsyncObject로 형변환 해서 저장한다.

        Int32 sendBytes;
        // 보낸 데이터 길이를 저장 할 변수 선언

        try
        {
            sendBytes = ao.WorkingSocket.EndSend(ar);
            // 보낸 데이터의 길이를 저장한다.
            // 예외가 발생하면 catch문으로 이동한다.
        }
        catch
        {
            return;
            // 예외 발생
        }
    }

    protected abstract void insertPacketInQueue(byte[] msgByte);
    public bool isConnected() { return m_isConnected; }
}
