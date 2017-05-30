using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CAM_SHAKE_EVENT
{
    public enum TYPE { NONE, BOSS_WALK, BOSS_CHARGE, BOSS_GROUND, BOSS_SHOCKWAVE, BOSS_SHOCKPUNCH, BOSS_JUMP, BOSS_DOWN, BOSS_CHARGE_GROUND,
        BOSS_NORMAL_ATTACK, BOSS_TA_1, BOSS_TA_2, BOSS_TA_3, SPARKY_C4, SPARKY_EB, SAM_SB, SAM_PUL }

    public struct SIZE
    {
        public const float BOSS_WALK_MIM = 1.5f;
        public const float BOSS_WALK_MAX = 1.8f;
        public const float BOSS_NORMAL_ATTACK_MIN = 3.0f;
        public const float BOSS_NORMAL_ATTACK_MAX = 5.0f;
        public const float BOSS_CHARGE_MIN = 3.0f;
        public const float BOSS_CHARGE_MAX = 5.0f;
        public const float BOSS_CHARGE_GROUND_MIN = 6.0f;
        public const float BOSS_CHARGE_GROUND_MAX = 10.0f;
        public const float BOSS_GROUND_MIN = 10.0f;
        public const float BOSS_GROUND_MAX = 20.0f;
        public const float BOSS_SHOCKWAVE_MIN = 5.0f;
        public const float BOSS_SHOCKWAVE_MAX = 10.0f;
        public const float BOSS_SHOCKPUNCH_MIN = 5.0f;
        public const float BOSS_SHOCKPUNCH_MAX = 10.0f;
        public const float BOSS_JUMP_MIN = 15.0f;
        public const float BOSS_JUMP_MAX = 30.0f;
        public const float BOSS_DOWN_MIN = 3.0f;
        public const float BOSS_DOWN_MAX = 5.0f;
        public const float BOSS_TA_1_MIN = 3.0f;
        public const float BOSS_TA_1_MAX = 5.0f;
        public const float BOSS_TA_2_MIN = 5.0f;
        public const float BOSS_TA_2_MAX = 8.0f;
        public const float BOSS_TA_3_MIN = 10.0f;
        public const float BOSS_TA_3_MAX = 12.0f;

        public const float SPARKY_RANGE_ATTACK_MIN = 2.0f;
        public const float SPARKY_RANGE_ATTACK_MAX = 5.0f;
        public const float SPARKY_C4_MIN = 6.0f;
        public const float SPARKY_C4_MAX = 10.0f;
        public const float SPARKY_EB_MIN = 6.0f;
        public const float SPARKY_EB_MAX = 10.0f;
        public const float SAM_SB_MIN = 6.0f;
        public const float SAM_SB_MAX = 10.0f;
        public const float SAM_PUL_MIN = 6.0f;
        public const float SAM_PUM_MAX = 10.0f;
    }
}

public class UserInput : MonoBehaviour
{
    private bool m_isDamaged;

    private Transform m_rotAxis;
    private Transform m_camera;
    private Transform m_viewTarget;
    private GameObject m_communicateArea;

    private CharacterCtrl_Own m_characterCtrl;

    public float ROT_RIGHT_SPEED = 10.0f;
    public float ROT_UP_SPEED = 2.0f;
    public float MIN_UP_ROT = 70.0f;
    public float MAX_UP_ROT = 300.0f;
    public float DAMP_TRACE = 10.0f;
    public float CAMERA_DISTANCE = 3.0f;

    private float m_rotUpDamper;
    private float m_rotRightDamper;

    private bool m_damagedShake;
    private bool m_eventShake;
    private bool m_camShackeEvent;
    private bool m_decreaseCamShakeBreadth;

    private float m_camShakeSize_Min;
    private float m_camShakeSize_Max;

    void Start()
    {
        init();
        m_damagedShake = false;
        m_eventShake = false;
        m_camShackeEvent = false;
        m_decreaseCamShakeBreadth = false;

        m_camShakeSize_Min = 0f;
        m_camShakeSize_Max = 0f;
    }

    public void init()
    {
        m_rotUpDamper = ROT_UP_SPEED / 20;
        m_rotRightDamper = ROT_RIGHT_SPEED / 20;

        ROT_UP_SPEED = m_rotUpDamper * ProjectMgr.getInstance().getMouseSensitive();
        ROT_RIGHT_SPEED = m_rotRightDamper * ProjectMgr.getInstance().getMouseSensitive();

        Transform[] allChild = transform.parent.GetComponentsInChildren<Transform>();

        m_communicateArea = GameObject.Find(OBJECT_NAME.COMUNICATION_OBJECT);
        m_characterCtrl = InGameMgr.getInstance().getOwnCharacterCtrl();
	}

    void Update()
    {
        characterCtrl();
        cameraRotate();
        setRotateSpeed();
        printExitUI();
    }

