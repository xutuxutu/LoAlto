using UnityEngine;
using System.Collections;

public class PREFAB_PATH
{
    //카메라
    public const string CHARACTER_CAMERA = "01. Prefab/06. Object/CharacterCamera";
    //캐릭터 - 공통
    public const string CHARACTER_REVIVAL_EFFECT = "01. Prefab/03. Effect/04. Character_Common/fx_revival";
    public const string CHARACTER_REVIVAL_EFFECT_FINISH = "01. Prefab/03. Effect/04. Character_Common/fx_revival_finish";
    public const string CHARACTER_DAMAGED_1 = "01. Prefab/03. Effect/04. Character_Common/fx_damaged_1";
    public const string CHARACTER_ITEM_PARTS = "01. Prefab/06. Object/Item_Parts";
    public const string CHARACTER_ITEM_ENERGY_BATTERY = "01. Prefab/06. Object/Item_BatteryMp";
    //캐릭터 - 샘
    public const string CHARACTER_OWN_SAM = "01. Prefab/01. Character/Character_Own_Sam";
    public const string CHARACTER_OTHER_SAM = "01. Prefab/01. Character/Character_Other_Sam";
    public const string CHARACTER_SAM_MAIN_UI = "01. Prefab/04. UI/01. Character/MainUI_Sam";
    //캐릭터 - 스파키
    public const string CHARACTER_OWN_SPARKY = "01. Prefab/01. Character/Character_Own_Sparky";
    public const string CHARACTER_OTHER_SPARKY = "01. Prefab/01. Character/Character_Other_Sparky";
    public const string CHARACTER_SPARKY_MAIN_UI = "01. Prefab/04. UI/01. Character/MainUI_Sparky";
    //튜토리얼
    public const string QUEST_ITEM_SAM_STEAMBALL = "01. Prefab/06. Object/QuestItem_SteamBall";
    public const string QUEST_ITEM_SPARKY_CARTRIDGE = "01. Prefab/06. Object/QuestItem_Cartridge";
    //스파키 - 공격 관련
    public const string CHARACTER_SPARKY_BULLET = "01. Prefab/03. Effect/02. Sparky/Gun/fx_bullet";
    public const string CHARACTER_SPARKY_BULLET_HIT_EFFECT_GROUND_1 = "01. Prefab/03. Effect/02. Sparky/Gun/fx_groundHit_1";
    public const string CHARACTER_SPARKY_BULLET_HIT_EFFECT_GROUND_2 = "01. Prefab/03. Effect/02. Sparky/Gun/fx_groundHit_2";
    public const string CHARACTER_SPARKY_BULLET_HIT_EFFECT_CREATURE = "01. Prefab/03. Effect/02. Sparky/Gun/fx_creatureHit";
    public const string EXPLOSION_BULLET = "01. Prefab/03. Effect/02. Sparky/Gun/ExplosionBullet";
    public const string EXPLOSION_BUULET_HIT = "01. Prefab/03. Effect/02. Sparky/Gun/fx_explosionBulletHit";
    public const string CHARACTER_SPARKY_RAPID_FIRE_BULLET_1 = "01. Prefab/03. Effect/02. Sparky/Gun/fx_rapidFireBullet_1";
    public const string CHARACTER_SPARKY_RAPID_FIRE_BULLET_2 = "01. Prefab/03. Effect/02. Sparky/Gun/fx_rapidFireBullet_2";
    public const string C4_BOMB = "01. Prefab/03. Effect/02. Sparky/Gun/C4_Bomb";
    public const string C4_BOMB_EXPLOSION = "01. Prefab/03. Effect/02. Sparky/Gun/fx_C4Explosion";
    //스파키 - 행동 관련
    public const string CHARACTER_SPARKY_RUNNING_EFFECT_RIGHT = "01. Prefab/03. Effect/02. Sparky/Behavior/fx_running_right";
    public const string CHARACTER_SPARKY_RUNNING_EFFECT_LEFT = "01. Prefab/03. Effect/02. Sparky/Behavior/fx_running_left";
    public const string CHARACTER_SPARKT_JUMP_EFFECT = "01. Prefab/03. Effect/02. Sparky/Behavior/fx_jumpdust";
    public const string CHARACTER_SPARKT_LANDING_EFFECT = "01. Prefab/03. Effect/02. Sparky/Behavior/fx_Landingdust";
    //샘 - 공격관련
    public const string CHARACTER_SAM_NOMAL_ATTACK_1 = "01. Prefab/03. Effect/01. Sam/Attack/fx_normalAttack_1";
    public const string CHARACTER_SAM_NOMAL_ATTACK_2 = "01. Prefab/03. Effect/01. Sam/Attack/fx_normalAttack_2";

