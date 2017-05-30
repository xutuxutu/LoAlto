using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RootMotion.FinalIK;
using System;
using DB_DATA;

namespace CHARACTER_SPARKY
{
    public enum SKILL { EXPLOSION_B, C4_BOMB, TESLA_B, RAPID_FIRE }
    public enum AUDIO_SOURCE { FIRE, SPIN, FOOT_STEP }
}

struct NET_SPARKY_ATTACK_INFO
{
    public const bool IS_ATTACK = true;
    public const bool IS_FIRE = false;
}

public class Character_Own_Sparky : CharacterCtrl_Own
{
    //이동 속도
    private float m_walkSpeed;

    //EP소모량
    private float m_explostionBulletEp_Consumtion = 12;
    private float m_c4BombEP_Consumption = 13;
    private float m_teslaBulletEP_Consumption = 10;
    private float m_rapidFireEP_Consumption = 10;
    //속사
    private bool m_isRapidFire;
    private float m_rapidFireWarmUpTime;
    private float m_rapidFireDurationTime;
    //C4폭탄
    private bool m_throwC4Bomb;
    private Transform m_c4ThrowPosition;
    private C4_Bomb m_curC4Bomb;

    //총 머터리얼
    private Material m_gunMaterial;
    //총 위치
    private Transform m_gunPosition;

    //총구 회전 관련
    private Transform m_gunMuzzle;
    private float m_curRotSpeed;
    private float m_gunRotSpeed;
    private float m_gunMaxRotSpeed;
    private float m_gunMaxRotSpeed_RapidFire;
    private float m_gunIncreseRotSpeed;
    private float m_gunDecreseRotSpeed;

    private Transform m_viewTarget;
    private Transform m_firePoint;
    //발사 이펙트
    private GameObject m_fireEffect;
    public GameObject m_emptyGunShell;
    //발자국 이펙트 발생 위치
    private Transform m_leftFoot;
    private Transform m_rightFoot;

    //게이지 UI
    private Image m_overHeatGuage;
    private Image m_rapidFireGuage;

    //공격관련 변수
    private float m_normalAtkPoint;
    private bool m_isFire;
    private bool m_rangeAttack;
    private bool m_closeAttack;
    private bool m_comboOrder;
    private int m_smashComboNum;
    private int SMASH_MAX_COMBO;

    private ATTACK.TYPE m_curAttackType;

    private float attackRange = 20.0f;
    private Vector3 m_fireDirection;
    private Vector3 m_hitPoint;
    private Vector3 m_hitPointNormal;
    private GameObject m_hitTarget;

    private float MAX_OVER_HEAT_GUAGE;
    private float m_curOverHeatGuage;

    private float m_increaseOverHeatGuage;
    private float m_decreaseOverHeatGuage;
    private bool m_isOverHeat;

    //총구 반동
    private float m_gunRecoilY;
    private float m_gunRecoilX;

    //IK
    private AimIK m_aimIK;
    private FullBodyBipedIK m_fbbIK;
    private LookAtIK m_lookAtIK;
    private IKEffector leftHand { get { return m_fbbIK.solver.leftHandEffector; } }
    private IKEffector rightHand { get { return m_fbbIK.solver.rightHandEffector; } }

    private Quaternion leftHandRotationRelative;

    public override void init()
    {
        initType(CHARACTER.TYPE.SPARKY);
        setCharacterState(CHARACTER.STATE.IDLE);

        initIK();
        initComponent();

        m_rapidFireWarmUpTime = 0.3f;
        m_rapidFireDurationTime = 5.0f;
    }

    public override void initState(DB_DATA.CHARACTER_INFO status)
    {
        base.initState(status.HLTH_PONT, status.ENRG_PONT, status.DFND_PONT, status.UABL_RGNT, status.HEAL_SCND, status.RUNN_SPED, status.JUMP_SPED, status.GRVT_PONT, status.DDGE_SPED);
        m_walkSpeed = status.WALK_SPED;

        initData();
        initAttackInfo();
    }
    public override void setNormalAttackPoint(int type, int atkPoint) { m_normalAtkPoint = atkPoint; }

    public void initState(float maxHP, float maxEP, float atkPoint, float defPoint, float RUT, float HPS, float runSpd, float walkSpd, float jSpd, float gravity, float dgSPD)
    {
        base.initState(maxHP, maxEP, defPoint, RUT, HPS, runSpd, jSpd, gravity, dgSPD);
        m_walkSpeed = walkSpd;
        m_normalAtkPoint = atkPoint;

        initData();
        initAttackInfo();
    }

