using UnityEngine;
using System.Collections;

public class ObjectStartConditionActiveCtrl : ObjectEvent
{

    public enum ACTION_TYPE { ACTIVE, DEACTIVE, }

    public ACTION_TYPE m_actionType;
    public StartCondition m_startCondition;

    void Start()
    {
        init();
    }

    public override void startEvent()
    {
        switch (m_actionType)
        {
            case ACTION_TYPE.ACTIVE:
                m_startCondition.setActive(true);
                break;
            case ACTION_TYPE.DEACTIVE:
                m_startCondition.setActive(false);
                break;
        }
    }

    public override void endEvent()
    {
    }
}