    public const string CHARACTER_SAM_NORMAL_ATTACK_HIT_1 = "01. Prefab/03. Effect/01. Sam/Attack/fx_normalAttackHit_1";
    public const string CHARACTER_SAM_NORMAL_ATTACK_HIT_2 = "01. Prefab/03. Effect/01. Sam/Attack/fx_normalAttackHit_2";
    public const string CHARACTER_SAM_NORMAL_ATTACK_HIT_3 = "01. Prefab/03. Effect/01. Sam/Attack/fx_normalAttackHit_3";

    //크리쳐 
    public const string CREATURE_DRONE_PREFAB = "01. Prefab/02. Creature/Creature_Drone";
    public const string CREATURE_OWL_AIM = "01. Prefab/02. Creature/Creature_Owl_Aim";
    //크리쳐 - 이펙트
    public const string CREATURE_DRONE_GUN_FIRE = "01. Prefab/03. Effect/03. Creature/fx_Creature_DroneGunFire";
    public const string CREATURE_DRONE_BULLET = "01. Prefab/03. Effect/03. Creature/fx_Creature_DroneBullet";
    public const string CREATURE_DRONE_BULLET_HIT = "01. Prefab/03. Effect/03. Creature/fx_Creature_DroneBulletHit";
    public const string CREATURE_DRONE_EXPLOSION = "01. Prefab/03. Effect/03. Creature/fx_Creature_Drone_Explosion";
    public const string CREATURE_OWL_HOWLING = "01. Prefab/03. Effect/03. Creature/fx_Creature_Owl_Howling";
    public const string CREATURE_FRANKY_ATTACK_NORMAL_01 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_Normal_01";
    public const string CREATURE_FRANKY_ATTACK_GROUND_01 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_Ground_01";
    public const string CREATURE_FRANKY_ATTACK_TRIPLE_01 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_Triple_01";
    public const string CREATURE_FRANKY_ATTACK_TRIPLE_02 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_Triple_02";
    public const string CREATURE_FRANKY_ATTACK_SHOCKWAVE_01 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_ShockWave_01";
    public const string CREATURE_FRANKY_ATTACK_SHOCKWAVE_02 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_ShockWave_02";
    public const string CREATURE_FRANKY_ATTACK_SHOCKWAVE_03 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_ShockWave_03";
    public const string CREATURE_FRANKY_ATTACK_SHOCKWAVE_04 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_ShockWave_04";
    public const string CREATURE_FRANKY_ATTACK_CHARGE_PUNCH_01 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_Charge_Punch_01";
    public const string CREATURE_FRANKY_ATTACK_CHARGE_PUNCH_02 = "01. Prefab/03. Effect/03. Creature/fx_Creature_Franky_Attack_Charge_Punch_02";
    public const string CREATURE_KNIGHT_GUN_FIRE_01 = "01. Prefab/03. Effect/03. Creature/fx_Creature_gun_fire01";
    public const string CREATURE_KNIGHT_GUN_FIRE_02 = "01. Prefab/03. Effect/03. Creature/fx_Creature_gun_fire02";
    public const string CREATURE_KNIGHT_GUN_FIRE_03 = "01. Prefab/03. Effect/03. Creature/fx_Creature_gun_fire03";
    public const string CREATURE_FIRE_ATTACK_01 = "01. Prefab/03. Effect/03. Creature/fx_Creature_FireAttack_01";
    //크리쳐 - UI
    public const string CREATURE_GUIDELINE_CONTROL = "01. Prefab/04. UI/02. Creature/Creature_GuideLine_Control";
    public const string CREATURE_GUIDELINE_DRONE = "01. Prefab/04. UI/02. Creature/Creature_GuideLine_Drone";
    public const string CREATURE_GUIDELINE_FRANKY_CIRCLE = "01. Prefab/04. UI/02. Creature/Creature_GuideLine_Franky_Circle";
    public const string CREATURE_GUIDELINE_FRANKY_SECTOR = "01. Prefab/04. UI/02. Creature/Creature_GuideLine_Franky_Rectangle";
    public const string CREATURE_GUIDELINE_FRANKY_RECTANGLE = "01. Prefab/04. UI/02. Creature/Creature_GuideLine_Franky_Sector";

