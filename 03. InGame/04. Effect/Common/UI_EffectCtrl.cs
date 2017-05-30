using UnityEngine;
using System.Collections;

public class UI_EffectCtrl : MonoBehaviour
{
    private Camera m_characterCamera;

    private float SCREEN_HEIGHT;
    private float SCREEN_WITDH;

    Vector3 ScreenCenter;
    Vector3 ScreenBounds;

    // Use this for initialization
    void Start ()
    {
        m_characterCamera = GameObject.Find(OBJECT_NAME.CAMERA).GetComponent<Camera>();
    }

    public void setScreenInfo()
    {
        SCREEN_HEIGHT = Screen.height;
        SCREEN_WITDH = Screen.width;
        ScreenCenter = new Vector3(SCREEN_WITDH, SCREEN_HEIGHT, 0) / 2;
        ScreenBounds = ScreenCenter * 0.9f;
    }

    // Update is called once per frame
    void Update ()
    {
        setScreenInfo();

        Vector3 screenPos = new Vector3(SCREEN_WITDH / 2, SCREEN_HEIGHT / 2, 0);

        Vector3 pos = m_characterCamera.ScreenToWorldPoint(screenPos);

        transform.position = pos;
    }
}
