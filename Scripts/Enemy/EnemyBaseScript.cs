using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// エネミーの基本情報の継承元
/// MonoBehaviourに全エネミーが持つであろう
/// ステータスが追加されるだけ。
/// </summary>
public abstract class EnemyBaseScript : StatusBase
{
    #region 変数
    protected int m_id;
    protected int m_exp;
    protected float m_searchRange;  // 索敵範囲
    protected float m_battleRange;  // 戦闘範囲
    protected int m_itemGetRate;    // アイテム取得確率
    protected int m_maxNumItem;     // 最大取得アイテム数
    protected int[] m_item;         // 取得したアイテム

    // バトル時の挙動
    protected bool m_activeAction;  // エネミーがアクションを起こしているか(起こしていればtrue)

    protected NavMeshAgent m_agent;       // AI
    protected EnemyStatusInfo info;

    public int exp { get { return m_exp; } }
    public int[] item { get { return m_item; } }
    public NavMeshAgent Agent
    {
        get { return m_agent; }
    }

    #endregion



    #region ステータス系
    /// <summary>
    /// 2022/12/15
    /// エネミーの全ての基礎ステータスを設定する関数
    /// level = プレイヤーのレベル + 〇～〇の値 (最低1)
    /// hp = level * 〇～〇の値 (最低1)
    /// at = level * 〇～〇の値 (最低1)
    /// pr = attack * 〇.〇～〇.〇の値（最低1,四捨五入）
    /// ag = level * 〇.〇～〇.〇の値（最低1,四捨五入）
    /// exp = level ; 〇～〇の値（最低0,四捨五入）
    /// </summary>
    /// <param name="enemyId">基底のエネミー番号</param>
    protected override void SetAllStatus(int id)
    {
        info = m_mySystem.eStatusInfo;

        #region m_level
        m_level = m_mySystem.battleMana.player.level + m_mySystem.GetRandomValue(info.lvMag[id, 0], info.lvMag[id, 1]);
        if (m_level <= 0)
        { m_level = 1; }
        #endregion
        m_currentHp = m_maxHp = m_level * m_mySystem.GetRandomValue(info.hpMag[id, 0], info.hpMag[id, 1]);
        m_attack = m_level * m_mySystem.GetRandomValue(info.atMag[id, 0], info.atMag[id, 1]);
        m_protect = Mathf.RoundToInt((float)m_attack * (float)(m_mySystem.GetRandomValue(info.prMag[id, 0], info.prMag[id, 1]) / 10)); // 割る10
        m_agility = Mathf.RoundToInt((float)m_level * (float)(m_mySystem.GetRandomValue(info.agMag[id, 0], info.agMag[id, 1]) / 10)); // 割る10
        m_exp = m_level * m_mySystem.GetRandomValue(info.expMag[id, 0], info.expMag[id, 1]);

        m_walk = info.walk[id];
        m_run = info.run[id];
        m_rot = info.rot[id];
        m_turn = info.turn[id];
        m_searchRange = info.searchRange[id];
        m_battleRange = info.battleRange[id];

        #region 基本必須ステータスが0になるらないようにするけどチェック
        if (m_maxHp <= 0)
        { m_maxHp = 1; }

        if (m_attack <= 0)
        { m_attack = 1; }

        if (m_protect <= 0)
        { m_protect = 1; }

        if (m_agility <= 0)
        { m_agility = 1; }

        if (m_exp < 0)
        { m_exp = 0; }
        #endregion


    }



    /// <summary>
    /// 2022/02/09
    /// </summary>
    /// <returns>取得アイテム数</returns>
    private int GetItemNum()
    {
        // アイテム取得率より大きければ取得アイテム0
        if (m_itemGetRate < m_mySystem.GetRandomValue())
        { return 0; }


        // 取得アイテム数
        int getItemNum = 0;


        // 最大取得アイテム数分
        // for中ひたすら抽選(アイテム取得可能でも0の可能性はある)
        for (int i = 0; i < m_maxNumItem; i++)
        {
            if (m_itemGetRate > m_mySystem.GetRandomValue())
            {
                // 取得可能アイテム数++
                getItemNum++;
                continue;
            }

            break;
        }

        // 取得アイテム数
        return getItemNum;
    }



    /// <summary>
    /// 2022/02/18
    /// アイテムを取得する関数
    /// </summary>
    private void GetItem()
    {
        // アイテム取得数
        m_item = new int[GetItemNum()];

        // 取得予定アイテムで一番低い確率のアイテム番号
        int rateComparison = 0;
        int randVal = 0;



        for (int i = 0; i < m_item.Length; i++)
        {
            randVal = m_mySystem.GetRandomValue(1, MySystem.PERCENT + 1);

            // 全アイテム分
            for (int j = 0; j < MySystem.ITEMTYPE; j++)
            {
                // j番目のアイテムの確率が乱数以上 && j番目のアイテムが他のアイテムの確率より小さければ
                if (m_mySystem.dropItemInfo.rate[j, m_id] >= randVal &&
                    m_mySystem.dropItemInfo.rate[rateComparison, m_id] >= m_mySystem.dropItemInfo.rate[j, m_id])
                {
                    // 現最低確率とj番目のアイテムの確率が同じなら乱数割る2で"割り切れなかったら"コンティニュー
                    if (m_mySystem.dropItemInfo.rate[rateComparison, m_id] == m_mySystem.dropItemInfo.rate[j, m_id] &&
                       randVal / 2 != 0)
                    { continue; }

                    rateComparison = j;
                }
            }

            // 最終的に一番低かったアイテム(番号)を取得
            m_item[i] = rateComparison;
            rateComparison = 0;
            randVal = 0;
        }
    }

    #endregion


    /// <summary>
    /// 2022/02/09
    /// プレイヤーを追尾してモードを変える関数
    /// </summary>
    protected void SearchPlayersInSight(float searchRange, float battleRange)
    {
        Vector3 pPos = m_battleMana.player.transform.position;
        Vector3 ePos = transform.position;
        float dis = Vector3.Distance(pPos, ePos);

        // 追跡範囲なら
        if(dis < searchRange)
        {
            m_agent.destination = pPos;
            m_agent.speed = m_walk;
        }
        else
        { m_agent.speed = 0.0f; }

        // バトル範囲なら
        if(dis < battleRange)
        {
            m_agent.speed = 0.0f;
            m_mySystem.gameMode = GameMode.Battle;
            m_battleMana.enemy = this;
        }
    }

    
    // ********************************************* //

    /// <summary>
    ///  2021/12/06
    /// エネミーのアクション(コマンド)処理
    /// </summary>
    protected abstract IEnumerator ActionStart();




    // ============= Start ・ Update =============== //
    protected override void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();

        SetAllStatus(m_id);
        m_activeAction = false;
    }

    protected override void Update()
    {
    }
}