    public override void initSkillStatus(CHARACTER_SKILL_DATA skillData, bool isActive)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)skillData.SKIL_TYPE;
        m_checkSkillCoolTime.initSkillInfo(skillData.SKIL_TYPE, (int)skillData.COOL_TIME, isActive);

        switch(skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B :
                m_explostionBulletEp_Consumtion = skillData.USNG_MPNT;
                ExplosionBullet.initStatus(skillData.OBJT_SPED, skillData.GRVT_POWR);
                ExplosionBulletHit.initStatus(skillData.BOMB_ATPT, skillData.ATCK_RNGE);
                break;
            case CHARACTER_SPARKY.SKILL.C4_BOMB :
                m_c4BombEP_Consumption = skillData.USNG_MPNT;
                C4_Bomb.initStatus(skillData.BOMB_ATPT, skillData.THRW_POWR, skillData.ATCK_RNGE);
                break;
            case CHARACTER_SPARKY.SKILL.TESLA_B :
                //m_teslaBulletEP_Consumption = skillData.USNG_MPNT;
                break;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE :
                //m_rapidFireEP_Consumption = skillData.USNG_MPNT;
                break;
        }
    }

    public override float getCharacterMoveSpeed()
    {
        if (m_rangeAttack)
            return m_walkSpeed;
        else
            return getMoveSpeed();
    }

    public new void initComponent()
    {
        base.initComponent();

        m_checkSkillCoolTime = new CheckSkillCoolTime_Sparky();
        Transform[] allChilds = transform.parent.GetComponentsInChildren<Transform>();

        for (int i = 0; i < allChilds.Length; i++)
        {
            if (allChilds[i].name == OBJECT_NAME.GUN_MUZZLE)
                m_gunMuzzle = allChilds[i];

            if (allChilds[i].name == OBJECT_NAME.FIRE_POINT)
                m_firePoint = allChilds[i];

            if (allChilds[i].name == OBJECT_NAME.VIEW_TARGET)
                m_viewTarget = allChilds[i];

            if (allChilds[i].name.Equals(OBJECT_NAME.FIRE_EFFECT))
                m_fireEffect = allChilds[i].gameObject;

            if (allChilds[i].name.Equals(OBJECT_NAME.RIGHT_FOOT))
                m_rightFoot = allChilds[i];

            if (allChilds[i].name.Equals(OBJECT_NAME.LEFT_FOOT))
                m_leftFoot = allChilds[i];

            if (allChilds[i].name.Equals(OBJECT_NAME.GUN_POSITION))
                m_gunPosition = allChilds[i];

            if (allChilds[i].name.Equals(OBJECT_NAME.C4_THROW_POSITION))
                m_c4ThrowPosition = allChilds[i];
        }

        m_gunMaterial = m_gunMuzzle.GetComponent<Renderer>().material;
        m_overHeatGuage = GameObject.Find(OBJECT_NAME.OVERHEAT_GUAGE).GetComponent<Image>();
        m_rapidFireGuage = GameObject.Find(OBJECT_NAME.RAPID_FIRE_GUAGE).GetComponent<Image>();
        setLayerWeight(CHARACTER.LAYER.UPPERBODY, 0f);
    }

    public void initAttackInfo()
    {
        Skill_First = skill_ExplosionBullet;
        Skill_Second = skill_C4Bomb;
        Skill_Third = skill_TeslaBullet;
        Skill_Fourth = skill_rapidFire;

        setPossibleAttack(true);
        //setRangeAttackAnimType();
        //스매쉬공격 관련 변수
        m_smashComboNum = 0;
        SMASH_MAX_COMBO = 2;
        m_comboOrder = false;
        //일반탄 총구 발사 이펙트
        m_fireEffect.SetActive(false);
        m_emptyGunShell.SetActive(false);
        //오버히트
        MAX_OVER_HEAT_GUAGE = 100.0f;
        m_curOverHeatGuage = 0.0f;
        //오버히트 증감량
        m_increaseOverHeatGuage = 20f;
        m_decreaseOverHeatGuage = 40f;
        m_isOverHeat = false;
        //총구회전
        m_curRotSpeed = 0f;
        m_gunRotSpeed = 0.0f;
        m_gunMaxRotSpeed = 20.0f;
        m_gunMaxRotSpeed_RapidFire = 30.0f;
        //회전 증감량
        m_gunIncreseRotSpeed = 100.0f;
        m_gunDecreseRotSpeed = 100.0f;
        //스킬
        m_isRapidFire = false;
        m_throwC4Bomb = false;
#if SERVER_ON
        //hitPoint 송신 코루틴 
        StartCoroutine(sendAimPoint());
#endif
    }

    public void initIK()
    {
        m_aimIK = GetComponent<AimIK>();
        m_fbbIK = GetComponent<FullBodyBipedIK>();
        m_lookAtIK = GetComponent<LookAtIK>();

        m_aimIK.Disable();
        m_fbbIK.Disable();
        m_lookAtIK.Disable();

        m_fbbIK.solver.OnPostUpdate += OnPostFBBIK;
    }

    public void setAimPoint()
    {
        //find out how the left hand is positioned relative to the right hand position
        Vector3 toLeftHand = leftHand.bone.position - rightHand.bone.position;
        Vector3 toLeftHandRelative = rightHand.bone.InverseTransformDirection(toLeftHand);

        leftHandRotationRelative = Quaternion.Inverse(rightHand.bone.rotation) * leftHand.bone.rotation;

        Vector3 ikPosition = m_aimIK.solver.GetIKPosition();
        ikPosition = Vector3.Lerp(ikPosition, m_hitPoint, 100 * Time.deltaTime);

        m_aimIK.solver.IKPosition = ikPosition;
        m_lookAtIK.solver.IKPosition = m_aimIK.solver.IKPosition;

        m_aimIK.solver.Update();

        // position the left hand on the gun
        leftHand.position = rightHand.bone.position + rightHand.bone.TransformDirection(toLeftHandRelative);
        leftHand.positionWeight = 1f;

        //Making sure right hand won't budge during solving
        rightHand.position = rightHand.bone.position;
        rightHand.positionWeight = 1f;
        m_fbbIK.solver.GetLimbMapping(FullBodyBipedChain.RightArm).maintainRotationWeight = 1f;

        m_fbbIK.solver.Update();

        m_lookAtIK.solver.Update();
    }

    public void Update()
    {
        jumping();
        move();
        setHitPoint();
        normalAttack();
        setHeatPoint();
        checkIsGround(0.6f);

        selfDamage();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isActive() == true)
        {
            if (isDodge() == false && isDown() == false)
                setAimPoint();
        }

        m_curRotSpeed += m_gunRotSpeed;
        m_gunMuzzle.Rotate(Vector3.up * m_curRotSpeed);
    }

    //ik
    private void OnPostFBBIK()
    {
        leftHand.bone.rotation = rightHand.bone.rotation * leftHandRotationRelative;
    }
    //----------------------------------------------------------------------------------------------------------------------//
    //원거리 공격 관련 코드
    public override void attack()
    {
        if (checkPossibleAttack() == true)
        {
            if (m_isOverHeat == true)
                return;
            else
            {
                m_curAttackType = ATTACK.TYPE.NOMAL;
                setPossibleJump(false); //점프 불가
                m_rangeAttack = true;
                setAttackAnim(ATTACK.TYPE.NOMAL);
                playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_UP);
#if SERVER_ON
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_NORMAL_ATTACK_INFO, true, NET_SPARKY_ATTACK_INFO.IS_ATTACK);
#endif
            }
        }
    }
       
    public override void smash()
    {
        if (m_closeAttack == false)
        {
            m_curAttackType = ATTACK.TYPE.SMASH;
            m_closeAttack = true;
            m_smashComboNum = 1;
            setAttackAnim(ATTACK.TYPE.SMASH);
            //WindowServerMgr.GetInstance().SendPacket(SendPacketType.REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO, m_isSmash, m_smashComboNum);
        }
        else
        {
            if (m_smashComboNum == SMASH_MAX_COMBO)
                return;
            m_comboOrder = true;
            m_smashComboNum += 1;
            setAttackAnim(ATTACK.TYPE.SMASH);
            //WindowServerMgr.GetInstance().SendPacket(SendPacketType.REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO, m_isSmash, m_smashComboNum);
        }
    }

    public void skill_ExplosionBullet()
    {
        if (checkPossibleUseSkill(m_explostionBulletEp_Consumtion, (int)CHARACTER_SPARKY.SKILL.EXPLOSION_B) == false)
            return;

        if (m_checkSkillCoolTime.checkSkillCoolTime((int)CHARACTER_SPARKY.SKILL.EXPLOSION_B) == true)
        {
            InGameMgr.getInstance().printErrorMessage("아직 사용하실 수 없습니다.", true);
            return;
        }

        if (m_rangeAttack == false)
        {
            if (isPossibleJump() == false)
                return;
        }

        if (m_rangeAttack == true)
        {
            playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_DOWN);
            m_curAttackType = ATTACK.TYPE.NONE;
            m_rangeAttack = false;
            m_isFire = false;
            setAttackAnim(ATTACK.TYPE.NOMAL);
        }

        useSkill((int)CHARACTER_SPARKY.SKILL.EXPLOSION_B);

        Vector3 gunVector = (m_firePoint.position - m_gunPosition.position).normalized;
        gunVector = transform.InverseTransformDirection(gunVector);

        Vector3 moveVec = (m_hitPoint - m_firePoint.position).normalized;
        ObjectPool_Sparky.getInstance().fireExplosionBullet(m_firePoint.position, moveVec);

        StartCoroutine(m_checkSkillCoolTime.printSkillCoolTime((int)CHARACTER_SPARKY.SKILL.EXPLOSION_B));
        StartCoroutine(explosionRecoil());
        setAnimTrigger("Skill_ExplosionBullet");
        decreaseEP(20);

