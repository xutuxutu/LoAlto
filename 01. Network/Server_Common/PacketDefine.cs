using System.Collections.Generic;

namespace NET_HTTP
{
    public struct URL_NAME
    {
        public const string JOIN = "InsertUserInfo";
        public const string LOGIN = "Login";
        public const string CHARACTER_DATA = "GetStandardCharacterStatus";
        public const string CREATURE_DATA = "GetStandardCreatureStatus";
        public const string ITEM_DATA = "GetStandardItemStatus";
    }

    namespace SEND
    {
        public struct JOIN_ACCOUNT_INFO
        {
            public string USER_IDNT;
            public string USER_PSWD;
        }

        public struct LOGIN_ACCOUNT_INFO
        {
            public string USER_IDNT;
            public string USER_PSWD;
        }
    }

    namespace RECV
    {
        public struct JSON_TYPE_IDENTIFY
        {
            public string type;
        }

        public struct JOIN_RESULT_INFO
        {
            public string isSuccess;
            public string text;
        }

        public struct LOGIN_RESULT_INFO
        {
            public string pID;
            public string isSuccess;
            public string text;
        }
        
        public struct CHARACTER_DB_DATA
        {
            public string type;
            public string isSuccess;
            public List<CHARACTER_INFO> result;
            public List<CHARACTER_NORMAL_ATTACK_INFO> aResult;
            public List<CHARACTER_SKILL_DATA> sResult;
        }

        public struct CHARACTER_INFO
        {
            public string CHAR_TYPE;    //캐릭터 타입
            public string HLTH_PONT;    //체력
            public string ENRG_PONT;    //에너지포인트
            //public string ATCK_PT01;    //기본 공격력_1
            //public string ATCK_PT02;    //기본 공격력_2
            //public string ATCK_PT03;    //기본 공격력_3
            public string DFND_PONT;    //방어력
            public string HEAL_SCND;    //초당 체력 재생률
            public string UABL_RGNT;    //회복 불가 시간
            public string WALK_SPED;    //걷기 속도
            public string RUNN_SPED;    //달리기 속도
            public string JUMP_SPED;    //점프 속도
            public string GRVT_PONT;    //중력 값
            public string DDGE_SPED;    //회피 속도
        }

        public struct CHARACTER_NORMAL_ATTACK_INFO
        {
            public string CHAR_TYPE;
            public string ATCK_TYPE;
            public string ATCK_PONT;
        }

        public struct CHARACTER_SKILL_DATA
        {
            public string CHAR_TYPE;
            public string SKIL_TYPE;
            public string BOMB_ATPT;    //C4
            public string CNTT_ATPT;    //SB
            public string ATCK_RNGE;    //C4, EB
            public string ATCK_WDTH;    //SB
            public string ATCK_LNTH;    //SB
            public string USNG_MPNT;
            public string COOL_TIME;
            public string THRW_POWR;    //C4
            public string GRVT_POWR;    //EB
            public string OBJT_SPED;    //EB
        }

        public struct CREATURE_DB_DATA
        {
            public string type;
            public string isSuccess;
            public List<CREATURE_COMMON_DATA> result;
            public List<CREATURE_ATTACK_DATA> aResult;
            public List<CREATURE_DROP_DATA> iResult;
        }

        public struct CREATURE_ITEM_DB_DATA
        {
            public string type;
            public string isSuccess;
            public List<CREATURE_ITEM_DATA> result;
        }


        public class CREATURE_COMMON_DATA
        {
            public string CRIT_TYPE;                       // 크리쳐 종류
            public string HLTH_PONT;                       // 체력

            public string DEFE_POWR;                       // 방어력
            public string DEFE_SHLD;                  // 방패 감소율

            public string RELZ_RAIS;                       // 인식 범위(반지름)
            public string RELZ_TIME;                  // 인식 체크 시간

