using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 2021/10/11
/// 複数のスクリプトで使う関数等の置き場
/// </summary>
public class MySystem : MonoBehaviour
{
    #region 変数
    // ゲーム共通
    private const int FRAMERATE = 60;       // フレームレート
    public const int PARTYTYPE = 1; // 操作キャラの種類
    public const int ENEMYTYPE = 1; // エネミーの種類
    public const int ITEMTYPE = 3;  // アイテムの種類
    public const int PERCENT = 100; // 100
    public const int MAXLEVEL = 100;       // 最大レベル



    // CVS(ステータス情報)
    private PartyStatusInfo m_playerStatusInfo; // プレイヤーのステータスの素
    private EnemyStatusInfo m_enemyStatusInfo;  // エネミーのステータスの素
    private DropItemInfo m_dropItemInfo;        // エネミーからドロップするアイテムの素
    
    public PartyStatusInfo pStatusInfo　{ get { return m_playerStatusInfo; } }
    public EnemyStatusInfo eStatusInfo　{ get { return m_enemyStatusInfo; } }
    public DropItemInfo dropItemInfo 　{ get { return m_dropItemInfo; } }
    


    // ボタンの入力関連 
    private const float m_canButtonInterval = 0.75f; // ボタンが入力不能な時間
    private bool m_canButton;                       // ボタンが押せるか
    private float m_inputButtonTimer;               // ボタンが押されてからの時間計測

    public bool canButton { get { return m_canButton; } }



    // システム関連
    private BattleManager m_battleMana;
    private SpritePlefabManager m_spritePlefabMana;
    private System.Random m_rand = new System.Random(); // 乱数を作成する機械
    private GameMode m_nowGameMode;            // 現在のゲームモード

    public BattleManager battleMana { get { return m_battleMana; } }
    public SpritePlefabManager spritePlefabMana { get { return m_spritePlefabMana; } }
    public GameMode gameMode
    {
        get { return m_nowGameMode; }
        set { m_nowGameMode = value; }
    }

    #endregion



    #region 便利系

    /// <summary>
    /// 2022/01/24
    /// x,z座標を持つ2つの距離を求める関数。
    /// xyz座標の距離を求めたい場合は、
    /// Vector3.Distanceを使って。
    /// </summary>
    /// <param name="vec1">1つ目の座標</param>
    /// <param name="vec2">2つ目の座標</param>
    /// <returns>距離の絶対値</returns>
    public float MathDistance(Vector3 vec1, Vector3 vec2, bool abs = true)
    {
        float posX = Mathf.Pow(vec1.x - vec2.x, 2);
        float posZ = Mathf.Pow(vec1.z - vec2.z, 2);

        if (abs)
        { return Mathf.Abs(Mathf.Sqrt(posX + posZ)); }
        else
        { return Mathf.Sqrt(posX + posZ); }
    }



    /// <summary>
    /// 2021/11/18
    /// ランダムな値を取得する関数。
    /// デフォルトでは0～100の値を返す。
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public int GetRandomValue(int min = 0, int max = 101)
    { return m_rand.Next(min, max); }



    /// <summary>
    /// 2022/02/17
    /// クイックソート
    /// ※2022/02/18追記
    /// アイテム取得確率に使用目的だったが作成後、使用できない(しても意味がない)ことに
    /// 気づき使用していないが、成果として残す。
    /// </summary>
    /// <param name="array">ソートしたい配列</param>
    /// <param name="left">配列の最初の要素</param>
    /// <param name="right">配列の最後の要素</param>
    public void ArraySort(ref int[] array, int left, int right)
    {
        // 最初の用ぞ番号と最後の用ぞ番号が同じならソートはしなくていい。
        if (left >= right)
        { return; }

        // クイックソートの基準値
        int pivot = array[left];

        int i = left;
        int j = right;

        // ソートする場所が交わるまで繰り返す。
        while (true)
        {
            while (array[i] < pivot)
            { i++; }

            while (array[j] > pivot)
            { j--; }

            // 交わっている
            if (i >= j)
            { break; }

            // 交わっていないがwhileを抜けた
            else
            {
                ValueSwap(ref array[i], ref array[j]);
                i++;
                j--;
            }
        }

        // 再帰
        ArraySort(ref array, left, i - 1);
        ArraySort(ref array, j + 1, right);
    }



