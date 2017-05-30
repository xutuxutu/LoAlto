using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(UserInfo))]

public abstract class CharacterCtrl_Other : CharacterInfo
{
    //캐릭터 transform
    private Transform m_character;

    private bool m_isActive;
    private bool m_isDie;
    private bool m_isDown;
    private CapsuleCollider m_dieStateCollider;

    //게임 오브젝트
    private CharacterController m_controller;
    protected Animator m_animController;

    //이동관련 변수
    private bool m_isOnMovingObject;
    public float MOVE_SPEED_FORWARD = 5.0f;
    public float MOVE_SPEED_BACKWARD = 2.0f;
    public float MOVE_SPEED_RIGHT = 5.0f;
    private Vector3 m_movePos;

    //회피관련
    private bool m_isDodge;
    protected CHARACTER.DODGE_DIRECTION m_dodgeDirection;

    //애니메이션 설정 변수
    private bool m_isMove;
    private float m_animForward;
    private float m_animRight;

    //점프 관련 변수
    public float JUMP_FORCE = 5.0f;
    private bool m_isJump;
    private float m_jumpSpeed;

    //네트워크 관련 변수
    private Vector3 m_nextPosition;
    private float m_nextAngle;

    //공격관련 변수
    //private bool m_beAttacked;
    //이펙트 관련
    //사운드
    protected AudioSource[] m_audioSource;

    public abstract void init();
    // Use this for initialization
    public void initData()
    {
        m_movePos = Vector3.zero;
  
        m_isJump = false;
        m_jumpSpeed = 0.0f;

        //m_beAttacked = false;
        m_isDodge = false;

        m_isActive = false;
        m_isDie = false;
    }

    public abstract void initState(DB_DATA.CHARACTER_INFO status);
    public abstract void initSkillStatus(DB_DATA.CHARACTER_SKILL_DATA skillData, bool isActive);

    protected void initComponent()
    {
        m_character = transform.parent;

        m_audioSource = m_character.GetComponents<AudioSource>();
        m_controller = m_character.GetComponent<CharacterController>();
        m_animController = m_character.GetComponentInChildren<Animator>();
        m_dieStateCollider = m_character.GetComponent<CapsuleCollider>();
        m_dieStateCollider.enabled = false;
    }

    public void setStartTransform(Transform setTransform)
    {
        transform.parent.position = setTransform.position;
        transform.parent.rotation = setTransform.rotation;
    }

    public void setTransformInfo(Vector3 position, float yAngle, bool isMove, float[] anim)
    {
        m_nextPosition = position;
        m_nextAngle = yAngle;
        //m_isMove = isMove;
        m_animForward = anim[0];
        m_animRight = anim[1];
    }

    public void interpolateMove() //상대 플레이어의 위치,회전 보간
    {
        if(m_isActive == false)
        {
            if (m_isDie == false)
                return;
        }
        //if (m_beAttacked == true)
          //  return;

        /*
        Vector3 nextPos = Vector3.zero;
        
        if(isGround(0.3f) == null || m_isJump == true)
            nextPos = m_nextPosition;
        else
            nextPos = new Vector3(m_nextPosition.x, transform.position.y, m_nextPosition.z);

        setMoveAnim(m_animForward, m_animRight);

        if (Vector3.SqrMagnitude(nextPos) < 0.05f)
        {
            transform.position = m_nextPosition;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, nextPos, 20 * Time.deltaTime);
        */

        Vector3 moveVec = new Vector3(m_nextPosition.x, 0, m_nextPosition.z) - new Vector3(m_character.position.x, 0, m_character.position.z);
        setMoveAnim(m_animForward, m_animRight);

        if (Vector3.SqrMagnitude(moveVec) < 0.05f)
        {
            if(m_isOnMovingObject == false)
                m_character.position = m_nextPosition;
        }
        else if(Vector3.SqrMagnitude(moveVec) > 6)
        {
            m_character.position = m_nextPosition;
        }
        else
        {
            moveVec.Normalize();
            setMoveSpeed(ref moveVec);
            m_controller.Move(moveVec * Time.deltaTime);
        }

        if (m_isOnMovingObject == false || m_isJump == true)
        {
            Vector3 nextYpos = new Vector3(m_character.position.x, m_nextPosition.y, m_character.position.z);
            m_character.position = Vector3.Lerp(m_character.position, nextYpos, 20 * Time.deltaTime);
        }
    }

    public void interpolateRotation()
    {
        if (m_isActive == false)
            return;

        float rotSpeed = 30f;
        float rotAngle = m_nextAngle - m_character.rotation.eulerAngles.y;

        Quaternion rot = Quaternion.Euler(Vector3.up * rotAngle + m_character.rotation.eulerAngles);
        m_character.rotation = Quaternion.Slerp(m_character.rotation, rot, Time.deltaTime * rotSpeed);
    }

    public void setMoveSpeed(ref Vector3 moveVec)
    {
        if (m_isDodge == true)
        {
            moveVec.x *= getDodgeSpeed();
            moveVec.z *= getDodgeSpeed();
        }
        else
        {
            moveVec.x *= getCharacterMoveSpeed();
            moveVec.z *= getCharacterMoveSpeed();
        }
    }

    public void checkIsGround(float landHeight)
    {
        GameObject ground = isGround(landHeight);

        setLandPosition();

        if (ground != null)
        {
            if (m_isJump == false)
            {
                if (ground.CompareTag(TAG.MOVING_OBJECT))
                {
                    m_isOnMovingObject = true;
                    setParents(ground.transform);
                    return;
                }
            }
            else
            {
                setJumpAnim(false);
            }
        }
        m_isOnMovingObject = false;
        setParents(null);
    }