            public string PATL_SPED;                  // 감시 움직임 속도
            public string TRCE_SPED;                  // 추적 움직임 속도
            public string BESG_SPED;                  // 포위 움직임 속도
            public string RETR_SPED;                  // 후퇴 움직임 속도
            public string THRE_SPED;                  // 위협 움직임 속도
            public string BESG_RAIS;                  // 포위 최대 범위 반지름
            public string RETR_RAIS;                  // 후퇴 최대 범위 반지름
            public string THRE_RAIS;                  // 위협 최대 범위 반지름
            public string RETR_MOVE;                  // 후퇴 움직임 속도
            public string THRE_MOVE;                  // 위협 움직임 속도

            public string DRON_CONT;                       // 드론개수
        }

        public class CREATURE_ATTACK_DATA
        {
            public string CRIT_TYPE;                       // 크리쳐 종류
            public string ATCK_TYPE;                       // 공격 종류
            public string ATCK_PONT;                  // 공격 포인트
            public string ATCK_DSTC;                  // 공격 포인트
            public string ATCK_RNGE;                  // 공격 포인트
            public string ATCK_DLAY;                    // 공격 딜레이 시간
        }

        public class CREATURE_DROP_DATA
        {
            public string CRIT_TYPE;                       // 크리쳐 종류
            public string ITEM_TYPE;                       // 아이템 종류

            public string DROP_MINI;                       // 드랍 아이템 최솟값
            public string DROP_MAXI;                       // 드랍 아이템 최댓값
        }

        public class CREATURE_ITEM_DATA
        {
            public string ITEM_TYPE;                       // 아이템 종류
            public string GAIN_VALU;                       // 얻을이득값

            public string CHSE_ACCL;                  // 움직임 가속도
            public string CHSE_SPED;                  // 움직임 기본속력
        }
    }
}
namespace NET_OUTGAME
{
    namespace SEND
    {
        public enum PACKET_TYPE
        {
            SEND_PLAYER_ID_EM = 0,
            SEND_CHARACTER_SELECT_INFO_EM = 1,
            SEND_PLAYER_READY_EM = 2,

            SEND_PLAYER_EXIT_ROOM_EM = 6,
        }

        public struct PACKET_SIZE
        {
            public const int SEND_PLAYER_ID = sizeof(int) * 3;
            public const int SEND_CHARACTER_SELECT_INFO = sizeof(int) * 4;
            public const int SEND_PLAYER_READY = sizeof(int) * 3 + sizeof(bool);
            public const int SEND_PLAYER_EXIT_ROOM = sizeof(int) * 3;
        }
    }
    namespace RECV
    {
        public enum PACKET_TYPE
        {
            RECV_PLAYER_ID_CONFIRM_EM = 0,
            RECV_CHARACTER_SELECT_INFO_EM = 1,
            RECV_PLAYER_READY_EM = 2,
            RECV_GAME_START_COUNT_DOWN_EM = 3,
            RECV_GAME_START_EM = 4,
            RECV_ALL_USER_ENTER_EM = 5,
            RECV_PLAYER_EXIT_ROOM_EM = 6,
        }

        public struct PACKET_SIZE
        {
            public const int RECV_PLAYER_ID_CONFIRM = sizeof(int);
            public const int RECV_CHARACTER_SELECT_INFO = sizeof(int) * 3 + sizeof(bool) * 2;
            public const int RECV_PLAYER_READY = sizeof(int) * 2 + sizeof(bool);
            public const int RECV_GAME_START_COUNT_DOWN = sizeof(int);
            public const int RECV_GAME_START = sizeof(int);
            public const int RECV_ALL_USER_ENTER = sizeof(int) * 3;
            public const int RECV_PLAYER_EXIT_ROOM = sizeof(int);

