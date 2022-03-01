using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeltonScript : EnemyBaseScript
{
    // =================== 変数 ==================== //

    private readonly int[] m_itemRate = new int[3]
    {
        60, // 薬草 
        30, // 凄い薬草
        10  // 煙玉
    };  // 取得アイテムの入手率
    private enum ItemName { Herb, GreateHerb, Smoke }
    protected enum ActionPattern
    {
        Attack,
        Protect,
        Miss,
        None
    }


    // ================= function ================== //



    /// <summary>
    /// 2022/02/27
    /// int値からActionPatternの値に変換する関数
    /// </summary>
    /// <param name="num">ActionPattern内の値</param>
    /// <returns>引数の値がコマンド値でなければNoneを返す</returns>
    private ActionPattern ConvertIntToEnumActionPattern(int num)
    {
        // intからstringに変換
        foreach (int i in System.Enum.GetValues(typeof(ActionPattern)))
        {
            // 代入されたコマンド番号とEnumのコマンド番号が一致
            // コマンド名の代入とマネージャーの変数の値を変更
            if (num == i)
            { return (ActionPattern)System.Enum.ToObject(typeof(ActionPattern), i); }
        }

        return ActionPattern.None;
    }

    /// <summary>
    /// 2022/02/27
    /// </summary>
    protected override IEnumerator ActionStart()
    {
        m_activeAction = true;

        // プレイヤーのタイミング処理が終わって1秒してから実行
        yield return new WaitForSeconds(1f);

        // 命中率生成
        int hitRate = m_mySystem.GetRandomValue(0, (int)ActionPattern.None);
        string commandName = ConvertIntToEnumActionPattern(hitRate).ToString();
        ActionPattern command = ConvertIntToEnumActionPattern(hitRate);



        // アニメーション開始
        // アニメーション終了まで待機
        // ダメージ計算
        myAnim.SetBool(commandName, true);
        yield return new WaitWhile(() => m_battleMana.player.activeAnim == commandName);

        Debug.Log(commandName);

        // コマンドの実行
        switch (command)
        {
            case ActionPattern.Attack: m_battleMana.Attack((int)CharType.Enemy, attack); break;
            case ActionPattern.Protect: m_battleMana.Protect((int)CharType.Enemy, protect); break;
            case ActionPattern.Miss: m_battleMana.Miss(); break;
        }


        // コマンド後カウンターが可能な場合カウンター
        if (m_battleMana.counter == CharType.Enemy)
        {
            myAnim.SetBool(ActionPattern.Attack.ToString(), true);
            yield return new WaitWhile(() => activeAnim == BattleCommand.Attack.ToString());

            m_battleMana.Counter();
        }

        m_activeAction = false;
    }

    // 現状エネミーからの攻撃などはないから、
    // EnemyBaseのStartとUpdateを呼ぶだけ
    // ============= Start ・ Update =============== //
    protected override void Start()
    {
        m_id = 0;
        m_itemGetRate = 50;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // コマンド実行中且つ
        // タイミングが終了
        if(m_battleMana.battleMode == BattleMode.Execution &&
           m_battleMana.timing.timingEnd && !m_activeAction)
        { StartCoroutine(ActionStart());  }

        // ゲームモードがフリーなら探索
        // そうでなければNavMeshの動きを停止。
        if (m_mySystem.gameMode == GameMode.Free)
        { SearchPlayersInSight(m_searchRange, m_battleRange); }
        else
        { m_currentSpeed = 0.0f; }

        // NavMeshで移動していればアニメーション
        m_myAnim.SetFloat("Walk", m_currentSpeed / m_run);
    }
}
