using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySelectItem : MonoBehaviour
{
    private MySystem mySystem;
    [SerializeField]private ItemBoxManager itemBoxMana;
    [SerializeField]private ItemManager itemMana;
    [SerializeField] private Image selectItem;
    [SerializeField] private Image decision;
    [SerializeField] private Image itemNone;
    private Color colNone = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    private Color colNormal = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // アイテムボックスが表示されている && アイテムを所持している。
        if (itemBoxMana.Call && itemBoxMana.fade && itemBoxMana.fadeClear &&
            itemMana.HasItem)
        {
            selectItem.color = colNormal;
            decision.color = colNormal;
            itemNone.color = colNone;

            selectItem.sprite = mySystem.spritePlefabMana.GetItemSprite(itemMana.SelectItemEnumNum);
        }

        else if(itemBoxMana.Call && itemBoxMana.fade && itemBoxMana.fadeClear)
        {
            selectItem.color = colNone;
            decision.color = colNone;
            itemNone.color = colNormal;
        }

        else
        {
            selectItem.color = colNone;
            decision.color = colNone;
            itemNone.color = colNone;
        }
    }
}
