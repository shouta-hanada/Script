using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 複製する時などに使うスプライト(画像)をまとめておくクラス
// numが要素数以上の場合どちらにしろエラーだから書いていない。
// 呼び出し->mySystem.spritePlefabMana.Get○○(num)
public class SpritePlefabManager : MonoBehaviour
{
    [SerializeField] private Sprite[] itemSprite = new Sprite[(int)ItemName.ItemMax]; // アイテムのスプライト(アイテム数分)
    public Sprite GetItemSprite(int num) { return itemSprite[num]; }

    [SerializeField] private Sprite[] itemPlateSprite = new Sprite[(int)ItemName.ItemMax];  // アイテムの名前が描かれたスプライト
    public Sprite GetItemPlateSprite(int num) { return itemPlateSprite[num]; }          

    [SerializeField] private Sprite[] charSprite = new Sprite[MySystem.PARTYTYPE];  // キャラの画像
    public Sprite GetCharSprite(int num) { return charSprite[num]; }

    [SerializeField] private Sprite[] numSprite = new Sprite[10];  // 値のスプライト(0~9)
    public Sprite GetNumSprite(int num) { return numSprite[num]; }
}
