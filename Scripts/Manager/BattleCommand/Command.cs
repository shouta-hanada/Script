using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Command : MonoBehaviour
{
    #region 変数

    // コマンドUI系
    private const float COMMANDAPPEARSPEED = 12000.0f;  // 0.1秒で出てくる(1200->1秒)
    [SerializeField] private GameObject m_commandParent;  // commandActionの親オブジェクト
    private Vector3 m_commandUnAppearPos = new Vector3(1200.0f, 0.0f, 0.0f);    // コマンドが表示される位置
    private Vector3 m_commandAppearPos = new Vector3(750.0f, 0.0f, 0.0f);       // コマンドが表示されない位置

    // コマンド実行系
    private const int RUNAWAYRATE = 50;     // 逃げられる確率(百分率)
    private const float WAITTIME = 1.0f;    // コマンド処理間の待機時間
    private int m_activeCommandNum;         // 実行するコマンドの番号

    // システム系
    private BattleManager m_battleMana;   // バトル管理
    private ItemBoxManager m_itemBoxMana; // アイテムボックス 

    #endregion



    #region 関数

    /// <summary>
    /// 2022/02/27
    /// コマンドを制御する関数。
    /// バトル開始時にバトルマネージャーに呼び出される。
    /// </summary>
    public void BattleCommandFunc()
    {
        switch(m_battleMana.battleMode)
        {
            // コマンド選択状態なら
            // コマンドUIを表示位置に移動させる
            // 実行コマンド番号の選択
            // 実行コマンド番号が有効な番号ならコマンドを実行
            case BattleMode.Command: 
                StartCoroutine(SetCommandAppearPosition(m_commandParent, m_commandAppearPos));
                SetActiveCommandNum();
                
                // 何かしらのコマンドが選択されたら実行
                if(m_activeCommandNum != (int)BattleCommand.None)
                { StartCoroutine(PlayCommand()); }

                break;

            // コマンド選択状態じゃなければ
            // コマンドUIを非表示位置に移動させる
            default: StartCoroutine(SetCommandAppearPosition(m_commandParent, m_commandUnAppearPos));
                break;
        }
    }



    /// <summary>
    /// 2022/02/27
    /// コマンドをクリックしたときそれに合う
    /// アクションを実行する関数。
    /// </summary>
    /// <param name="num">enumBattlCommandで割り振られたコマンド番号。</param>
    public void OnClickCommand(int num)
    {
        m_activeCommandNum = num;
        StartCoroutine(PlayCommand());
    }



    /// <summary>
    /// 2022/02/27
    /// キーボード入力でコマンドを切り替える関数。
    /// </summary>
    private void SetActiveCommandNum()
    {
        if (Input.GetKeyDown(KeyCode.W)) { m_activeCommandNum = (int)BattleCommand.Attack; }
        else if (Input.GetKeyDown(KeyCode.A)) { m_activeCommandNum = (int)BattleCommand.Item; }
        else if (Input.GetKeyDown(KeyCode.S)) { m_activeCommandNum = (int)BattleCommand.Protect; }
        else if (Input.GetKeyDown(KeyCode.D)) { m_activeCommandNum = (int)BattleCommand.RunAway; }
    }

    

    /// <summary>
    /// 2022/02/27
    /// int値からBattleCommandの値に変換する関数
    /// </summary>
    /// <param name="num">BattleCommand内の値</param>
    /// <returns>引数の値がコマンド値でなければNoneを返す</returns>
    private BattleCommand ConvertIntToEnumBattleCommand(int num)
    {
        // intからstringに変換
        foreach (int i in System.Enum.GetValues(typeof(BattleCommand)))
        {
            // 代入されたコマンド番号とEnumのコマンド番号が一致
            // コマンド名の代入とマネージャーの変数の値を変更
            if (num == i)
            { return (BattleCommand)System.Enum.ToObject(typeof(BattleCommand), i); }
        }

        return BattleCommand.None;
    }



    /// <summary>
    /// 2022/02/27
    /// プレイヤーが選択したコマンドを実行する関数
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayCommand()
    {
        #region タイミング開始から終了までの処理

        m_battleMana.battleMode = BattleMode.Execution;
        // コマンドが選択された時点でタイミング処理開始
        StartCoroutine(m_battleMana.timing.TimingRun());

        // タイミング処理が終わるまでコマンド選択ができる
        while (!m_battleMana.timing.timingEnd)
        {
            SetActiveCommandNum();
            yield return null;
        }

        // 最終的に選択されたコマンド名の取得
        string commandName = ConvertIntToEnumBattleCommand(m_activeCommandNum).ToString();
        m_battleMana.battleCommand = ConvertIntToEnumBattleCommand(m_activeCommandNum);

        // タイミング処理が終わった後少し待機
        yield return new WaitForSeconds(WAITTIME);

        #endregion



        m_battleMana.battleMode = BattleMode.Execution;



        #region コマンド実行処理

        // 乱数生成
        int hitRand = m_battleMana.mySystem.GetRandomValue();

        // 命中率が乱数以上ならコマンド実行
        if (m_battleMana.timing.hitRate >= hitRand && Exception())
        {
            // コマンドに合うアニメーションを再生
            // アニメーションのクリップ名がコマンドアニメーション以外の物になるまで待機
            m_battleMana.player.myAnim.SetBool(commandName, true);
            yield return new WaitWhile(() => m_battleMana.player.activeAnim == commandName);



            // コマンドの実行
            switch (m_battleMana.battleCommand)
            {
                case BattleCommand.Attack: m_battleMana.Attack((int)CharType.Player, m_battleMana.player.attack); break;
                case BattleCommand.Protect: m_battleMana.Attack((int)CharType.Player, m_battleMana.player.protect); break;
                case BattleCommand.Item: m_itemBoxMana.StartCoroutine(m_itemBoxMana.BattleModeItem()); break;
                case BattleCommand.RunAway: m_battleMana.mySystem.gameMode = GameMode.RunAway; break;
            }
        }

        else
        {
            // コマンドに合うアニメーションを再生
            // アニメーションのクリップ名がコマンドアニメーション以外の物になるまで待機
            m_battleMana.player.myAnim.SetBool("Miss", true);
            yield return new WaitWhile(() => m_battleMana.player.activeAnim == "Miss");

            m_battleMana.Miss();
        }

        #endregion



        // コマンド後カウンターが可能な場合カウンター
        if(m_battleMana.counter == CharType.Player)
        {
            m_battleMana.player.myAnim.SetBool(BattleCommand.Attack.ToString(), true);
            yield return new WaitWhile(() => m_battleMana.player.activeAnim == BattleCommand.Attack.ToString());

            m_battleMana.Counter(); 
        }



        #region 終了又は再帰処理

        // コマンド実行後待機
        yield return new WaitForSeconds(WAITTIME);

        // 計算中は待機(時間はかからない)
        while (m_battleMana.damageCalcuration)
        {
            // 連続コマンド選択の場合再帰
            if (m_battleMana.oneMoreCommand)
            { StartCoroutine(PlayCommand()); }

            yield return null;
        }

        // 選択コマンドを無しにして、不要かもしれないけどbreak文
        m_activeCommandNum = (int)BattleCommand.None;
        yield break;

        #endregion

    }



    /// <summary>
    /// 2022/02/27
    /// アイテム、逃げるは条件が少し特殊なため、こちらで処理
    /// </summary>
    /// <returns>実行可能ならtrue</returns>
    private bool Exception()
    {
        switch(m_battleMana.battleCommand)
        {
            // タイミングの判定がgood以上ならtrueを返す
            case BattleCommand.Item: 
                return m_battleMana.timing.hitRate >= Timing.GOODHITRATE;

            // プレイヤーの素早さが敵の素早さ以上又は
            // 確率で逃げられるか
            case BattleCommand.RunAway:
                return m_battleMana.player.agility > m_battleMana.enemy.agility ||
                       RUNAWAYRATE >= m_battleMana.mySystem.GetRandomValue();

            // 他のコマンドの場合true
            default: return true;
        }
    }



    /// <summary>
    /// 2021/11/25
    /// コマンドの位置を変更する関数。
    /// </summary>
    /// <param name="command">動かすコマンド</param>
    /// <param name="vec">変更後の位置</param>
    private IEnumerator SetCommandAppearPosition(GameObject command, Vector3 vec)
    {
        float dis = float.MaxValue;
        float speed = COMMANDAPPEARSPEED * Time.deltaTime;
        Vector3 sPos = command.transform.localPosition;

        
        
        while(dis > 0.1f)
        {
            sPos = Vector3.MoveTowards(sPos, vec, speed);
            dis = Vector3.Distance(sPos, vec);
            command.transform.localPosition = sPos;
            yield return null;
        }
    }


    #endregion

    // ============= Start ・ Update =============== //
    private void Start()
    {
        m_battleMana = GameObject.Find("GameManager").GetComponent<BattleManager>();
        m_itemBoxMana = GameObject.Find("ItemBox").GetComponent<ItemBoxManager>();

        m_activeCommandNum = (int)BattleCommand.None;

        // コマンド(UI)の位置を画面外に送る
        m_commandParent.transform.localPosition = m_commandUnAppearPos;
    }

    private void Update()
    {
    }
}
