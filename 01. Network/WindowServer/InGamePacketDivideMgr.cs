using UnityEngine;
using System;

public class InGamePacketDivideMgr : PacketDivideMgr
{
    public void DivideLoginPacket(ref int pID_Own, ref int pID_Other, byte[] data)
    {
        int curToken = 12;
        DividePacketArray(ref pID_Own, data, ref curToken);
        DividePacketArray(ref pID_Other, data, ref curToken);
    }

    public void DividePlayerJumpInfo(ref bool isJump, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref isJump, data, ref curTocken);
    }

    public void DividePlayerTransformInfo(ref Vector3 position, ref float yAngle, ref bool isMove, ref float[] anim, byte[] data)
    {
        int curTocken = 0;
        DividePacketArray(ref position.x, data, ref curTocken);
        DividePacketArray(ref position.y, data, ref curTocken);
        DividePacketArray(ref position.z, data, ref curTocken);

        DividePacketArray(ref yAngle, data, ref curTocken);

        DividePacketArray(ref isMove, data, ref curTocken);

        DividePacketArray(ref anim[0], data, ref curTocken);
        DividePacketArray(ref anim[1], data, ref curTocken);
    }

    public void DivideObjectInfo(ref int typeID, ref int objID, ref int buttonID, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref typeID, data, ref curTocken);
        DividePacketArray(ref objID, data, ref curTocken);
        DividePacketArray(ref buttonID, data, ref curTocken);
    }

    public void DivideObjectStateInfo( ref int typeID, ref int objID, ref int state, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref typeID, data, ref curTocken);
        DividePacketArray(ref objID, data, ref curTocken);
        DividePacketArray(ref state, data, ref curTocken);
    }

    public void DivideCreatureDamageInfo(ref int creatrueType, ref int creatureID, ref int damage, ref Vector3 atkPos, byte[] data)
    {
        int curTocken = 12; 
        DividePacketArray(ref creatrueType, data, ref curTocken);
        DividePacketArray(ref creatureID, data, ref curTocken);
        DividePacketArray(ref damage, data, ref curTocken);
        DividePacketArray(ref atkPos.x, data, ref curTocken);
        DividePacketArray(ref atkPos.y, data, ref curTocken);
        DividePacketArray(ref atkPos.z, data, ref curTocken);
    }

    public void DivideCreatureStateInfo(ref int creatrueType, ref int creatureID, ref int state, ref int targetIndex, ref float _x, ref float _y, ref float _z, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref creatrueType, data, ref curTocken);
        DividePacketArray(ref creatureID, data, ref curTocken);
        DividePacketArray(ref state, data, ref curTocken);
        DividePacketArray(ref targetIndex, data, ref curTocken);
        //
        DividePacketArray(ref _x, data, ref curTocken);
        DividePacketArray(ref _y, data, ref curTocken);
        DividePacketArray(ref _z, data, ref curTocken);
    }

    public void DivideCreatureTransformInfo(ref int _creatrueType, ref int _creatureID, ref float _x, ref float _y, ref float _z, ref float _rx, ref float _ry, ref float _rz, ref float _w, byte[] _data)
    {
        int curTocken = 0;
        DividePacketArray(ref _creatrueType, _data, ref curTocken);
        DividePacketArray(ref _creatureID, _data, ref curTocken);
        DividePacketArray(ref _x, _data, ref curTocken);
        DividePacketArray(ref _y, _data, ref curTocken);
        DividePacketArray(ref _z, _data, ref curTocken);
        DividePacketArray(ref _rx, _data, ref curTocken);
        DividePacketArray(ref _ry, _data, ref curTocken);
        DividePacketArray(ref _rz, _data, ref curTocken);
        DividePacketArray(ref _w, _data, ref curTocken);
    }

    public void DivideCharacterDamageInfo(ref int creatrueType, ref int creatureID, ref float damage, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref creatrueType, data, ref curTocken);
        DividePacketArray(ref creatureID, data, ref curTocken);
        DividePacketArray(ref damage, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_NORMAL_ATTACK_INFO_FC(ref bool attackInfo, ref bool attackType, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref attackInfo, data, ref curTocken);
        DividePacketArray(ref attackType, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SPARKY_SMASH_ATTACK_INFO_FC(ref bool isSmash, ref int comboNum, byte[] data)
    {
        int curTocken = 0;
        DividePacketArray(ref isSmash, data, ref curTocken);
        DividePacketArray(ref comboNum, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SPARKY_AIM_POINT_FC(ref Vector3 aimPoint, byte[] data)
    {
        int curTocken = 0;
        DividePacketArray(ref aimPoint.x, data, ref curTocken);
        DividePacketArray(ref aimPoint.y, data, ref curTocken);
        DividePacketArray(ref aimPoint.z, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_DODGE_INFO_FC(ref bool isDodge, ref int dodgeDirection, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref isDodge, data, ref curTocken);
        DividePacketArray(ref dodgeDirection, data, ref curTocken);
    }
    public void DIVIDE_CHARACTER_EXPLOSION_BULLET_INFO_FC(ref bool isRangeAttack, ref Vector3 firePoint, ref Vector3 fireDirection, byte[] data)
    {
        int curTocken = 12;

        DividePacketArray(ref isRangeAttack, data, ref curTocken);
        DividePacketArray(ref firePoint.x, data, ref curTocken);
        DividePacketArray(ref firePoint.y, data, ref curTocken);
        DividePacketArray(ref firePoint.z, data, ref curTocken);
        DividePacketArray(ref fireDirection.x, data, ref curTocken);
        DividePacketArray(ref fireDirection.y, data, ref curTocken);
        DividePacketArray(ref fireDirection.z, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_DIE_INFO_FC(ref bool isDie, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref isDie, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SPARKY_RAPID_FIRE_INFO_FC(ref bool useRapidFire, ref bool isFire, byte[] data)
    {
        int curTocken = 12;
        DividePacketArray(ref useRapidFire, data, ref curTocken);
        DividePacketArray(ref isFire, data, ref curTocken);
    }
    
    public void DIVIDE_GAME_RETRY_FC(ref int pID, ref bool retry, byte[] data)
    {
        int curTocken = sizeof(int) * 2;
        DividePacketArray(ref pID, data, ref curTocken);
        DividePacketArray(ref retry, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SPARKY_C4_BOMB_THROW_INFO_FC(ref int pID, ref Vector3 throwPosition, byte[] data)
    {
        int curTocken = 12;

        DividePacketArray(ref throwPosition.x, data, ref curTocken);
        DividePacketArray(ref throwPosition.y, data, ref curTocken);
        DividePacketArray(ref throwPosition.z, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO_FC(ref int pID, ref Vector3 position, ref Vector3 rotation, byte[] data)
    {
        int curTocken = 0;

        DividePacketArray(ref position.x, data, ref curTocken);
        DividePacketArray(ref position.y, data, ref curTocken);
        DividePacketArray(ref position.z, data, ref curTocken);
        DividePacketArray(ref rotation.x, data, ref curTocken);
        DividePacketArray(ref rotation.y, data, ref curTocken);
        DividePacketArray(ref rotation.z, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SPARKY_C4_BOMB_DETONATION_ORDER_FC(ref int pID, ref Vector3 detonatePosition, byte[] data)
    {
        int curTocken = 12;

        DividePacketArray(ref detonatePosition.x, data, ref curTocken);
        DividePacketArray(ref detonatePosition.y, data, ref curTocken);
        DividePacketArray(ref detonatePosition.z, data, ref curTocken);
    }


    public void DIVIDE_CHARACTER_SAM_NORMAL_ATTACK_INFO_FC(ref bool isComboOrder, ref int comboNum, byte[] data)
    {
        int curTocken = 12;

        DividePacketArray(ref isComboOrder, data, ref curTocken);
        DividePacketArray(ref comboNum, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SAM_STEAM_BLOW_INFO_FC(ref bool useSteamBlow, ref bool isContact, byte[] data)
    {
        int curTocken = 12;

        DividePacketArray(ref useSteamBlow, data, ref curTocken);
        DividePacketArray(ref isContact, data, ref curTocken);
    }

    public void DIVIDE_CHARACTER_SAM_BATTLE_IDLE_FC(ref bool isBattle, byte[] data)
    {
        int curTocken = 12;

        DividePacketArray(ref isBattle, data, ref curTocken);
    }

    public void DIVIDE_TRIGGER_ACTIVE_FC(ref int triggerNum, byte[] data)
    {
        int curTocken = 12;

        DividePacketArray(ref triggerNum, data, ref curTocken);
    }
}