    //매니저 클래스
    public const string INGAME_NETWORK_MANAGER = "01. Prefab/05. Global/NetworkManager";
    public const string PROJECT_MANAGER = "01. Prefab/05. Global/ProjectManager";

}

public class SOUND_PATH
{
    //캐릭터 - 스파키
    public const string GUN_FIRE = "03. Sound/01. Character/02. Sparky/Fire/se_Minigun_Firing_once";
    public const string GUN_SPIN_UP = "03. Sound/01. Character/02. Sparky/Spin/se_CharaB_Minigun_SpinUp";
    public const string GUN_SPIN_LOOP = "03. Sound/01. Character/02. Sparky/Spin/se_CharaB_Minigun_Loop";
    public const string GUN_SPIN_DOWN = "03. Sound/01. Character/02. Sparky/Spin/se_CharaB_Minigun_SpinDown";
    public const string RUN_FOOT_STEP_LEFT = "03. Sound/01. Character/02. Sparky/Move/se_Run_Left";
    public const string RUN_FOOT_STEP_RIGHT = "03. Sound/01. Character/02. Sparky/Move/se_Run_Right";
    public const string RUN_FOOT_STEP_LANDING = "03. Sound/01. Character/02. Sparky/Move/se_Jump_Landing_Sparky";
    //캐릭터 샘
    public const string PUNCH_SWING_1 = "03. Sound/01. Character/01. Sam/01. Attack/se_Punch_Swing_01";
    public const string PUNCH_SWING_2 = "03. Sound/01. Character/01. Sam/01. Attack/se_Punch_Swing_02";
    public const string PUNCH_SWING_3 = "03. Sound/01. Character/01. Sam/01. Attack/se_Punch_Swing_03";
    public const string PUNCH_HIT_1 = "03. Sound/01. Character/01. Sam/01. Attack/se_Punch_Hit_01";
    public const string PUNCH_HIT_2 = "03. Sound/01. Character/01. Sam/01. Attack/se_Punch_Hit_02";
    public const string PUNCH_HIT_3 = "03. Sound/01. Character/01. Sam/01. Attack/se_Punch_Hit_03";
    public const string STEAM_BLOW_0 = "03. Sound/01. Character/01. Sam/01. Attack/se_Sam_SteamBlow_0";
    public const string STEAM_BLOW_1 = "03. Sound/01. Character/01. Sam/01. Attack/se_Sam_SteamBlow_1";
    public const string STEAM_BLOW_VOICE = "03. Sound/01. Character/01. Sam/01. Attack/se_Sam_SteamBlow_Voice";
    public const string PULVERIZE_START = "03. Sound/01. Character/01. Sam/01. Attack/se_Sam_Pulverize_Start";
    public const string PULVERIZE_LAND = "03. Sound/01. Character/01. Sam/01. Attack/se_Sam_Pulverize_Land";

