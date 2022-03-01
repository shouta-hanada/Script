using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemDup : MonoBehaviour
{
    private Image plate;
    private Image item; 
    private float plPX =   0.0f, plPY = 120.0f ; // プレートのポジション
    private float plSX = 870.0f, plSY = 105.0f;  // プレートのサイズ
    private float itPX = 340.0f, itPY = 0.0f ;   // アイテムのポジション
    private float itSX = 100.0f, itSY = 100.0f;  // アイテムのサイズ
    [SerializeField]private MySystem mySystem;
    private ItemManager itemMana;

    

    public void InitItem(int num)
    {
        int maxChild = transform.childCount;
        float newPlPy = plPY;

        // 生成元(itemObj)の子供の数を取得
        // 生成元の最後の子の位置を取得
        // 再度プレートのポジションを計算。
        if (maxChild != 0)
        {
            Vector2 plPos = transform.GetChild(maxChild - 1).
                 GetComponent<RectTransform>().localPosition;
            newPlPy = plPos.y - plPY;
        }


        // プレートの作成 (obj, parent)
        // プレートの位置設定
        Image pl = Instantiate(plate, transform);
        pl.rectTransform.sizeDelta = new Vector2(plSX, plSY);
        pl.rectTransform.localPosition = new Vector2(plPX, newPlPy);
        pl.sprite = mySystem.spritePlefabMana.GetItemPlateSprite(num);
        pl.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        // アイテムアイコンの作成
        // アイコンの位置設定
        // アイコンのスプライトの変更
        Image it = Instantiate(item, pl.transform);
        it.rectTransform.sizeDelta = new Vector2(itSX, itSY);
        it.rectTransform.localPosition = new Vector2(itPX, itPY);
        it.sprite = mySystem.spritePlefabMana.GetItemSprite(num);
        it.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        pl.gameObject.AddComponent<ItemParameter>().itemNum = num;

        bool s = pl.gameObject.activeSelf;

        itemMana.ResizeTheNumberOfItem();
    }

    // Start is called before the first frame update
    void Start()
    {
        itemMana = GameObject.Find("Items").GetComponent<ItemManager>();
        plate = new GameObject("").AddComponent<Image>();
        item = new GameObject("").AddComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // DebugKeyCode
        if (Input.GetKeyDown(KeyCode.L))
        { InitItem(mySystem.GetRandomValue(0, 3)); } 
    }
}