    public GameObject isGround(float dist)
    {
        LayerMask mask = (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13);
        mask = ~mask;
        RaycastHit hit;
        Debug.DrawRay(m_character.position + Vector3.up * 0.2f, -m_character.up * dist, Color.blue);
        Ray ray = new Ray(m_character.position + Vector3.up * 0.2f, -m_character.up);

        if (Physics.Raycast(ray, out hit, dist, mask))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    public void setLandPosition()
    {
        if (m_isJump == false || m_isMove == false)
        {
            LayerMask mask = (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13);
            mask = ~mask;
            RaycastHit hit;
            Ray ray = new Ray(m_character.position + Vector3.up * 0.2f, -m_character.up);

            if (Physics.Raycast(ray, out hit, 1f, mask))
            {
                m_character.position = hit.point;
            }
        }
    }

    public void setParents(Transform parents)
    {
        m_character.parent = parents;
    }

    public void Dodge(bool isDodge, CHARACTER.DODGE_DIRECTION dodgeDirecion)
    {
        m_dodgeDirection = dodgeDirecion;
        switch (m_dodgeDirection)
        {
            case CHARACTER.DODGE_DIRECTION.STOP:
            case CHARACTER.DODGE_DIRECTION.FORWARD:
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.FORWARD;
                break;

            case CHARACTER.DODGE_DIRECTION.FORWARD_RIGHT:
            case CHARACTER.DODGE_DIRECTION.RIGHT:
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.RIGHT;
                break;

            case CHARACTER.DODGE_DIRECTION.LEFT:
            case CHARACTER.DODGE_DIRECTION.FORWARD_LEFT:
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.LEFT;
                break;

            case CHARACTER.DODGE_DIRECTION.BACK:
            case CHARACTER.DODGE_DIRECTION.BACK_LEFT:
            case CHARACTER.DODGE_DIRECTION.BACK_RIGHT:
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.BACK;
                break;
        }
        setDodgeAnim(isDodge, m_dodgeDirection);
    }

    public void resetDodgeInfo()
    {
        m_isDodge = false;
        m_dodgeDirection = CHARACTER.DODGE_DIRECTION.STOP;
        setDodgeAnim(false, m_dodgeDirection);
    }

    //이동
    public float[] getMoveAnim()
    {
        float[] anim = new float[2];
        anim[0] = m_animController.GetFloat("forward");
        anim[1] = m_animController.GetFloat("right");

        return anim;
    }

    public void setMoveAnim(float forward, float right)
    {
        m_isMove = false;
        if (forward != 0 || right != 0)
            m_isMove = true;

        m_animController.SetBool("isMove", m_isMove);
        m_animController.SetFloat("forward", forward);
        m_animController.SetFloat("right", right);
    }

    public void setJumpAnim(bool isJump)
    {
        m_isJump = isJump;
        m_animController.SetBool("isJump", m_isJump);
    }

    public void setDodgeAnim(bool isDodge, CHARACTER.DODGE_DIRECTION direction)
    {
        m_isDodge = isDodge;
        if (isDodge == true)
            m_animController.SetTrigger("dodgeOrder");

        m_animController.SetBool("isDodge", isDodge);
        m_animController.SetInteger("dodgeDirection", (int)direction);
    }

    //피격
    public abstract void damaged(float damage);
    /*
    public void damaged(float damage)
    {
        m_beAttacked = true;
        m_animController.SetTrigger("isDamaged");
        Invoke("resetDamagedInfo", 0.4f);
    }*/
    public void down()
    {
        resetAttackInfo();
        m_isDodge = false;
        m_isMove = false;
        m_isDown = true;
        m_animController.SetTrigger("down");
    }

    public void resetDownInfo()
    {
        m_isDown = false;
    }
    public void die(bool isDie)
    {
        if(isDie == true)
        {
            if (m_isDie == false)
            {
                resetAttackInfo();
                m_isDie = true;
                m_isActive = false;

                m_controller.enabled = false;
                m_dieStateCollider.enabled = true;
                m_animController.SetTrigger("isDie");

                if (InGameMgr.getInstance().checkAllCharacterDead())
                    InGameMgr.getInstance().printGameOverUI(true);
            }
            else
                Debug.Log("Packet Error : Character already dead");
        }
        else
        {
            if(m_isDie == true)
            {
                m_isDie = false;
                //부활
                m_animController.SetTrigger("revival");
                Invoke("revival", 3f);
            }
            else
                Debug.Log("Packet Error : Character already Alive");
        }
    }

    public void revival()
    {
        m_isActive = true;
        m_isDie = false;
        m_controller.enabled = true;
        m_dieStateCollider.enabled = false;
    }

    public void printDeathUI()
    {
        //m_deathUI.CrossFadeAlpha(1f, 1.6f, false); 
    }

    public Vector3 getCurPosition()
    {
        if (m_character == null)
            return transform.parent.position;
        return m_character.position;
    }
    public bool isActive() { return m_isActive; }
    public bool isDie() { return m_isDie; }
    public bool isDodge() { return m_isDodge; }
    public bool isDown() { return m_isDown; }

    //setter
    public void setActive(bool isActive) { m_isActive = isActive; }

    //가상함수
    public abstract void setActive();
    public abstract void setDeActive();
    //공격
    public abstract void setAttackInfo(bool attackInfo, bool comboOrder);
    public abstract void setSmashAnim(bool isSmash, int comboNum);
    public abstract void resetAttackInfo();

    //이펙트
    public abstract void printJumpEffect();
    public abstract void printLandingEffect();
    public abstract void printRunningEffect(int type);
}
