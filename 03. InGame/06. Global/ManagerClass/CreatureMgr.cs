//#define debug
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CreatureMgr : MonoBehaviour
{
    // -----------------------------------------------------------------------
    public struct Creature_Packet_Data
    {
        // -----------------------------------------------------------------------
        int i_size;
        int i_currentIndex;
        byte[] m_data;
        // -----------------------------------------------------------------------
        // Get, Set 함수
        public int GetSize() { return i_size; }
        public byte[] GetData() { return m_data; }
        // -----------------------------------------------------------------------
        // 데이터를 집어 넣는다.
        public bool InsertData(byte[] _data, int _size)
        {
            if (i_currentIndex + _size > i_size)
            {
                //Debug.Log("Out of range");
                return false;
            }
            Array.Copy(_data, 0, m_data, i_currentIndex, _size);
            i_currentIndex += _size;

            return true;
        }
        // -----------------------------------------------------------------------
        // 데이터 크기를 재설정
        public void Resize(int _size)
        {
            i_currentIndex = 0;
            i_size = _size;
            m_data = new byte[i_size];
            Array.Clear(m_data, i_currentIndex, i_size);
            i_currentIndex = 0;
        }
        // -----------------------------------------------------------------------
        // 데이터값 깨끗하게 초기화
        public void Clear()
        {
            i_currentIndex = 0;
            Array.Clear(m_data, i_currentIndex, i_size);
        }
        // -----------------------------------------------------------------------
    };
    // -----------------------------------------------------------------------
    public static readonly float GRAVITY = -9.8f * 8.0f;
    // -----------------------------------------------------------------------
    public enum CreatureState { NONE, IDLE, SPOT, READY, MOVE, JUMP, PATROL, PROTECT, TRACE, ATTACK, ONLY_VALUE_ANI_STATE, HIT, ONLY_VALUE_HIT, BREAKDOWN, DIE, CREATE, MOVE_RETRY, ROAR };
    public enum CreatureMessage { SET_STATE, SET_TRANSFORM, SET_FORWARD };
    public enum CreatureType { TROLL, OWL, DRONE, BOSS_FRANKY, KNIGHT_SWORD, SHIELD_BEARER, OWL_AIM };
    public enum CreatureMoveType { NORMAL_01, NORMAL_02, NORMAL_03, NORMAL_04, SPECIAL_01, SPECIAL_02, SPECIAL_03, SPECIAL_04 };
    public enum CreatureAttackType { NONE, NORMAL_01, NORMAL_02, NORMAL_03, NORMAL_04, NORMAL_05, SPECIAL_01, SPECIAL_02, SPECIAL_03, SPECIAL_04, SPECIAL_05 };
    public enum CreatureDropItemType { PARTS, ENERGY };
    public enum CreatureCameraEffectType { WALK, CHARGE, }
    // -----------------------------------------------------------------------
    private string[] str_creatureNaming;
    // -----------------------------------------------------------------------
    private GameObject[][] m_creatureList;
    private GameObject[] m_partsItem;
    private static CreatureMgr m_instance = null;
    private Creature_Packet_Data m_sendPacketData = new Creature_Packet_Data();
    private List<DB_DATA.CREATURE_COMMON_DATA> [] m_creatureCommonDataList;
    private List<DB_DATA.CREATURE_ATTACK_DATA> [] m_creatureAttackDataList;
    private List<DB_DATA.CREATURE_DROP_DATA> [] m_creatureDropDataList;
    private List<DB_DATA.CREATURE_ITEM_DATA> m_creatureItemDataList;
    private int i_creatureTypeNum = 10;
    private int i_creatureMaximum = 100;
    private int i_playerID = 0;
    private uint i_packetIndexCount = 0;
    private int[] i_creatureCount;
    private bool b_CreaturesActive = false;
    private bool b_CreaturesFixedState = false;
    private bool m_recieveCreatureDBData = false;
    private bool m_recieveItemDBData = false;
    private bool m_initObjects = false;
    // -----------------------------------------------------------------------
    // Get, Set 함수
    public string GetCreatureNaming(int _type) { return str_creatureNaming[_type]; }
    public int GetPlayerID() { return i_playerID; }
    public int GetCreatureCount()
    {
        int _count = 0;
        for (int i = 0; i < i_creatureTypeNum; ++i)
            _count += i_creatureCount[i];
        return _count;
    }
    public int GetCreatureCount(int _type)
    {
        if (_type < i_creatureTypeNum)
            return i_creatureCount[_type];
        return -1;
    }
    public List<DB_DATA.CREATURE_COMMON_DATA> GetCreatureCommonData(int _index) { return m_creatureCommonDataList[_index]; }
    public List<DB_DATA.CREATURE_ATTACK_DATA> GetCreatureAttackData(int _index) { return m_creatureAttackDataList[_index]; }
    public List<DB_DATA.CREATURE_DROP_DATA> GetCreatureDropData(int _index) { return m_creatureDropDataList[_index]; }
    public List<DB_DATA.CREATURE_ITEM_DATA> GetCreatureItemData() { return m_creatureItemDataList; }
    public uint GetPackIndexCount() { return i_packetIndexCount; }
    public int GetTypeCount() { return i_creatureTypeNum; }
    public bool GetCreaturesActive() { return b_CreaturesActive; }
    public bool GetCreaturesFixedState() { return b_CreaturesFixedState; }
    public Vector3 GetCreaturePosition(int _type, int _index) { return m_creatureList[_type][_index].transform.position; }
    public Quaternion GetCreatureRotate(int _type, int _index) { return m_creatureList[_type][_index].transform.rotation; }
    public GameObject GetItem(CreatureDropItemType _type) { return m_partsItem[(int)_type]; }
    public DB_DATA.CREATURE_ITEM_DATA GetItemData(int _type) { return m_creatureItemDataList[_type]; }
    public void SetCreaturesActive(bool _active) { b_CreaturesActive = _active; }
    public void SetPlayerID(int _id) { i_playerID = _id; }
    // -----------------------------------------------------------------------
    // 초기화 함수
    void Awake()
    {
        // 핸들값 설정
        m_instance = this;
        //
        m_creatureItemDataList = new List<DB_DATA.CREATURE_ITEM_DATA>();
        m_creatureCommonDataList = new List<DB_DATA.CREATURE_COMMON_DATA>[i_creatureTypeNum];
        m_creatureAttackDataList = new List<DB_DATA.CREATURE_ATTACK_DATA>[i_creatureTypeNum];
        m_creatureDropDataList = new List<DB_DATA.CREATURE_DROP_DATA>[i_creatureTypeNum];
        for(int i = 0; i < i_creatureTypeNum; ++i)
        {
            m_creatureCommonDataList[i] = new List<DB_DATA.CREATURE_COMMON_DATA>();
            m_creatureAttackDataList[i] = new List<DB_DATA.CREATURE_ATTACK_DATA>();
            m_creatureDropDataList[i] = new List<DB_DATA.CREATURE_DROP_DATA>();
        }
        //크리쳐의 종류만큼 2차원 배열을 할당.
        m_creatureList = new GameObject[i_creatureTypeNum][];
        //각 크리쳐의 갯수만큼 1차원 배열을 할당.
        for (int i = 0; i < i_creatureTypeNum; i++)
            m_creatureList[i] = new GameObject[i_creatureMaximum];
        // 생성된 크리쳐들의 갯수를 담을 변수 선언
        i_creatureCount = new int[i_creatureTypeNum];
        // Drop Item 설정
        m_partsItem = new GameObject[2];
        m_partsItem[0] = Resources.Load(PREFAB_PATH.CHARACTER_ITEM_PARTS) as GameObject;
        m_partsItem[1] = Resources.Load(PREFAB_PATH.CHARACTER_ITEM_ENERGY_BATTERY) as GameObject;
        //
        str_creatureNaming = new string[7];
        str_creatureNaming[0] = "Creature_Troll_";
        str_creatureNaming[1] = "Creature_Owl_";
        str_creatureNaming[2] = "Creature_Drone_";
        str_creatureNaming[3] = "Creature_Boss_Franky_";
        str_creatureNaming[4] = "Creature_Knight_Sword_";
        str_creatureNaming[5] = "Creature_Shield_Bearer_";
        str_creatureNaming[6] = "Creature_Owl_Aim_";
    }
    // -----------------------------------------------------------------------
    // 모든 스크립트의 Awake 함수가 실행하고 나서 한번 실행
    void Start()
    {
#if SERVER_ON
        RequierDBCreatureData();
        StartCoroutine("RecieveDBData");
#else
        // 크리쳐 오브젝트들 초기화
        InitCreatureObjects();
#endif
    }
    // -----------------------------------------------------------------------
    // 매 프레임마다 실행
    /*void Update ()
    {
    }*/
    // -----------------------------------------------------------------------
    // CreatureManager 핸들값 얻기
    public static CreatureMgr getInstance()
    {
        if (m_instance == null)
        {
            //Debug.Log(" CreatureMgr Handle is null !!! ");
            return null;
        }
        return m_instance;
    }
    // -----------------------------------------------------------------------
    // 지정된 시간마다 크리쳐들의 Transform 정보 서버로 보내기
    private IEnumerator CreaturesTransform()
    {
        yield return new WaitForSeconds(2.0f);
#if SERVER_OFF
        yield return null;
#else
        while (true)
        {
#if SERVER_ON
            //Debug.Log("SendUDPCreaturesTransformInfo Begin");
            InGameServerMgr.getInstance().SendUDPCreaturesTransformInfo();
            //Debug.Log("SendUDPCreaturesTransformInfo End");
#endif
            // 0.03초 동안 기다렸다가 다음으로 넘어감
            yield return new WaitForSeconds(0.05f);
        }
#endif
    }
    private IEnumerator RecieveDBData()
    {
#if SERVER_ON
        //while (!m_recieveCreatureDBData || !m_recieveItemDBData) { yield return null; }
        DefaultCreatureDBData();
        DefaultItemDBData();
#else
#endif
        // 크리쳐 오브젝트들 초기화
        InitCreatureObjects();
        yield return null;
    }
    // -----------------------------------------------------------------------
    // 생성되어있는 모든 크리쳐 데이터 얻기
    public void InitCreatureObjects()
    {
#if SERVER_OFF
        DefaultCreatureDBData();
        DefaultItemDBData();
#endif
        if (m_initObjects)
        {
            Debug.Log("aleady CreatureInfo Initialize");
            return;
        }
        Debug.Log("initialize CreatureInfo");
        Debug.Log("---------------------------------------------------");
        m_initObjects = true;
        string creatureName;
        GameObject creatureTarget;
        // 크리쳐의 종류만큼 반복
        for (int typeNum = 0; typeNum < str_creatureNaming.Length; typeNum++)
        {
            // 크리쳐 카운터 초기화
            i_creatureCount[typeNum] = 0;
            //종류의 갯수만큼 반복
            for (int creatureNum = 0; creatureNum < i_creatureMaximum; creatureNum++)
            {
                // 크리쳐 리스트 정리
                m_creatureList[typeNum][creatureNum] = null;
                //오브젝트 이름 설정
                creatureName = str_creatureNaming[typeNum] + creatureNum;
                //이름으로 오브젝트 탐색
                creatureTarget = GameObject.Find(creatureName);
                if (creatureTarget == null)
                {
                    Debug.Log("non creature target : " + creatureName);
                    break;
                }
                // Creature Count 증가
                i_creatureCount[typeNum]++;
                // Creature ID 설정
                creatureTarget.GetComponent<Creature>().SetType(typeNum, creatureNum);
                // Creature 초기화
                creatureTarget.GetComponent<Creature>().Initialization();
                //배열에 크리쳐를 집어넣음
                m_creatureList[typeNum][creatureNum] = creatureTarget;
                Debug.Log("init : " + creatureTarget);
            }
        }
        Debug.Log("---------------------------------------------------");
    }
    // -----------------------------------------------------------------------
    // Creature가 공격받아 Damage 값 주기
    public void damageToCreature(int creatureType, int creatureID, int damage, Vector3 pos)
    {
        int hitDamage = damage % 1000;
        int attackerID = damage - (hitDamage % 1000) % 1000000;
        GameObject targetCreature = findCreature(creatureType, creatureID);
        targetCreature.GetComponent<Creature>().damaged(attackerID, damage, pos);
#if debug
        Debug.Log(targetCreature);
#endif
    }
    // -----------------------------------------------------------------------
    // Creature Type, ID 값으로 Creature 찾기
    public GameObject findCreature(int creatureType, int creatureID)
    {
#if debug
        Debug.Log("FindCreature TYPE : " + creatureType + " +, " + creatureID);
#endif
        return m_creatureList[creatureType][creatureID];
    }
    // -----------------------------------------------------------------------
    // Creature Transform 패킷 정보 받기
    public void RecvTransformData(RecvPacket _data)
    {
    }
    // -----------------------------------------------------------------------
    // DB에서 크리쳐 정보 얻기
    public void RequierDBCreatureData()
    {
#if SERVER_ON
        HTTPManager.getInstance().SEND_HTTP(NET_HTTP.URL_NAME.CREATURE_DATA);
        HTTPManager.getInstance().SEND_HTTP(NET_HTTP.URL_NAME.ITEM_DATA);
#endif
#if TEST
#endif
    }
    // -----------------------------------------------------------------------
    // DB에서 받은 데이터 사용하기
    public void recvCreatureDataResult(List<NET_HTTP.RECV.CREATURE_COMMON_DATA> _commonData,
        List<NET_HTTP.RECV.CREATURE_ATTACK_DATA> _attackData,
        List<NET_HTTP.RECV.CREATURE_DROP_DATA> _dropData)
    {
        _commonData.ForEach(CreatureMgr.getInstance().SetDBCreatureCommonData);
        _attackData.ForEach(CreatureMgr.getInstance().SetDBCreatureAttackData);
        _dropData.ForEach(CreatureMgr.getInstance().SetDBCreatureDropData);
        m_recieveCreatureDBData = true;
    }
    // -----------------------------------------------------------------------
    // DB에서 받은 데이터 사용하기
    public void recvItemDataResult(List<NET_HTTP.RECV.CREATURE_ITEM_DATA> _itemData)
    {
        _itemData.ForEach(CreatureMgr.getInstance().SetDBCreatureItemData);
        m_recieveItemDBData = true;
    }
    // -----------------------------------------------------------------------
    // Creature 정보 처리
    public void SetDBCreatureCommonData(NET_HTTP.RECV.CREATURE_COMMON_DATA _commonData)
    {
        m_creatureCommonDataList[int.Parse(_commonData.CRIT_TYPE)].Add(HTTPManager.getInstance().parsingPacket().PARS_CREATURE_COMMON_FC(_commonData));
    }
    // -----------------------------------------------------------------------
    // Creature 정보 처리
    public void SetDBCreatureAttackData(NET_HTTP.RECV.CREATURE_ATTACK_DATA _attackData)
    {
        m_creatureAttackDataList[int.Parse(_attackData.CRIT_TYPE)].Add(HTTPManager.getInstance().parsingPacket().PARS_CREATURE_ATTACK_FC(_attackData));
    }
    // -----------------------------------------------------------------------
    // Creature 정보 처리
    public void SetDBCreatureDropData(NET_HTTP.RECV.CREATURE_DROP_DATA _dropData)
    {
        m_creatureDropDataList[int.Parse(_dropData.CRIT_TYPE)].Add(HTTPManager.getInstance().parsingPacket().PARS_CREATURE_DROP_FC(_dropData));
    }
    // -----------------------------------------------------------------------
    // Creature 정보 처리
    public void SetDBCreatureItemData(NET_HTTP.RECV.CREATURE_ITEM_DATA _itemData)
    {
        m_creatureItemDataList.Add(HTTPManager.getInstance().parsingPacket().PARS_CREATURE_ITEM_FC(_itemData));
    }
    // -----------------------------------------------------------------------
    // Creature 상태 정보 패킷 받기
    public void setCreatureState(int creatureType, int creatureID, int creatureState, int targetIndex, float x, float y, float z)
    {
        if (m_creatureList[creatureType][creatureID] != null)
        {
            // 크리쳐 활성화 해두기
            m_creatureList[creatureType][creatureID].SetActive(true);
            // 크리처 상태 정보 넘기기
            m_creatureList[creatureType][creatureID].GetComponent<Creature>().ReciveCreatureState(CreatureMessage.SET_STATE, creatureState, targetIndex, x, y, z);
        }
    }
    // -----------------------------------------------------------------------
    // 크리쳐들의 Transform 정보 패킷 보내기
    public byte[] GetSendUDPPacketData(NET_INGAME.SEND.PACKET_TYPE _packetType, uint _index)
    {
        // 패킷데이터 정리
        if (m_sendPacketData.GetData() != null)
            m_sendPacketData.Clear();
        // 패킷 종류의 따라 데이터 만들기
        switch (_packetType)
        {
            case NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO:
                {
                    // 데이터 크기 재설정 ( 패킷종류 + 패킷크기 + 플레이어ID + 패킷Index + 크리쳐종류 + 크리쳐아이디 + 크리쳐 Position, Rotate 정보 )
                    m_sendPacketData.Resize(sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + (sizeof(int) * 2 + sizeof(float) * 7) * i_creatureCount[(int)CreatureType.DRONE]);
                    // 패킷 종류 넣기
                    m_sendPacketData.InsertData(BitConverter.GetBytes((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO), sizeof(int));
                    // 패킷 크기 넣기
                    m_sendPacketData.InsertData(BitConverter.GetBytes(m_sendPacketData.GetSize()), sizeof(int));
                    // 플레이어 ID 넣기
                    m_sendPacketData.InsertData(BitConverter.GetBytes(ProjectMgr.getInstance().getOwnID()), sizeof(int));
                    // 패킷 Index 넣기
                    m_sendPacketData.InsertData(BitConverter.GetBytes(_index), sizeof(int));
                    // 생성되어 있는 크리쳐들의 정보를 구한다.
                    for (int creatureType = (int)CreatureType.DRONE; creatureType < m_creatureList.Length; ++creatureType)
                    {
                        for (int creatureID = 0; creatureID < m_creatureList[creatureType].Length; ++creatureID)
                        {
                            // 크리쳐가 생성되어있을때
                            if (m_creatureList[creatureType][creatureID] != null)
                            {
                                Creature creature = m_creatureList[creatureType][creatureID].GetComponent<Creature>();
                                // 크리쳐가 살아 있을때
                                if (creature.gameObject.activeSelf && creature.GetAlive())
                                {
                                    // 크리쳐 종류
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.GetCreatureType()), sizeof(int));
                                    // 크리쳐 ID
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.GetCreatureID()), sizeof(int));
                                    // Position X, Y, Z
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.transform.position.x), sizeof(float));
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.transform.position.y), sizeof(float));
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.transform.position.z), sizeof(float));
                                    // Quaternion X, Y, Z, W
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.transform.rotation.x), sizeof(float));
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.transform.rotation.y), sizeof(float));
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.transform.rotation.z), sizeof(float));
                                    m_sendPacketData.InsertData(BitConverter.GetBytes(creature.transform.rotation.w), sizeof(float));
                                }
                            }
                        }
                        break;
                    }
                }
                break;
        }
        // Packet Count 
        ++i_packetIndexCount;
        // 데이터값 반환
        return m_sendPacketData.GetData();
    }
    // -----------------------------------------------------------------------
    // Creature Transform 정보 패킷 받기
    public void setCreatureTransform(int _type, int _id, float _x, float _y, float _z, float _rx, float _ry, float _rz, float _w)
    {
#if debug
        Debug.Log("_type " + _type + ", _id : " + _id);
#endif
        if(m_creatureList[_type][_id] != null)
            m_creatureList[_type][_id].GetComponent<Creature>().ReciveCreatureTransform(CreatureMessage.SET_TRANSFORM, _x, _y, _z, _rx, _ry, _rz, _w);
    }
    // -----------------------------------------------------------------------
    // Creature AI 활성화
    public void CreaturesActiveAIUpdate()
    {
        StartCoroutine("CreaturesActiveAIUpdateCoroutine");
    }
    public IEnumerator CreaturesActiveAIUpdateCoroutine()
    {
        //Debug.Log("Ready AI Coroutine");
        while(!m_initObjects)
        {
            yield return null;
        }
        ////Debug.Log("Start AI Coroutine");
        //Debug.Log("---------------------------------------------------");
        bool b_master = false;
        GameObject other = GameObject.FindWithTag(TAG.CHARACTER_OTHER);
        if (ProjectMgr.getInstance().isHost())//ProjectMgr.getInstance().getOwnID() == )
        {
            b_master = true;
           // Debug.Log("Client - AI Control");
            //
            StopCoroutine("CreaturesTransform");
            StartCoroutine("CreaturesTransform");
        }
        else
            Debug.Log("Client - AI Not Control");
        for (int creatureType = 0; creatureType < m_creatureList.Length; ++creatureType)
        {
            for (int creatureID = 0; creatureID < m_creatureList[creatureType].Length; ++creatureID)
            {
                if (m_creatureList[creatureType][creatureID] != null)
                {
                    if (m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.DRONE)
                        m_creatureList[creatureType][creatureID].GetComponent<Creature>().IdleFunc();
                    m_creatureList[creatureType][creatureID].GetComponent<Creature>().SetActiveAI(b_master);
                    // 생성된 크리쳐들의 playerOwn, Other 값 설정
                    m_creatureList[creatureType][creatureID].GetComponent<Creature>().ClientPlayersInfoInit();
                    //Debug.Log("Active : " + m_creatureList[creatureType][creatureID]);
                }
                else
                    break;
            }
        }
        // 현재 클라이언트의 플레이어 아이디값 얻기
        //i_playerID = GameObject.FindWithTag(TAG.CHARACTER_OWN).GetComponentInChildren<UserInfo>().getUserID();
        i_playerID = ProjectMgr.getInstance().getOwnID();
        // 크리쳐 활성화
        b_CreaturesActive = true;
        //Debug.Log("End AI Active");
        Debug.Log("---------------------------------------------------");
        yield return null;
    }
    // -----------------------------------------------------------------------
    // 
    public void AllCreatureFixedIdleState()
    {
        for (int creatureType = 0; creatureType < m_creatureList.Length; ++creatureType)
        {
            for (int creatureID = 0; creatureID < m_creatureList[creatureType].Length; ++creatureID)
            {
                if (m_creatureList[creatureType][creatureID] != null && m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetAlive()
                    //&& m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.OWL
                    && m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.DRONE
                    && m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.OWL_AIM)
                //&& m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.DRONE)
                {
                    m_creatureList[creatureType][creatureID].GetComponent<Creature>().StopAction();
                }
            }
        }
        // 크리쳐 비활성화
        b_CreaturesFixedState = true;
    }
    // -----------------------------------------------------------------------
    //
    public void AllCreatureClearFixedState()
    {
        for (int creatureType = 0; creatureType < m_creatureList.Length; ++creatureType)
        {
            for (int creatureID = 0; creatureID < m_creatureList[creatureType].Length; ++creatureID)
            {
                if (m_creatureList[creatureType][creatureID] != null && !m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetAction()
                    //&& m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.OWL
                    && m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.DRONE
                    && m_creatureList[creatureType][creatureID].GetComponent<Creature>().GetCreatureType() != (int)CreatureType.OWL_AIM)
                {
                    m_creatureList[creatureType][creatureID].GetComponent<Creature>().StartAction();
                }
            }
        }
        // 크리쳐 활성화
        b_CreaturesFixedState = false;
    }
    // -----------------------------------------------------------------------
    //
    public void AllCreatureTakeImmediateAction(bool _immediate)
    {
        for (int creatureType = 0; creatureType < m_creatureList.Length; ++creatureType)
        {
            for (int creatureID = 0; creatureID < m_creatureList[creatureType].Length; ++creatureID)
            {
                if (m_creatureList[creatureType][creatureID] != null && m_creatureList[creatureType][creatureID].gameObject.activeSelf)
                {
                    m_creatureList[creatureType][creatureID].GetComponent<Creature>().TakeImmediateAction(_immediate);
                }
            }
        }
    }
    // -----------------------------------------------------------------------
    //
    void DefaultItemDBData()
    {
        // PARTS
        {
            DB_DATA.CREATURE_ITEM_DATA data = new DB_DATA.CREATURE_ITEM_DATA();
            data.ITEM_TYPE = 0;
            data.GAIN_VALU = 1;
            data.CHSE_ACCL = 7.0f;
            data.CHSE_SPED = 3.0f;
            m_creatureItemDataList.Add(data);
        }
        // BATTERY
        {
            DB_DATA.CREATURE_ITEM_DATA data = new DB_DATA.CREATURE_ITEM_DATA();
            data.ITEM_TYPE = 1;
            data.GAIN_VALU = 10;
            data.CHSE_ACCL = 7.0f;
            data.CHSE_SPED = 3.0f;
            m_creatureItemDataList.Add(data);
        }
    }
    // -----------------------------------------------------------------------
    //
    void DefaultCreatureDBData()
    {
        // OWL
        {
            DB_DATA.CREATURE_COMMON_DATA data = new DB_DATA.CREATURE_COMMON_DATA();
            DB_DATA.CREATURE_DROP_DATA dropItem = new DB_DATA.CREATURE_DROP_DATA();
            data.CRIT_TYPE = 1;
            data.HLTH_PONT = 1400;
            data.RELZ_RAIS = 7;
            data.DEFE_POWR = 1;
            data.PATL_SPED = 8.0f;
            data.TRCE_SPED = 8.0f;
            data.DRON_CONT = 12;
            dropItem.DROP_MAXI = 20;
            dropItem.DROP_MINI = 10;
            m_creatureCommonDataList[data.CRIT_TYPE].Add(data);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
        }
        // DRONE
        {
            DB_DATA.CREATURE_COMMON_DATA data = new DB_DATA.CREATURE_COMMON_DATA();
            DB_DATA.CREATURE_DROP_DATA dropItem = new DB_DATA.CREATURE_DROP_DATA();
            DB_DATA.CREATURE_ATTACK_DATA attack = new DB_DATA.CREATURE_ATTACK_DATA();
            data.CRIT_TYPE = 2;
            data.HLTH_PONT = 50;
            data.RELZ_RAIS = 0;
            data.RELZ_TIME = 6.0f;
            data.DEFE_POWR = 1;
            data.PATL_SPED = 9.6f;
            data.TRCE_SPED = 9.6f;
            attack.ATCK_PONT = 20;
            attack.ATCK_DSTC = 20.0f;
            attack.ATCK_DLAY = 0.7f;
            dropItem.DROP_MAXI = 4;
            dropItem.DROP_MINI = 1;
            m_creatureCommonDataList[data.CRIT_TYPE].Add(data);
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
        }
        // FRANKY
        {
            DB_DATA.CREATURE_COMMON_DATA data = new DB_DATA.CREATURE_COMMON_DATA();
            DB_DATA.CREATURE_DROP_DATA dropItem = new DB_DATA.CREATURE_DROP_DATA();
            DB_DATA.CREATURE_ATTACK_DATA [] attack = new DB_DATA.CREATURE_ATTACK_DATA[9];
            for (int i = 0; i < 9; ++i)
                attack[i] = new DB_DATA.CREATURE_ATTACK_DATA();
            data.CRIT_TYPE = 3;
            data.HLTH_PONT = 2700;
            data.RELZ_RAIS = 25;
            data.RELZ_TIME = 0.3f;
            data.DEFE_POWR = 1;
            data.TRCE_SPED = 10.0f;
            m_creatureCommonDataList[data.CRIT_TYPE].Add(data);
            attack[0].ATCK_PONT = 20;
            attack[0].ATCK_DSTC = 3.5f;
            attack[0].ATCK_RNGE = 0.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[0]);
            attack[1].ATCK_PONT = 12;
            attack[1].ATCK_DSTC = 3.5f;
            attack[1].ATCK_RNGE = 0.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[1]); //내려찍기
            attack[2].ATCK_PONT = 30;
            attack[2].ATCK_DSTC = 5.0f;
            attack[2].ATCK_RNGE = 7.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[2]);
            attack[3].ATCK_PONT = 20;
            attack[3].ATCK_DSTC = 4.0f;
            attack[3].ATCK_RNGE = 0.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[3]);
            attack[4].ATCK_PONT = 20;
            attack[4].ATCK_DSTC = 8.0f;
            attack[4].ATCK_RNGE = 8.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[4]);
            attack[5].ATCK_PONT = 40;
            attack[5].ATCK_DSTC = 4.0f;
            attack[5].ATCK_RNGE = 0.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[5]);   //착지
            attack[6].ATCK_PONT = 30;
            attack[6].ATCK_DSTC = 10.0f;
            attack[6].ATCK_RNGE = 7.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[6]);
            attack[7].ATCK_PONT = 35;
            attack[7].ATCK_DSTC = 15.0f;
            attack[7].ATCK_RNGE = 0.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[7]);
            attack[8].ATCK_PONT = 40;
            attack[8].ATCK_DSTC = 20.0f;
            attack[8].ATCK_RNGE = 8.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[8]);
        }
        // KNIGHT
        {
            DB_DATA.CREATURE_COMMON_DATA data = new DB_DATA.CREATURE_COMMON_DATA();
            DB_DATA.CREATURE_DROP_DATA dropItem = new DB_DATA.CREATURE_DROP_DATA();
            DB_DATA.CREATURE_ATTACK_DATA [] attack = new DB_DATA.CREATURE_ATTACK_DATA[2];
            for (int i = 0; i < 2; ++i)
                attack[i] = new DB_DATA.CREATURE_ATTACK_DATA();
            data.CRIT_TYPE = 4;
            data.HLTH_PONT = 120;
            data.RELZ_RAIS = 20;
            data.RELZ_TIME = 0.4f;
            data.DEFE_POWR = 0;
            data.PATL_SPED = 4.0f;
            data.TRCE_SPED = 4.8f;
            data.BESG_SPED = 4.8f;
            data.RETR_SPED = 2.3f;
            data.THRE_SPED = 2.5f;
            data.BESG_RAIS = 5.0f;
            data.RETR_RAIS = 7.0f;
            data.THRE_RAIS = 3.0f;
            data.RETR_MOVE = 2.0f;
            data.THRE_MOVE = 2.0f;
            attack[0].ATCK_PONT = 25;
            attack[0].ATCK_DSTC = 2f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[0]);
            attack[1].ATCK_PONT = 35;
            attack[1].ATCK_DSTC = 20.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[1]);
            dropItem.DROP_MAXI = 4;
            dropItem.DROP_MINI = 1;
            m_creatureCommonDataList[data.CRIT_TYPE].Add(data);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
        }
        // BEARER
        {
            DB_DATA.CREATURE_COMMON_DATA data = new DB_DATA.CREATURE_COMMON_DATA();
            DB_DATA.CREATURE_DROP_DATA dropItem = new DB_DATA.CREATURE_DROP_DATA();
            DB_DATA.CREATURE_ATTACK_DATA [] attack = new DB_DATA.CREATURE_ATTACK_DATA[2];
            for (int i = 0; i < 2; ++i)
                attack[i] = new DB_DATA.CREATURE_ATTACK_DATA();
            data.CRIT_TYPE = 5;
            data.HLTH_PONT = 300;
            data.RELZ_RAIS = 15;
            data.RELZ_TIME = 0.6f;
            data.DEFE_POWR = 2;
            data.DEFE_SHLD = 20.0f;
            data.PATL_SPED = 4.0f;
            data.TRCE_SPED = 4.8f;
            data.BESG_SPED = 4.8f;
            data.RETR_SPED = 2.3f;
            data.THRE_SPED = 2.5f;
            data.BESG_RAIS = 4.0f;
            data.RETR_RAIS = 5.0f;
            data.THRE_RAIS = 2.0f;
            data.RETR_MOVE = 1.0f;
            data.THRE_MOVE = 1.0f;
            attack[0].ATCK_PONT = 25;
            attack[0].ATCK_DSTC = 2f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[0]);
            attack[1].ATCK_PONT = 35;
            attack[1].ATCK_DSTC = 20.0f;
            m_creatureAttackDataList[data.CRIT_TYPE].Add(attack[1]);
            dropItem.DROP_MAXI = 4;
            dropItem.DROP_MINI = 1;
            m_creatureCommonDataList[data.CRIT_TYPE].Add(data);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
            m_creatureDropDataList[data.CRIT_TYPE].Add(dropItem);
        }
        // AIM
        {
        }
    }
    // -----------------------------------------------------------------------
}
