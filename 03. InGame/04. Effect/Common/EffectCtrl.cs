using UnityEngine;
using System.Collections;

public class EffectCtrl : MonoBehaviour
{
    public enum EFFECT_TYPE { ONCE, LOOP }

    public EFFECT_TYPE m_effectType;
    public float m_playTime;
    private AudioSource m_effectSound;

    public void init()
    {
        m_effectSound = GetComponent<AudioSource>();
    }

    public void setPosition_RandomRotScale(Vector3 position)
    {
        float size = UnityEngine.Random.Range(0.3f, 0.8f);
        float rot = UnityEngine.Random.Range(0, 360);

        if (m_effectSound != null)
            m_effectSound.Play();

        transform.localScale = new Vector3(size, size, 1);
        transform.Rotate(Vector3.forward * rot);
        transform.position = position;

        if (m_effectType == EFFECT_TYPE.ONCE)
            Invoke("deActive", m_playTime);
    }

    public void setPosition(Vector3 position)
    {
        if (m_effectSound != null)
            m_effectSound.Play();

        transform.position = position;

        if(m_effectType == EFFECT_TYPE.ONCE)
            Invoke("deActive", m_playTime);
    }

    public void setPosition(Vector3 position, Vector3 direction)
    {
        if(m_effectSound != null)
            m_effectSound.Play();

        transform.position = position + direction * 0.1f;
        transform.up = direction;

        if (m_effectType == EFFECT_TYPE.ONCE)
            Invoke("deActive", m_playTime);
    }

    public void deActive() { gameObject.SetActive(false); }
}
