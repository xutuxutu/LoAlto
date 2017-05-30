using UnityEngine;
using System.Collections;

public class HTTPHandlingManager
{
    public DB_DATA.CHARACTER_INFO PARS_CHARACTER_INFO_FC(NET_HTTP.RECV.CHARACTER_INFO data)
    {
        Debug.Log("pars characterStatus");
        DB_DATA.CHARACTER_INFO characterInfo = new DB_DATA.CHARACTER_INFO();

        characterInfo.CHAR_TYPE = (CHARACTER.TYPE)int.Parse(data.CHAR_TYPE);
        characterInfo.HLTH_PONT = float.Parse(data.HLTH_PONT);
        characterInfo.ENRG_PONT = float.Parse(data.ENRG_PONT);
        characterInfo.DFND_PONT = float.Parse(data.DFND_PONT);
        characterInfo.HEAL_SCND = float.Parse(data.HEAL_SCND);
        characterInfo.UABL_RGNT = float.Parse(data.UABL_RGNT);
        characterInfo.WALK_SPED = float.Parse(data.WALK_SPED);
        characterInfo.RUNN_SPED = float.Parse(data.RUNN_SPED);
        characterInfo.JUMP_SPED = float.Parse(data.JUMP_SPED);
        characterInfo.GRVT_PONT = float.Parse(data.GRVT_PONT);
        characterInfo.DDGE_SPED = float.Parse(data.DDGE_SPED);

        return characterInfo;
    }

    public DB_DATA.CHARACTER_SKILL_DATA PARS_CHARACTER_SKILL_DATA_FC(NET_HTTP.RECV.CHARACTER_SKILL_DATA data)
    {
        Debug.Log("pars characterSkill");
        DB_DATA.CHARACTER_SKILL_DATA skillData = new DB_DATA.CHARACTER_SKILL_DATA();

        skillData.CHAR_TYPE = (CHARACTER.TYPE)int.Parse(data.CHAR_TYPE);
        skillData.SKIL_TYPE = int.Parse(data.SKIL_TYPE);
        skillData.BOMB_ATPT = float.Parse(data.BOMB_ATPT);
        skillData.CNTT_ATPT = float.Parse(data.CNTT_ATPT);
        skillData.ATCK_RNGE = float.Parse(data.ATCK_RNGE);
        skillData.ATCK_WDTH = float.Parse(data.ATCK_WDTH);
        skillData.ATCK_LNTH = float.Parse(data.ATCK_LNTH);
        skillData.USNG_MPNT = float.Parse(data.USNG_MPNT);
        skillData.COOL_TIME = float.Parse(data.COOL_TIME);
        skillData.THRW_POWR = float.Parse(data.THRW_POWR);
        skillData.GRVT_POWR = float.Parse(data.GRVT_POWR);
        skillData.OBJT_SPED = float.Parse(data.OBJT_SPED);

        return skillData;
    }

