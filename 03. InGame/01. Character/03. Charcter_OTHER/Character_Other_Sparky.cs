using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using RootMotion.FinalIK;
using DB_DATA;

public class Character_Other_Sparky : CharacterCtrl_Other
{
    //이동 속도
    private float m_walkSpeed;

    private GameObject bullet;
    private Transform m_firePoint;
    private GameObject m_fireEffect;
    public GameObject m_emptyGunShell;

    //발자국 이펙트 발생 위치
    private Transform m_leftFoot;
    private Transform m_rightFoot;

    private bool m_isFire;
    private float m_attackRange = 20.0f;
    private bool m_rangeAttack;
    private bool m_closeAttack;
    private Vector3 m_hitPoint;
    private Vector3 m_nextHitPoint;
    private Vector3 m_fireDirection;
    private Vector3 m_hitPointNormal;
    private GameObject m_hitTarget;

    private Vector3 m_explosionBulletFirePoint;
    private Vector3 m_explosionBulletFireDirection;

    //스킬 관련
    private bool m_useRapidFire;
    private bool m_throwC4Bomb;
    private C4_Bomb m_curC4Bomb;
    private Vector3 m_c4NextPos;
    private Vector3 m_c4NextRot;

    //총기 관련
    private Material m_gunMaterial;
    private Transform m_gunMuzzle;

    private float m_curRotSpeed;
    private float m_gunRotSpeed;
    private float m_gunMaxRotSpeed;
    private float m_gunMaxRotSpeed_RapidFire;
    private float m_gunIncreseRotSpeed;
    private float m_gunDecreseRotSpeed;
    //발자국 이펙트 발생위치
    private Transform m_runningDustPosition;

    //IK
    private AimIK m_aimIK;
    private FullBodyBipedIK m_fbbIK;
    private LookAtIK m_lookAtIK;
    private IKEffector leftHand { get { return m_fbbIK.solver.leftHandEffector; } }
    private IKEffector rightHand { get { return m_fbbIK.solver.rightHandEffector; } }

    private Quaternion leftHandRotationRelative;
    private Transform spineRotation;

    public override void init()
    {
        initType(CHARACTER.TYPE.SPARKY);
        initIK();
        initComponent();
    }

    public override void initState(DB_DATA.CHARACTER_INFO status)
    {
        base.initState(status.HLTH_PONT, status.ENRG_PONT, status.DFND_PONT, status.UABL_RGNT, status.HEAL_SCND, status.RUNN_SPED, status.JUMP_SPED, status.GRVT_PONT, status.DDGE_SPED);
        m_walkSpeed = status.WALK_SPED;

        initData();
        initAttackInfo();
    }

    public void initState(float maxHP, float maxEP, float atkPoint, float defPoint, float RUT, float HPS, float runSpd, float walkSpd, float jSpd, float gravity, float dgSPD)
    {
        base.initState(maxHP, maxEP, defPoint, RUT, HPS, runSpd, jSpd, gravity, dgSPD);
        m_walkSpeed = walkSpd;

        initData();
        initAttackInfo();
    }

    public override void initSkillStatus(CHARACTER_SKILL_DATA skillData, bool isActive)
    {
        CHARACTER_SPARKY.SKILL skillType = (CHARACTER_SPARKY.SKILL)skillData.SKIL_TYPE;

        switch (skillType)
        {
            case CHARACTER_SPARKY.SKILL.EXPLOSION_B:
                ExplosionBullet.initStatus(skillData.OBJT_SPED, skillData.GRVT_POWR);
                ExplosionBulletHit.initStatus(skillData.BOMB_ATPT, skillData.ATCK_RNGE);
                break;
            case CHARACTER_SPARKY.SKILL.C4_BOMB:
                C4_Bomb.initStatus(skillData.BOMB_ATPT, skillData.THRW_POWR, skillData.ATCK_RNGE);
                break;
            case CHARACTER_SPARKY.SKILL.TESLA_B:
                //m_teslaBulletEP_Consumption = skillData.USNG_MPNT;
                break;
            case CHARACTER_SPARKY.SKILL.RAPID_FIRE:
                //m_rapidFireEP_Consumption = skillData.USNG_MPNT;
                break;
        }
    }