    void LateUpdate()
    {
        dynamicCamera();
        cameraShake();
    }

    public void characterCtrl()
    {
        characterMove();
        characterSkill();
        characterDodge();
        communicateToObject();
    }

    public void cameraRotate()
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isLockCameraCtrl() == true)
            return;

        //좌/우 회전
        Quaternion rot = Quaternion.Euler(Vector3.up * ROT_RIGHT_SPEED * Input.GetAxis("Mouse X") + m_rotAxis.transform.rotation.eulerAngles);
        m_rotAxis.transform.rotation = Quaternion.Slerp(m_rotAxis.transform.rotation, rot, Time.deltaTime * DAMP_TRACE);
        
        //미리 회전 할 방향을 구함.
        float rotDir = -Input.GetAxis("Mouse Y");

        //미리 회전 후의 각도를 구함.
        Vector3 nextRot = Vector3.right * rotDir * ROT_UP_SPEED * Time.deltaTime;

        nextRot.x = Mathf.Clamp(nextRot.x, -30f, 30f);
        nextRot += m_viewTarget.transform.eulerAngles;

        if (nextRot.x > MIN_UP_ROT && nextRot.x < 200)
            return;

        if (nextRot.x < MAX_UP_ROT && nextRot.x > 200)
            return;
        
        m_viewTarget.rotation = Quaternion.Euler(nextRot);
    }

    public void dynamicCamera()
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isLockDynamicCamera() == true)
        {
            //m_viewTarget.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }

        float cameradist = CAMERA_DISTANCE;
        Vector3 rayDir = m_camera.position - m_viewTarget.position;

        RaycastHit hit;
        LayerMask mask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13) | (1 << 14) | (1 << 16) | (1 << 17);
        mask = ~mask;
        if (Physics.Raycast(m_viewTarget.position, rayDir.normalized, out hit, CAMERA_DISTANCE, mask))
        {
            Vector3 dist = hit.point - m_viewTarget.position;
            cameradist = dist.magnitude;
        }

        m_camera.position = Vector3.Lerp(m_camera.position, m_viewTarget.position + -m_viewTarget.forward.normalized * (cameradist - 0.5f), Time.deltaTime * DAMP_TRACE);
        /*
        else
            m_camera.position = Vector3.Lerp(m_camera.position, m_viewTarget.position + -m_viewTarget.forward.normalized * 5f, Time.deltaTime * DAMP_TRACE);
        */
        m_camera.LookAt(m_viewTarget);
    }

    public void cameraShake()
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isLockCameraCtrl() == true)
            return;

        if (InGameMgr.getInstance().getOwnCharacterCtrl().m_damageAnim)
        {
            switch(InGameMgr.getInstance().getOwnCharacterCtrl().getCharacterStateDisorder())
            {
                case ATTACK.STATE_DISORDER.NONE :
                case ATTACK.STATE_DISORDER.NOCK_BACK :
                    if (m_damagedShake)
                        m_camera.Translate((Vector3.right * UnityEngine.Random.Range(0.5f, 2f) + Vector3.up * UnityEngine.Random.Range(0.5f, 2f)) * Time.deltaTime);
                    else
                        m_camera.Translate((Vector3.right * -UnityEngine.Random.Range(0.5f, 2f) - Vector3.up * -UnityEngine.Random.Range(0.5f, 2f)) * Time.deltaTime);
                    break;
                case ATTACK.STATE_DISORDER.DOWN :
                    if (m_damagedShake)
                        m_camera.Translate((Vector3.right * UnityEngine.Random.Range(1.0f, 2.5f) + Vector3.up * UnityEngine.Random.Range(1.0f, 2.5f)) * Time.deltaTime);
                    else
                        m_camera.Translate((Vector3.right * -UnityEngine.Random.Range(1.0f, 2.5f) - Vector3.up * -UnityEngine.Random.Range(1.0f, 2.5f)) * Time.deltaTime);
                    break;
            }
            m_damagedShake = !m_damagedShake;
        }

        if (m_camShackeEvent)
        {
            if (m_eventShake)
                m_camera.Translate((Vector3.right * UnityEngine.Random.Range(m_camShakeSize_Min, m_camShakeSize_Max) 
                    + Vector3.up * UnityEngine.Random.Range(m_camShakeSize_Min, m_camShakeSize_Max)) * Time.deltaTime);
            else
                m_camera.Translate((Vector3.right * -UnityEngine.Random.Range(m_camShakeSize_Min, m_camShakeSize_Max) 
                    - Vector3.up * -UnityEngine.Random.Range(m_camShakeSize_Min, m_camShakeSize_Max)) * Time.deltaTime);
            m_eventShake = !m_eventShake;
        }
    }

    public IEnumerator decreaseShakeBreadth(float time)
    {
        yield return new WaitForSeconds(time);
        while (m_camShackeEvent)
        {
            m_camShakeSize_Max = m_camShakeSize_Max / 2;
            m_camShakeSize_Min = m_camShakeSize_Min / 2;
            yield return new WaitForSeconds(time);
        }
    }

    public void characterMove()
    {
        Vector3 moveVector = Vector3.zero;
        if (Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.S) == false && Input.GetKey(KeyCode.D) == false)
        {
            moveVector = Vector3.zero;
        }
        else
        {
            float forward = Input.GetAxis("Vertical");
            float right = Input.GetAxis("Horizontal");

            moveVector = Vector3.forward * forward + Vector3.right * right;
           
        }
        m_characterCtrl.setMoveVector(moveVector);
    }

    public void characterDodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            m_characterCtrl.dodge();
    }

    public void communicateToObject()
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isActive())
        {
            if (Input.GetKeyDown(KeyCode.F))
                m_communicateArea.SendMessage("startEvent");

            if (Input.GetKeyUp(KeyCode.F))
                m_communicateArea.SendMessage("endEvent");
        }
    }

    public void characterSkill()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            m_characterCtrl.Skill_First();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            m_characterCtrl.Skill_Second();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            m_characterCtrl.Skill_Third();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            m_characterCtrl.Skill_Fourth();
    }


    public void setControlCamera(Transform rotAxis, Transform camera, Transform viewTarget, float cameraDist, float dampTrace)
    {
        m_rotAxis = rotAxis;
        m_camera = camera;
        m_viewTarget = viewTarget;
        CAMERA_DISTANCE = cameraDist;
        DAMP_TRACE = dampTrace;
    }

    public void setRotateSpeed()
    {
        if(Input.GetKeyDown(KeyCode.LeftBracket))
        {
            ProjectMgr.getInstance().decreaseMouseSensitive();
            ROT_UP_SPEED = m_rotUpDamper * ProjectMgr.getInstance().getMouseSensitive();
            ROT_RIGHT_SPEED = m_rotRightDamper * ProjectMgr.getInstance().getMouseSensitive();

            InGameMgr.getInstance().printErrorMessage("마우스 감도 감소 : " + ProjectMgr.getInstance().getMouseSensitive(), true);
        }

        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            ProjectMgr.getInstance().increaseMouseSensitive();
            ROT_UP_SPEED = m_rotUpDamper * ProjectMgr.getInstance().getMouseSensitive();
            ROT_RIGHT_SPEED = m_rotRightDamper * ProjectMgr.getInstance().getMouseSensitive();

            InGameMgr.getInstance().printErrorMessage("마우스 감도 증가 : " + ProjectMgr.getInstance().getMouseSensitive(), true);
        }
    }

    public void playCamShakeEvent(CAM_SHAKE_EVENT.TYPE eventType, bool decrease, float decreaseTime)
    {
        m_camShackeEvent = true;

        switch(eventType)
        {
            case CAM_SHAKE_EVENT.TYPE.BOSS_NORMAL_ATTACK :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_NORMAL_ATTACK_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_NORMAL_ATTACK_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_WALK :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_WALK_MIM;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_WALK_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_CHARGE:
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_CHARGE_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_CHARGE_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_CHARGE_GROUND :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_CHARGE_GROUND_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_CHARGE_GROUND_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_GROUND :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_GROUND_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_GROUND_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_SHOCKWAVE :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_SHOCKWAVE_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_SHOCKWAVE_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_SHOCKPUNCH :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_SHOCKPUNCH_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_SHOCKPUNCH_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_JUMP:
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_JUMP_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_JUMP_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_DOWN :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_DOWN_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_DOWN_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_TA_1 :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_TA_1_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_TA_1_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_TA_2:
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_TA_2_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_TA_2_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.BOSS_TA_3:
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.BOSS_TA_3_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.BOSS_TA_3_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.SPARKY_C4:
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.SPARKY_C4_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.SPARKY_C4_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.SPARKY_EB :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.SPARKY_EB_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.SPARKY_EB_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.SAM_SB :
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.SAM_SB_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.SAM_SB_MAX;
                break;
            case CAM_SHAKE_EVENT.TYPE.SAM_PUL:
                m_camShakeSize_Min = CAM_SHAKE_EVENT.SIZE.SAM_PUL_MIN;
                m_camShakeSize_Max = CAM_SHAKE_EVENT.SIZE.SAM_PUM_MAX;
                break;
        }

        if (decrease)
            StartCoroutine("decreaseShakeBreadth", decreaseTime);
    }

    public void stopCamShakeEvent() { m_camShackeEvent = false; }

    public void printExitUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (InGameMgr.getInstance().getOwnCharacterCtrl().isActive() == true)
            {
                if (InGameMgr.getInstance().isActiveExitUI() == false)
                    InGameMgr.getInstance().activeExitUI();
            }
            else if (InGameMgr.getInstance().isActiveExitUI() == true)
            {
                InGameMgr.getInstance().deActiveExitUI();
            }
        }
    }
}