            public static int getTypeToSize(NET_OUTGAME.RECV.PACKET_TYPE type)
            {
                switch (type)
                {
                    case PACKET_TYPE.RECV_PLAYER_ID_CONFIRM_EM:
                        return RECV_PLAYER_ID_CONFIRM;
                    case PACKET_TYPE.RECV_CHARACTER_SELECT_INFO_EM:
                        return RECV_CHARACTER_SELECT_INFO;
                    case PACKET_TYPE.RECV_PLAYER_READY_EM:
                        return RECV_PLAYER_READY;
                    case PACKET_TYPE.RECV_GAME_START_COUNT_DOWN_EM:
                        return RECV_GAME_START_COUNT_DOWN;
                    case PACKET_TYPE.RECV_GAME_START_EM:
                        return RECV_GAME_START;
                    case PACKET_TYPE.RECV_ALL_USER_ENTER_EM:
                        return RECV_ALL_USER_ENTER;
                    case PACKET_TYPE.RECV_PLAYER_EXIT_ROOM_EM :
                        return RECV_PLAYER_EXIT_ROOM;
                }
                return 0;
            }
        }
    }
}



namespace NET_INGAME
{
    namespace SEND
    {
        public enum PACKET_TYPE
        {
            NONE = -1,
            REQUEST_PLAYER_LOGIN_EM,
            REQUEST_PLAYER_READY_EM,

            REQUEST_TRANSFORM_EM = 7,               // 플레이어 이동 시작 요청
            REQUEST_JUMP_EM = 8,
            REQUEST_OBJECT_ACTIVE_EM = 9,
            REQUEST_OBJECT_DEACTIVE_EM = 10,

            //REQUEST_CHARCTER_ATTACK_INFO = 11,

            REQUEST_CREATURE_DAMAGE_INFO = 12,      
            REQUEST_CREATURE_STATE_INFO = 13,
            REQUEST_CHARACTER_DAMAGE_INFO = 14,
            REQUEST_CREATURE_TRANSFORM_INFO = 15,

            REQUEST_CHARACTER_NORMAL_ATTACK_INFO = 16,
            REQUEST_CHARACTER_SPARKY_SMASH_ATTACK_INFO = 17,
            REQUEST_OBJECT_STATE_EM = 18,
            REQUEST_CHARACTER_SPARKY_AIM_POINT_EM = 19,

            REQUEST_CHARACTER_SAM_BATTLE_IDLE_INFO_EM = 20,

            REQUEST_CHARACTER_DODGE_INFO_EM = 21,
            REQUEST_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_EM = 22,
            REQUEST_CHARACTER_DIE_INFO_EM = 23,
            REQUEST_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM = 24,
            REQUEST_CHARACTER_SAM_NORAML_ATTACK_INFO_EM = 25,
            REQUEST_PLAYER_GAME_RETRY_EM = 26,
            REQUEST_CHARCATER_SPARKY_C4_BOMB_THROW_INFO_EM = 27,
            REQUEST_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM = 28,
            REQUEST_CHARCATER_SPARKY_C4_BOMB_DETONATION_ORDER_EM = 29,
            REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_EM = 30,
            REQUEST_CHARACTER_DOWN_INFO_EM = 31,
            REQUEST_CAHRACTER_SAM_PULVERIZE_INFO_EM = 33,
            REQUEST_TRIGGER_ACTIVE_EM = 34,
        }

