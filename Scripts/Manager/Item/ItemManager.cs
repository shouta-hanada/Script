using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    #region 変数

    private ItemBoxManager itemBoxMana;
    private MySystem mySystem;

    [SerializeField]private List<Image> childImg = new List<Image>();     // 子のImage
    [SerializeField] private int maxChildCount;
    private const float maxItemVecY = 121.0f;       // 表示アイテムの最大ベクトルY
    private const float minItemVecY = -121.0f;      // 表示アイテムの最低ベクトルY
    private const float selectItemVecX = -100.0f;   // 選択されてるアイテムのX値
    private Vector3 upDownVec = new Vector3(0.0f, 120.0f, 0.0f);    // 上下させるときの追加ベクトル

    // キー入力関連
    private const float overTimer = 0.5f;     // 長押し判定される時間
    private const float scrollWait = 0.1f; // 長押しでスクロールする時の待ち時間
    private float inputTimer;   // キーを入力してからの時間
    private bool inputKey;      // キーを一回入力したか
    private bool longInputKey;  // 長押ししているか

    // 選択アイテム関連
    private bool hasItem;
    public bool HasItem { get { return hasItem; } }

    private const int reflectedItemNum = 3;    // 映すアイテム数
    private int bottomOfTopItem; // 一番上の一番下のアイテム(0 + reflectedItemNum)
    private int topOfBottomItem; // 一番下の一番上のアイテム
    [SerializeField]private int selectItemNum;  // 選択されたアイテム番号

    public int MaxChildCount { get { return maxChildCount; } }
    public int SelectItemEnumNum
    {
        get
        {
            ItemParameter get = childImg[selectItemNum].GetComponent<ItemParameter>();

            switch(get.itemNum)
            {
                case (int)ItemName.Herb: return (int)ItemName.Herb; 
                case (int)ItemName.GreatHerb: return (int)ItemName.GreatHerb; 
                case (int)ItemName.Smoke: return (int)ItemName.Smoke;
                default: return int.MaxValue;
            }
        }
    }
    [SerializeField] private Slider sideSlider; // 選択中のアイテムのおおよその位置

    #endregion 



    #region 関数

    /// <summary>
    /// 2021/12/15
    /// アイテムが表示範囲内であれば表示する関数。
    /// </summary>
    private void ReflectedRange()
    {
        for (int i = 0; i < childImg.Count; i++)
        {
            if(!childImg[i].gameObject.activeSelf)
            { childImg[i].gameObject.SetActive(true); }

            // 範囲内であれば表示
            if (childImg[i].rectTransform.localPosition.y <= maxItemVecY &&
                childImg[i].rectTransform.localPosition.y >= minItemVecY)
            {
                childImg[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                childImg[i].rectTransform.GetChild(0).GetComponent<Image>().color =
                    new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                childImg[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                childImg[i].rectTransform.GetChild(0).GetComponent<Image>().color =
                    new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
        }
    }



    /// <summary>
    /// 2021/12/16
    /// 選択されたアイテムがとびでる関数
    /// </summary>
    private void SelectItem()
    {
        for (int i = 0; i < childImg.Count; i++)
        {
            Vector3 myPos = childImg[i].rectTransform.localPosition;

            // iが選択されたアイテムなら選択したときのx値を指定
            // そうでなければ0
            if (i == selectItemNum)
            { myPos.x = selectItemVecX; }
            
            else
            { myPos.x = 0; }

            childImg[i].rectTransform.localPosition = myPos;
        }
    }



    /// <summary>
    /// 2021/12/16
    /// 一番上と一番下のアイテムの位置を調べる関数。
    /// </summary>
    /// <param name="firstOrLast">一番上のアイテムを調べたいときはtrue</param>
    /// <returns>先頭のアイテムが表示範囲内あるいは、
    /// 末尾のアイテムが表示範囲内でなければtrueを返す</returns>
    private bool CheckPosOfFirstAndLastItems(bool firstOrLast)
    {
        // 一番上を調べる
        if(firstOrLast &&
           childImg[0].rectTransform.localPosition.y < maxItemVecY &&
           childImg[0].rectTransform.localPosition.y > minItemVecY)
        { return false; }

        // 一番下を調べる
        else if (!firstOrLast &&
            childImg[childImg.Count-1].rectTransform.localPosition.y > minItemVecY &&
            childImg[childImg.Count - 1].rectTransform.localPosition.y < maxItemVecY)
        { return false; }


        return true;
    }



    /// <summary>
    /// 2022/01/14
    /// 途中のアイテム番号と位置を調べる関数。
    /// CheckPosOfFirstAndLastItemsがtrueのときに使うことになると思う。
    /// </summary>
    /// <param name="upOrDown">上にスクロールする(selectNumを0に近づけたい)ときはtrue</param>
    /// <returns>上にスクロールするとき、選択されているアイテムが表示上一番上ならtrueを返す</returns>
    private bool CheckItemNumberAndPositionOnTheWay(bool upOrDown)
    {
        int numOfPass = 0; // 通過した回数
        for (int i = 0; i < childImg.Count; i++)
        {
            // childImg[i]が表示範囲なら
            if (childImg[i].rectTransform.localPosition.y <= maxItemVecY &&
                childImg[i].rectTransform.localPosition.y >= minItemVecY)
            {
                // 上にスクロール && 一回しか通過していない && 選択されているアイテムなら
                if(upOrDown && numOfPass == 0 && i == selectItemNum ||
                   !upOrDown && numOfPass == reflectedItemNum - 1 && i == selectItemNum)
                { return true; }

                numOfPass++;
            }
        }
        return false;
    }



    /// <summary>
    /// 2021/12/16
    /// アイテムの位置とSelectItemNumを変更する関数
    /// </summary>
    /// <param name="vec">追加で動かしたいベクトル</param>
    /// <param name="addSelectNum">上にスクロールするときは1、下は-1</param>
    private void ScrollItems(Vector3 vec, int addSelectNum)
    {
        // 選択アイテム番号がアイテムの最後未満且つ追加するのが正の値なら(下スクロール)
        // 選択アイテム番号が1以上且つ追加するのが負の値なら(上スクロール)
        bool canScroll = selectItemNum < childImg.Count - 1 && addSelectNum > 0 ||
                         selectItemNum > 0 && addSelectNum < 0;

        // 入力時間を足す
        inputTimer += Time.deltaTime;

        // 長押しされている && 入力時間がスクロールする時に待つ時間以上
        if (longInputKey && inputTimer >= scrollWait &&
            canScroll)
        {
            for (int i = 0; i < childImg.Count; i++)
            { childImg[i].rectTransform.localPosition += vec; }
            selectItemNum += addSelectNum;
            inputTimer = 0.0f;
        }

        // キーが入力されていない状態なら
        else if (!inputKey)
        {
            if (canScroll)
            {
                for (int i = 0; i < childImg.Count; i++)
                { childImg[i].rectTransform.localPosition += vec; }
                selectItemNum += addSelectNum;
            }
            inputKey = true;
            inputKey = true;
        }

        // 選択したアイテムを少し飛び出させる
        // アイテムを非表示(表示)させる
        SelectItem();
        ReflectedRange();
    }



    /// <summary>
    /// 2021/12/17
    /// SelectItemNumを変更する関数
    /// </summary>
    /// <param name="addSelectNum">下にスクロールしたいときは-1を渡す。上にスクロールしたいときは1を渡す。</param>
    private void ScrollItems(int addSelectNum)
    {
        // 選択アイテム番号がアイテムの最後未満且つ追加するのが正の値なら(下スクロール)
        // 選択アイテム番号が1以上且つ追加するのが負の値なら(上スクロール)
        bool canScroll = selectItemNum < childImg.Count - 1 && addSelectNum > 0 ||
                         selectItemNum > 0 && addSelectNum < 0;

        // 入力時間を足す
        inputTimer += Time.deltaTime;

        // 長押しされている && 入力時間がスクロールする時に待つ時間以上
        if (longInputKey && inputTimer >= scrollWait)
        {
            if (canScroll)
            { selectItemNum += addSelectNum; }
            inputTimer = 0.0f;
        }

        // 一回もキーを入力されていないとき
        else if (!inputKey)
        {
            
            if(canScroll)
            { selectItemNum += addSelectNum; }
            inputKey = true;
        }

        SelectItem();
    }



    /// <summary>
    /// 2022/01/17
    /// 途中からのアイテムの位置とSelectItemNumを変更する関数
    /// </summary>
    /// <param name="vec">追加で動かしたいベクトル</param>
    /// <param name="valueInMiddle">childImgの途中の要素(番号(SelectItemNum))</param>
    /// <param name="mode">上に動かすときはtrue、下はfalse</param>
    private void ScrollItems(Vector3 vec, int valueInMiddle, bool mode)
    {
        if(mode)
        {
            for (int i = valueInMiddle; i < childImg.Count; i++)
            { childImg[i].rectTransform.localPosition += vec; }
        }
        else
        {
            for (int i = valueInMiddle; i >= 0; i--)
            { childImg[i].rectTransform.localPosition += vec; }
        }

        SelectItem();

    }



    /// <summary>
    /// 2022/01/13
    /// アイテム数のサイズ変更
    /// ややこしいけど、
    /// childCount = 1のobjは GetChild(0)だから、
    /// maxchildCountが0(子なし)が0以上になった場合
    /// iに0が入り、GetChild(0)を取得のような形になる。
    /// (分かりやすさ的にはchildImgをClearして、全て取得の方がいいけど、
    /// アイテム数が増えるにつれ処理が重くなるのは困る。)
    /// </summary>
    public void ResizeTheNumberOfItem()
    {
        // 現在の子の数が前フレームの最大子数より大きければ
        if(transform.childCount > maxChildCount)
        { itemBoxMana.AddChildImg(maxChildCount, transform.childCount); }

        hasItem = true;
        maxChildCount = transform.childCount;
        bottomOfTopItem = reflectedItemNum - 1;
        topOfBottomItem = childImg.Count - reflectedItemNum;
    }



    /// <summary>
    /// 2021/12/23
    /// アイテムボックスを開いたときのアイテムの初期化処理。
    /// </summary>
    public void Initialized()
    {
        selectItemNum = 0;
        SelectItem();

        // 最初に表示されるアイテムをアクティブ
        // その他は非アクティブ
        // 非アクティブのアイテムはスクロールしたときにアクティブになる。
        for (int i = 0; i < childImg.Count; i++)
        {
            if (i < reflectedItemNum)
            { childImg[i].gameObject.SetActive(true); }
            else 
            { childImg[i].gameObject.SetActive(false); }
        }

        // 一番上のアイテムのポジションが定位置にいない間
        // アイテム全体を-120する(一番上のアイテムのY軸がバグ以外で0以下になることはない)
        while (childImg[0].rectTransform.localPosition.y >= maxItemVecY)
        {
            for (int i = 0; i < childImg.Count; i++)
            { childImg[i].rectTransform.localPosition += -upDownVec; }
        }
    }



    /// <summary>
    /// 2021/12/23
    /// アイテムボックスを閉じるときのアイテムの終了処理
    /// </summary>
    public void Finalized()
    {
        for (int i = 0; i < childImg.Count; i++)
        {
            // 表示されているアイテム以外のα値を0にする。
            if(childImg[i].rectTransform.localPosition.y >= maxItemVecY &&
                childImg[i].rectTransform.localPosition.y <= minItemVecY)
            {
                childImg[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                childImg[i].rectTransform.GetChild(0).GetComponent<Image>().color =
                    new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            
            // α値が0以下のアイテムを非アクティブにする。
            if(childImg[i].color.a <= 0.0f)
            {  childImg[i].gameObject.SetActive(false); }
        }
    }

    #endregion

    // ============= Start ・ Update =============== //
    private void Start()
    {
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        itemBoxMana = GetComponentInParent<ItemBoxManager>();

        ResizeTheNumberOfItem();
        maxChildCount = transform.childCount;
        hasItem = false;
        

        bottomOfTopItem = reflectedItemNum-1;
        topOfBottomItem = childImg.Count - reflectedItemNum;
        selectItemNum = 0;
        SelectItem();
        ReflectedRange();
        Finalized();
    }

    private void Update()
    {
        // アイテムボックス横のスライダー
        // 非数か否か
        if(!float.IsNaN((float)selectItemNum / ((float)childImg.Count - 1)))
        { sideSlider.value = (float)selectItemNum / ((float)childImg.Count - 1); }

        #region ボタン入力処理
        // 長押ししたとき
        if (inputTimer >= overTimer)
        { longInputKey = true; }

        // WキーかSキーを上げた時
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            inputKey = false;
            longInputKey = false;
            inputTimer = 0.0f;
        }

        // アイテムボックスが表示されているときだけ処理
        if(itemBoxMana.Call && itemBoxMana.fade && itemBoxMana.fadeClear)
        {
            ReflectedRange();

            // 上にスクロール
            // 選択アイテム番号だけが移動
            // w && 選択アイテムが表示アイテムの一番上のとき
            // w && アイテムの先頭が見えているとき
            // w && アイテムの末尾が見えているとき && 選択アイテムが表示アイテムの一番上のとき
            if (Input.GetKey(KeyCode.W) && !CheckItemNumberAndPositionOnTheWay(true) ||
                Input.GetKey(KeyCode.W) && !CheckPosOfFirstAndLastItems(true) ||
                Input.GetKey(KeyCode.W) && !CheckPosOfFirstAndLastItems(false) &&
                !CheckItemNumberAndPositionOnTheWay(true))
            { ScrollItems(-1); }

            // アイテム全体が移動
            else if (Input.GetKey(KeyCode.W))
            { ScrollItems(-upDownVec, -1);}

            // * * * * * * * * * * * * * * * * * * * * * * * //

            // 下にスクロールする。
            else if (Input.GetKey(KeyCode.S) && !CheckItemNumberAndPositionOnTheWay(false) ||
                     Input.GetKey(KeyCode.S) && !CheckPosOfFirstAndLastItems(false) ||
                     Input.GetKey(KeyCode.S) && !CheckPosOfFirstAndLastItems(true) &&
                     selectItemNum < bottomOfTopItem)
            { ScrollItems(1); }

            else if (Input.GetKey(KeyCode.S) && CheckPosOfFirstAndLastItems(false))
            { ScrollItems(upDownVec, 1); }

            // * * * * * * * * * * * * * * * * * * * * * * * //

            // アイテムの使用
            if (Input.GetKeyDown(KeyCode.Return) && maxChildCount != 0 &&
                childImg[selectItemNum].gameObject.GetComponent<ItemParameter>().UseItem())
            {
                // 削除する画像(アイテム)
                GameObject goodbye = childImg[selectItemNum].gameObject;
                int dChildCount = childImg[selectItemNum].transform.childCount;
                
                // 消される前のアイテム番号
                int pastSelect = selectItemNum;

                // Listから削除
                itemBoxMana.RemovedChildImg(childImg[selectItemNum], dChildCount);
                childImg.RemoveAt(selectItemNum);
                
                // ゲームから削除
                Destroy(goodbye);
                maxChildCount = transform.childCount;
                

                // アイテムがなくなった場合何もしない
                if(childImg.Count == 0)
                { 
                    selectItemNum = 0;
                    hasItem = false;
                }

                // 先頭と末尾のアイテムが見えている
                else if(!CheckPosOfFirstAndLastItems(true) &&
                   !CheckPosOfFirstAndLastItems(false))
                {
                    if (selectItemNum + 1 == pastSelect)
                    { SelectItem(); }
                    else
                    { ScrollItems(upDownVec, selectItemNum, true); }
                }

                // 先頭が見えていて末尾のアイテムが見えていない
                else if(!CheckPosOfFirstAndLastItems(true) &&
                         CheckPosOfFirstAndLastItems(false))
                { ScrollItems(upDownVec, selectItemNum, true); }

                // 先頭が見えなくて末尾のアイテムがいる
                else if(CheckPosOfFirstAndLastItems(true) &&
                       !CheckPosOfFirstAndLastItems(false))
                {
                    // 末尾を使った場合
                    if (selectItemNum+1 == pastSelect)
                    { ScrollItems(-upDownVec, selectItemNum, false); }

                    // 末尾以外を使った場合
                    else
                    { ScrollItems(-upDownVec, selectItemNum-1, false); }
                }

                // 先頭も末尾も見えない
                else
                { ScrollItems(upDownVec, selectItemNum, true); }

                itemBoxMana.itemSelect = true;
            }
        }
        #endregion
    }
}