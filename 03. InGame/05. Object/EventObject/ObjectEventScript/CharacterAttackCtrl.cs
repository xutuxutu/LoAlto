using UnityEngine;
using System.Collections;
using System;

public class CharacterAttackCtrl : ObjectEvent
{
    public enum CTRL_TYPE {  ACTIVE, DEACTIVE }
    public CTRL_TYPE m_ctrlType;

	// Use this for initialization
	void Start ()
    {
        init();
	}
	
    public override void startEvent()
    {
        if(isActive() == true)
        {
            switch(m_ctrlType)
            {
                case CTRL_TYPE.ACTIVE :
                    InGameMgr.getInstance().getOwnCharacterCtrl().setPreventAttack(false);
                    break;
                case CTRL_TYPE.DEACTIVE :
                    InGameMgr.getInstance().getOwnCharacterCtrl().setPreventAttack(true);
                    break;
            }
        }
    }

    public override void endEvent()
    {
    }
}