    public new void initComponent()
    {
        base.initComponent();

        Transform[] allChilds = transform.parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < allChilds.Length; i++)
        {
            if (allChilds[i].name == OBJECT_NAME.GUN_MUZZLE)
                m_gunMuzzle = allChilds[i].transform;

            if (allChilds[i].name == OBJECT_NAME.FIRE_POINT)
                m_firePoint = allChilds[i].transform;

            if (allChilds[i].name.Equals(OBJECT_NAME.FIRE_EFFECT))
                m_fireEffect = allChilds[i].gameObject;

            if (allChilds[i].name.Equals(OBJECT_NAME.RUNNING_DUST_POSITION))
                m_runningDustPosition = allChilds[i];

            if (allChilds[i].name.Equals(OBJECT_NAME.RIGHT_FOOT))
                m_rightFoot = allChilds[i];

            if (allChilds[i].name.Equals(OBJECT_NAME.LEFT_FOOT))
                m_leftFoot = allChilds[i];
        }

        m_gunMaterial = m_gunMuzzle.GetComponent<Renderer>().material;
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

        m_aimIK.solver.IKPosition = m_hitPoint;
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

    public void initAttackInfo()
    {
        //기본 공격
        m_rangeAttack = false;
        m_closeAttack = false;
        m_isFire = false;

        //총기 회전
        m_curRotSpeed = 0f;
        m_gunRotSpeed = 0.0f;
        m_gunMaxRotSpeed = 20.0f;
        m_gunMaxRotSpeed_RapidFire = 30.0f;
        m_gunIncreseRotSpeed = 100.0f;
        m_gunDecreseRotSpeed = 100.0f;

        //스킬 관련
        m_useRapidFire = false;

        m_hitPoint = Vector3.zero;
        m_nextHitPoint = Vector3.zero;
        m_hitTarget = null;
        m_hitPointNormal = Vector3.zero;
        m_fireEffect.SetActive(false);
        m_emptyGunShell.SetActive(false);

        m_explosionBulletFirePoint = Vector3.zero;
        m_explosionBulletFireDirection = Vector3.zero;

        m_curC4Bomb = null;
    }

    public override void setActive()
    {
        setActive(true);
#if SERVER_ON
        //hitPoint 분석 코루틴
        StartCoroutine("analyseAimPoint");
#endif
    }

    public override void setDeActive()
    {
        setActive(false);
    }

    public override float getCharacterMoveSpeed()
    {
        if (m_rangeAttack)
            return m_walkSpeed;
        else
            return getMoveSpeed();
    }

    public void Update()
    {
        interpolateMove();
        interpolateRotation();
        interpolateHitPoint();
        checkIsGround(0.3f);
        rotateGunMuzzle();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isActive() == false)
            return;

        if(isDodge() == false && isDown() == false)
            setAimPoint();  

