using UnityEngine;
using System.Collections;

namespace CHARACTER
{
    public enum TYPE { NONE, SAM, SPARKY }
    public enum STATE { IDLE, MOVE, JUMP_NOMAL, JUMP_DOUBLE, ATTACK, ATTACKED, DODGE, DEAD, REVIVE, }
    public enum DODGE_DIRECTION { FORWARD ,RIGHT, LEFT, BACK, FORWARD_LEFT, FORWARD_RIGHT, BACK_RIGHT, BACK_LEFT, STOP, }
    public enum LAYER { BASE, DAMAGED, RUNNING, UPPERBODY, }
}

public abstract class CharacterInfo : MonoBehaviour
{
    private CHARACTER.TYPE m_characterType; //캐릭터 종류
    private CHARACTER.STATE m_characterState; //캐릭터 상태
    private ATTACK.STATE_DISORDER m_characterStateDisorder;

    //DB
    private float MAX_HP;               //최대 체력
    private float m_curHP;              //현재 체력
    private float MAX_EP;               //최대 에너지 포인트
    private float m_curEP;              //현재 에너지 포인트

    private float m_defPoint;           //방어력

    private float m_healingPerSecond;   //초당 체력 재생률
    private float m_regenUnableTime;    //체력 회복 불가능 시간.

    private float MOVE_SPEED;           //이동속도
    private float JUMP_SPEED;           //점프속도
    private float GRAVITY;              //중력 값
    private float DODGE_SPEED;          //회피 속도

    private float m_finalAttackedTime;  //마지막으로 피격당한 시간.
    private int m_partsNum;             //소지한 파츠갯수.

    public void initType(CHARACTER.TYPE characterType)
    {
        m_characterType = characterType;
    }

    protected void initState(float maxHP, float maxEP, float defPoint, float RUT, float HPS, float mvSpd, float jSpd, float gravity, float dgSPD)
    {
        MAX_HP = maxHP;
        m_curHP = maxHP;
        MAX_EP = maxEP;
        m_curEP = maxEP;
        m_defPoint = defPoint;
        m_regenUnableTime = RUT;
        m_healingPerSecond = HPS;

        MOVE_SPEED = mvSpd;
        JUMP_SPEED = jSpd;
        GRAVITY = gravity;
        DODGE_SPEED = dgSPD;

        m_finalAttackedTime = 0.0f;

        m_characterState = CHARACTER.STATE.IDLE;
        m_characterStateDisorder = ATTACK.STATE_DISORDER.NONE;
    }
    //getter
    public CHARACTER.TYPE getCharacterType() { return m_characterType; }
    public CHARACTER.STATE getCharacterState() { return m_characterState; }
    public ATTACK.STATE_DISORDER getCharacterStateDisorder() { return m_characterStateDisorder; }

    public float getMaxHP() { return MAX_HP; }
    public float getCurHP() { return m_curHP; }

    public float getMaxEP() { return MAX_EP; }
    public float getCurEP() { return m_curEP; }

    public float getCurHP_Percentage() { return m_curHP / MAX_HP; }
    public float getCurEP_Percentage() { return m_curEP / MAX_EP; }

    public float getHealingPerSecond() { return m_healingPerSecond; }
    public float getRegenUnableTime() { return m_regenUnableTime; }

    public float getDefPoint() { return m_defPoint; }

    public void setAttackedTime(float time) { m_finalAttackedTime = time; }

    public float getMoveSpeed() { return MOVE_SPEED; }

    public float getJumpSpeed() { return JUMP_SPEED; }
    public float getGravity() { return GRAVITY; }

    public float getDodgeSpeed() { return DODGE_SPEED; }

    public int getPartsNumber() { return m_partsNum; }

    //setter
    public void setCharacterStateDisorder(ATTACK.STATE_DISORDER stateDisorder) { m_characterStateDisorder = stateDisorder; }
    public void setHP(float amount) { m_curHP = amount; }
    public void setEP(float amount) { m_curEP = amount; }
    public void setPartsNum(int num) { m_partsNum = num; }

    public void increaseHP(float amount)
    {
        m_curHP += amount;
        if(m_curHP >= MAX_HP)
            m_curHP = MAX_HP;
    }

    public void decreaseHP(float damage)
    {
        m_curHP -= damage;
        if (m_curHP <= 0)
            m_curHP = 0;
    }

    public void increaseEP(float amount)
    {
        m_curEP += amount;
        if (m_curEP >= MAX_EP)
            m_curEP = MAX_EP;
    }

    public void decreaseEP(float amount)
    {
        m_curEP -= amount;
        if (m_curEP <= 0)
            m_curEP = 0;
    }

    public void increasePartsNum(int num) { m_partsNum += num; }
    public void decreasePartsNum(int num) { m_partsNum -= num; }

    //setter
    public void setCharacterState(CHARACTER.STATE state) { m_characterState = state; }

    //abstract
    public abstract float getCharacterMoveSpeed();
}
