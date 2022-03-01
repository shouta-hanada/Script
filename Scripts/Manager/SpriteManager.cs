using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    private MySystem mySystem;  // システム

    

    
    // ********************************************* //
    /// <summary>
    /// 2021/11/15
    /// 数値を画像化して表示する関数。
    /// </summary>
    /// <param name="num">数値化したい値</param>
    /// <param name="property">
    /// 配置するスプライトの配置後のプロパティ配列
    /// [0]->Width [1]->Height [2]->一番左のローカルポスX [3]->ローカルポスY [4]->文字間隔  
    /// </param>
    /// <param name="parent">親にしたいオブジェクト</param>
    public void NumInImage(int num, float[] property, GameObject parent)
    {
        // 数字のスプライトを入れる入れ物
        List<Sprite> numSprite = new List<Sprite>();
        // ImageComponentを付ける空のオブジェ
        List<GameObject> spriteObj = new List<GameObject>();


        // 桁数 1桁は確実にある(0も1桁)
        int digit = 1;

        // iが10で割れる間
        for (int i = num; i >= 10; i /= 10)
        { digit++; }

        // numSpriteにスプライトを追加していく
        for (int i = digit; i > 0; i--)
        { numSprite.Add(mySystem.spritePlefabMana.GetNumSprite(mySystem.NthDigitVal(num, i)) ); }

        for (int i = 0; i < numSprite.Count; i++)
        {
            // 空のオブジェの生成と親の設定
            spriteObj.Add(new GameObject());
            spriteObj[i].transform.parent = parent.transform;
            
            // Imageコンポーネントを空のオブジェに追加し、取得
            Component expImgCom = spriteObj[i].AddComponent(typeof(Image));
            Image img = expImgCom.GetComponent<Image>();

            // imgスプライトに[i]のスプライトを入れる
            img.sprite = numSprite[i];

            // imgのサイズと位置の初期化と修正
            img.rectTransform.sizeDelta = new Vector2(property[0], property[1]);
            img.rectTransform.localPosition = new Vector2(property[2], property[3]);
            img.rectTransform.localPosition = new Vector2(property[2] + (i * property[4]), property[3]);
        }
    }

    // ********************************************* //
    /// <summary>
    /// 2021/11/19
    /// アイテムを表示する関数。
    /// </summary>
    /// <param name="item">表示したいしたいアイテム</param>
    /// <param name="property">
    /// 配置するスプライトの配置後のプロパティ配列
    /// [0]->Width [1]->Height [2]->一番左のローカルポスX [3]->ローカルポスY [4]->文字間隔  
    /// </param>
    /// <param name="parent">親にしたいオブジェクト</param>
    public void ItemInImage(int[] item, float[] property, GameObject parent)
    {
        // 数字のスプライトを入れる入れ物
        List<Sprite> itemSprite = new List<Sprite>();
        // ImageComponentを付ける空のオブジェ
        List<GameObject> spriteObj = new List<GameObject>();

        // スプライトを生成
        for(int i = 0; i < item.Length; i++)
        {
            switch(item[i])
            {
                case (int)ItemName.Herb: 
                    itemSprite.Add(mySystem.spritePlefabMana.GetItemSprite((int)ItemName.Herb));
                    break;

                case (int)ItemName.GreatHerb:
                    itemSprite.Add(mySystem.spritePlefabMana.GetItemSprite((int)ItemName.GreatHerb));
                    break;

                case (int)ItemName.Smoke:
                    itemSprite.Add(mySystem.spritePlefabMana.GetItemSprite((int)ItemName.Smoke));
                    break;
            }
        }

        // スプライトをImage化
        for (int i = 0; i < itemSprite.Count; i++)
        {
            // 空のオブジェの生成と親の設定
            spriteObj.Add(new GameObject());
            spriteObj[i].transform.parent = parent.transform;

            // Imageコンポーネントを空のオブジェに追加し、取得
            Component expImgCom = spriteObj[i].AddComponent(typeof(Image));
            Image img = expImgCom.GetComponent<Image>();

            // imgスプライトに[i]のスプライトを入れる
            img.sprite = itemSprite[i];

            // imgのサイズと位置の初期化と修正
            img.rectTransform.sizeDelta = new Vector2(property[0], property[1]);
            img.rectTransform.localPosition = new Vector2(property[2], property[3]);
            img.rectTransform.localPosition = new Vector2(property[2] + (i * property[4]), property[3]);
        }
    }

    // ********************************************* //
    /// <summary>
    /// 2021/11/19
    /// キャラを表示する関数。
    /// </summary>
    /// <param name="character">表示したいしたいキャラ</param>
    /// <param name="property">
    /// 配置するスプライトの配置後のプロパティ配列
    /// [0]->Width [1]->Height [2]->一番左のローカルポスX [3]->ローカルポスY [4]->文字間隔  
    /// </param>
    /// <param name="parent">親にしたいオブジェクト</param>
    public void CharInImage(int[] character, float[] property, GameObject parent)
    {
        // 数字のスプライトを入れる入れ物
        List<Sprite> charSprite = new List<Sprite>();
        // ImageComponentを付ける空のオブジェ
        List<GameObject> charObj = new List<GameObject>();

        // スプライトを生成
        for (int i = 0; i < character.Length; i++)
        {
            switch (character[i])
            {
                case 0: charSprite.Add(mySystem.spritePlefabMana.GetCharSprite(0)); break;
            }
        }

        // スプライトをImage化
        for (int i = 0; i < charSprite.Count; i++)
        {
            // 空のオブジェの生成と親の設定
            charObj.Add(new GameObject());
            charObj[i].transform.parent = parent.transform;

            // Imageコンポーネントを空のオブジェに追加し、取得
            Component expImgCom = charObj[i].AddComponent(typeof(Image));
            Image img = expImgCom.GetComponent<Image>();

            // imgスプライトに[i]のスプライトを入れる
            img.sprite = charSprite[i];

            // imgのサイズと位置の初期化と修正
            img.rectTransform.sizeDelta = new Vector2(property[0], property[1]);
            img.rectTransform.localPosition = new Vector2(property[2], property[3]);
            img.rectTransform.localPosition = new Vector2(property[2] + (i * property[4]), property[3]);
        }
    }

    // ********************************************* //
    /// <summary>
    /// UIWaveAnimationをAddComponentする関数。
    /// 子がオブジェクトであれば親がいればどれにでもAddできるはずだが、
    /// 親(空)/子(image)を想定している。
    /// </summary>
    /// <param name="parent">親</param>
    public IEnumerator SpriteAddWave(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            UIWaveAnimation uiAnim = child.AddComponent<UIWaveAnimation>();
            uiAnim.StartCoroutine(uiAnim.StartWaveAnim(25.0f));

            yield return new WaitForSeconds(0.1f);
        }
    }


    private void Start()
    {
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
    }
}
