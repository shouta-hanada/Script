using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaveAnimation : MonoBehaviour
{
    // =================== 変数 ==================== //
    private Image image;

    // ================= function ================== //
    /// <summary>
    /// 2021/11/17
    /// 呼び出されたらwaveTop分Yを上げ、
    /// waveTopの値まで行ったら最初の高さに戻す関数。
    /// </summary>
    /// <param name="waveTop">波の最大高さ</param>
    /// <returns></returns>
    public IEnumerator StartWaveAnim(float waveTop)
    {
        if (!image)
        { image = GetComponent<Image>(); }

        // 自身と目的地のポジションの宣言
        Vector2 startMyPos, myPos;
        startMyPos = myPos = image.rectTransform.localPosition;
        Vector2 tPos = new Vector2(myPos.x, myPos.y + waveTop);

        float dis = float.MaxValue;
        // waveTopまでの距離が0.1以下になるまで
        while (dis > 0.1f)
        {
            myPos = image.rectTransform.localPosition;
            myPos = Vector2.MoveTowards(myPos, tPos, Time.deltaTime * 100);
            image.rectTransform.localPosition = myPos;
            dis = Vector2.Distance(myPos, tPos);
            yield return null;
        }

        // startMyPosまでの距離が0.1以下になるまで
        dis = float.MaxValue;
        while (dis > 0.1f)
        {
            myPos = image.rectTransform.localPosition;
            myPos = Vector2.MoveTowards(myPos, startMyPos, Time.deltaTime * 100);
            image.rectTransform.localPosition = myPos;
            dis = Vector2.Distance(myPos, startMyPos);
            yield return null;
        }

        yield return null;
    }

    // ============= Start ・ Update =============== //
    // Start is called before the first frame update
    void Start()
    {
    }
}