    //오브젝트
    public const string DOOR_OPERATE = "03. Sound/03. Object/02. Door/se_DoorLoopStart";
    public const string DOOR_OPERATING = "03. Sound/03. Object/02. Door/se_DoorLoop";
    public const string DOOR_STOP = "03. Sound/03. Object/02. Door/se_DoorLoopEnd";
    public const string ELEVATOR_OPERATE = "03. Sound/03. Object/03. Elevator/se_ElevatorOperate";
    public const string ELEVATOR_OPERATING = "03. Sound/03. Object/03. Elevator/se_ElevatorOperating";
    public const string ELEVATOR_SQUEAK = "03. Sound/03. Object/03. Elevator/se_ElevatorSqueak";
    public const string ELEVATOR_STOP = "03. Sound/03. Object/03. Elevator/se_ElevatorStop";
    //크리쳐
    //드론
    public const string CREATURE_DRONE_FIRE_1 = "03. Sound/02. Creature/02. Drone/se_DroneAttack_001";
    public const string CREATURE_DRONE_FIRE_2 = "03. Sound/02. Creature/02. Drone/se_DroneAttack_002";
    public const string CREATURE_DRONE_FIRE_3 = "03. Sound/02. Creature/02. Drone/se_DroneAttack_003";
    public const string CREATURE_DRONE_DIE_1 = "03. Sound/02. Creature/02. Drone/se_Explosions_001";
    public const string CREATURE_DRONE_DIE_2 = "03. Sound/02. Creature/02. Drone/se_Explosions_002";
    //일반병
    public const string CREATURE_NORMALSOLDIER_NORMAL_ATK_1 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureNM_Att001";
    public const string CREATURE_NORMALSOLDIER_NORMAL_ATK_2 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureNM_Att002";
    public const string CREATURE_NORMALSOLDIER_NORMAL_ATK_3 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureNM_Att003";
    public const string CREATURE_NORMALSOLDIER_NORMAL_ATK_4 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureNM_Att004";

    public const string CREATURE_NORMALSOLDIER_RANGE_ATK_1 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureGUN_Att001";
    public const string CREATURE_NORMALSOLDIER_RANGE_ATK_2 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureGUN_Att002";
    public const string CREATURE_NORMALSOLDIER_RANGE_ATK_3 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureGUN_Att003";
    public const string CREATURE_NORMALSOLDIER_RANGE_ATK_4 = "03. Sound/02. Creature/03. NormalSoldier/se_CreatureGUN_Att004";
    //프랭키
    public const string CREATURE_BOSS_FRANKY_NORMAL_ATK_1 = "03. Sound/02. Creature/04. Franky/NormalAttack_01/se_Franky_NormalAttack";
    public const string CREATURE_BOSS_FRANKY_CHARGE_STEP_1 = "03. Sound/02. Creature/04. Franky/Charge/se_Franky_ChargeFootStep01";
    public const string CREATURE_BOSS_FRANKY_CHARGE_STEP_2 = "03. Sound/02. Creature/04. Franky/Charge/se_Franky_ChargeFootStep02";
    public const string CREATURE_BOSS_FRANKY_CHARGE_PUNCH_1 = "03. Sound/02. Creature/04. Franky/ChargePunch/se_Franky_ChargePunch01";
    public const string CREATURE_BOSS_FRANKY_CHARGE_PUNCH_2 = "03. Sound/02. Creature/04. Franky/ChargePunch/se_Franky_ChargePunch02";
    public const string CREATURE_BOSS_FRANKY_DEATH_1 = "03. Sound/02. Creature/04. Franky/Death/se_Franky_Death01";
    public const string CREATURE_BOSS_FRANKY_DEATH_2 = "03. Sound/02. Creature/04. Franky/Death/se_Franky_Death02";
    public const string CREATURE_BOSS_FRANKY_DEATH_3 = "03. Sound/02. Creature/04. Franky/Death/se_Franky_Death03";
    public const string CREATURE_BOSS_FRANKY_DOWN_1 = "03. Sound/02. Creature/04. Franky/Down/se_Franky_Down01";
    public const string CREATURE_BOSS_FRANKY_SHOCK_WAVE_1 = "03. Sound/02. Creature/04. Franky/ShockWave/se_Franky_ShockWave01";
    public const string CREATURE_BOSS_FRANKY_SHOCK_WAVE_2 = "03. Sound/02. Creature/04. Franky/ShockWave/se_Franky_ShockWave02";
    public const string CREATURE_BOSS_FRANKY_TRIPPLE_ATTACK_1 = "03. Sound/02. Creature/04. Franky/TrippleAttack/se_Franky_TrippleAttack_01";
    public const string CREATURE_BOSS_FRANKY_TRIPPLE_ATTACK_2 = "03. Sound/02. Creature/04. Franky/TrippleAttack/se_Franky_TrippleAttack_02";
    public const string CREATURE_BOSS_FRANKY_TRIPPLE_ATTACK_3 = "03. Sound/02. Creature/04. Franky/TrippleAttack/se_Franky_TrippleAttack_03";
    public const string CREATURE_BOSS_FRANKY_DOWN_2 = "03. Sound/02. Creature/04. Franky/Down/se_Franky_Down02";
    public const string CREATURE_BOSS_FRANKY_SHOCK_WAVE_3 = "03. Sound/02. Creature/04. Franky/ShockWave/se_Franky_ShockWave03";