#if SERVER_ON
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_EM, true, m_firePoint.position.x, m_firePoint.position.y, m_firePoint.position.z, moveVec.x, moveVec.y, moveVec.z);
#endif
    }

    public void skill_C4Bomb()
    {
        float epComsum = m_c4BombEP_Consumption;

        if (m_throwC4Bomb == true)
            epComsum = 0;

        if (m_checkSkillCoolTime.checkSkillCoolTime((int)CHARACTER_SPARKY.SKILL.C4_BOMB) == true)
        {
            InGameMgr.getInstance().printErrorMessage("아직 사용하실 수 없습니다.", true);
            return;
        }

        if (checkPossibleUseSkill(epComsum, (int)CHARACTER_SPARKY.SKILL.C4_BOMB) == false)
            return;

        if (m_rangeAttack == false)
        {
            if (isPossibleJump() == false)
                return;
        }
        /*
        if (m_rangeAttack == true)
        {
            playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_DOWN);
            m_curAttackType = ATTACK.TYPE.NONE;
            m_rangeAttack = false;
            m_isFire = false;
            setAttackAnim(ATTACK.TYPE.NOMAL);
        }*/

        useSkill((int)CHARACTER_SPARKY.SKILL.C4_BOMB);
        //setLayerWeight(CHARACTER.LAYER.UPPERBODY, 1f);

        if (m_throwC4Bomb == false)
        {
            decreaseEP(m_c4BombEP_Consumption);
            Vector3 gunVector = (m_firePoint.position - m_gunPosition.position).normalized;
            gunVector = transform.InverseTransformDirection(gunVector);

            Vector3 moveVec = (m_hitPoint - m_firePoint.position).normalized;

            m_throwC4Bomb = true;
            m_animController.SetTrigger("throwC4Bomb");
            ((CheckSkillCoolTime_Sparky)m_checkSkillCoolTime).throwC4Bomb();
            m_curC4Bomb = ObjectPool_Sparky.getInstance().throwC4Bomb(m_c4ThrowPosition.position, moveVec);

#if SERVER_ON
            InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_THROW_INFO_EM, m_c4ThrowPosition.position.x, m_c4ThrowPosition.position.y, m_c4ThrowPosition.position.z);
#endif
        }
        else
        {
            if (m_curC4Bomb.isPossibleDetonation() == true)
                skill_C4Bomb_Detonate();
        }
    }

    public void skill_C4Bomb_Detonate()
    {
        m_throwC4Bomb = false;
        m_animController.SetTrigger("detonateC4Bomb");
        m_curC4Bomb.detonation();
        m_curC4Bomb = null;
        StartCoroutine(m_checkSkillCoolTime.printSkillCoolTime((int)CHARACTER_SPARKY.SKILL.C4_BOMB));
    }

    public void skill_TeslaBullet()
    {

    }

    public void skill_rapidFire()
    {
        if (checkPossibleUseSkill(m_rapidFireEP_Consumption, 3) == false)
            return;

        if (m_isRapidFire == true)
            return;

        if (m_rangeAttack == false)
        {
            if (isPossibleJump() == false)
                return;
        }

        useSkill((int)CHARACTER_SPARKY.SKILL.RAPID_FIRE);

        m_animController.SetBool("rapidFire", true);
        //m_rapidFireAnimCtrl.SetBool("isFire", true);
        decreaseEP(m_rapidFireEP_Consumption);

        if (m_rangeAttack == true)
        {
            playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_DOWN);
            m_curAttackType = ATTACK.TYPE.NONE;
            m_rangeAttack = false;
            m_isFire = false;
            setAttackAnim(ATTACK.TYPE.NOMAL);
        }

