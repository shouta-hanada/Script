using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutBase : MonoBehaviour
{
    #region 変数

    [SerializeField]protected List<Image> m_childImg = new List<Image>();
    protected bool m_fade;          // alpha値が1ならtrue
    protected bool m_pastFade;      // 1フレーム前のfade
    protected bool m_fadeStart;     // フェードを開始するか
    protected bool m_fadeClear;     // フェードが完了しているか
    protected float m_alpha;        // α値
    protected float m_fadeSpeed;    // フェードするスピード

    public bool fade { get { return m_fade; } }
    public bool fadeStart { set { m_fadeStart = value; } }
    public bool fadeClear { get { return m_fadeClear; } }


    protected MySystem m_mySystem;

    #endregion



    #region 関数

    /// <summary>
    /// 2022/02/27
    /// フェードインアウトを実行する関数。
    /// </summary>
    private void FadeInOut()
    {
        if (!fade) { m_alpha += m_fadeSpeed * Time.deltaTime; }
        else if (fade) { m_alpha -= m_fadeSpeed * Time.deltaTime; }

        for (int i = 0; i < m_childImg.Count; i++)
        {
            Color col = m_childImg[i].color;
            col.a = m_alpha;
            m_childImg[i].color = col;
        }
    }



    /// <summary>
    /// 2022/02/27
    /// ImgのRayによる当たり判定を有効にするか否か
    /// </summary>
    /// <param name="enabled">trueで当たり判定が付く</param>
    private void RaycastTargetEnabled(bool enabled)
    {
        for (int i = 0; i < m_childImg.Count; i++)
        { m_childImg[i].raycastTarget = enabled; }
    }



    /// <summary>
    /// 2022/02/27
    /// childImgの要素数を増やす関数
    /// </summary>
    /// <param name="pastNum">増やす前のシーン上の子の最大数</param>
    /// <param name="addNum">現在のシーン上の子の最大数</param>
    public void AddChildImg(int pastNum, int addNum)
    {
        for (int i = pastNum; i < pastNum + addNum; i++)
        {
            // 新しく追加されたImageをGetComponent
            m_childImg.Add(transform.GetChild(i).GetComponent<Image>());
        }
    }



    /// <summary>
    /// 2022/02/28
    /// childImgの中はだいたい
    /// [0]oyaObj
    /// [1]koObj
    /// [2]koObj_2
    /// [3]oyaObj_2
    /// [4]koObj
    /// [5]koObj_2
    /// のようになっている
    /// </summary>
    /// <param name="obj">Listから外したいオブジェクト</param>
    /// <param name="onChild">オブジェクトについてる子の数(0でもいい)</param>
    public void RemovedChildImg(Image obj, int onChild)
    {
        // リムーブする予定の要素番号
        int removeNum = 0;

        // 消したいオブジェクトの要素番号を取得
        foreach (Image i in m_childImg)
        {
            // リムーブ予定のImageと一致したら抜ける
            if (obj == i)
            { break; }

            removeNum++;
        }

        // 消したいオブジェクトとその子供を消していく。
        // 子は親Imgの次の要素になる。
        for (int i = 0; i < onChild+1; i++)
        { m_childImg.RemoveAt(removeNum); }
    }

    #endregion



    // ============= Start ・ Update =============== //
    private void Awake()
    {
        m_mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        GetComponentsInChildren(m_childImg);
    }



    protected virtual void Start()
    {
        // 初期化・透明化
        m_pastFade = m_fade = false;
        m_fadeStart = false;
        m_alpha = 0.0f;
        m_fadeSpeed = 10.0f;
        for(int i = 0; i < m_childImg.Count; i++)
        {
            Color col = m_childImg[i].color;
            col.a = m_alpha;
            m_childImg[i].color = col;
        }
    }



    protected virtual void Update()
    {
        // 不透明ならtrue
        if(m_alpha >= 1) { m_fade = true; RaycastTargetEnabled(true); }
        else if(m_alpha <= 0) { m_fade = false; RaycastTargetEnabled(false); }

        // fadeStartが呼ばれて、フェード状態が切り替わってなければフェード
        if(m_fadeStart && m_pastFade == fade)
        { m_fadeClear = false; FadeInOut(); }
        
        else
        {
            m_fadeStart = false;
            m_pastFade = fade;
            m_fadeClear = true;
        }
    }
}
