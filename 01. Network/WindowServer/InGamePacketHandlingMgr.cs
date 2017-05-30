//#define PACKET_LOG
using UnityEngine;
using System.Collections;
using System;


public class InGamePacketHandlingMgr : PacketHandlingMgr
{
    private InGamePacketCreateMgr m_createPacket;
    private InGamePacketDivideMgr m_dividePacket;

    private RecvPacket m_transformInfo;
    private RecvPacket m_sparkyAimPoint;
    private RecvPacket m_c4Transform;
    private RecvPacket[][] m_creatureTransformInfo;

    public InGamePacketHandlingMgr()
    {
#if debug
        Debug.Log("InGame PacketHandlingManager create");
#endif
        m_createPacket = new InGamePacketCreateMgr();
        m_dividePacket = new InGamePacketDivideMgr();
        setPacketManager(m_dividePacket, m_createPacket);

        m_transformInfo = new RecvPacket();
        m_transformInfo.reset();
        m_sparkyAimPoint = new RecvPacket();
        m_sparkyAimPoint.reset();
        m_c4Transform = new RecvPacket();
        m_c4Transform.reset();
        m_creatureTransformInfo = new RecvPacket[10][];

        for (int i = 0; i < 10; ++i)
        {
            m_creatureTransformInfo[i] = new RecvPacket[100];
            for (int j = 0; j < 100; ++j)
                m_creatureTransformInfo[i][j].reset();
        }
    }

    protected override void SetRecvPacketDictionary()
    {
        //connect
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.PLAYER_LOGIN_PERMISSION_EM, PLAYER_LOGIN_PERMISSION_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_START_GAME__EM, START_GAME_FC);