    public DB_DATA.CHARACTER_NORMAL_ATTACK_DATA PARS_CHARACTER_NORMAL_ATTACK_DATA_FC(NET_HTTP.RECV.CHARACTER_NORMAL_ATTACK_INFO data)
    {
        Debug.Log("pars characterAttack");
        DB_DATA.CHARACTER_NORMAL_ATTACK_DATA attackData = new DB_DATA.CHARACTER_NORMAL_ATTACK_DATA();

        attackData.CHAR_TYPE = (CHARACTER.TYPE)int.Parse(data.CHAR_TYPE);
        attackData.ATCK_TYPE = int.Parse(data.ATCK_TYPE);
        attackData.ATCK_PONT = float.Parse(data.ATCK_PONT);

        return attackData;
    }
    public DB_DATA.CREATURE_COMMON_DATA PARS_CREATURE_COMMON_FC(NET_HTTP.RECV.CREATURE_COMMON_DATA data)
    {
        Debug.Log("pars CREATURE_COMMON_DATA");
        DB_DATA.CREATURE_COMMON_DATA creatureData = new DB_DATA.CREATURE_COMMON_DATA();

        creatureData.CRIT_TYPE = int.Parse(data.CRIT_TYPE);
        creatureData.HLTH_PONT = int.Parse(data.HLTH_PONT);

        creatureData.DEFE_POWR = int.Parse(data.DEFE_POWR);
        creatureData.DEFE_SHLD = float.Parse(data.DEFE_SHLD);

        creatureData.RELZ_RAIS = int.Parse(data.RELZ_RAIS);
        creatureData.RELZ_TIME = float.Parse(data.RELZ_TIME);


        creatureData.PATL_SPED = float.Parse(data.PATL_SPED);
        creatureData.TRCE_SPED = float.Parse(data.TRCE_SPED);
        creatureData.BESG_SPED = float.Parse(data.BESG_SPED);
        creatureData.RETR_SPED = float.Parse(data.RETR_SPED);
        creatureData.THRE_SPED = float.Parse(data.THRE_SPED);
        creatureData.BESG_RAIS = float.Parse(data.BESG_RAIS);
        creatureData.RETR_RAIS = float.Parse(data.RETR_RAIS);
        creatureData.THRE_RAIS = float.Parse(data.THRE_RAIS);
        creatureData.RETR_MOVE = float.Parse(data.RETR_MOVE);
        creatureData.THRE_MOVE = float.Parse(data.THRE_MOVE);

        creatureData.DRON_CONT = int.Parse(data.DRON_CONT);

        return creatureData;
    }

    public DB_DATA.CREATURE_ATTACK_DATA PARS_CREATURE_ATTACK_FC(NET_HTTP.RECV.CREATURE_ATTACK_DATA data)
    {
        Debug.Log("pars CREATURE_ATTACK_DATA");
        DB_DATA.CREATURE_ATTACK_DATA creatureData = new DB_DATA.CREATURE_ATTACK_DATA();

        creatureData.CRIT_TYPE = int.Parse(data.CRIT_TYPE);
        creatureData.ATCK_TYPE = int.Parse(data.ATCK_TYPE);
        creatureData.ATCK_PONT = float.Parse(data.ATCK_PONT);
        creatureData.ATCK_DSTC = float.Parse(data.ATCK_DSTC);
        creatureData.ATCK_RNGE = float.Parse(data.ATCK_RNGE);
        creatureData.ATCK_DLAY = float.Parse(data.ATCK_DLAY);

        return creatureData;
    }

    public DB_DATA.CREATURE_DROP_DATA PARS_CREATURE_DROP_FC(NET_HTTP.RECV.CREATURE_DROP_DATA data)
    {
        Debug.Log("pars CREATURE_DROP_DATA");
        DB_DATA.CREATURE_DROP_DATA creatureData = new DB_DATA.CREATURE_DROP_DATA();

        creatureData.CRIT_TYPE = int.Parse(data.CRIT_TYPE);
        creatureData.ITEM_TYPE = int.Parse(data.ITEM_TYPE);
        creatureData.DROP_MINI = int.Parse(data.DROP_MINI);
        creatureData.DROP_MAXI = int.Parse(data.DROP_MAXI);

        return creatureData;
    }

    public DB_DATA.CREATURE_ITEM_DATA PARS_CREATURE_ITEM_FC(NET_HTTP.RECV.CREATURE_ITEM_DATA data)
    {
        Debug.Log("pars CREATURE_ITEM_DATA");
        DB_DATA.CREATURE_ITEM_DATA creatureData = new DB_DATA.CREATURE_ITEM_DATA();

        creatureData.ITEM_TYPE = int.Parse(data.ITEM_TYPE);
        creatureData.GAIN_VALU = int.Parse(data.GAIN_VALU);
        creatureData.CHSE_ACCL = float.Parse(data.CHSE_ACCL);
        creatureData.CHSE_SPED = float.Parse(data.CHSE_SPED);

        return creatureData;
    }
}

