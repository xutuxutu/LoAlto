using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialGuideUI : MonoBehaviour
{
    private Camera m_characterCamera;
    private float SCREEN_HEIGHT;
    private float SCREEN_WITDH;

    Vector3 ScreenCenter;
    Vector3 ScreenBounds;

    private Image m_tutorialGuideUI_Background;
    private Text m_tutorialGuideUI_Text;

    private float m_fadeInTime;
    private float m_fadeOutTime;

    private bool m_isActive;

    private GameObject m_tutorialQuestTarget;
    // Use this for initialization
    void Start ()
    {
        setScreenInfo();

        m_characterCamera = InGameMgr.getInstance().getCharacterCamera().GetComponent<Camera>();
        m_tutorialGuideUI_Background = GetComponentInChildren<Image>();
        m_tutorialGuideUI_Text = GetComponentInChildren<Text>();

        m_tutorialGuideUI_Background.canvasRenderer.SetAlpha(0);
        m_tutorialGuideUI_Text.canvasRenderer.SetAlpha(0);
        m_isActive = false;

        m_fadeInTime = 1f;
        m_fadeOutTime = 1f;
    }

    public void setScreenInfo()
    {
        SCREEN_HEIGHT = Screen.height;
        SCREEN_WITDH = Screen.width;
        ScreenCenter = new Vector3(SCREEN_WITDH, SCREEN_HEIGHT, 0) / 2;
        ScreenBounds = ScreenCenter * 0.9f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_tutorialQuestTarget == null)
            return;

        setScreenInfo();

        Vector3 cameraForward = m_characterCamera.transform.forward;
        Vector3 cameraToTarget = (m_tutorialQuestTarget.transform.position - m_characterCamera.transform.position).normalized;
        /*
        float degree = Vector3.Dot(cameraForward, cameraToTarget);
        degree = Mathf.Acos(degree);
        degree = Mathf.Rad2Deg * degree;
        */
        Vector3 pos = m_characterCamera.WorldToScreenPoint(m_tutorialQuestTarget.transform.position);

        if (m_isActive == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_characterCamera.ScreenPointToRay(pos), out hit, 20f))
            {
                if (hit.collider.CompareTag(TAG.CHARACTER_OWN))
                {
                    m_tutorialGuideUI_Background.canvasRenderer.SetAlpha(0.5f);
                    m_tutorialGuideUI_Text.canvasRenderer.SetAlpha(0.5f);
                }
                else
                {
                    m_tutorialGuideUI_Background.canvasRenderer.SetAlpha(1f);
                    m_tutorialGuideUI_Text.canvasRenderer.SetAlpha(1f);
                }
            }
        }
        transform.position = pos;
        if (m_isActive == true)
        {
            if (pos.z > 0)
            {
                m_tutorialGuideUI_Background.canvasRenderer.SetAlpha(1);
                m_tutorialGuideUI_Text.canvasRenderer.SetAlpha(1);
            }
            else
            {
                m_tutorialGuideUI_Background.canvasRenderer.SetAlpha(0);
                m_tutorialGuideUI_Text.canvasRenderer.SetAlpha(0);
            }
        }
        /*
        else
        {
            m_questTargetUI_Arrow.transform.parent.gameObject.SetActive(true);
            if (pos.z < 0)
                pos *= -1;

            //make 00 center of screen instead of botton left
            pos -= ScreenCenter;

            //fine angle from center of screen to mouse position
            //화면 중심에서 마우스 위치까지의 각도
            float angle = Mathf.Atan2(pos.y, pos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            pos = ScreenCenter + new Vector3(sin * 150, cos * 150, 0);

            //y = mx * b format
            float m = cos / sin;

            //check up and down first
            //먼저 위 아래를 확인.
            if (cos > 0)
                pos = new Vector3(-ScreenBounds.y / m, ScreenBounds.y, 0);
            else //down
                pos = new Vector3(ScreenBounds.y / m, -ScreenBounds.y, 0);

            //if out of bounds, get point on appropriate side
            //좌 우 확인
            if (pos.x > ScreenBounds.x)
                pos = new Vector3(ScreenBounds.x, -ScreenBounds.x * m, 0);
            else if (pos.x < -ScreenBounds.x)
                pos = new Vector3(-ScreenBounds.x, ScreenBounds.x * m, 0);

            pos += ScreenCenter;

            m_questTagetUI.transform.position = pos;
            m_questTargetUI_Arrow.transform.parent.position = pos;
            m_questTargetUI_Arrow.transform.parent.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
        */
    }

    public void fadeInUI() //밝아짐
    {
        if (m_isActive == false)
        {
            m_isActive = true;
            m_tutorialGuideUI_Background.CrossFadeAlpha(1, m_fadeInTime, false);
            m_tutorialGuideUI_Text.CrossFadeAlpha(1, m_fadeInTime, false);
        }
    }

    public void fadeOutUI()
    {
        if (m_isActive == true)
        {
            m_isActive = false;
            m_tutorialGuideUI_Background.CrossFadeAlpha(0, m_fadeOutTime, false);
            m_tutorialGuideUI_Text.CrossFadeAlpha(0, m_fadeOutTime, false);
        }
    }

    public void setTargetObject(GameObject target) { m_tutorialQuestTarget = target; }
    public void setQuestString(string questString) { m_tutorialGuideUI_Text.text = questString; }
}