    /// <summary>
    /// 2022/02/17
    /// 値を入れ替える関数(ソート用)
    /// </summary>
    /// <param name="a">値</param>
    /// <param name="b">値</param>
    private void ValueSwap(ref int a, ref int b)
    {
        int hold = a;
        a = b;
        b = hold;
    }



    /// <summary>
    /// 2021/11/15
    /// int型のn桁番目の値を取得する関数
    /// 
    /// 計算式の例
    /// num = 145, digit = 2(つまり4を取得したい)
    /// digPow = 10^2 = 100
    /// (145 - (145/100) * 100) = 145 - (1 * 100) = 45 
    /// 45/(10^1) = 4
    /// 
    /// 1桁目を取得する時は、上記の計算式だと0しか返さないため、
    /// (->((int)Mathf.Pow(10, (1 -1)))でn/0になる。)
    /// 1桁目の計算式は、
    /// 145 - (145/10) * 10 = 145 - 140 = 5
    /// になる。
    /// </summary>
    /// <param name="num">調べたい値</param>
    /// <param name="digit">調べたい桁</param>
    /// <returns></returns>
    public int NthDigitVal(int num, int digit)
    {
        // 1桁未満の場合0を返す
        if (digit <= 0) { return 0; }

        int val = 0;    // 値の結果の入れ物

        // 10の調べたい桁べき乗
        int digPow = (int)Mathf.Pow(10, digit);

        // 1桁目を調べたいとき
        if (digit == 1) { val = num - (num / digPow) * digPow; }

        else
        { val = (num - (num / digPow) * digPow) / (int)Mathf.Pow(10, (digit - 1)); }

        return val;
    }

    #endregion


    /// <summary>
    /// 2021/10/29
    /// α値を変更する関数。
    /// colorへの代入は行われない。(for文で呼び出すとα値が狂うため。)
    /// </summary>
    /// <param name="alphaVal">α値</param>
    /// <param name="speed">フェード速度</param>
    /// <param name="fade">alphaを下げたいならtrueを渡す</param>
    public void FadeInOut(ref float alphaVal, float speed, bool fade)
    {
        if (!fade) { alphaVal += speed * Time.deltaTime; }
        else if (fade) { alphaVal -= speed * Time.deltaTime; }
    }

    

    /// <summary>
    /// 2021/11/11
    /// ボタンが入力可能になるまでの時間を計測する関数
    /// </summary>
    public IEnumerator ButtonTimer()
    {
        m_canButton = false;

        while(m_inputButtonTimer < m_canButtonInterval)
        { 
            m_inputButtonTimer += Time.deltaTime;
            yield return null;
        }

        m_inputButtonTimer = 0.0f;
        m_canButton = true;
    }


    // ============= Start ・ Update =============== //
    private void Awake()
    {
        Application.targetFrameRate = FRAMERATE;

        m_playerStatusInfo = new PartyStatusInfo();
        m_enemyStatusInfo = new EnemyStatusInfo();
        m_dropItemInfo = new DropItemInfo();
        m_playerStatusInfo.Init();
        m_enemyStatusInfo.Init();
        m_dropItemInfo.Init();
    }

    private void Start()
    {
        

        m_nowGameMode = GameMode.Free;
        m_battleMana = GetComponent<BattleManager>();
        m_spritePlefabMana = GetComponent<SpritePlefabManager>();
        m_canButton = true;
        m_inputButtonTimer = 0.0f;

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        { Application.Quit(); }

        if(m_nowGameMode == GameMode.Battle)
        { Cursor.visible = true; }
        else { Cursor.visible = false; }
    }
}

// =================== enum ==================== //
public enum GameMode
{
    Free,
    Battle,
    RunAway,
    Scene,
    GameOver,
    Pause,
}

public enum ItemName
{
    Herb,
    GreatHerb,
    Smoke,
    ItemMax
}

public enum CharType
{
    Player,
    Enemy,
    None
}