#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM, m_isRapidFire, m_isFire);
#endif

        StartCoroutine("rapidFire");
    }

    private IEnumerator rapidFire()
    {
        bool warmUp = false;
        float increaseGuagePerSecond = 1 / m_rapidFireWarmUpTime;
        float decreaseGuagePerSecond = 1 / m_rapidFireDurationTime;
        float curGuageAmount = m_rapidFireGuage.fillAmount;

        float totalRotSpeed = m_gunMaxRotSpeed_RapidFire - m_gunRotSpeed;
        float increaseRotSpeed = totalRotSpeed / m_rapidFireWarmUpTime;

        playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_UP);
        while (warmUp == false)
        {
            m_gunRotSpeed += increaseRotSpeed * Time.deltaTime;
            if (m_gunRotSpeed > m_gunMaxRotSpeed_RapidFire)
                m_gunRotSpeed = m_gunMaxRotSpeed_RapidFire;

            curGuageAmount += (increaseGuagePerSecond * Time.deltaTime);
            if (curGuageAmount >= 1)
            {
                curGuageAmount = 1;
                warmUp = true;
            }
            m_rapidFireGuage.fillAmount = curGuageAmount;

            yield return null;
        }

        m_isFire = true;
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM, m_isRapidFire, m_isFire);
#endif
        m_animController.SetBool("isFire", true);
        m_fireEffect.SetActive(true);
        playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_LOOP);

        while (m_isRapidFire)
        {
            //게이지 하락
            curGuageAmount -= decreaseGuagePerSecond * 0.05f;
            if (curGuageAmount <= 0)
            {
                curGuageAmount = 0;
                m_isRapidFire = false;
                m_isFire = false;
            }
            m_rapidFireGuage.fillAmount = curGuageAmount;

            //총기 반동
            m_gunRecoilX = UnityEngine.Random.Range(-0.1f, 0.1f);
            m_gunRecoilY = UnityEngine.Random.Range(0f, -0.1f);

            ObjectPool_Sparky.getInstance().fireNormalBullet(m_firePoint.position, m_fireDirection);
            transform.Rotate(Vector3.up * m_gunRecoilX * 10f);
            m_viewTarget.Rotate(Vector3.right * m_gunRecoilY * 3f);

            //피격 판정
            if (m_hitTarget != null)
            {
                if (m_hitTarget.transform.root.CompareTag(TAG.CREATURE))
                {
                    m_hitTarget.transform.root.GetComponent<Creature>().damaged(ProjectMgr.getInstance().getOwnID(), 6, transform.position);
                }
                ObjectPool_Sparky.getInstance().printNormalBulletHit_Ground(m_hitPoint, m_hitPointNormal);
            }

            //총알 속도
            yield return new WaitForSeconds(0.05f);
        }
        endRapidFire();
    }

    private void endRapidFire()
    {
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM, m_isRapidFire, m_isFire);
#endif
        playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_DOWN);
        m_fireEffect.SetActive(false);
        m_animController.SetBool("isFire", false);
        //m_rapidFireAnimCtrl.SetBool("isFire", false);
        //m_rapidFireAnimCtrl.SetBool("isCoolTime", true);
        StartCoroutine(m_checkSkillCoolTime.printSkillCoolTime((int)CHARACTER_SPARKY.SKILL.RAPID_FIRE));
    }

    public override void useSkill(int type)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)type;

        switch(skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B :
                setPossibleAttack(false);
                setPossibleMove(false);
                break;
            case CHARACTER_SPARKY.SKILL.C4_BOMB :
                break;
            case CHARACTER_SPARKY.SKILL.TESLA_B :
                break;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE :
                m_isRapidFire = true;
                setPossibleMove(false);
                break; 
        }

        setPossibleJump(false);
        setIsPossibleDodge(false);

        setUseSkill(true);
        setIsBattle(true);
    }

    public override void endSkill()
    {
        setUseSkill(false);
    }

    public void resetAttackInfo()
    {
		if (isDown() == false && isDie() == false) 
		{
			setPossibleJump (true);
			setPossibleMove (true);
		}
		m_curAttackType = ATTACK.TYPE.NONE;
        m_rangeAttack = false;
        m_isFire = false;
        setAttackAnim(ATTACK.TYPE.NOMAL);

        playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_DOWN);