        public struct PACKET_SIZE
        {
            //캐릭터 공통
            public const int PLAYER_LOGIN = sizeof(int) * 3; //12
            public const int PLAYER_READY = sizeof(int) * 3; //12
            public const int CHARACTER_NORMAL_ATTACK_INFO = sizeof(int) * 3 + sizeof(bool) * 2;   //14
            public const int PLAYER_JUMP = sizeof(int) * 3 + sizeof(bool); //9
            public const int PLAYER_TRANSFORM = (sizeof(int) * 3) + (sizeof(float) * 6) + sizeof(bool); //37
            public const int CHARACTER_DAMAGE_INFO = sizeof(int) * 6 + sizeof(float);         //20
            public const int CHARACTER_DODGE_INFO = sizeof(int) * 4 + sizeof(bool);
            public const int CHARACTER_DIE_INFO = sizeof(int) * 3 + sizeof(bool);
            public const int PLAYER_GAME_RETRY = sizeof(int) * 3 + sizeof(bool);            //13
            public const int CHARACTER_DOWN_INFO = sizeof(int) * 3;
            //오브젝트
            public const int OBJECT_ACTIVE = sizeof(int) * 6; //16
            public const int OBJECT_DEACTIVE = sizeof(int) * 6; //16
            public const int OBJECT_STATE = sizeof(int) * 6;
            public const int TRIGGER_ACTIVE = sizeof(int) * 4;
            //크리쳐
            public const int CREATURE_DAMAGE_INFO = sizeof(int) * 6;        //20
            public const int CREATURE_STATE_INFO = sizeof(int) * 7 + sizeof(float) * 3;         //40
            public const int CREATURE_TRANSFORM_INFO = sizeof(int) * 5 + sizeof(float) * 7 + sizeof(int);     //44
            //캐릭터 - 스파키
            public const int CHARACTER_SPARKY_SMASH_ATTACK_INFO = sizeof(int) * 3 + sizeof(bool) * 1 + sizeof(int);   //9
            public const int CHARACTER_SPARKY_AIM_POINT_INFO = sizeof(int) * 3 + sizeof(float) * 3;     //24
            public const int CHARACTER_SPARKY_EXPLOSION_BULLET_INFO = sizeof(int) * 3 + sizeof(bool) + sizeof(float) * 6; //33
            public const int CHARACTER_SPARKY_RAPID_FIRE_INFO = sizeof(int) * 3 + sizeof(bool) * 2;
            public const int CHARACTER_SPARKY_C4_BOMB_THROW_INFO = sizeof(int) * 3 + sizeof(float) * 3;
            public const int CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO = sizeof(int) * 3 + sizeof(float) * 6;
            public const int CHARACTER_SPARKY_C4_DETONATION_ORDER = sizeof(int) * 3 + sizeof(float) * 3;
            //캐릭터 - 샘
            public const int CHARACTER_SAM_NORAML_ATTACK_INFO = sizeof(int) * 3 + sizeof(bool) + sizeof(int);
            public const int CHARACTER_SAM_STEAM_BLOW_INFO = sizeof(int) * 3 + sizeof(bool) * 2;
            public const int CHARACTER_SAM_BATTLE_IDLE_INFO = sizeof(int) * 3 + sizeof(bool);
            public const int CAHRACTER_SAM_PULVERIZE_INFO = sizeof(int) * 3;
        }
    }

    namespace RECV
    {
        public enum PACKET_TYPE
        {
            NONE = -1,
            PLAYER_LOGIN_PERMISSION_EM,
            NOTIFY_START_GAME__EM,

            NOTIFY_TRANSFORM_EM = 7,
            NOTYFY_JUMP_EM = 8,
            NOTYFY_OBJECT_ACTIVE_EM = 9,
            NOTYFY_OBJECT_DEACTIVE_EM = 10,

            NOTIFY_CREATURE_DAMAGE_INFO = 12,
            NOTIFY_CREATURE_STATE_INFO = 13,
            NOTIFY_CHARACTER_DAMAGE_INFO = 14,
            NOTIFY_CREATURE_TRANSFORM_INFO = 15,

            NOTIFY_CHARACTER_NORMAL_ATTACK_INFO = 16,
            NOTIFY_CHARACTER_SPARKY_SMASH_ATTACK_INFO = 17,
            NOTIFY_OBJECT_STATE_EM = 18,
            NOTIFY_CHARACTER_SPARKY_AIM_POINT_EM = 19,

            NOTIFY_CHARACTER_SAM_BATTLE_IDLE_INFO_EM = 20,

            NOTIFY_CHARACTER_DODGE_INFO_EM = 21,
            NOTIFY_CHARACTER_SPARKY_EXPLOSION_BULLET_INFO_EM = 22,
            NOTIFY_CHARACTER_DIE_INFO_EM = 23,
            NOTIFY_CHARACTER_SPARKY_RAPID_FIRE_INFO_EM = 24,
            NOTIFY_CHARACTER_SAM_NORAML_ATTACK_INFO_EM = 25,
            NOTIFY_PLAYER_GAME_RETRY_EM = 26,
            NOTIFY_CHARCATER_SPARKY_C4_BOMB_THROW_INFO_EM = 27,
            NOTIFY_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM = 28,
            NOTIFY_CHARCATER_SPARKY_C4_BOMB_DETONATION_ORDER_EM = 29,
            NOTIFY_CHARACTER_SAM_STEAM_BLOW_INFO_EM = 30,
            NOTIFY_CHARACTER_DOWN_EM = 31,
            NOTIFY_CAHRACTER_SAM_PULVERIZE_INFO_EM = 33,
            NOTIFY_TRIGGER_ACTIVE_EM = 34,
    }

