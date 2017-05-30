using UnityEngine;
using System;
using System.Collections;

public class OutGamePacketHandlingMgr : PacketHandlingMgr
{
    private OutGamePacketCreateMgr m_createPacket;
    private OutGamePacketDivideMgr m_dividePacket;

    public OutGamePacketHandlingMgr()
    {
        m_createPacket = new OutGamePacketCreateMgr();
        m_dividePacket = new OutGamePacketDivideMgr();

        setPacketManager(m_dividePacket, m_createPacket);
    }

    protected override void SetRecvPacketDictionary()
    {
        //connect
        RecvPacketDictionary.Add((int)NET_OUTGAME.RECV.PACKET_TYPE.RECV_PLAYER_ID_CONFIRM_EM, RECV_PLAYER_ID_CONFIRM_FC);
        RecvPacketDictionary.Add((int)NET_OUTGAME.RECV.PACKET_TYPE.RECV_CHARACTER_SELECT_INFO_EM, RECV_CHARACTER_SELECT_INFO_FC);
        RecvPacketDictionary.Add((int)NET_OUTGAME.RECV.PACKET_TYPE.RECV_PLAYER_READY_EM, RECV_PLAYER_READY_FC);
        RecvPacketDictionary.Add((int)NET_OUTGAME.RECV.PACKET_TYPE.RECV_GAME_START_COUNT_DOWN_EM, RECV_GAME_START_COUNT_DOWN_FC);
        RecvPacketDictionary.Add((int)NET_OUTGAME.RECV.PACKET_TYPE.RECV_GAME_START_EM, RECV_GAME_START_FC);
        RecvPacketDictionary.Add((int)NET_OUTGAME.RECV.PACKET_TYPE.RECV_ALL_USER_ENTER_EM, RECV_ALL_USER_ENTER_FC);
        RecvPacketDictionary.Add((int)NET_OUTGAME.RECV.PACKET_TYPE.RECV_PLAYER_EXIT_ROOM_EM, RECV_PLAYER_EXIT_ROOM_FC);
    }

    public override void InsertPacketInQueue(byte[] packet)
    {
        int _type = -1;
        NET_OUTGAME.RECV.PACKET_TYPE type = NET_OUTGAME.RECV.PACKET_TYPE.RECV_PLAYER_ID_CONFIRM_EM; //ref를 쓰기위한 초기화
        if (packet == null)
        {
            
            Debug.Log("RecvPacket is Null Data : InsertPacketInQueue");
            return;
        }

        m_dividePacket.DividePacketArray(ref _type, packet);
        type = (NET_OUTGAME.RECV.PACKET_TYPE)_type;

        int rPacketSize = NET_OUTGAME.RECV.PACKET_SIZE.getTypeToSize(type);           //받은 패킷의 사이즈를 알기위한 변수
        RecvPacket data;
        if (rPacketSize == 0)
        {
            Debug.Log("Non Define PacketType");
        }
        else if (packet.Length > rPacketSize)
        {
            byte[] _data = new byte[rPacketSize]; //처리할 패킷을 저장할 변수
            byte[] _packet = new byte[packet.Length - rPacketSize]; //뭉쳐서 온 패킷을 저장할 변수

            Array.Copy(packet, 0, _data, 0, rPacketSize); //처리할 패킷을 저장
            Array.Copy(packet, rPacketSize, _packet, 0, packet.Length - rPacketSize); //나머지 패킷을 저장
            data = new RecvPacket(type, _data);

            m_recvPacketQue.Enqueue(data);  //데이터 집어넣기
       
            InsertPacketInQueue(_packet); //재귀
        }
        else
        {
            data = new RecvPacket(type, packet);
            m_recvPacketQue.Enqueue(data);
        }
    }

    public override void analysePacket()
    {
        while (m_recvPacketQue.Count > 0)
        {
            this.DividePacket(m_recvPacketQue.Dequeue());
        }
    }

    private void RECV_PLAYER_ID_CONFIRM_FC(byte[] data)
    {
        OutGameInputManager.getInstance().waitOtherUser();
    }

    private void RECV_ALL_USER_ENTER_FC(byte[] data)
    {
        int[] pID = new int[2];
        for (int i = 0; i < 2; ++i)
            pID[i] = -1;

        m_dividePacket.DIVIDE_ALL_USER_ENTER_FC(ref pID, data);

        if (ProjectMgr.getInstance().getOwnID() == pID[0])
            ProjectMgr.getInstance().setOtherID(pID[1]);

        else if(ProjectMgr.getInstance().getOwnID() == pID[1])
            ProjectMgr.getInstance().setOtherID(pID[0]);

        ProjectMgr.getInstance().setHost();
        Debug.Log(ProjectMgr.getInstance().getOwnID() + " " + pID[0] + " " + pID[1] + " " + ProjectMgr.getInstance().isHost());
        OutGameInputManager.getInstance().changeCharacterSelectUI();
    }

    private void RECV_CHARACTER_SELECT_INFO_FC(byte[] data)
    {
        int pID = -1;
        int charType = -1;
        bool isSelect = false;
        bool isSuccess = false;
        m_dividePacket.DIVIDE_CHARACTER_SELECT_INFO_FC(ref pID, ref charType, ref isSelect, ref isSuccess, data);

        if(ProjectMgr.getInstance().getOwnID() == pID)
            OutGameInputManager.getInstance().recvCharacterSelectInfo_Own(isSuccess, isSelect, (CHARACTER.TYPE)charType);
        else if(isSuccess == true)
            OutGameInputManager.getInstance().recvCharacterSelectInfo_Other(isSelect, (CHARACTER.TYPE)charType);
    }

    private void RECV_PLAYER_READY_FC(byte[] data)
    {
        int pID = -1;
        bool isReady = false;
        m_dividePacket.DIVIDE_PLAYER_READY_FC(ref pID, ref isReady, data);
        OutGameInputManager.getInstance().recvSelectReady(isReady);
    }

    private void RECV_GAME_START_COUNT_DOWN_FC(byte[] data)
    {
        OutGameInputManager.getInstance().recvGameStart();
    }

    private void RECV_GAME_START_FC(byte[] data)
    {
        OutGameInputManager.getInstance().setCharacterType();
        OutGameMgr.getInstance().startGame();
    }

    private void RECV_PLAYER_EXIT_ROOM_FC(byte[] data)
    {
        OutGameInputManager.getInstance().recvExitCharacterSelectRoom();
    }
}
