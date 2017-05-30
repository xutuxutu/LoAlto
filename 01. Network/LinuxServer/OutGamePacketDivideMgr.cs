using UnityEngine;
using System.Collections;

public class OutGamePacketDivideMgr : PacketDivideMgr
{
    public void DIVIDE_CHARACTER_SELECT_INFO_FC(ref int pID, ref int charType, ref bool isSelect, ref bool isSuccess, byte[] data)
    {
        int curToken = sizeof(int);
        DividePacketArray(ref pID, data, ref curToken);
        DividePacketArray(ref charType, data, ref curToken);
        DividePacketArray(ref isSelect, data, ref curToken);
        DividePacketArray(ref isSuccess, data, ref curToken);
    }

    public void DIVIDE_PLAYER_READY_FC(ref int pID, ref bool isReady, byte[] data)
    {
        int curToken = sizeof(int);
        DividePacketArray(ref pID, data, ref curToken);
        DividePacketArray(ref isReady, data, ref curToken);
    }

    public void DIVIDE_ALL_USER_ENTER_FC(ref int[] pID, byte[] data)
    {
        int curToken = sizeof(int);
        for(int i = 0; i < pID.Length; ++i)
            DividePacketArray(ref pID[i], data, ref curToken);
    }
}