    //환경음
    public const string AMBIENT_RADIO_SET_SOUND_START = "03. Sound/04. Ambient/03. RadioSet/se_TwoWay_Raido_Start";
    public const string AMBIENT_RADIO_SET_SOUND_LOOP = "03. Sound/04. Ambient/03. RadioSet/se_TwoWay_Raido_Loop";
    public const string AMBIENT_RADIO_SET_SOUND_END = "03. Sound/04. Ambient/03. RadioSet/se_TwoWay_Raido_End";
}

public class RESOURCE_PATH
{
    //폰트
    public const string FONT_NUMBER_0 = "01. Prefab/04. UI/03. Number/Number_0";
    public const string FONT_NUMBER_1 = "01. Prefab/04. UI/03. Number/Number_1";
    public const string FONT_NUMBER_2 = "01. Prefab/04. UI/03. Number/Number_2";
    public const string FONT_NUMBER_3 = "01. Prefab/04. UI/03. Number/Number_3";
    public const string FONT_NUMBER_4 = "01. Prefab/04. UI/03. Number/Number_4";
    public const string FONT_NUMBER_5 = "01. Prefab/04. UI/03. Number/Number_5";
    public const string FONT_NUMBER_6 = "01. Prefab/04. UI/03. Number/Number_6";
    public const string FONT_NUMBER_7 = "01. Prefab/04. UI/03. Number/Number_7";
    public const string FONT_NUMBER_8 = "01. Prefab/04. UI/03. Number/Number_8";
    public const string FONT_NUMBER_9 = "01. Prefab/04. UI/03. Number/Number_9";
    public const string FONT_SLASH = "01. Prefab/04. UI/03. Number/Slash";
}

//레이어 번호
public class LAYER
{
    public const int Character = 8;
    public const int Creature = 9;
    public const int EventTrigger = 10;
    public const int Effect = 11;
}

//태그 이름
public class TAG
{
    //오브젝트
    public const string BUTTON = "BUTTON";
    public const string MOVING_SCRIPT = "MOVING_SCRIPT";
    public const string MOVING_OBJECT = "MOVING_OBJECT";
    public const string PASSAGE_WAY = "PASSAGE_WAY";
    public const string RISING_OBJECT = "RISING_OBJECT";
    public const string ROTATING_OBJECT = "ROTATING_OBJECT";
    public const string GROUND = "GROUND";
    public const string STAIR = "STAIR";
    public const string WALL = "WALL";
    public const string QUEST_ITEM = "QUEST_ITEM";
    //캐릭터 - 공통
    public const string CHARACTER_OWN = "CHARACTER_OWN";
    public const string CHARACTER_OTHER = "CHARACTER_OTHER";
    public const string COMMUNICATE_OBJECT = "COMMUNICATE_OBJECT";
    //캐릭터 - 스파키
    public const string EXPLOSION_BULLET = "EXPLOSION_BULLET";
    //몬스터
    public const string CREATURE = "CREATURE";
    public const string WAYPOINT = "WAYPOINT";
}