#if SERVER_ON
            InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_NORMAL_ATTACK_INFO, m_rangeAttack, NET_SPARKY_ATTACK_INFO.IS_ATTACK);
#endif
    }

    public override void resetSkillState()
    {
    }

    public void resetSmashInfo()
    {
        m_curAttackType = ATTACK.TYPE.NONE;
        setPossibleJump(true);  //점프 가능
        setPossibleMove(true);  //이동 가능
        m_closeAttack = false;
        m_comboOrder = false;
        m_smashComboNum = 0;
        setAttackAnim(ATTACK.TYPE.SMASH);
        //WindowServerMgr.GetInstance().SendPacket(SendPacketType.REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO, m_isSmash, m_smashComboNum);
    }

    public void updateSmashInfo()
    {
        m_comboOrder = false;
        setAttackAnim(ATTACK.TYPE.SMASH);
        //WindowServerMgr.GetInstance().SendPacket(SendPacketType.REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO, m_isSmash, m_smashComboNum);
    }

    public override void resetAttackState()
    {
        switch (m_curAttackType)
        {
            case ATTACK.TYPE.NOMAL:
                resetAttackInfo();
                break;
            case ATTACK.TYPE.SMASH:
                resetSmashInfo();
                break;
        }
    }

    public override void checkAttackState()
    {
        switch (m_curAttackType)
        {
            case ATTACK.TYPE.NOMAL:
                if (m_rangeAttack == true)
                    resetAttackInfo();
                break;
            case ATTACK.TYPE.SMASH:
                if (m_closeAttack == true)
                {
                    if (m_comboOrder == false)
                        resetSmashInfo();
                    else
                        updateSmashInfo();
                }
                break;
        }
    }

    public void setHitPoint()
    {
        //Debug.DrawRay(m_viewTarget.transform.position, m_viewTarget.forward * attackRange, Color.green);

        LayerMask mask = (1 << 8) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13);
        mask = ~mask;
        RaycastHit hit;
        if (Physics.Raycast(m_viewTarget.position, m_viewTarget.forward, out hit, attackRange, mask))
        {
            /*
            float dist = Vector3.Distance(transform.position, hit.point);
            dist = Mathf.Abs(dist);
            if (dist < 1.5f)
            {
                m_hitPoint = hit.point * 2f;
            }
            else
            {
                
            }
            */
            m_hitPoint = hit.point;
            m_hitPointNormal = hit.normal;
            m_hitTarget = hit.collider.gameObject;
        }
        else
        {
            m_hitPoint = m_viewTarget.transform.position + m_viewTarget.forward * attackRange;
            m_hitTarget = null;
        }

        m_fireDirection = (m_hitPoint - m_firePoint.transform.position).normalized;
    }

    public void normalAttack()
    {
        if (m_isRapidFire == false)
        {
            //공격중
            if (m_rangeAttack == true && m_isOverHeat == false)
            {
                if (m_gunRotSpeed < m_gunMaxRotSpeed)
                    m_gunRotSpeed += m_gunIncreseRotSpeed * Time.deltaTime;

                else if (m_isFire == false)
                {
                    m_gunRotSpeed = m_gunMaxRotSpeed;
                    m_isFire = true;
                    m_fireEffect.SetActive(true);
                    m_emptyGunShell.SetActive(true);
                    StartCoroutine(fire());
                    playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_LOOP);
#if SERVER_ON
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_NORMAL_ATTACK_INFO, m_isFire, NET_SPARKY_ATTACK_INFO.IS_FIRE);
#endif
                }
                increaseHeatGuage();
            }
            else//공격 불가 및 공격 중 아님
            {
                if (m_fireEffect.activeSelf == true)
                {
                    m_fireEffect.SetActive(false);
                    m_emptyGunShell.SetActive(false);
                }
                decreaseHeatGuage();
            }
        }
    }

    public void closeAttack()
    {

    }

    public void setHeatPoint()
    {
        //게이지 적용
        m_overHeatGuage.fillAmount = m_curOverHeatGuage / MAX_OVER_HEAT_GUAGE;

        //게이지 색
        if (m_isOverHeat == true)
            m_overHeatGuage.color = Color.red;
        else if (m_overHeatGuage.fillAmount <= 0.2f)
            m_overHeatGuage.color = Color.green;
        else if (m_overHeatGuage.fillAmount <= 0.8f)
            m_overHeatGuage.color = Color.yellow;
        else
            m_overHeatGuage.color = Color.red;
    }

    public void increaseHeatGuage()
    {
        if(m_curOverHeatGuage > MAX_OVER_HEAT_GUAGE / 2)
            m_gunMaterial.color = Color.Lerp(m_gunMaterial.color, Color.red, 0.5f * Time.deltaTime);

        //히트 게이지 상승
        if (m_curOverHeatGuage < MAX_OVER_HEAT_GUAGE)
        {
            m_curOverHeatGuage += m_increaseOverHeatGuage * Time.deltaTime;
        }
        else
        {
            //overHeat 상태
            m_curOverHeatGuage = MAX_OVER_HEAT_GUAGE;
            m_isOverHeat = true;
            resetAttackInfo();
        }
    }

    public void decreaseHeatGuage()
    {
        if(m_isOverHeat == false)
            m_gunMaterial.color = Color.Lerp(m_gunMaterial.color, Color.white, 0.4f * Time.deltaTime); 
        else
            m_gunMaterial.color = Color.Lerp(m_gunMaterial.color, Color.white, 0.18f * Time.deltaTime);

        //히트 게이지 하락
        if (m_curOverHeatGuage > 0)
            m_curOverHeatGuage -= m_decreaseOverHeatGuage * Time.deltaTime;
        
        else
        {
            m_curOverHeatGuage = 0;
            m_isOverHeat = false;
        }

        if (m_gunRotSpeed > 0)
        {
            m_gunRotSpeed -= m_gunDecreseRotSpeed * Time.deltaTime;
        }
        else
            m_gunRotSpeed = 0.0f;
    }

    public IEnumerator fire()
    {
        while (m_isFire)
        {
            setIsBattle(true);
            m_gunRecoilX = UnityEngine.Random.Range(-0.1f, 0.1f);
            m_gunRecoilY = UnityEngine.Random.Range(0f, -0.1f);

            float bulleyRecoilX = UnityEngine.Random.Range(-0.2f, 0.2f);
            float bulleyRecoilY = UnityEngine.Random.Range(-0.2f, 0.2f);
            float bulleyRecoilZ = UnityEngine.Random.Range(-0.2f, 0.2f);

            float muzzleRot = UnityEngine.Random.Range(0, 360);
            Vector3 bulletRecoil = new Vector3(bulleyRecoilX, bulleyRecoilY, bulleyRecoilZ);

            if (m_hitTarget != null)
            {
                if (m_hitTarget.transform.root.CompareTag(TAG.CREATURE))
                {
                    m_hitTarget.transform.root.GetComponent<Creature>().damaged(ProjectMgr.getInstance().getOwnID(), (int)m_normalAtkPoint, transform.position);

                    if(m_hitTarget.transform.root.GetComponent<Creature>().GetCreatureType() == (int)CreatureMgr.CreatureType.OWL ||
                        m_hitTarget.transform.root.GetComponent<Creature>().GetCreatureType() == (int)CreatureMgr.CreatureType.DRONE)

                        ObjectPool_Sparky.getInstance().printNormalBulletHit_Ground(m_hitPoint + bulletRecoil, m_hitPointNormal);
                    else
                        ObjectPool_Sparky.getInstance().printNormalBulletHit_Creature(m_hitPoint + bulletRecoil);
                }
                else
                    ObjectPool_Sparky.getInstance().printNormalBulletHit_Ground(m_hitPoint + bulletRecoil, m_hitPointNormal);
            }
            m_hitPoint += bulletRecoil;
            m_fireEffect.transform.Rotate(Vector3.forward * muzzleRot);
            m_fireDirection = (m_hitPoint - m_firePoint.transform.position).normalized;

            ObjectPool_Sparky.getInstance().fireNormalBullet(m_firePoint.position, m_fireDirection);
            transform.Rotate(Vector3.up * m_gunRecoilX * 10f);
            m_viewTarget.Rotate(Vector3.right * m_gunRecoilY * 3f);

            //총알 속도
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator explosionRecoil()
    {
        for (int i = 0; i < 3; i++)
        {
            m_viewTarget.Rotate(Vector3.right * -4.5f);
            yield return null;
        }
        for (int i = 0; i < 13; i++)
        {
            m_viewTarget.Rotate(Vector3.right * 1f);
            yield return null;
        }
    }

    public override void setIsBattle(bool _isBattle)
    {
        if (_isBattle == true)
            m_battleTime = 0;

        if (m_isBattle != _isBattle)
        {
            if (_isBattle == false)
                recovery(true);
            else
                recovery(false);

            m_isBattle = _isBattle;
        }
    }

    public override void printRunningEffect(int type)
    {
        type += 4;
        SOUND_POOL.SPARKY.FOOT_STEP footType = (SOUND_POOL.SPARKY.FOOT_STEP)type;
        switch (footType)
        {
            case SOUND_POOL.SPARKY.FOOT_STEP.LEFT :
                ObjectPool_Sparky.getInstance().printRunningEffect(m_leftFoot.position, footType);
                break;
            case SOUND_POOL.SPARKY.FOOT_STEP.RIGHT :
                ObjectPool_Sparky.getInstance().printRunningEffect(m_rightFoot.position, footType);
                break;
        }
        m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.FOOT_STEP].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)footType), 0.5f);
    }

    public override void printJumpEffect()
    {
        ObjectPool_Sparky.getInstance().printJumpEffect(transform.position - Vector3.up * 0.2f);
    }

    public override void printLandingEffect()
    {
        ObjectPool_Sparky.getInstance().printLandingEffect(transform.position);
        m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.FOOT_STEP].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)SOUND_POOL.SPARKY.FOOT_STEP.LANDING), 0.5f);
    }

    public void playGunSound(SOUND_POOL.SPARKY.GUN type)
    {   
        switch(type)
        {
            case SOUND_POOL.SPARKY.GUN.SPIN_UP:
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].Stop();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)SOUND_POOL.SPARKY.GUN.SPIN_UP), 0.5f);
                break;
            case SOUND_POOL.SPARKY.GUN.SPIN_LOOP :
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.FIRE].Play();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].Play();
                break;
            case SOUND_POOL.SPARKY.GUN.SPIN_DOWN :
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].Stop();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.FIRE].Stop();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)SOUND_POOL.SPARKY.GUN.SPIN_DOWN), 0.5f);
                break;
        }
    }

    public IEnumerator sendAimPoint()
    {
        while (InGameMgr.getInstance().isStart() == false)
            yield return null;

        while (true)
        {
            InGameServerMgr.getInstance().SendPacket_UDP(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SPARKY_AIM_POINT_EM, m_hitPoint);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void setAttackAnim(ATTACK.TYPE attackType)
    {
        switch (attackType)
        {
            case ATTACK.TYPE.NOMAL:
                m_animController.SetBool("rangeAttack", m_rangeAttack);
                m_animController.SetBool("isFire", m_isFire);

                if (m_rangeAttack == true)
                    setLayerWeight(CHARACTER.LAYER.UPPERBODY, 1f);
                else
                    setLayerWeight(CHARACTER.LAYER.UPPERBODY, 0f);
                break;
            case ATTACK.TYPE.SMASH:
                m_animController.SetBool("closeAttack", m_closeAttack);
                m_animController.SetBool("comboOrder", m_comboOrder);
                break;
        }
    }

    public override void setDamagedAnim()
    {
        setDamagedInfo(true);
        setPossibleMove(false);
        setDamagedInfo(true);
        m_animController.SetTrigger("isDamaged");
        setLayerWeight(CHARACTER.LAYER.DAMAGED, 1f);
    }

    public override void resetDamagedInfo()
    {
        setDamagedInfo(false);
        setCharacterStateDisorder(ATTACK.STATE_DISORDER.NONE);
        m_animController.SetBool("isDamaged", false);
        setLayerWeight(CHARACTER.LAYER.DAMAGED, 0f);
        setPossibleMove(true);
    }

    public override void setLayerWeight(CHARACTER.LAYER layerIndex, float weight)
    {
        switch(layerIndex)
        {
            case CHARACTER.LAYER.BASE :
                Debug.Log("Can't set BaseLayer Weight");
                break;
            case CHARACTER.LAYER.DAMAGED :
                //m_animController.SetLayerWeight((int)CHARACTER.LAYER.DAMAGED, weight);
                break;
            case CHARACTER.LAYER.UPPERBODY :
                m_animController.SetLayerWeight((int)CHARACTER.LAYER.UPPERBODY, weight);
                break;
        }
    }

    public override bool checkPossibleAttack()
    {
        if (isActive() == false)
            return false;

        if(isPreventAttack() == true)
        {
            InGameMgr.getInstance().printErrorMessage("보급품을 획득해야 미니건을 발사 할 수 있습니다.", false);
            return false;
        }
        if (isPossibleAttack() == true && isPossibleJump() == true)
        {
            if (m_rangeAttack == false && isDodge() == false)
                return true;
        }
        return false;
    }

    public override bool checkPossibleUseSkill(float ep_Consumtion, int type)
    {
        if (isActive() == true && isPreventAttack() == false)
            if (isPossibleDodge() == true && isPossibleMove() == true)
                if (usingSkill() == false && m_checkSkillCoolTime.isUsebleSkill(type) == true)
                {
                    if (getCurEP() > ep_Consumtion)
                        return true;
                    else
                        InGameMgr.getInstance().printErrorMessage("에너지가 부족합니다.", false);
                }
        return false;
    }
}        