        m_curRotSpeed += m_gunRotSpeed;
        m_gunMuzzle.Rotate(Vector3.up * m_curRotSpeed);
    }

    //ik
    private void OnPostFBBIK()
    {
        leftHand.bone.rotation = rightHand.bone.rotation * leftHandRotationRelative;
    }

    public void interpolateHitPoint()
    {
        if (isActive() == true)
        {
            Vector3 lerpVector = m_nextHitPoint - m_hitPoint;
            Vector3 moveVector = lerpVector.normalized * 30f * Time.deltaTime;

            if (Vector3.SqrMagnitude(lerpVector) < 0.02f)
            {
                m_hitPoint = m_nextHitPoint;
                return;
            }

            //m_hitPoint += moveVector;
            m_hitPoint = Vector3.Lerp(m_hitPoint, m_nextHitPoint, 0.5f);
            m_fireDirection = (m_hitPoint - m_firePoint.transform.position).normalized;

            setHitTarget();
        }
        else
        {
            LayerMask mask = (1 << 8) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13);
            mask = ~mask;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 20f, mask))
            {
                m_hitPoint = hit.point;
                m_hitPointNormal = hit.normal;
                m_hitTarget = hit.collider.gameObject;
            }
            else
            {
                m_hitPoint = transform.position + transform.forward * 20f;
                m_hitTarget = null;
            }
        }
    }

    public void setHitTarget()
    {
        RaycastHit hit;
        LayerMask mask = (1 << 8) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13);
        mask = ~mask;
        if (Physics.Raycast(m_firePoint.position, m_fireDirection, out hit, m_attackRange, mask))
        {
            m_hitTarget = hit.collider.gameObject;
            m_hitPointNormal = hit.normal;
        }
        else
            m_hitTarget = null;
    }

    public void normalAttack()
    {
    }

    public void rotateGunMuzzle()   //공격 방식을 바꾸더라도 회전의 감소가 멈추지 않도록 따로 구현.
    {
        if (m_rangeAttack == true)
        {
            if (m_gunRotSpeed < m_gunMaxRotSpeed)
                m_gunRotSpeed += m_gunIncreseRotSpeed * Time.deltaTime;
            else
                m_gunRotSpeed = m_gunMaxRotSpeed;
        }
        else if (m_useRapidFire == true)
        {
            if (m_gunRotSpeed < m_gunMaxRotSpeed_RapidFire)
                m_gunRotSpeed += m_gunIncreseRotSpeed * Time.deltaTime;
            else
                m_gunRotSpeed = m_gunMaxRotSpeed;
        }
        else
        {
            if (m_gunRotSpeed > 0)
                m_gunRotSpeed -= m_gunDecreseRotSpeed * Time.deltaTime;
            else
                m_gunRotSpeed = 0.0f;
        }
    }

    public void skill_ExplosionBullet(bool rangeAttack, Vector3 firePoint, Vector3 fireDirection)
    {
        m_explosionBulletFirePoint = firePoint;
        m_explosionBulletFireDirection = fireDirection;

        if (rangeAttack == true)
        {
            ObjectPool_Sparky.getInstance().fireExplosionBullet(m_explosionBulletFirePoint, m_explosionBulletFireDirection);
            m_animController.SetTrigger("explosionBullet");
        }
    }

    public void throwC4Bomb(Vector3 position)
    {
        m_throwC4Bomb = true;
        m_animController.SetTrigger("throwC4Bomb");
        m_curC4Bomb = ObjectPool_Sparky.getInstance().throwC4Bomb(position, Vector3.zero);
        StartCoroutine("setC4BombTransform");
    }

    public void detonateC4Bomb(Vector3 position)
    {
        m_throwC4Bomb = false;
        m_animController.SetTrigger("detonateC4Bomb");
        ObjectPool_Sparky.getInstance().printC4BombExplosion(position);
        m_curC4Bomb.gameObject.SetActive(false);
        m_curC4Bomb = null;
        StopCoroutine("setC4BombTransform");
    }

    public void setC4BombNextTransform(Vector3 position, Vector3 rotation)
    {
        m_c4NextPos = position;
        m_c4NextRot = rotation;
    }

    public IEnumerator setC4BombTransform()
    {
        while(m_throwC4Bomb == true)
        {
            m_curC4Bomb.transform.position = Vector3.Lerp(m_curC4Bomb.transform.position, m_c4NextPos, 10 * Time.deltaTime);
            m_curC4Bomb.transform.rotation = Quaternion.Slerp(m_curC4Bomb.transform.rotation, Quaternion.Euler(m_c4NextRot), 10 * Time.deltaTime);
            yield return null;
        }
    }

    public void teslaAttack()
    {

    }

    public IEnumerator fire()
    {
        while (m_isFire)
        {
            //발사 이펙트
            ObjectPool_Sparky.getInstance().fireNormalBullet(m_firePoint.position, m_fireDirection);

            float muzzleRot = UnityEngine.Random.Range(0, 360);
            m_fireEffect.transform.Rotate(Vector3.forward * muzzleRot);
            //타격 이펙트
            if (m_hitTarget != null)
            {
                playGunSound(SOUND_POOL.SPARKY.GUN.FIRE);
                ObjectPool_Sparky.getInstance().printNormalBulletHit_Creature(m_hitPoint);
            }
            else
                ObjectPool_Sparky.getInstance().printNormalBulletHit_Ground(m_hitPoint, m_hitPointNormal);
            //총알 속도
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void skill_RapidFire(bool useRapidFire, bool isFire)
    {
        if (m_useRapidFire == false)
        {
            if (useRapidFire == true)
            {
                m_useRapidFire = true;
                m_rangeAttack = false;
                m_isFire = false;
                m_animController.SetBool("isFire", false);
                m_animController.SetBool("rangeAttack", false);
                m_animController.SetBool("rapidFire", true);

                return;
            }
        }

        if (useRapidFire == true)
        {
            if (isFire == true)
            {
                m_fireEffect.SetActive(true);
                StartCoroutine("rapid_Fire");
                m_animController.SetBool("isFire", true);
            }
        }
        else
        {
            m_useRapidFire = false;
            m_fireEffect.SetActive(false);
            m_animController.SetBool("rapidFire", false);
            m_animController.SetBool("isFire", false);
            StopCoroutine("rapid_Fire");
        }
    }

    public IEnumerator rapid_Fire()
    {
        while (true)
        {
            ObjectPool_Sparky.getInstance().fireNormalBullet(m_firePoint.position, m_fireDirection);

            yield return new WaitForSeconds(0.05f);
        }
    }

    public override void setAttackInfo(bool attackInfo, bool attackType)
    {
        if (attackType == NET_SPARKY_ATTACK_INFO.IS_ATTACK)
        {
            m_rangeAttack = attackInfo;
            if(m_rangeAttack == true)
                playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_UP);
            else
            {
                playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_DOWN);
                m_isFire = false;
                m_fireEffect.SetActive(false);
                m_emptyGunShell.SetActive(false);
                m_animController.SetBool("isFire", m_isFire);
            }
            m_animController.SetBool("rangeAttack", m_rangeAttack);
        }
        else if (attackType == NET_SPARKY_ATTACK_INFO.IS_FIRE)
        {
            playGunSound(SOUND_POOL.SPARKY.GUN.SPIN_LOOP);
            m_isFire = attackInfo;
            m_fireEffect.SetActive(true);
            m_emptyGunShell.SetActive(true);
            StartCoroutine(fire());
            m_animController.SetBool("isFire", m_isFire);
        }
    }

    public override void resetAttackInfo()
    {
        if (m_rangeAttack == true)
        {
            m_rangeAttack = false;
            m_fireEffect.SetActive(false);
            m_emptyGunShell.SetActive(false);
        }
    }

    public override void damaged(float damage)
    {
        m_animController.SetTrigger("isDamaged");
        //Invoke("resetDamagedInfo", 0.4f);
    }

    public override void setSmashAnim(bool isSmash, int comboNum)
    {

    }

    public void setHitPoint(Vector3 hitPoint)
    {
        m_nextHitPoint =  hitPoint;
    }

    public IEnumerator analyseAimPoint()
    {
        while (InGameMgr.getInstance().isStart() == false)
            yield return null;

        PacketHandlingMgr packetMgr = InGameServerMgr.getInstance().getPacketHandlingManager();
        InGamePacketHandlingMgr inGamePacketMgr = (InGamePacketHandlingMgr)packetMgr;

        while (true)
        {
            inGamePacketMgr.analyseSparkyAimPoint();
            yield return null;
        }
    }

    public override void printRunningEffect(int type)
    {
        type += 4;
        SOUND_POOL.SPARKY.FOOT_STEP footType = (SOUND_POOL.SPARKY.FOOT_STEP)type;
        switch (footType)
        {
            case SOUND_POOL.SPARKY.FOOT_STEP.LEFT:
                ObjectPool_Sparky.getInstance().printRunningEffect(m_leftFoot.position, footType);
                break;
            case SOUND_POOL.SPARKY.FOOT_STEP.RIGHT:
                ObjectPool_Sparky.getInstance().printRunningEffect(m_rightFoot.position, footType);
                break;
        }
        //m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.FOOT_STEP].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)footType), 0.5f);
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
        switch (type)
        {
            case SOUND_POOL.SPARKY.GUN.SPIN_UP:
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].Stop();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)SOUND_POOL.SPARKY.GUN.SPIN_UP), 0.5f);
                break;
            case SOUND_POOL.SPARKY.GUN.SPIN_LOOP:
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.FIRE].Play();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].Play();
                break;
            case SOUND_POOL.SPARKY.GUN.SPIN_DOWN:
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].Stop();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.FIRE].Stop();
                m_audioSource[(int)CHARACTER_SPARKY.AUDIO_SOURCE.SPIN].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)SOUND_POOL.SPARKY.GUN.SPIN_DOWN), 0.5f);
                break;
        }
    }
}