//오브젝트 이름
public class OBJECT_NAME
{
    //OutGame
    public const string BUTTON_LOGIN = "Button_Login";
    public const string BUTTON_JOIN = "Button_Join";
    public const string BUTTON_DEVELOPER_INFO = "Button_DeveloperInfo";
    public const string BUTTON_EXIT = "Button_Exit";
    public const string LOGIN_ID_INPUT_FIELD = "Login_InputField_ID";
    public const string LOGIN_PW_INPUT_FIELD = "Login_InputField_PW";
    public const string JOIN_ID_INPUT_FIELD = "Join_InputField_ID";
    public const string JOIN_PW_INPUT_FIELD = "Join_InputField_PW";
    public const string JOIN_PW_INPUT_FIELD_CF = "Join_InputField_PW_Confirm";
    public const string INPUT_FIELD_TEXT = "InputText";
    public const string LOGIN_UI = "LoginUI";
    public const string JOIN_UI = "JoinUI";
    public const string JOIN_CONFIRM_BUTTON = "Join_Button_Confirm";
    public const string JOIN_CANCLE_BUTTON = "Join_Button_Cancle";
    public const string LOGIN_CONFIRM_BUTTON = "Login_Button_Login";
    public const string LOGIN_CANCLE_BUTTON = "Login_Button_Cancle";
    public const string GAME_MESSAGE_UI = "GameMessageUI";
    public const string GAME_MESSAGE_TEXT = "MessageText";
    public const string GAME_MESSAGE_CONFIRM_BUTTON = "Button_Message_Confirm";

    //InGame
    //캐릭터 공통
    public const string CHARACTER_OWN = "Character_OWN";
    public const string CHARACTER_OTHER = "Character_OTHER";
    public const string CAMERA = "CharacterCamera";
    public const string DEAD_VIEW_CAMERA_CTRL = "DeadViewCameraCtrl";
    public const string DEAD_VIEW_CAMERA_TARGET = "DeadViewTarget";
    public const string VIEW_TARGET = "ViewObject";
    public const string COMUNICATION_OBJECT = "CommunicationObject";
    public const string ATTACK_AREA = "AttackArea";
    public const string RUNNING_DUST_POSITION = "RunningDustPosition";

    //캐릭터 공통 - UI
    public const string TUTORIAL_IMAGE = "TutorialImage_";
    public const string TUTORIAL_GUIDE_UI = "TutorialGuideUI";
    public const string DIALOGUE_SCREEN = "DialogueScreen";
    public const string DIALOGUE_SCRIPT = "DialogueScript";
    public const string DIALOGUE_SCRIPT_BACK = "DialogueScript_Back";
    public const string DIALOGUE_LAYOUT = "DialougeLayout";
    public const string HP_GUAGE = "HP_Guage";
    public const string HP_GUAGE_ALPHA = "HP_Guage_Alpha";
    public const string HP_GUAGE_PISTON = "HP_Guage_Piston";
    public const string HP_GUAGE_PISTON_BAR = "HP_Guage_Piston_Bar";
    public const string EP_GUAGE = "EP_Guage";
    public const string EP_GUAGE_ALPHA = "EP_Guage_Alpha";
    public const string EP_GUAGE_PISTON = "EP_Guage_Piston";
    public const string EP_GUAGE_PISTON_BAR = "EP_Guage_Piston_Bar";
    public const string OBJECT_UI = "ObjectUI";
    public const string OBJECT_UI_TEXT = "ObjectUI_Text";
    public const string QUEST_UI = "QuestUI";
    public const string QUEST_TARGET_UI = "QuestTargetUI";
    public const string QUEST_TARGET_UI_ARROW = "QuestTargetUI_Arrow";
    public const string QUEST_UI_TEXT = "QuestUI_Text";
    public const string QUEST_UI_MAIN_ALARM = "MainQuestAlarm";
    public const string QUEST_UI_MAIN_ALARM_TEXT = "QuestText";
    public const string ERROR_MESSGE = "ErrorMessage";
    public const string DAMAGED_UI = "DamagedUI";
    public const string DEATH_UI = "DeathUI";
    public const string PARTS_GET_EFFECT = "PartsGetEffect_";
    public const string PARTS_NUMBER = "PartsNumber_";
    public const string REVIVAL_UI = "RevivalUI";
    public const string REVIVAL_Bar = "RevivalBar";
    public const string REVIVAL_GUAGE = "RevivalGuage";
    public const string GAME_OVER_UI = "GameOverUI";
    public const string GAME_OVER_WAIT_OTHER_UI = "GameOver_WaitOther";
    public const string INTRODUCE_VIREO = "ScreenSaver";
    public const string COMMON_UI = "CommonUI";
    public const string EXIT_UI = "ExitUI";

