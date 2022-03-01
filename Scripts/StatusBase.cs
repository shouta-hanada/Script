using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ステータスの抽象クラス
// プレイヤーもエネミーにも必要となる変数などを保有
// GetとSetを分離しているのはキャラによってステータスの変動時の挙動が違うため。
public abstract class StatusBase : MonoBehaviour
{
    #region 変数

    // 戦闘時ステータス
    protected const int m_maxLevel = 100;   // 最大レベル
    [SerializeField]protected int m_level;      // レベル
    [SerializeField]protected int m_maxHp;      // 最大HP
    [SerializeField]protected int m_currentHp;  // 現在のHP
    [SerializeField]protected int m_attack;     // 攻撃力
    [SerializeField]protected int m_protect;    // 防御力
    [SerializeField]protected int m_agility;    // 素早さ

    public int level { get { return m_level; } }
    public int maxHp { get { return m_maxHp; } }
    public int currentHp
    {
        get { return m_currentHp; }
        set { m_currentHp += value; }
    }
    public int attack { get { return m_attack; } }
    public int protect { get { return m_protect; } }
    public int agility { get { return m_agility; } }



    // 探索時ステータス
    protected float m_walk;         // 歩く速度
    protected float m_run;          // 走る速度
    protected float m_currentSpeed; // 現在の速度
    protected float m_rot;          // 回転速度
    protected float m_turn;         // 振り向き速度

    public float walk { get { return m_walk; } }
    public float run { get { return m_run; } }
    public float currentSpeed { get { return m_currentSpeed; } }
    public float rot { get { return m_rot; } }
    public float turn { get { return m_turn; } }


    // システム
    protected MySystem m_mySystem;          // システム
    protected BattleManager m_battleMana;   // バトル管理
    


    // アニメーション
    protected Animator m_myAnim;    // アニメーター

    public string activeAnim { get { return m_myAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name; } }
    public Animator myAnim { get { return m_myAnim; } }

    #endregion


    protected abstract void SetAllStatus(int id);   // StatusBaseのステータス値を全て設定する関数

    /// <summary>
    /// 2022/02/13
    /// 目標に向かってゆっくり振り向く関数
    /// </summary>
    /// <param name="target">目標のオブジェクト</param>
    protected void LookAtTarget(GameObject target)
    {
        float speed = m_turn * Time.deltaTime;
        Vector3 dir = target.transform.position - transform.position;
        Quaternion q = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speed);
    }



    /// <summary>
    /// 2022/03/01
    /// 再生中のブールアニメーションを終了
    /// </summary>
    protected void BoolAnimationEnd()
    { m_myAnim.SetBool(activeAnim, false); }



    protected virtual void Awake()
    {
        m_mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        m_battleMana = GameObject.Find("GameManager").GetComponent<BattleManager>();
        m_myAnim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