namespace DB_DATA
{
    public class CHARACTER_INFO
    {
        public CHARACTER.TYPE CHAR_TYPE;    //캐릭터 타입
        public float HLTH_PONT;    //체력
        public float ENRG_PONT;    //에너지포인트
        //public float ATCK_PT01;    //기본 공격력_1
        //public float ATCK_PT02;    //기본 공격력_2
        //public float ATCK_PT03;    //기본 공격력_3
        public float DFND_PONT;    //방어력
        public float HEAL_SCND;    //초당 체력 재생률
        public float UABL_RGNT;    //회복 불가 시간
        public float WALK_SPED;    //걷기 속도
        public float RUNN_SPED;    //달리기 속도
        public float JUMP_SPED;    //점프 속도
        public float GRVT_PONT;    //중력 값
        public float DDGE_SPED;    //회피 속도
    }

    public class CHARACTER_NORMAL_ATTACK_DATA
    {
        public CHARACTER.TYPE CHAR_TYPE;
        public int ATCK_TYPE;
        public float ATCK_PONT;
    }
       

    public class CHARACTER_SKILL_DATA
    {
        public CHARACTER.TYPE CHAR_TYPE;
        public int SKIL_TYPE;
        public float BOMB_ATPT;
        public float CNTT_ATPT;
        public float ATCK_RNGE;
        public float ATCK_WDTH;
        public float ATCK_LNTH;
        public float USNG_MPNT;
        public float COOL_TIME;
        public float THRW_POWR;
        public float GRVT_POWR;
        public float OBJT_SPED;
    }
    public class CREATURE_COMMON_DATA
    {
        public int CRIT_TYPE = 0;                       // 크리쳐 종류
        public int HLTH_PONT = 0;                       // 체력

        public int DEFE_POWR = 0;                       // 방어력
        public float DEFE_SHLD = 0.0f;                  // 방패 감소율

        public int RELZ_RAIS = 0;                       // 인식 범위(반지름)
        public float RELZ_TIME = 0.0f;                  // 인식 체크 시간

        public float PATL_SPED = 0.0f;                  // 감시 움직임 속도
        public float TRCE_SPED = 0.0f;                  // 추적 움직임 속도
        public float BESG_SPED = 0.0f;                  // 포위 움직임 속도
        public float RETR_SPED = 0.0f;                  // 후퇴 움직임 속도
        public float THRE_SPED = 0.0f;                  // 위협 움직임 속도
        public float BESG_RAIS = 0.0f;                  // 포위 최대 범위 반지름
        public float RETR_RAIS = 0.0f;                  // 후퇴 최대 범위 반지름
        public float THRE_RAIS = 0.0f;                  // 위협 최대 범위 반지름
        public float RETR_MOVE = 0.0f;                  // 후퇴 움직임 속도
        public float THRE_MOVE = 0.0f;                  // 위협 움직임 속도

        public int DRON_CONT = 0;                       // 드론개수
    }

    public class CREATURE_ATTACK_DATA
    {
        public int CRIT_TYPE = 0;                       // 크리쳐 종류
        public int ATCK_TYPE = 0;                       // 공격 종류
        public float ATCK_PONT = 0.0f;                  // 공격 포인트
        public float ATCK_DSTC = 0.0f;                  // 공격 포인트
        public float ATCK_RNGE = 0.0f;                  // 공격 포인트
        public float ATCK_DLAY = 0.0f;                  // 공격 딜레이
    }

    public class CREATURE_DROP_DATA
    {
        public int CRIT_TYPE = 0;                       // 크리쳐 종류
        public int ITEM_TYPE = 0;                       // 아이템 종류

        public int DROP_MINI = 0;                       // 드랍 아이템 최솟값
        public int DROP_MAXI = 0;                       // 드랍 아이템 최댓값
    }

    public class CREATURE_ITEM_DATA
    {
        public int ITEM_TYPE = 0;                       // 아이템 종류
        public int GAIN_VALU = 0;                       // 얻을이득값

        public float CHSE_ACCL = 0.0f;                  // 움직임 가속도
        public float CHSE_SPED = 0.0f;                  // 움직임 기본속력
    }
}