    //캐릭터 - 스파키
    public const string FIRE_POINT = "FirePoint";
    public const string FIRE_EFFECT = "FireEffect";
    public const string GUN_POSITION = "Bone_Gun";
    public const string EXPLOSION_POSITION = "ExplosionPosition";
    public const string RIGHT_FOOT = "Bip001 R Toe0";
    public const string LEFT_FOOT = "Bip001 L Toe0";
    public const string SPINE = "Bip001 Spine1";
    public const string GUN_MUZZLE = "Gun_01";
    public const string C4_THROW_POSITION = "C4ThrowPosition";

    //캐릭터 - 샘
    public const string ATTACK_AREA_LEFT = "AttackArea_Left";
    public const string ATTACK_AREA_RIGHT = "AttackArea_Right";

    //캐릭터 - 스파키 - UI
    public const string OVERHEAT_GUAGE = "HeatGuage";
    public const string RAPID_FIRE_GUAGE = "RapidFireGuage";
    public const string BULLET_SLOT = "BulletSlot";
    public const string COOLTIME_EXPLOSION_BULLET_UI = "CoolTime_ExplosionBullet";
    public const string COOLTIME_C4BOMB_UI = "CoolTime_C4Bomb";
    public const string COOLTIME_TESLA_BULLET_UI = "CoolTime_TeslaBullet";
    public const string COOLTIME_RAPID_FIRE_UI = "CoolTime_RapidFire";

    //캐릭터 - 샘 - UI
    public const string COOL_TIME_STEAM_BLOW_UI = "CoolTime_SteamBlow";
    public const string COOL_TIME_PULVERIZE_UI = "CoolTime_Pulverize";

    //크리쳐
    public const string CREATURE_OWL_AIM = "Creature_Owl_Aim";

    //오브젝트 - 공통
    public const string GAME_EVENT = "GameEvent";
    public const string NETWORK_MANAGER = "NetworkManager";
    public const string CORRIDOR_CAMERA = "CorridorCamera";
    public const string BLACK_SCREEN = "BlackScreen";
    public const string QUEST_TRIGGER = "QuestTrigger";
    public const string LOADING_IMAGE = "LoadingImage";
}

public class SCENE
{
    public const string OUT_GAME = "OutGameScene(160902)";
    public const string AREA1_STAGE1 = "Ar1St1(160831)";
    public const string AREA1_STAGE2 = "Ar1St2(160517)";
    public const string AREA1_STAGE3 = "Ar1St3(160528)";
    public const string AREA1_STAGE4 = "Ar1St4(160607)";
    public const string ENDING = "EndingScene";

    public enum INDEX { OUT_GAME = -1, Ar1St1, Ar1St2, Ar1St3, Ar1St4, ENDING }
}

namespace ATTACK
{
    public enum TYPE { NONE, NOMAL, SMASH, };
    public enum STATE_DISORDER { NONE, NOCK_BACK, DOWN, };
}

public struct ATTACK_INFO
{
    public float damage;
    public Vector3 attackPos;
    public float forcedDist;
    public ATTACK.STATE_DISORDER stateDisorder;
}