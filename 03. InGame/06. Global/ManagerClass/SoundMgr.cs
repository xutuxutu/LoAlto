using UnityEngine;
using System.Collections.Generic;

namespace SOUND_POOL
{
    namespace SPARKY
    {
        public enum GUN { FIRE = 0, SPIN_LOOP, SPIN_UP, SPIN_DOWN }
        public enum FOOT_STEP { LEFT = 4, RIGHT, LANDING }
    }
    namespace SAM
    {
        public enum ATTACK { SWING = 0, HIT, }
        public enum SKILL { STEAM_BLOW_START = 6, STEAM_BLOW_HIT, STEAM_BLOW_VOICE, PUVERIZE_START = 9, PUVERIZE_LAND }
        public enum FOOT_STEP { LEFT = 4, RIGHT, LANDING }
    }
    namespace OBJECT
    {
        public enum DOOR { OPERATE = 0, OPERATING, STOP }
        public enum ELEVATOR { OPERATE = 3, OPERATING, SQUEAK, STOP }
    }
    namespace CREATURE
    {
        public enum DRONE { ATTACK_1 = 0, ATTACK_2, ATTACK_3, DIE_1, DIE_2 }
        public enum NORMAL_SOLDIER { N_ATTACK_1 = 5, N_ATTACK_2, N_ATTACK_3, N_ATTACK_4, R_ATTACK_1 = 9, R_ATTACK_2, R_ATTACK_3, R_ATTACK_4 }
        public enum BOSS_FRANKY
        {
            N_ATTACK_1 = 13, CHARGE_FOOT_STEP_1, CHARGE_FOOT_STEP_2, CHARGE_PUNCH_1, CHARGE_PUNCH_2, DEATH_1, DEATH_2, DEATH_3,
            DOWN_1, DOWN_2, SHOCK_WAVE_1, SHOCK_WAVE_2, TRIPPLE_ATTACK_1, TRIPPLE_ATTACK_2, TRIPPLE_ATTACK_3, SHOCK_WAVE_3
        }
    }
    namespace AMBIENT
    {
        public enum RADIO_SET { START = 0, LOOP, END, }
    }
}

public class SoundMgr : MonoBehaviour
{
    private static SoundMgr m_instance;

    private List<AudioClip> m_sparkySoundList;
    private List<AudioClip> m_samSoundList;
    private List<AudioClip> m_objectSoundList;
    private List<AudioClip> m_creatureSoundList;
    private List<AudioClip> m_ambientSoundList;

    // Use this for initialization
    void Awake()
    {
        m_instance = this;
        initSoundList();
    }

    public static SoundMgr getInstance() { return m_instance; }

    public void initSoundList()
    {
        m_sparkySoundList = new List<AudioClip>();
        m_samSoundList = new List<AudioClip>();
        m_objectSoundList = new List<AudioClip>();
        m_creatureSoundList = new List<AudioClip>();
        m_ambientSoundList = new List<AudioClip>();

        initSparkySoundList();
        initSamSoundList();
        initDoorSound();
        initElevatorSound();
        initCreatureSound();
        initAmbientSound();
    }

    private void initSparkySoundList()
    {
        //0 ~ 6
        m_sparkySoundList.Add(Resources.Load(SOUND_PATH.GUN_FIRE) as AudioClip);
        m_sparkySoundList.Add(Resources.Load(SOUND_PATH.GUN_SPIN_LOOP) as AudioClip);
        m_sparkySoundList.Add(Resources.Load(SOUND_PATH.GUN_SPIN_UP) as AudioClip);
        m_sparkySoundList.Add(Resources.Load(SOUND_PATH.GUN_SPIN_DOWN) as AudioClip);
        m_sparkySoundList.Add(Resources.Load(SOUND_PATH.RUN_FOOT_STEP_LEFT) as AudioClip);
        m_sparkySoundList.Add(Resources.Load(SOUND_PATH.RUN_FOOT_STEP_RIGHT) as AudioClip);
        m_sparkySoundList.Add(Resources.Load(SOUND_PATH.RUN_FOOT_STEP_LANDING) as AudioClip);
    }

