using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// アイテムボックスを管理(フェード処理を行うだけではある)
// 他アイテムクラスの概要
// DisplaySelectItem >> アイテムが選択されたときの処理
// ItemDup >> アイテムを複製する処理
// ItemManager >> アイテムの数, スクロール, フェードを行う。
public class ItemBoxManager : FadeInOutBase
{
    // =================== 変数 ==================== //
    private bool call;
    public bool Call
    { 
        get { return call; } 
    }
    private ItemManager itemMana; // アイテム一覧の管理(初期化等用)
    private BattleManager battleMana;
    public bool itemSelect;

    private Transform returnButtonTrans;
    private Vector2 returnButtonAppearPos = new Vector2(780.0f, 400.0f);
    private Vector2 returnButtonUnAppearPos = new Vector2(1200.0f, 400.0f);
    private const float appearSpeed = 12000;
    private bool returnButtonAppear; 

    // ================= function ================== //
    public IEnumerator BattleModeItem()
    {
        itemSelect = false;
        call = true;
        itemMana.Initialized();
        fadeStart = true;
        returnButtonAppear = true;

        yield return new WaitWhile(()=>!itemSelect);
        itemMana.Finalized();
        call = false;
        fadeStart = true;
        returnButtonAppear = false;

        battleMana.battleMode = BattleMode.Command;
        battleMana.battleCommand = BattleCommand.None;
    }

    /// <summary>
    /// リターンボタンを表示非表示させる関数
    /// ボタン入力に対応させるためにややこしい書き方になっている。
    /// </summary>
    /// <param name="mode">表示するときはtrue</param>
    public void ReturnButtonAppear(bool mode)
    {
        // ボタンで入力されたとき
        if(m_mySystem.gameMode == GameMode.Battle &&
           battleMana.battleCommand == BattleCommand.Item && !mode)
        {
            returnButtonAppear = mode;
            itemSelect = true;
        }
        
        // modeの条件でベクトル変更(省略if)
        Vector2 vec = (mode) ? returnButtonAppearPos : returnButtonUnAppearPos;

        Vector2 reVec = returnButtonTrans.localPosition;

        // 指定ベクトルまでの距離が0.1f以上あれば
        if(Vector2.Distance(reVec, vec) > 0.1f)
        {
           returnButtonTrans.localPosition = Vector2.MoveTowards(
           returnButtonTrans.localPosition,
           vec,
           Time.deltaTime * appearSpeed);
        }
    }


    // ============= Start ・ Update =============== //
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        itemMana = transform.Find("Items").GetComponent<ItemManager>();
        battleMana = GameObject.Find("GameManager").GetComponent<BattleManager>();
        returnButtonTrans = transform.Find("ReturnButton");
        call = false;
        itemSelect = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (m_mySystem.gameMode != GameMode.Battle && Input.GetKeyDown(KeyCode.E) && fadeClear)
        {
            call = !call;

            if (itemMana.transform.childCount == 0)
            {
                if (call)
                { m_mySystem.gameMode = GameMode.Pause; }
                else
                { m_mySystem.gameMode = GameMode.Free; }
            }

            else
            {
                if (call)
                { itemMana.Initialized(); m_mySystem.gameMode = GameMode.Pause; }
                else
                { itemMana.Finalized(); m_mySystem.gameMode = GameMode.Free; }
            }



            fadeStart = true;
        }

        // バトル中にアイテムコマンド選択肢てない時に表示されていたら
        else if (m_mySystem.gameMode == GameMode.Battle && 
                 m_mySystem.battleMana.battleCommand != BattleCommand.Item && 
                 fadeClear && call)
        { 
            itemMana.Finalized();
            call = false;
        }



        // バトル中のアイテム中断処理
        if(m_mySystem.battleMana.battleCommand == BattleCommand.Item &&
           !itemSelect && Input.GetKeyDown(KeyCode.Tab))
        { itemSelect = true; }

        ReturnButtonAppear(returnButtonAppear); 
    }
}
