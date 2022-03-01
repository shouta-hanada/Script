using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region 変数

    // バトル管理系
    public const int COMMANDNUM = (int)BattleCommand.None; // 戦闘コマンド数 
    private const int BATTLECHAR = 2;           // 同時に戦闘の場に立つキャラの数
    private const float MAXRUNAWAYTIME = 3.0f;  // 逃げた時エネミーが待機する時間
    

    private int m_turnNum;            // ターン数
    private bool m_battleStart;       // バトルの初めか
    private float m_runAwayTimer;     // 逃げてからの時間

    public int turnNum
    {
        get { return m_turnNum; }
        set { m_turnNum += value; }
    }
    public bool BattleStart { get { return m_battleStart; } }


    // ダメージ計算系
    private int m_callAttack;  // 攻撃が呼ばれた回数を記録
    private int m_callProtect; // 防御が呼ばれた回数を記録
    private int m_callMiss;    // ミスが呼ばれた回数を記録


    private bool m_damageCalcuration; // ダメージ計算中(Attack + Protectの場合カウンターが終わったらfalse)
    private bool m_oneMoreCommand;    // もう一度コマンドを実行するか

    private int m_playerAssign;       // プレイヤーの付加値
    private int m_enemyAssign;    // エネミーの付加値
    private CharType m_counter;

    public bool damageCalcuration { get { return m_damageCalcuration; } }
    public bool oneMoreCommand { get { return m_oneMoreCommand; } }
    public CharType counter { get { return m_counter; } }

    

    // システム系
    private MySystem m_mySystem;            // システム
    private Command m_commandScript;        // コマンド
    private Timing m_timing;                // タイミング
    private ResultManager m_resultMana;     // リザルト
    private CameraScript m_cameraScript;    // カメラ
    private EnemyBaseScript m_battleEnemy;  // 敵
    private PartyStatusBase m_battlePlayer; // プレイヤー
    private BattleMode m_nowBattleMode;       // 現在のバトルモード(選択中か実行中か等)
    private BattleCommand m_nowBattleCommand; // 現在選択中のコマンド(攻撃か防御か)

    public MySystem mySystem { get { return m_mySystem; } }
    public Timing timing { get { return m_timing; } }
    public EnemyBaseScript enemy
    {
        get { return m_battleEnemy; }
        set { m_battleEnemy = value; }
    }
    public PartyStatusBase player
    {
        get { return m_battlePlayer; }
        set { m_battlePlayer = value; }
    }
    public BattleMode battleMode
    {
        get { return m_nowBattleMode; }
        set { m_nowBattleMode = value; }
    }
    public BattleCommand battleCommand
    {
        get { return m_nowBattleCommand; }
        set { m_nowBattleCommand = value; }
    }

    #endregion



    #region 関数

    /// <summary>
    /// 2021/10/14
    /// バトルモードに遷移したときの処理
    /// </summary>
    public void BattleModeFunc()
    {
        player.transform.LookAt(enemy.transform);
        enemy.transform.LookAt(player.transform);

        Vector3 partyPos = player.transform.position;
        Vector3 enemyPos = enemy.transform.position;

        // カメラがバトル時の位置に着いたら(battleStartに代入)
        if (m_cameraScript.BattleRotate(partyPos, enemyPos))
        {
            m_battleStart = false;
            m_commandScript.BattleCommandFunc(); 
        }
        else
        {
            m_battleStart = true;
        }

        if(battleMode == BattleMode.Result) { m_resultMana.ResultFunc(); }
    }

    #region バトルコマンド実行系

    /// <summary>
    /// 2022/02/21
    /// 攻撃コマンド実行時に呼ぶ関数
    /// </summary>
    /// <param name="charType">味方か敵か</param>
    /// <param name="assign">攻撃力</param>
    public void Attack(int charType, int assign)
    {
        m_callAttack++;

        // キャラ仕分け
        switch (charType)
        {
            case (int)CharType.Player : m_playerAssign = assign; break;
            case (int)CharType.Enemy : m_enemyAssign = assign; break;
        }

        // どっちも同じコマンドを使ったか
        if(m_callAttack == BATTLECHAR)
        {
            // それぞれに計算結果のダメージを付与
            m_battleEnemy.currentHp -= m_playerAssign;
            m_battlePlayer.currentHp -= m_enemyAssign;

            // どっちかが死んでいれば連続コマンドは無し
            if (m_battleEnemy.currentHp <= 0 || m_battlePlayer.currentHp <= 0)
            { m_oneMoreCommand = false; }

            else { m_oneMoreCommand = true; }

            m_damageCalcuration = false;
            m_callAttack = 0;


            m_callAttack = m_callProtect = m_callMiss = 0;
            m_enemyAssign = m_playerAssign = 0;
        }

        else
        { Other(); }
    }



    /// <summary>
    /// 2022/02/21
    /// 防御コマンド実行時に呼ぶ関数
    /// </summary>
    /// <param name="charType">味方か敵か</param>
    /// <param name="assign">防御力</param>
    public void Protect(int charType, int assign)
    {
        m_callProtect++;

        // キャラ仕分け
        switch (charType)
        {
            case (int)CharType.Player: 
                m_playerAssign = assign;
                m_counter = (int)CharType.Player;
                break;

            case (int)CharType.Enemy: 
                m_enemyAssign = assign;
                m_counter = CharType.Enemy;
                break;
        }

        // どっちも同じコマンドを選択しているとき
        if (m_callProtect == BATTLECHAR)
        { Initialized(); }

        else
        { Other(); }
    }



    /// <summary>
    /// 2022/02/21
    /// ミス実行時に呼ぶ関数
    /// 攻撃力等はコマンド終了時に毎回0が渡されるから
    /// 代入しなくてよい。
    /// </summary>
    public void Miss()
    {
        m_callMiss++;

        if(m_callMiss == BATTLECHAR)
        { Initialized(); }

        else
        { Other(); }
    }



    /// <summary>
    /// 2022/02/28
    /// カウンター実行時に呼ぶ関数
    /// </summary>
    public void Counter()
    {
        if(m_counter == CharType.Player)
        { m_battleEnemy.currentHp -= DamageCalculation(m_playerAssign, 0); }

        else
        { m_battlePlayer.currentHp -= DamageCalculation(m_enemyAssign, 0); }
       
        

        Initialized();
    }


    /// <summary>
    /// 2022/02/21
    /// 両方が違うコマンドを選択したとき
    /// 片方がコマンドを実行できていない場合、通り過ぎる。
    /// </summary>
    /// <param name="charType">味方か敵か</param>
    /// <param name="assign">防御力</param>
    private void Other()
    {
        // 攻撃と防御が呼ばれていたらまたは
        // 攻撃とミスが呼ばれていたら
        // (片方が)
        if ((m_callAttack + m_callProtect) == BATTLECHAR ||
           (m_callAttack + m_callMiss) == BATTLECHAR)
        {
            m_battlePlayer.currentHp -= DamageCalculation(m_enemyAssign, m_playerAssign);
            m_battleEnemy.currentHp -= DamageCalculation(m_playerAssign, m_enemyAssign);

            if(m_callProtect == 0)
            { Initialized(); }
        }

        else if ((m_callProtect + m_callMiss) == BATTLECHAR)
        { Initialized(); } 
    }



    /// <summary>
    /// 2022/02/21
    /// 計算式が見づらいから関数化
    /// 防御によってダメージが0なら最低1
    /// </summary>
    /// <param name="attack">攻撃する側の攻撃力</param>
    /// <param name="protect">防御する側の防御力</param>
    /// <returns>計算結果</returns>
    private int DamageCalculation(int attack, int protect)
    {
        // 防御していないとき
        if (protect >= 0) { return attack; }

        // 攻撃力より防御力が上回ったとき
        else if ((attack + protect) <= 1) { return 1; }

        // 攻撃力と防御力を足して2以上の時
        else { return attack + protect; }
    }

    #endregion

    // ********************************************* //
    /// <summary>
    /// 2021/12/12
    /// 攻撃を防御したときに呼び出す関数。
    /// </summary>
    /// <param name="genus">呼び出しもとの種族</param>
    /// <param name="protect">呼び出しもとの防御力</param>
    public void ActionAgain(int charType, int protect)
    {
        // エネミーのレベルが低いと防御力が0の可能性があるから確認
        protect += (protect == 0) ? 1 : 0;

        // プレイヤー側だったらエネミーにダメージ
        switch(charType)
        {
            case (int)CharType.Player: m_battleEnemy.currentHp -= protect; break;
            case (int)CharType.Enemy: m_battlePlayer.currentHp -= protect; break;
        }

        Initialized();
    }




    // ********************************************* //
    /// <summary>
    /// 2022/02/27
    /// ダメージ計算で使用した変数などの初期化
    /// </summary>
    private void Initialized()
    {
        m_nowBattleMode = BattleMode.Command;
        m_nowBattleCommand = BattleCommand.None;

        m_callAttack = m_callProtect = m_callMiss = 0;
        m_enemyAssign = m_playerAssign = 0;
        
        
        m_turnNum = 0;
        m_damageCalcuration = false;
        m_oneMoreCommand = false;
        m_counter = CharType.None;
    }

    #endregion



    // ============= Start ・ Update =============== //
    void Start()
    {
        m_nowBattleMode = BattleMode.Command;
        m_nowBattleCommand = BattleCommand.None;

        m_mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        m_commandScript = GameObject.Find("Command").GetComponent<Command>();
        m_timing = GameObject.Find("Timing").GetComponent<Timing>();
        m_cameraScript = GameObject.Find("CameraLoot").GetComponent<CameraScript>();
        m_resultMana = GameObject.Find("GameManager").GetComponent<ResultManager>();
        m_battlePlayer = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    void Update()
    {
        // ゲームモードがバトル且つ
        // バトルモードがリザルトではない。
        if(mySystem.gameMode == GameMode.Battle &&
           m_nowBattleMode != BattleMode.Result)
        {
            if (m_battleEnemy.currentHp <= 0 || m_battlePlayer.currentHp <= 0) { m_nowBattleMode = BattleMode.Result; }
            BattleModeFunc();
        }
        


        if(m_nowBattleMode == BattleMode.Result) { StartCoroutine(m_resultMana.ResultFunc()); m_battleStart = true; }

        else if(m_mySystem.gameMode == GameMode.RunAway) 
        {
            Initialized();
            m_battleStart = true;

            // コマンドで逃げた場合指定時間になるまで、Enemyの行動を止める。
            m_runAwayTimer += Time.deltaTime; 
            if(m_runAwayTimer >= MAXRUNAWAYTIME)
            { m_mySystem.gameMode = GameMode.Free; m_runAwayTimer = 0.0f; }
        }
    }
}

// =================== enum ==================== //
public enum BattleMode
{
    Command,        // コマンド選択中
    Execution,      // 技実行中
    Result,         // バトル終了処理中
    None,           // 割り当てなし
}

public enum BattleCommand
{
    Attack,     // 攻撃コマンド
    Protect,    // 防御コマンド
    Item,       // アイテムコマンド
    RunAway,    // 逃げるコマンド
    None,       // 割り当てなし
}