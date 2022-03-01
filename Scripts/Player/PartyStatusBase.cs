using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PartyStatusBase : StatusBase
{
    #region 変数

    protected int m_id;
    protected int m_maxExp;      // 経験値の入れ物
    protected int m_currentExp;  // 経験値
    protected int m_hitRate;     // 命中率

    private const float m_sliderSpeed = 0.5f; // スライダーの増減速度

    private bool m_guard; // ガードができたか

    private Slider m_hpSlider;  // HPのスライダー
    private Slider m_expSlider; // 経験値のスライダー
    private Text m_levelText;   // ステータスUI

    protected Camera m_mainCamera;  // カメラ

    private PartyStatusInfo info;

    public int currentExp
    {
        get { { return m_currentExp; } }
        set { { m_currentExp += value; } }
    }
    public int hitRate { get { return m_hitRate; } }
    public bool guard
    { 
        get { return m_guard; } 
        set { m_guard = value; }
    }

    #endregion



    #region 関数

    #region 移動系

    /// <summary>
    /// 2021/11/22
    /// カメラを基準に歩く方向を切り替える関数。
    /// </summary>
    /// <param name="camera">目標</param>
    protected void SwitchDirWalk(GameObject camera)
    {
        float speed = m_turn * Time.deltaTime;
        Vector3 dir;
        Quaternion target = transform.rotation;
        Vector3 camV = camera.transform.forward;

        // 向きの設定 見る方向の設定
        if (Input.GetKey(KeyCode.W))
        {
            dir = new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(dir);
        }

        if (Input.GetKey(KeyCode.S))
        {
            dir = new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(-dir);
        }

        if (Input.GetKey(KeyCode.A))
        {
            dir = Quaternion.Euler(0, 90, 0) * new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(-dir);
        }

        if (Input.GetKey(KeyCode.D))
        {
            dir = Quaternion.Euler(0, 90, 0) * new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(dir);
        }


        // 一定のスピードでプレイヤーのY軸回転を変更
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, speed);
    }



    /// <summary>
    /// 2021/11/22
    /// 歩く、走るの挙動を行う関数。
    /// W + Shiftの時は走る。
    /// W,A,S,Dが押されているときは歩く。
    /// WASDが押されていなく、動いている間は減速。
    /// </summary>
    protected void WalkAndRun()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) ||
           Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.RightShift))
        {
            if (m_currentSpeed <= m_run)
            { m_currentSpeed += m_run * Time.deltaTime; }
        }

        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (m_currentSpeed <= m_walk)
            { m_currentSpeed += m_walk * Time.deltaTime; }
        }

        else
        {
            if (Mathf.Abs(m_currentSpeed) > 0.1f)
            { m_currentSpeed = -m_run * Time.deltaTime; }
            else
            { m_currentSpeed = 0.0f; }
        }
    }

    #endregion



    #region ステータス系
    protected override void SetAllStatus(int id)
    {
        info = m_mySystem.pStatusInfo;

        if(m_level < info.level[id])
        { m_level = info.level[id]; }
        m_currentHp = m_maxHp = m_level * info.hpMag[id];
        m_attack = m_level * info.atMag[id];
        m_protect = Mathf.RoundToInt((float)m_attack * (float)info.prMag[id] / 10); // 割る10
        m_agility = Mathf.RoundToInt((float)m_level * (float)info.agMag[id] / 10); // 割る10
        m_maxExp = m_level * info.expMag[id];

        m_walk = info.walk[id];
        m_run = info.run[id];
        m_rot = info.rot[id];
        m_turn = info.turn[id];

        #region 基本必須ステータスが0になるらないようにするけどチェック
        if (m_maxHp <= 0)
        { m_maxHp = 1; }

        if (m_attack <= 0)
        { m_attack = 1; }

        if (m_protect <= 0)
        { m_protect = 1; }

        if (m_agility <= 0)
        { m_agility = 1; }
        #endregion
    }



    /// <summary>
    /// 2022/02/13
    /// 経験値が最大になっているか確認する関数。
    /// レベルアップ後のステータス設定を行う。
    /// </summary>
    protected void CheckLevelUp()
    {
        if(m_currentExp >= m_maxExp && m_level < m_maxLevel)
        {
            m_expSlider.value = 0.0f;
            m_currentExp = m_currentExp - m_maxExp;
            SetAllStatus(m_id);
        }
    }



    /// <summary>
    /// 2022/02/13
    /// Sliderのvalueが変動する関数
    /// </summary>
    /// <param name="slider">変動させたいSlider</param>
    /// <param name="nume">変動後の値</param>
    /// <param name="deno">値の最大値(分母)</param>
    private void TransSliderVal
        (Slider slider, int nume , int deno)
    {
        float sVal = slider.value;
        float dVal = (float)nume / (float)deno;
        float dSpeed = m_sliderSpeed * Time.deltaTime;

        slider.value = Mathf.MoveTowards(sVal, dVal, dSpeed);
    }

    #endregion



    #endregion

    // ============= Start ・ Update =============== //
    protected override void Start()
    {
        m_mainCamera = Camera.main;
        m_hpSlider = transform.Find("ThisCanvas/HP").GetComponent<Slider>();
        m_expSlider = transform.Find("ThisCanvas/EXP").GetComponent<Slider>();
        m_levelText = transform.Find("ThisCanvas/Lv").GetComponent<Text>(); 


        m_hpSlider.value = currentHp;
        m_expSlider.value = currentExp;
    }

    protected override void Update()
    {
        TransSliderVal(m_hpSlider, m_currentHp, m_maxHp);
        TransSliderVal(m_expSlider, m_currentExp, m_maxExp);

        CheckLevelUp();

        m_levelText.text = "Lv." + m_level.ToString();

        m_myAnim.SetFloat("Walk", m_currentSpeed/m_run);
    }


    // パーティに加わるキャラの種族(ステータスID用)
    protected enum PartyID
    {
        Skelton,
    }
}
