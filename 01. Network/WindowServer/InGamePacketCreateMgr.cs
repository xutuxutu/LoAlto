using UnityEngine;

public class InGamePacketCreateMgr : PacketCreateMgr
{
    public InGamePacketCreateMgr()
    {
        init();
    }

    protected override void setSendPacketDictionary()
    {
        //connect
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_LOGIN_EM, REQUEST_PLAYER_LOGIN_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_READY_EM, REQUEST_PLAYER_READY_FC);

        //ingame
        //캐릭터 - 공통
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_TRANSFORM_EM, REQUEST_TRANSFORM_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_JUMP_EM, REQUEST_JUMP_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DAMAGE_INFO, REQUEST_CHARACTER_DAMAGE_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DODGE_INFO_EM, REQUEST_CHARACTER_DODGE_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DIE_INFO_EM, REQUEST_CHARACTER_DIE_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_GAME_RETRY_EM, REQUEST_PLAYER_GAME_RETRY_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DOWN_INFO_EM, REQUEST_CHARACTER_DOWN_INFO_FC);

        //오브젝트
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_ACTIVE_EM, REQUEST_OBJECT_ACTIVE_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_DEACTIVE_EM, REQUEST_OBJECT_DEACTIVE_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_STATE_EM, REQUEST_OBJECT_STATE_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_TRIGGER_ACTIVE_EM, REQUEST_TIRRGER_ACTIVE_FC);

        //크리쳐
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_DAMAGE_INFO, REQUEST_CREATURE_DAMAGE_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_STATE_INFO, REQUEST_CREATURE_STATE_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO, REQUEST_CREATURE_TRANSFORM_INFO_FC);

        //캐릭터 - 스파키
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_NORMAL_ATTACK_INFO, REQUEST_CHARACTER_NORMAL_ATTACK_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO, REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_AIM_POINT_EM, REQUEST_CHARACTER_SPARKY_AIM_POINT_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_EM, REQUEST_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM, REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_THROW_INFO_EM, REQUEST_CHARACTER_SPARKY_C4_BOMB_THROW_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM, REQUEST_CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_DETONATION_ORDER_EM, REQUEST_CHARACTER_SPARKY_C4_BOMB_DETONATION_ORDER_FC);

        //캐릭터 - 샘
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_NORAML_ATTACK_INFO_EM, REQUEST_CHARACTER_SAM_NORMAL_ATTACK_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_EM, REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_BATTLE_IDLE_INFO_EM, REQUEST_CHARACTER_SAM_BATTLE_IDLE_INFO_FC);
        SendPacketDictionary.Add((int)NET_INGAME.SEND.PACKET_TYPE.REQUEST_CAHRACTER_SAM_PULVERIZE_INFO_EM, REQUEST_CHARACTER_SAM_PUVERIZE_INFO_FC);
    }

    public byte[] REQUEST_PLAYER_LOGIN_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.PLAYER_LOGIN];
        int curToken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_LOGIN_EM, ref curToken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.PLAYER_LOGIN, ref curToken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curToken);

        return data;
    }
    
    public byte[] REQUEST_PLAYER_READY_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.PLAYER_READY];
        int curToken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_READY_EM, ref curToken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.PLAYER_READY, ref curToken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curToken);

        return data;
    }

    public byte[] REQUEST_TRANSFORM_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.PLAYER_TRANSFORM];
        int curTocken = 0;

        Vector3 position = InGameMgr.getInstance().getOwnCharacterPosition();
        Quaternion rotation = InGameMgr.getInstance().getOwnCharacterRotation();
        float[] anim = InGameMgr.getInstance().getOwnCharacterCtrl().getMoveAnim();

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_TRANSFORM_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.PLAYER_TRANSFORM, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);

        MakeByteArray(ref data, position.x, ref curTocken);
        MakeByteArray(ref data, position.y, ref curTocken);
        MakeByteArray(ref data, position.z, ref curTocken);

        MakeByteArray(ref data, rotation.eulerAngles.y, ref curTocken);

        MakeByteArray(ref data, InGameMgr.getInstance().getOwnCharacterCtrl().isMove(), ref curTocken);

        MakeByteArray(ref data, anim[0], ref curTocken);
        MakeByteArray(ref data, anim[1], ref curTocken);

        return data;
    }

    public byte[] REQUEST_JUMP_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.PLAYER_JUMP];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_JUMP_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.PLAYER_JUMP, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);

        return data;
    }

    public byte[] REQUEST_OBJECT_ACTIVE_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.OBJECT_ACTIVE];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_ACTIVE_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.OBJECT_ACTIVE, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[2], ref curTocken);

        return data;
    }

    public byte[] REQUEST_OBJECT_DEACTIVE_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.OBJECT_DEACTIVE];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_DEACTIVE_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.OBJECT_DEACTIVE, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[2], ref curTocken);

        return data;
    }

    public byte[] REQUEST_OBJECT_STATE_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.OBJECT_STATE];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_STATE_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.OBJECT_STATE, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[2], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CREATURE_DAMAGE_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CREATURE_DAMAGE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_DAMAGE_INFO, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CREATURE_DAMAGE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curTocken);                                //몬스터 타입
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);                                //몬스터 종류
        MakeByteArray(ref data, (int)m_parameter[2], ref curTocken);                                //피해량

        return data;
    }

    public byte[] REQUEST_CREATURE_STATE_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CREATURE_STATE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_STATE_INFO, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CREATURE_STATE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[2], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[3], ref curTocken);
        // x, y, z
        MakeByteArray(ref data, (float)m_parameter[4], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[5], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[6], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_DAMAGE_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DAMAGE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DAMAGE_INFO, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DAMAGE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[2], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[3], ref curTocken);

        return data;
    }
    // 크리쳐들의 Transform 정보를 담는 패킷 생성
    public byte[] REQUEST_CREATURE_TRANSFORM_INFO_FC()
    {
        // TCP
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CREATURE_TRANSFORM_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CREATURE_TRANSFORM_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter_[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter_[1], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter_[2], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter_[3], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter_[4], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter_[5], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter_[6], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter_[7], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter_[8], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter_[9], ref curTocken);

        return data;
        // 데이터 크기만큼 데이터 생성( 패킷종류 + 패킷Index + 패킷크기 + 크리쳐종류 + 크리쳐아이디 + 크리쳐 Position, Rotate 정보 )
        /*int i_dataSize = sizeof(int) + sizeof(int) + sizeof(int) + (sizeof(int) * 2 + sizeof(float) * CreatureMgr.getInstance().GetCreatureCount());
        byte[] data = new byte[i_dataSize];
        int curTocken = 0;
        // 패킷종류
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO, ref curTocken);
        // 패킷Index
        MakeByteArray(ref data, m_udp_CreatureTransformIndex, ref curTocken);
        // 패킷크기
        MakeByteArray(ref data, i_dataSize, ref curTocken);
        // 크리쳐 정보
        CreatureMgr creatureMgr = CreatureMgr.getInstance();
        for (int creatureType = 0; creatureType < creatureMgr.GetTypeCount(); ++creatureType)
        {
            for(int creatureIndex = 0; creatureIndex < creatureMgr.GetCreatureCount(creatureType); ++creatureIndex)
            {
                Vector3 vec3_pos = creatureMgr.GetCreaturePosition(creatureType, creatureIndex);
                Quaternion quat_rot = creatureMgr.GetCreatureRotate(creatureType, creatureIndex);
                // Position
                MakeByteArray(ref data, vec3_pos.x, ref curTocken);
                MakeByteArray(ref data, vec3_pos.y, ref curTocken);
                MakeByteArray(ref data, vec3_pos.z, ref curTocken);
                // Rotate
                MakeByteArray(ref data, quat_rot.x, ref curTocken);
                MakeByteArray(ref data, quat_rot.y, ref curTocken);
                MakeByteArray(ref data, quat_rot.z, ref curTocken);
                MakeByteArray(ref data, quat_rot.w, ref curTocken);
            }
        }
        // Packet Index 증가
        ++m_udp_CreatureTransformIndex;
        // 데이터 반환
        return data;*/
        //return CreatureMgr.getInstance().GetSendPacketData(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CREATURE_TRANSFORM_INFO, m_udp_CreatureTransformIndex);
    }

    public byte[] REQUEST_CHARACTER_NORMAL_ATTACK_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_NORMAL_ATTACK_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_NORMAL_ATTACK_INFO, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_NORMAL_ATTACK_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[1], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_SMASH_ATTACK_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_SMASH_ATTACK_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SPARKY_AIM_POINT_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_AIM_POINT_INFO];
        int curTocken = 0;
        Vector3 aimPoint = (Vector3)m_parameter[0];
         
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_AIM_POINT_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_AIM_POINT_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, aimPoint.x, ref curTocken);
        MakeByteArray(ref data, aimPoint.y, ref curTocken);
        MakeByteArray(ref data, aimPoint.z, ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_DODGE_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DODGE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DODGE_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DODGE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_EXPLOSION_BULLET_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_EXPLOSION_BULLET_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[2], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[3], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[4], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[5], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[6], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_DIE_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DIE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DIE_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DIE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_RAPID_FIRE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_RAPID_FIRE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[1], ref curTocken);

        return data;
    }

    public byte[] REQUEST_PLAYER_GAME_RETRY_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.PLAYER_GAME_RETRY];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_PLAYER_GAME_RETRY_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.PLAYER_GAME_RETRY, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SPARKY_C4_BOMB_THROW_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_C4_BOMB_THROW_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_THROW_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_C4_BOMB_THROW_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[2], ref curTocken);

        return data;
    }


    public byte[] REQUEST_CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[2], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[3], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[4], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[5], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SPARKY_C4_BOMB_DETONATION_ORDER_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_C4_DETONATION_ORDER];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_DETONATION_ORDER_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SPARKY_C4_DETONATION_ORDER, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[1], ref curTocken);
        MakeByteArray(ref data, (float)m_parameter[2], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SAM_STEAM_BLOW_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SAM_STEAM_BLOW_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[1], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SAM_NORMAL_ATTACK_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SAM_NORAML_ATTACK_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_NORAML_ATTACK_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SAM_NORAML_ATTACK_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[1], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_SAM_BATTLE_IDLE_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SAM_BATTLE_IDLE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_BATTLE_IDLE_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_SAM_BATTLE_IDLE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (bool)m_parameter[0], ref curTocken);

        return data;
    }

    public byte[] REQUEST_CHARACTER_DOWN_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DOWN_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DOWN_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CHARACTER_DOWN_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);

        return data;
    }
    public byte[] REQUEST_CHARACTER_SAM_PUVERIZE_INFO_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.CAHRACTER_SAM_PULVERIZE_INFO];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_CAHRACTER_SAM_PULVERIZE_INFO_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.CAHRACTER_SAM_PULVERIZE_INFO, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);

        return data;
    }

    public byte[] REQUEST_TIRRGER_ACTIVE_FC()
    {
        byte[] data = new byte[NET_INGAME.SEND.PACKET_SIZE.TRIGGER_ACTIVE];
        int curTocken = 0;

        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_TYPE.REQUEST_TRIGGER_ACTIVE_EM, ref curTocken);
        MakeByteArray(ref data, NET_INGAME.SEND.PACKET_SIZE.TRIGGER_ACTIVE, ref curTocken);
        MakeByteArray(ref data, ProjectMgr.getInstance().getOwnID(), ref curTocken);
        MakeByteArray(ref data, (int)m_parameter[0], ref curTocken);

        return data;
    }
}