    private void initSamSoundList()
    {
        //0 ~ 5
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PUNCH_SWING_1) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PUNCH_SWING_2) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PUNCH_SWING_3) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PUNCH_HIT_1) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PUNCH_HIT_2) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PUNCH_HIT_3) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.STEAM_BLOW_0) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.STEAM_BLOW_1) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.STEAM_BLOW_VOICE) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PULVERIZE_START) as AudioClip);
        m_samSoundList.Add(Resources.Load(SOUND_PATH.PULVERIZE_LAND) as AudioClip);
    }

    public void initDoorSound()
    {
        //0 ~ 2
        m_objectSoundList.Add(Resources.Load(SOUND_PATH.DOOR_OPERATE) as AudioClip);
        m_objectSoundList.Add(Resources.Load(SOUND_PATH.DOOR_OPERATING) as AudioClip);
        m_objectSoundList.Add(Resources.Load(SOUND_PATH.DOOR_STOP) as AudioClip);
    }

    public void initElevatorSound()
    {
        //3 ~ 6
        m_objectSoundList.Add(Resources.Load(SOUND_PATH.DOOR_OPERATE) as AudioClip);
        m_objectSoundList.Add(Resources.Load(SOUND_PATH.ELEVATOR_OPERATING) as AudioClip);
        m_objectSoundList.Add(Resources.Load(SOUND_PATH.ELEVATOR_SQUEAK) as AudioClip);
        m_objectSoundList.Add(Resources.Load(SOUND_PATH.ELEVATOR_STOP) as AudioClip);
    }

    public void initCreatureSound()
    {
        // Drone
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_DRONE_FIRE_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_DRONE_FIRE_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_DRONE_FIRE_3) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_DRONE_DIE_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_DRONE_DIE_2) as AudioClip);
        // Knight
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_NORMAL_ATK_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_NORMAL_ATK_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_NORMAL_ATK_3) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_NORMAL_ATK_4) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_RANGE_ATK_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_RANGE_ATK_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_RANGE_ATK_3) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_NORMALSOLDIER_RANGE_ATK_4) as AudioClip);
        // Franky
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_NORMAL_ATK_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_CHARGE_STEP_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_CHARGE_STEP_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_CHARGE_PUNCH_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_CHARGE_PUNCH_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_DEATH_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_DEATH_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_DEATH_3) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_DOWN_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_SHOCK_WAVE_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_SHOCK_WAVE_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_TRIPPLE_ATTACK_1) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_TRIPPLE_ATTACK_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_TRIPPLE_ATTACK_3) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_DOWN_2) as AudioClip);
        m_creatureSoundList.Add(Resources.Load(SOUND_PATH.CREATURE_BOSS_FRANKY_SHOCK_WAVE_3) as AudioClip);
    }

    public void initAmbientSound()
    {
        m_ambientSoundList.Add(Resources.Load(SOUND_PATH.AMBIENT_RADIO_SET_SOUND_START) as AudioClip);
        m_ambientSoundList.Add(Resources.Load(SOUND_PATH.AMBIENT_RADIO_SET_SOUND_LOOP) as AudioClip);
        m_ambientSoundList.Add(Resources.Load(SOUND_PATH.AMBIENT_RADIO_SET_SOUND_END) as AudioClip);
    }

    public AudioClip getSparkyAudioClip(int type) { return m_sparkySoundList[type]; }

    public AudioClip getSamAudioClip(int type) { return m_samSoundList[type]; }

    public AudioClip getObjectAudioClip(int type) { return m_objectSoundList[type]; }

    public AudioClip getCreatureAudioClip(int type) { return m_creatureSoundList[type]; }

    public AudioClip getAmbientAudioClip(int type) { return m_ambientSoundList[type]; }
}