        //ingame
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_TRANSFORM_EM, NOTIFY_TRANSFORM_PLAYER_FC);

        //캐릭터 - 공통
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_NORMAL_ATTACK_INFO, NOTIFY_CHARACTER_NORMAL_ATTACK_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTYFY_JUMP_EM, JUMP_PLAYER_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_DAMAGE_INFO, NOTIFY_CHARACTER_DAMAGE_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_DODGE_INFO_EM, NOTIFY_CHARACTER_DODGE_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_DIE_INFO_EM, NOTIFY_CHARACTER_DIE_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_PLAYER_GAME_RETRY_EM, NOTIFY_PLAYER_GAME_RETRY_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_DOWN_EM, NOTIFY_CHARACTER_DOWN_INFO_FC);

        //오브젝트
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTYFY_OBJECT_ACTIVE_EM, NOTTFY_OBJECT_ACTIVE_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTYFY_OBJECT_DEACTIVE_EM, NOTTFY_OBJECT_DEACTIVE_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_OBJECT_STATE_EM, NOTIFY_OBJECT_STATE_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_TRIGGER_ACTIVE_EM, NOTIFY_TIGGER_ACTIVE_FC);
        //크리쳐
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CREATURE_DAMAGE_INFO, NOTIFY_CREATURE_DAMAGE_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CREATURE_STATE_INFO, NOTIFT_CREATRUE_STATE_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CREATURE_TRANSFORM_INFO, NOTIFY_CREATRUE_TRANSFORM_INFO_FC);

        //캐릭터 - 스파키
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SPARKY_AIM_POINT_EM, NOTIFY_CHARACTER_SPARKY_AIM_POINT_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_EM, NOTIFY_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM, NOTIFY_CHARACTER_RAPID_FIRE_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARCATER_SPARKY_C4_BOMB_THROW_INFO_EM, NOTIFY_CHARACTER_C4_BOMB_THROW_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM, NOTIFY_CHARACTER_C4_BOMB_TRANSFORM_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARCATER_SPARKY_C4_BOMB_DETONATION_ORDER_EM, NOTIFY_CHARACTER_C4_BOMB_DETONATION_ORDER_FC);

        //캐릭터 - 샘
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SAM_NORAML_ATTACK_INFO_EM, NOTIFY_CHARACTER_SAM_NORMAL_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SAM_STEAM_BLOW_INFO_EM, NOTIFY_CHARACTER_SAM_STEAM_BLOW_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SAM_BATTLE_IDLE_INFO_EM, NOTIFY_CHARACTER_SAM_BATTLE_IDLE_INFO_FC);
        RecvPacketDictionary.Add((int)NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CAHRACTER_SAM_PULVERIZE_INFO_EM, NOTIFY_CHARACTER_SAM_PULVERIZE_INFO_FC);
    }

    public override void InsertPacketInQueue(byte[] recvPacket)
    {
        int _type = -1;
        int rPacketSize = -1;
        int pID = -1;
        //---------------------------------------------------------------------------------------------------------------//
        NET_INGAME.RECV.PACKET_TYPE type = NET_INGAME.RECV.PACKET_TYPE.PLAYER_LOGIN_PERMISSION_EM; //ref를 쓰기위한 초기화
        if (recvPacket == null)
        {
            Debug.Log("RecvPacket is Null Data : InsertPacketInQueue");
            return;
        }

        m_dividePacket.DividePacketArray(ref _type, ref rPacketSize, ref pID, recvPacket);
        if (rPacketSize < 0)
        {
            Debug.Log("RecvPacket Size is Out Of Size : InsertPacketInQueue");
            return;
        }
        type = (NET_INGAME.RECV.PACKET_TYPE)_type;
        //Debug.Log("recvPacket " + type);

        //rPacketSize = rPacketSize - sizeof(int) * 3;
        //byte[] packet = new byte[rPacketSize]; //처리할 패킷을 저장할 변수
        //Array.Copy(recvPacket, sizeof(int) * 3, packet, 0, recvPacket.Length - sizeof(int) * 3); //처리할 패킷을 저장

        RecvPacket data;
        if (recvPacket.Length > rPacketSize)    //받은 패킷의 사이즈가 처리할 패킷 사이즈보다 클 경우 -> 패킷이 뭉쳐져서 온 경우.
        {
            byte[] _data = new byte[rPacketSize]; //처리할 패킷을 저장할 변수
            byte[] _packet = new byte[recvPacket.Length - rPacketSize]; //뭉쳐서 온 패킷을 저장할 변수

            Array.Copy(recvPacket, 0, _data, 0, rPacketSize); //처리할 패킷을 저장
            Array.Copy(recvPacket, rPacketSize, _packet, 0, recvPacket.Length - rPacketSize); //나머지 패킷을 저장
            data = new RecvPacket(type, _data);

            m_recvPacketQue.Enqueue(data);

            m_dividePacket.DividePacketArray(ref _type, _packet);
            InsertPacketInQueue(_packet); //재귀
        }
        else        //패킷이 뭉쳐서 오지 않을 경우.
        {
            data = new RecvPacket(type, recvPacket);
            m_recvPacketQue.Enqueue(data);
        }
    }

    public void udpPacketHandler(byte[] packet)
    {
        int _type = -1;         //타입저장 변수
        int rPacketSize = -1;   //사이즈 저장 변수
        int _pID = -1;
        //--------------------------------------------------------------------------------------------------------------//
        NET_INGAME.RECV.PACKET_TYPE type = NET_INGAME.RECV.PACKET_TYPE.PLAYER_LOGIN_PERMISSION_EM; //ref를 쓰기위한 초기화
        if (packet == null)
        {
            Debug.Log("RecvPacket is Null Data : udpPacketHandler");
            return;
        }
        m_dividePacket.DividePacketArray(ref _type, ref rPacketSize, ref _pID, packet);
        type = (NET_INGAME.RECV.PACKET_TYPE)_type;

        byte[] _data = new byte[rPacketSize]; //처리할 패킷을 저장할 변수
        Array.Copy(packet, sizeof(int) * 3, _data, 0, packet.Length - sizeof(int) * 3); //처리할 패킷을 저장

        RecvPacket data = new RecvPacket(type, _data);
        if (packet.Length > rPacketSize)    //받은 패킷의 사이즈가 처리할 패킷 사이즈보다 클 경우 -> 패킷이 뭉쳐져서 온 경우.
        {
            byte[] _packet = new byte[packet.Length - rPacketSize]; //뭉쳐서 온 패킷을 저장할 변수

            Array.Copy(packet, rPacketSize, _packet, 0, packet.Length - rPacketSize); //나머지 패킷을 저장

            if (type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_TRANSFORM_EM)
            {
                m_transformInfo = data;
            }
            else if (type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CREATURE_TRANSFORM_INFO)
            {
                int _packetSize, _playerID, _packetIndex, _creatureType, _creatureID;
                int curToken = 0;
                //_packetSize = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                //_playerID = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                _packetIndex = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                // 
                while (curToken + sizeof(int) * 3 < rPacketSize)
                {
                    float[] transform = new float[7];
                    _creatureType = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                    _creatureID = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                    for (int i = 0; i < 7; ++i)
                    {
                        transform[i] = BitConverter.ToSingle(data.m_recvPacket, curToken);
                        curToken += sizeof(float);
                    }
                    CreatureMgr.Creature_Packet_Data transformPacketData = new CreatureMgr.Creature_Packet_Data();
                    transformPacketData.Resize(sizeof(int) * 2 + sizeof(float) * 7);
                    transformPacketData.InsertData(BitConverter.GetBytes(_creatureType), sizeof(int));
                    transformPacketData.InsertData(BitConverter.GetBytes(_creatureID), sizeof(int));
                    for (int i = 0; i < 7; ++i)
                        transformPacketData.InsertData(BitConverter.GetBytes(transform[i]), sizeof(float));
                    //
                    RecvPacket transformData = new RecvPacket(type, transformPacketData.GetData());
                    m_creatureTransformInfo[_creatureType][_creatureID] = transformData;
                    // 패킷데이터 정리
                    //transformPacketData.Clear();
                }
            }
            else if (type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SPARKY_AIM_POINT_EM)
            {
                m_sparkyAimPoint = data;
            }
            else if(type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM)
            {
                m_c4Transform = data;
            }
            else
            {
                Debug.Log("UDP Packet Error : None Define Type in UDP : " + type);
            }

            udpPacketHandler(_packet); //재귀
        }
        else        //패킷이 뭉쳐서 오지 않을 경우.
        {
            if (type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_TRANSFORM_EM)
            {
                m_transformInfo = data;
            }
            else if (type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CREATURE_TRANSFORM_INFO)
            {
                int _packetSize, _playerID, _packetIndex, _creatureType, _creatureID;
                int curToken = 0;
                //_packetSize = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                //_playerID = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                _packetIndex = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                // 
                while (curToken + sizeof(int) * 3 < rPacketSize)
                {
                    float[] transform = new float[7];
                    _creatureType = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                    _creatureID = BitConverter.ToInt32(data.m_recvPacket, curToken); curToken += sizeof(int);
                    for (int i = 0; i < 7; ++i)
                    {
                        transform[i] = BitConverter.ToSingle(data.m_recvPacket, curToken);
                        curToken += sizeof(float);
                    }
                    CreatureMgr.Creature_Packet_Data transformPacketData = new CreatureMgr.Creature_Packet_Data();
                    transformPacketData.Resize(sizeof(int) * 2 + sizeof(float) * 7);
                    transformPacketData.InsertData(BitConverter.GetBytes(_creatureType), sizeof(int));
                    transformPacketData.InsertData(BitConverter.GetBytes(_creatureID), sizeof(int));
                    for (int i = 0; i < 7; ++i)
                        transformPacketData.InsertData(BitConverter.GetBytes(transform[i]), sizeof(float));
                    //
                    RecvPacket transformData = new RecvPacket(type, transformPacketData.GetData());
                    m_creatureTransformInfo[_creatureType][_creatureID] = transformData;
                    // 패킷데이터 정리
                    //transformPacketData.Clear();
                }
            }
            else if (type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARACTER_SPARKY_AIM_POINT_EM)
            {
                m_sparkyAimPoint = data;
            }
            else if (type == NET_INGAME.RECV.PACKET_TYPE.NOTIFY_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM)
            {
                m_c4Transform = data;
            }
            else
            {
                Debug.Log("UDP Packet Error : None Define Type in UDP : " + type);
            }
        }
    }

    public void udpPacketClassify()
    {

    }

    public override void analysePacket()
    {
        if (m_transformInfo.comparePacketType(NET_INGAME.RECV.PACKET_TYPE.NONE) == false)
        {
            this.DividePacket(m_transformInfo);
            m_transformInfo.reset();
        }
        while (m_recvPacketQue.Count > 0)
        {
            this.DividePacket(m_recvPacketQue.Dequeue());
        }

        // CreatureTransformInfo Packet Check
        if (InGameMgr.getInstance().isStart())
        {
            for (int i = (int)CreatureMgr.CreatureType.DRONE; i < (int)CreatureMgr.CreatureType.DRONE+1; ++i)
            {
                for (int j = 0; j < CreatureMgr.getInstance().GetCreatureCount(i); ++j)
                {
                    if (m_creatureTransformInfo[i][j].comparePacketType(NET_INGAME.RECV.PACKET_TYPE.NONE) == false)
                    {
                        this.DividePacket(m_creatureTransformInfo[i][j]);
                        m_creatureTransformInfo[i][j].reset();
                    }
                }
            }
        }
    }

    public void analyseSparkyAimPoint()
    {
        if (m_sparkyAimPoint.comparePacketType(NET_INGAME.RECV.PACKET_TYPE.NONE) == false)
        {
            this.DividePacket(m_sparkyAimPoint);
            m_sparkyAimPoint.reset();

            if (m_c4Transform.m_recvPacket != null)
            {
                this.DividePacket(m_c4Transform);
                m_c4Transform.reset();
            }
        }
    }

    //로그인시 받는 패킷
    private void PLAYER_LOGIN_PERMISSION_FC(byte[] _data)
    {
        int pID_Own = -1;
        int pID_Other = -1;
        m_dividePacket.DivideLoginPacket(ref pID_Own, ref pID_Other, _data);

        Debug.Log(ProjectMgr.getInstance().getOwnID() + " / " + pID_Own + " / " + pID_Other);
#if TEST
        ProjectMgr.getInstance().setOwnID(pID_Own);
#endif
        ProjectMgr.getInstance().setOtherID(pID_Other);
        InGameMgr.getInstance().getUserInfo(USER_INFO.TYPE.OWN).setUserID(ProjectMgr.getInstance().getOwnID());
        InGameMgr.getInstance().getUserInfo(USER_INFO.TYPE.OTHER).setUserID(ProjectMgr.getInstance().getOtherID());

        ProjectMgr.getInstance().setHost();
        //캐릭터 시작 포인트 설정.
        InGameMgr.getInstance().setCharacterStartPosition();
        InGameMgr.getInstance().setStartGame(true);

#if PACKET_LOG
        Debug.Log("RECV READY PACEKT");
#endif

        //InGameServerMgr.getInstance().SendPacketTest(NET_INGAME.SEND.PACKET_TYPE.REQUEST_JUMP_EM, false);
    }

    private void START_GAME_FC(byte[] data)
    {
        InGameMgr.getInstance().getUserInfo(USER_INFO.TYPE.OWN).setUserID(ProjectMgr.getInstance().getOwnID());
        InGameMgr.getInstance().getUserInfo(USER_INFO.TYPE.OTHER).setUserID(ProjectMgr.getInstance().getOtherID());

        //캐릭터 시작 포인트 설정.
        InGameMgr.getInstance().setCharacterStartPosition();
        InGameMgr.getInstance().setStartGame(true);
    }

    //플레이어 이동 및 회전 정보 패킷
    private void NOTIFY_TRANSFORM_PLAYER_FC(byte[] _data)
    {
        Vector3 position = Vector3.zero;
        float yAngle = -1;
        bool isMove = false;
        float[] anim = new float[2];
        m_dividePacket.DividePlayerTransformInfo(ref position, ref yAngle, ref isMove, ref anim, _data);
#if PACkET_LOG
        Debug.Log("PLAYER_TRANSFORM : Position : " + position + " angle  : " + yAngle + " isMove : " + isMove + " anim : " + anim);
#endif
        InGameMgr.getInstance().getOtherCharacterCtrl().setTransformInfo(position, yAngle, isMove, anim);
    }

    //플레이어 점프시 받는 패킷
    private void JUMP_PLAYER_FC(byte[] _data)
    {
        bool isJump = false;
        m_dividePacket.DividePlayerJumpInfo(ref isJump, _data);
#if PACkET_LOG
        Debug.Log("Jump : pID : " + pID + " isJump" + isJump);
#endif
        InGameMgr.getInstance().getOtherCharacterCtrl().setJumpAnim(isJump);
    }

    private void NOTTFY_OBJECT_ACTIVE_FC(byte[] _data)
    {
#if PACKET_LOG
        Debug.Log("active");
#endif
        int typeID = -1;
        int objID = -1;
        int buttonID = -1;
        m_dividePacket.DivideObjectInfo(ref typeID, ref objID, ref buttonID, _data);
        ObjectMgr.getInstance().startEvent(typeID, objID, buttonID);
        Debug.Log("recv " + typeID + " " + objID + " " + buttonID);
    }

    private void NOTTFY_OBJECT_DEACTIVE_FC(byte[] _data)
    {
        int typeID = -1;
        int objID = -1;
        int buttonID = -1;
        m_dividePacket.DivideObjectInfo(ref typeID, ref objID, ref buttonID, _data);
        ObjectMgr.getInstance().endEvent(typeID, objID, buttonID);
    }

    private void NOTIFY_OBJECT_STATE_FC(byte[] _data)
    {
#if PACKET_LOG
        Debug.Log("object State");
#endif
        int typeID = -1;
        int objID = -1;
        int state = -1;
        m_dividePacket.DivideObjectStateInfo(ref typeID, ref objID, ref state, _data);
        ObjectMgr.getInstance().setState(typeID, objID, state);
    }

    // Creature 피격 정보 패킷
    private void NOTIFY_CREATURE_DAMAGE_INFO_FC(byte[] _data)
    {
        int creatureType = -1;
        int creatureID = -1;
        int damage = -1;
        Vector3 atkPos = Vector3.zero;
        m_dividePacket.DivideCreatureDamageInfo(ref creatureType, ref creatureID, ref damage, ref atkPos, _data);
        CreatureMgr.getInstance().damageToCreature(creatureType, creatureID, damage, atkPos);
    }
    // Creature 상태 정보 패킷

    private void NOTIFT_CREATRUE_STATE_INFO_FC(byte[] _data)
    {
        int creatureType = -1;
        int creatureID = -1;
        int creatureState = -1;
        int targetIndex = -1;
        float x = 0.0f, y = 0.0f, z = 0.0f;
        m_dividePacket.DivideCreatureStateInfo(ref creatureType, ref creatureID, ref creatureState, ref targetIndex, ref x, ref y, ref z, _data);
#if PACkET_LOG
        Debug.Log("CREATURE STATE : type : " + creatureType + " ID : " + creatureID + " State : " + creatureState + " targetIndex : " + targetIndex);
#endif
        if (creatureType > 0 && creatureID >= 0 && creatureState >= 0)
            CreatureMgr.getInstance().setCreatureState(creatureType, creatureID, creatureState, targetIndex, x, y, z);
    }

    private void NOTIFY_CHARACTER_DAMAGE_INFO_FC(byte[] _data)
    {
        int creatureType = -1;
        int creatureID = -1;
        float damage = -1;
        m_dividePacket.DivideCharacterDamageInfo(ref creatureType, ref creatureID, ref damage, _data);
        InGameMgr.getInstance().getOtherCharacterCtrl().damaged(damage);
    }

    // Creature Transform 정보
    private void NOTIFY_CREATRUE_TRANSFORM_INFO_FC(byte[] _data)
    {
#if PACkET_LOG
        Debug.Log("NOTIFY_CREATRUE_TRANSFORM_INFO_FC");
#endif
        int creatureType = -1;
        int creatureID = -1;
        float x = -1.0f, y = -1.0f, z = -1.0f, rx = -1.0f, ry = -1.0f, rz = -1.0f, w = -1.0f;
        m_dividePacket.DivideCreatureTransformInfo(ref creatureType, ref creatureID, ref x, ref y, ref z, ref rx, ref ry, ref rz, ref w, _data);
#if PACkET_LOG
        Debug.Log("CREATURE TRANSFORM : x " + x + ", y " + y + ", z " + z);
#endif
        CreatureMgr.getInstance().setCreatureTransform(creatureType, creatureID, x, y, z, rx, ry, rz, w);
    }

    private void NOTIFY_CHARACTER_NORMAL_ATTACK_INFO_FC(byte[] data)
    {
        bool attackInfo = false;
        bool attackType = false;

        m_dividePacket.DIVIDE_CHARACTER_NORMAL_ATTACK_INFO_FC(ref attackInfo, ref attackType, data);
        InGameMgr.getInstance().getOtherCharacterCtrl().setAttackInfo(attackInfo, attackType);
    }

    private void NOTIFY_CHARACTER_SPARKY_SMASH_ATTACK_INFO_FC(byte[] data)
    {
        bool isSmash = false;
        int comboNum = -1;

        m_dividePacket.DIVIDE_CHARACTER_SPARKY_SMASH_ATTACK_INFO_FC(ref isSmash, ref comboNum, data);
        InGameMgr.getInstance().getOtherCharacterCtrl().setSmashAnim(isSmash, comboNum);
    }

    private void NOTIFY_CHARACTER_SPARKY_AIM_POINT_FC(byte[] data)
    {
        Vector3 aimPoint = Vector3.zero;
        Character_Other_Sparky m_contoller = (Character_Other_Sparky)InGameMgr.getInstance().getOtherCharacterCtrl();

        m_dividePacket.DIVIDE_CHARACTER_SPARKY_AIM_POINT_FC(ref aimPoint, data);
        m_contoller.setHitPoint(aimPoint);
    }

    private void NOTIFY_CHARACTER_DODGE_INFO_FC(byte[] data)
    {
        bool isDodge = false;
        int dodgeDirection = -1;

        m_dividePacket.DIVIDE_CHARACTER_DODGE_INFO_FC(ref isDodge, ref dodgeDirection, data);
        InGameMgr.getInstance().getOtherCharacterCtrl().Dodge(isDodge, (CHARACTER.DODGE_DIRECTION)dodgeDirection);
    }

    private void NOTIFY_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_FC(byte[] data)
    {
        Character_Other_Sparky m_contoller = (Character_Other_Sparky)InGameMgr.getInstance().getOtherCharacterCtrl();

        bool isFire = false;
        Vector3 firePoint = Vector3.zero;
        Vector3 fireDirection = Vector3.zero;

        m_dividePacket.DIVIDE_CHARACTER_EXPLOSION_BULLET_INFO_FC(ref isFire, ref firePoint, ref fireDirection, data);
        m_contoller.skill_ExplosionBullet(isFire, firePoint, fireDirection);
    }

    private void NOTIFY_CHARACTER_DIE_INFO_FC(byte[] data)
    {
        bool isDie = false;

        m_dividePacket.DIVIDE_CHARACTER_DIE_INFO_FC(ref isDie, data);

        if (isDie == true)
            InGameMgr.getInstance().getOtherCharacterCtrl().die(isDie);
        else
            InGameMgr.getInstance().getOwnCharacterCtrl().revival();
    }

    private void NOTIFY_CHARACTER_RAPID_FIRE_INFO_FC(byte[] data)
    {
        Character_Other_Sparky m_contoller = (Character_Other_Sparky)InGameMgr.getInstance().getOtherCharacterCtrl();

        bool useRapidFire = false;
        bool isFire = false;

        m_dividePacket.DIVIDE_CHARACTER_SPARKY_RAPID_FIRE_INFO_FC(ref useRapidFire, ref isFire, data);
        m_contoller.skill_RapidFire(useRapidFire, isFire);
    }

    private void NOTIFY_PLAYER_GAME_RETRY_FC(byte[] data)
    {
        int pID = -1;
        bool retry = false;
        m_dividePacket.DIVIDE_GAME_RETRY_FC(ref pID, ref retry, data);

        Debug.Log("재도전 : " + retry);
        InGameMgr.getInstance().recvRetryInfo(pID, retry);
    }

    private void NOTIFY_CHARACTER_C4_BOMB_THROW_INFO_FC(byte[] data)
    {
        Character_Other_Sparky m_contoller = (Character_Other_Sparky)InGameMgr.getInstance().getOtherCharacterCtrl();

        int pID = -1;
        Vector3 throwPosition = Vector3.zero;

        m_dividePacket.DIVIDE_CHARACTER_SPARKY_C4_BOMB_THROW_INFO_FC(ref pID, ref throwPosition, data);
        m_contoller.throwC4Bomb(throwPosition);
    }

    private void NOTIFY_CHARACTER_C4_BOMB_TRANSFORM_INFO_FC(byte[] data)
    {
        Character_Other_Sparky m_contoller = (Character_Other_Sparky)InGameMgr.getInstance().getOtherCharacterCtrl();

        int pID = -1;
        Vector3 c4Position = Vector3.zero;
        Vector3 c4Rotation = Vector3.zero;

        m_dividePacket.DIVIDE_CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO_FC(ref pID, ref c4Position, ref c4Rotation, data);
        m_contoller.setC4BombNextTransform(c4Position, c4Rotation);
    }

    private void NOTIFY_CHARACTER_C4_BOMB_DETONATION_ORDER_FC(byte[] data)
    {
        Character_Other_Sparky m_contoller = (Character_Other_Sparky)InGameMgr.getInstance().getOtherCharacterCtrl();

        int pID = -1;
        Vector3 detonatePosition = Vector3.zero;

        m_dividePacket.DIVIDE_CHARACTER_SPARKY_C4_BOMB_DETONATION_ORDER_FC(ref pID, ref detonatePosition, data);
        m_contoller.detonateC4Bomb(detonatePosition);
    }

    private void NOTIFY_CHARACTER_SAM_NORMAL_INFO_FC(byte[] data)
    {
        Character_Other_Sam m_contoller = (Character_Other_Sam)InGameMgr.getInstance().getOtherCharacterCtrl();

        bool isComboOrder = false;
        int comboNum = -1;

        m_dividePacket.DIVIDE_CHARACTER_SAM_NORMAL_ATTACK_INFO_FC(ref isComboOrder, ref comboNum, data);
        m_contoller.setAttackInfo(isComboOrder, comboNum);
    }

    private void NOTIFY_CHARACTER_SAM_STEAM_BLOW_INFO_FC(byte[] data)
    {
        Character_Other_Sam m_contoller = (Character_Other_Sam)InGameMgr.getInstance().getOtherCharacterCtrl();

        bool useSteamBlow = false;
        bool isContact = false;

        m_dividePacket.DIVIDE_CHARACTER_SAM_STEAM_BLOW_INFO_FC(ref useSteamBlow, ref isContact, data);
        m_contoller.skill_SteamBlow(useSteamBlow, isContact);
    }

    private void NOTIFY_CHARACTER_SAM_BATTLE_IDLE_INFO_FC(byte[] data)
    {
        Character_Other_Sam m_contoller = (Character_Other_Sam)InGameMgr.getInstance().getOtherCharacterCtrl();

        bool isBattle = false;
        m_dividePacket.DIVIDE_CHARACTER_SAM_BATTLE_IDLE_FC(ref isBattle, data);
        m_contoller.setBattleIdleAnim(isBattle);
    }

    private void NOTIFY_CHARACTER_DOWN_INFO_FC(byte[] data)
    {
        InGameMgr.getInstance().getOtherCharacterCtrl().down();
    }

    private void NOTIFY_CHARACTER_SAM_PULVERIZE_INFO_FC(byte[] data)
    {
        Character_Other_Sam m_contoller = (Character_Other_Sam)InGameMgr.getInstance().getOtherCharacterCtrl();

        m_contoller.skill_Pulverize();
    }

    private void NOTIFY_TIGGER_ACTIVE_FC(byte[] data)
    {
        int triggerNum = -1;
        m_dividePacket.DIVIDE_TRIGGER_ACTIVE_FC(ref triggerNum, data);
        QuestMgr.getInstance().activeQuestTrigger(triggerNum);
    }
}
