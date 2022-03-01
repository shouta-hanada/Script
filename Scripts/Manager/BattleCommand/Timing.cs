using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timing : FadeInOutBase
{
    #region 変数

    // FadeInOutBase.childImg->0: BackGround, 1: Circle, 2: Target
    private const int CIRCLE = 1;   // chilImgに追加されるときのCircleオブジェクトの要素番号
    private Vector2 m_timingSize = new Vector2(500.0f, 500.0f);   // タイミングのサイズ


    private const float FADESPEAD = 5.0f;      // フェードスピード
    private const float MAXTIMINGTIME = 1.0f;  // 実質タイミングの何%を表す。1固定
    private const float TIMINGSPEED = 0.7f;    // タイミングが何秒で縮小するか(n倍)
    private float m_shrinkSpeed;  // shirinkSpeed * Time.deltatimeをすることで1秒でタイミングが消滅するためのもの
    private float m_timingTimer;  // 実質タイミングの何%を表す。


    private const float EXCELLENTMIN = 0.43f;   // エクセレント範囲の最小値 
    private const float EXCELLENTMAX = 0.53f;   // エクセレント範囲の最大値
    private const float GOODMIN = 0.3f;         // グッド範囲の最小値
    private const float GOODMAX = 0.7f;         // グッド範囲の最大値


    public const int EXCELLENTHITRATE = 100;   // エクセレント時の攻撃命中率
    public const int GOODHITRATE = 90;         // グッド時の攻撃命中率
    public const int BADHITRATE = 60;          // バッド時の攻撃命中率
    private int m_hitRate; // 命中率

    public int hitRate { get { return m_hitRate; } }


    private bool m_timingEnd;     // タイミング処理が終わったかどうか

    public bool timingEnd { get { return m_timingEnd; } }

    #endregion



    #region 関数

    /// <summary>
    /// 2022/02/27
    /// 攻撃タイミング(精度？)のUIを設定する関数。
    /// </summary>
    public IEnumerator TimingRun()
    {
        // タイミング処理の始まりと、
        // フェードの開始
        m_timingEnd = false;
        m_fadeStart = true;


        // タイミング処理
        while(true)
        {
            m_timingTimer -= Time.deltaTime * TIMINGSPEED;

            CircleScaling();

            // Spaceを押したとき、タイミングが開始されて何秒減ったかで判定
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (m_timingTimer > EXCELLENTMIN && m_timingTimer < EXCELLENTMAX)
                { m_hitRate = EXCELLENTHITRATE; }
                
                else if (m_timingTimer > EXCELLENTMAX && m_timingTimer < GOODMAX ||
                    m_timingTimer > GOODMIN && m_timingTimer < EXCELLENTMIN)
                { m_hitRate = GOODHITRATE; }
                
                else
                { m_hitRate = BADHITRATE; }

                break;
            }

            else if (m_timingTimer <= 0)
            {
                m_hitRate = 0;
                break;
            }

            yield return null;
        }

        m_fadeStart = true;
        yield return null;
        yield return new WaitWhile(() => !fadeClear);

        // 終了処理
        m_timingEnd = true;
        m_timingTimer = MAXTIMINGTIME;
        m_childImg[CIRCLE].rectTransform.sizeDelta = m_timingSize;
    }



    /// <summary>
    /// 2021/10/29
    /// 円を縮小させる関数
    /// </summary>
    private void CircleScaling()
    {
        Vector2 sizeDelta = m_childImg[CIRCLE].rectTransform.sizeDelta;
        sizeDelta.x -= (m_shrinkSpeed * Time.deltaTime) * TIMINGSPEED;
        sizeDelta.y -= (m_shrinkSpeed * Time.deltaTime) * TIMINGSPEED;
        m_childImg[CIRCLE].rectTransform.sizeDelta = sizeDelta;
    }

    #endregion


    // ============= Start ・ Update =============== //
    protected override　void Start()
    {
        base.Start();

        m_fadeSpeed = FADESPEAD;
        m_shrinkSpeed = m_childImg[1].rectTransform.rect.width;
        m_timingTimer = MAXTIMINGTIME;
    }

    protected override void Update()
    {
        base.Update();
    }
}
