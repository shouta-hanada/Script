using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    // =================== 変数 ==================== //
    private MySystem mySystem;          // システム
    private BattleManager battleMana;   // バトル管理
    private SpriteManager spriteMana;   // スプライト管理
    private PartyStatusBase player;     // プレイヤー
    private EnemyBaseScript enemy;      // エネミー

    private GameObject resultParent;                    // リザルト全体の親
    private List<Image> results = new List<Image>();    // リザルトの子のImage

    private const float fadeSpeed = 1.0f;               // フェードスピード
    private float resultAlpha;                          // resultsのα値
    private bool fade;                                  // alpha値が0のときfalseになる
    private bool fadeComp;                              // フェードが完了したか

    private const int maxGetItemNum = 5;

    private bool resultOnce;            // リザルト処理開始時一回だけ実行

    // EXP
    [SerializeField] private GameObject expValParent;   // 生成した経験値スプライト(Image)オブジェの親
    // Width, Height, 一番左のローカルポスX, ローカルポスY, 文字間隔  
    private static readonly float[] expValProperty = { 28.0f, 40.0f, -100.0f, 100.0f, 50.0f };

    // Turn
    [SerializeField] private GameObject turnValParent;
    private static readonly float[] turnValProperty = { 28.0f, 40.0f, -100.0f, 0.0f, 50.0f };

    // Item
    private ItemDup itemDup;
    [SerializeField] private GameObject itemParent;
    private static readonly float[] itemImgProperty = { 50.0f, 50.0f, 0.0f, -100.0f, 110.0f };

    // Down
    [SerializeField] private GameObject downParent;
    private static readonly float[] downImgProperty = { 40.0f, 40.0f, -80.0f, -200.0f, 50.0f };


    // ================= function ================== //

    /// <summary>
    /// 2022/02/11
    /// バトルのリザルト処理を実行する関数
    /// </summary>
    public IEnumerator ResultFunc()
    {
        // 呼び出されたときに変数が空であれば
        if (!player) { player = battleMana.player; }
        if (!enemy) { enemy = battleMana.enemy; }

        if (!fade)
        { StartCoroutine(FadeInOutResult(true)); }

        // プレイヤーが存在している且つ
        // プレイヤーのHPが0以下なら
        if (player && player.currentHp <= 0)
        { 
            // エンターキーが押されたら抜ける
            while(true)
            {
                PlayerDestroy();

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    ResultEnd();

                    yield break;
                }

                yield return null;
            }
        }

        // * * * * * * * * * * * * * * * * * * * * * * * //

        int[] returnItem = new int[maxGetItemNum];
        int exp = 0;

        // 関数入って一回目なら
        if (!resultOnce)
        {
            returnItem = enemy.item;
            player.currentExp += exp = enemy.exp;
            Destroy(enemy.gameObject);
            resultOnce = true;
        }

        // フェードが完了していてexpValParentに子が居なければUIの表示
        // ※fadeCompだけだと2回生成される。
        else if(fadeComp && expValParent.transform.childCount == 0)
        {
            spriteMana.NumInImage(exp, expValProperty, expValParent);
            spriteMana.StartCoroutine(spriteMana.SpriteAddWave(expValParent));

            spriteMana.NumInImage(battleMana.turnNum, turnValProperty, turnValParent);
            spriteMana.StartCoroutine(spriteMana.SpriteAddWave(turnValParent));


            // アイテムを取得する予定なら。
            if (returnItem != null)
            {
                spriteMana.ItemInImage(returnItem, itemImgProperty, itemParent);
                spriteMana.StartCoroutine(spriteMana.SpriteAddWave(itemParent));


                for (int i = 0; i < returnItem.Length; i++)
                {
                    // returnItem[i]のアイテムを1追加(=にしてるけど、実質+=)
                    switch (returnItem[i])
                    {
                        case (int)ItemName.Herb: itemDup.InitItem((int)ItemName.Herb); break;
                        case (int)ItemName.GreatHerb: itemDup.InitItem((int)ItemName.GreatHerb); break;
                        case (int)ItemName.Smoke: itemDup.InitItem((int)ItemName.Smoke); break;
                    }
                }
            }

            fadeComp = false;
        }

        // * * * * * * * * * * * * * * * * * * * * * * * //

        // Enterキーが押されたら
        // バトルモード、バトルコマンドを初期化
        // ゲームモードをフリーにして、コルーチンを抜ける。
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return)) 
            {
                ResultEnd();

                break; 
            }
            yield return null;
        }
    }

    // ********************************************* //
    /// <summary>
    /// 2021/11/11
    /// リザルトのイメージをフェードインフェードアウトする関数
    /// </summary>
    private IEnumerator FadeInOutResult(bool mode)
    {
        while (fade != mode)
        {
            mySystem.FadeInOut(ref resultAlpha, fadeSpeed, fade);
            for (int i = 0; i < results.Count; i++)
            {
                Color col = results[i].color;
                col.a = resultAlpha;
                results[i].color = col;
            }

            yield return null;
        }

        fadeComp = true;
    }

    // ********************************************* //
    /// <summary>
    /// 2021/11/19
    /// プレイヤーが死んだとき(HP0)の時の処理
    /// 他のパーティーのキャラが死んだときの処理は別に作る予定。
    /// (他のキャラを作っていないから、検証ができない。)
    /// </summary>
    private void PlayerDestroy()
    {
        spriteMana.NumInImage(0, expValProperty, expValParent);
        spriteMana.StartCoroutine(spriteMana.SpriteAddWave(expValParent));

        spriteMana.NumInImage(battleMana.turnNum, turnValProperty, turnValParent);
        spriteMana.StartCoroutine(spriteMana.SpriteAddWave(turnValParent));

        int[] charcter = new int[1] { 0 };
        spriteMana.CharInImage(charcter, downImgProperty, downParent);
        spriteMana.StartCoroutine(spriteMana.SpriteAddWave(downParent));

        Destroy(player.gameObject);
    }

    // ********************************************* //
    /// <summary>
    /// 2021/11/19
    /// リザルト画面終了直前処理(初期化)をする関数。
    /// </summary>
    private void ResultEnd()
    {
        // 不透明の間
        // 透明にする
        StartCoroutine(FadeInOutResult(false));

        // 表示したUIの削除
        // exp, turn, item, down
        for (int i = 0; i < expValParent.transform.childCount; i++)
        { Destroy(expValParent.transform.GetChild(i).gameObject); }

        for (int i = 0; i < turnValParent.transform.childCount; i++)
        { Destroy(turnValParent.transform.GetChild(i).gameObject); }
        
        for (int i = 0; i < itemParent.transform.childCount; i++)
        { Destroy(itemParent.transform.GetChild(i).gameObject); }
        
        for (int i = 0; i < downParent.transform.childCount; i++)
        { Destroy(downParent.transform.GetChild(i).gameObject); }

        Initialized();
    }

    private void Initialized()
    {
        battleMana.turnNum = 0;
        battleMana.battleMode = BattleMode.Command;
        battleMana.battleCommand = BattleCommand.None;
        mySystem.gameMode = GameMode.Free;

        resultOnce = false;
    }

    // ============= Start ・ Update =============== //
    void Start()
    {
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        battleMana = GameObject.Find("GameManager").GetComponent<BattleManager>();
        spriteMana = GameObject.Find("GameManager").GetComponent<SpriteManager>();
        itemDup = GameObject.Find("Items").GetComponent<ItemDup>();

        resultParent = transform.Find("SystemCanvas/Result").gameObject;
        resultParent.GetComponentsInChildren<Image>(results);

        resultAlpha = 0.0f;
        fade = false;
        for(int i = 0; i < results.Count; i++)
        {
            Color col = results[i].color;
            col.a = resultAlpha;
            results[i].color = col;
        }

        fadeComp = false;
        resultOnce = false;
    }

    void Update()
    {
        if (resultAlpha <= 0.0f) { fade = false; }
        else if (resultAlpha >= 1.0f) { fade = true; }
    }
}
