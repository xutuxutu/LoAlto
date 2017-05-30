#define CAPSTONE
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;

using System.Threading;

public struct HTTP_Packet
{
    public string m_type;
    public System.Object m_data;

    public HTTP_Packet(string type, System.Object data)
    {
        m_type = type;
        m_data = data;
    }

    public void reset()
    {
        m_type = null;
        m_data = null;
    }
}

class HTTPManager : MonoBehaviour
{
#if CAPSTONE
    string m_url = "http://192.168.0.10/index.php/hahahohouser/";
#else
    string m_url = "http://192.168.30.51/index.php/hahahohouser/";
#endif

    private HttpSendRecv m_httpSendRecv;
    private HTTPHandlingManager m_httpHandlingManager;
    private Queue<HTTP_Packet> m_httpMessageQueue;

    HTTP_Packet m_recvPacket;
    HTTP_Packet m_handlingPacket;

    private static HTTPManager m_instance = null;
    public static HTTPManager getInstance() { return m_instance; }

    //
    Mutex mtx;
    //

    void Awake()
    {
        m_instance = this;
        m_httpSendRecv = new HttpSendRecv();
        m_httpHandlingManager = new HTTPHandlingManager();
        m_httpMessageQueue = new Queue<HTTP_Packet>();
        mtx = new Mutex();
        m_recvPacket.reset();
        m_handlingPacket.reset();
    }

    public HTTPHandlingManager parsingPacket() { return m_httpHandlingManager; }
    //패킷 송신 함수
    public void SEND_HTTP( string _url)
    {
        //기본 URL + 함수 이름
        string resultUrl = m_url + _url;
        m_httpSendRecv.Send(resultUrl);
    }

    //패킷 송신 함수
    public void SEND_HTTP(object _message, string _url)
    {
        //구조체 -> 패킷 생성
        string jData = JsonMapper.ToJson(_message);
        byte[] packet = Encoding.UTF8.GetBytes(jData);
        //기본 URL + 함수 이름
        string resultUrl = m_url + _url;
        m_httpSendRecv.Send(resultUrl, packet);
    }

    //Recv 패킷 처리 함수
    public void RECV_HTTP(string recvData)
    {
        mtx.WaitOne();
        m_recvPacket.m_type = JsonMapper.ToObject<NET_HTTP.RECV.JSON_TYPE_IDENTIFY>(recvData).type;

        switch (m_recvPacket.m_type)
        {
            case NET_HTTP.URL_NAME.JOIN :
                m_recvPacket.m_data = JsonMapper.ToObject<NET_HTTP.RECV.JOIN_RESULT_INFO>(recvData);
                break;
            case NET_HTTP.URL_NAME.LOGIN:
                m_recvPacket.m_data = JsonMapper.ToObject<NET_HTTP.RECV.LOGIN_RESULT_INFO>(recvData);
                break;
            case NET_HTTP.URL_NAME.CHARACTER_DATA:
                m_recvPacket.m_data = JsonMapper.ToObject<NET_HTTP.RECV.CHARACTER_DB_DATA>(recvData);
                break;
            case NET_HTTP.URL_NAME.CREATURE_DATA:
                m_recvPacket.m_data = JsonMapper.ToObject<NET_HTTP.RECV.CREATURE_DB_DATA>(recvData);
                break;
            case NET_HTTP.URL_NAME.ITEM_DATA:
                m_recvPacket.m_data = JsonMapper.ToObject<NET_HTTP.RECV.CREATURE_ITEM_DB_DATA>(recvData);
                break;
        }

        Debug.Log("recv HTTP packet : " + m_recvPacket.m_type);
        m_httpMessageQueue.Enqueue(m_recvPacket);
        m_recvPacket.reset();
        mtx.ReleaseMutex();
    }

    public void analysePacket()
    {
        for (int i = 0; i < m_httpMessageQueue.Count; ++i)
        {
            m_handlingPacket = m_httpMessageQueue.Dequeue();
            if(m_handlingPacket.m_type == null)
            {
                Debug.Log("analysePacket : HTTP TYPE NULL");
                return;
            }
            if (m_handlingPacket.m_data != null)
            {
                Debug.Log("analysePacket : " + m_handlingPacket.m_type);
                switch (m_handlingPacket.m_type)
                {
                    case NET_HTTP.URL_NAME.JOIN :
                        NET_HTTP.RECV.JOIN_RESULT_INFO joinPacket = (NET_HTTP.RECV.JOIN_RESULT_INFO)m_handlingPacket.m_data;
                        OutGameInputManager.getInstance().recvJoinResult(joinPacket.isSuccess, joinPacket.text);
                        break;
                    case NET_HTTP.URL_NAME.LOGIN:
                        NET_HTTP.RECV.LOGIN_RESULT_INFO loginPacket = (NET_HTTP.RECV.LOGIN_RESULT_INFO)m_handlingPacket.m_data;
                        OutGameInputManager.getInstance().recvLoginResult(loginPacket.pID, loginPacket.isSuccess, loginPacket.text);
                        break;
                    case NET_HTTP.URL_NAME.CHARACTER_DATA :
                        NET_HTTP.RECV.CHARACTER_DB_DATA characterInfoPacket = (NET_HTTP.RECV.CHARACTER_DB_DATA)m_handlingPacket.m_data;
                        InGameMgr.getInstance().receiveGameData(characterInfoPacket.result, characterInfoPacket.aResult, characterInfoPacket.sResult);
                        break;
                    case NET_HTTP.URL_NAME.CREATURE_DATA:
                        NET_HTTP.RECV.CREATURE_DB_DATA creatureDataPacket = (NET_HTTP.RECV.CREATURE_DB_DATA)m_handlingPacket.m_data;
                        CreatureMgr.getInstance().recvCreatureDataResult(creatureDataPacket.result, creatureDataPacket.aResult, creatureDataPacket.iResult);
                        break;
                    case NET_HTTP.URL_NAME.ITEM_DATA:
                        NET_HTTP.RECV.CREATURE_ITEM_DB_DATA itemdataPacket = (NET_HTTP.RECV.CREATURE_ITEM_DB_DATA)m_handlingPacket.m_data;
                        CreatureMgr.getInstance().recvItemDataResult(itemdataPacket.result);
                        break;
                }
            }
            Debug.Log("recv HTTP packet : " + m_handlingPacket.m_type);
        }

    }
}