        public struct PACKET_SIZE
        {
            //캐릭터 - 공통
            public const int CHARACTER_NORMAL_ATTACK_INFO = sizeof(int) * 3 + sizeof(bool) * 2;   //14
            public const int PLAYER_LOGIN = sizeof(int) * 3; //12
            //public const int START_GAME = sizeof(int); //4
            public const int PLAYER_JUMP = sizeof(int) * 3 + sizeof(bool); //13
            public const int PLAYER_TRANSFORM = (sizeof(int) * 3) + (sizeof(float) * 6) + sizeof(bool); //37 //udp. 패킷 사이즈를 함께 보냄
            public const int CHARACTER_DAMAGE_INFO = sizeof(int) * 5 + sizeof(float);       //24
            public const int CHARACTER_DODGE_INFO = sizeof(int) * 4 + sizeof(bool);
            public const int CHARACTER_DIE_INFO = sizeof(int) * 3 + sizeof(bool);
            public const int PLAYER_GAME_RETRY = sizeof(int) * 3 + sizeof(bool);
            public const int CHARACTER_DOWN_INFO = sizeof(int) * 3;
            //오브젝트
            public const int OBJECT_ACTIVE = sizeof(int) * 6; //24
            public const int OBJECT_DEACTIVE = sizeof(int) * 6; //24
            public const int OBJECT_STATE = sizeof(int) * 6; //24
            public const int TRIGGER_ACTIVE = sizeof(int) * 4;
            //크리쳐
            public const int CREATURE_DAMAGE_INFO = sizeof(int) * 6;        //24
            public const int CREATURE_STATE_INFO = sizeof(int) * 7 + sizeof(float) * 3;         //40
            public const int CREATURE_TRANSFORM_INFO = sizeof(int) * 4 + sizeof(float) * 7 + sizeof(int); //44
            //캐릭터 - 스파키
            public const int CHARACTER_SPARKY_SMASH_ATTACK_INFO = sizeof(int) * 4 + sizeof(bool) * 1;   //17
            public const int CHARACTER_SPARKY_AIM_POINT_INFO = sizeof(int) * 3 + sizeof(float) * 3;     //24
            public const int CHARACTER_SPARKY_EXPLOSION_BULLET_INFO = sizeof(int) * 3 + sizeof(bool) + sizeof(float) * 6; //37
            public const int CHARACTER_SPARKY_RAPID_FIRE_INFO = sizeof(int) * 3 + sizeof(bool) * 2;
            public const int CHARACTER_SPARKY_C4_BOMB_THROW_INFO = sizeof(int) * 3 + sizeof(float) * 3;
            public const int CHARACTER_SPARKY_C4_BOMB_TRANSFORM_INFO = sizeof(int) * 3 + sizeof(float) * 6;
            public const int CHARACTER_SPARKY_C4_DETONATION_ORDER = sizeof(int) * 3 + sizeof(float) * 3;
            //캐릭터 - 샘
            public const int CHARACTER_SAM_NORAML_ATTACK_INFO = sizeof(int) * 3 + sizeof(bool) + sizeof(int);
            public const int CHARACTER_SAM_STEAM_BLOW_INFO = sizeof(int) * 3 + sizeof(bool) * 2;
            public const int CHARACTER_SAM_BATTLE_IDLE_INFO = sizeof(int) * 3 + sizeof(bool);
            public const int CAHRACTER_SAM_PULVERIZE_INFO = sizeof(int) * 3;
        }
    }
}