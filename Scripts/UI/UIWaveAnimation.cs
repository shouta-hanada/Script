using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaveAnimation : MonoBehaviour
{
    // =================== �ϐ� ==================== //
    private Image image;

    // ================= function ================== //
    /// <summary>
    /// 2021/11/17
    /// �Ăяo���ꂽ��waveTop��Y���グ�A
    /// waveTop�̒l�܂ōs������ŏ��̍����ɖ߂��֐��B
    /// </summary>
    /// <param name="waveTop">�g�̍ő卂��</param>
    /// <returns></returns>
    public IEnumerator StartWaveAnim(float waveTop)
    {
        if (!image)
        { image = GetComponent<Image>(); }

        // ���g�ƖړI�n�̃|�W�V�����̐錾
        Vector2 startMyPos, myPos;
        startMyPos = myPos = image.rectTransform.localPosition;
        Vector2 tPos = new Vector2(myPos.x, myPos.y + waveTop);

        float dis = float.MaxValue;
        // waveTop�܂ł̋�����0.1�ȉ��ɂȂ�܂�
        while (dis > 0.1f)
        {
            myPos = image.rectTransform.localPosition;
            myPos = Vector2.MoveTowards(myPos, tPos, Time.deltaTime * 100);
            image.rectTransform.localPosition = myPos;
            dis = Vector2.Distance(myPos, tPos);
            yield return null;
        }

        // startMyPos�܂ł̋�����0.1�ȉ��ɂȂ�܂�
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

    // ============= Start �E Update =============== //
    // Start is called before the first frame update
    void Start()
    {
    }
}


