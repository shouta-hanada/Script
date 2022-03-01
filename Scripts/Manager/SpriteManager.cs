using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    private MySystem mySystem;  // �V�X�e��

    

    
    // ********************************************* //
    /// <summary>
    /// 2021/11/15
    /// ���l���摜�����ĕ\������֐��B
    /// </summary>
    /// <param name="num">���l���������l</param>
    /// <param name="property">
    /// �z�u����X�v���C�g�̔z�u��̃v���p�e�B�z��
    /// [0]->Width [1]->Height [2]->��ԍ��̃��[�J���|�XX [3]->���[�J���|�XY [4]->�����Ԋu  
    /// </param>
    /// <param name="parent">�e�ɂ������I�u�W�F�N�g</param>
    public void NumInImage(int num, float[] property, GameObject parent)
    {
        // �����̃X�v���C�g��������ꕨ
        List<Sprite> numSprite = new List<Sprite>();
        // ImageComponent��t�����̃I�u�W�F
        List<GameObject> spriteObj = new List<GameObject>();


        // ���� 1���͊m���ɂ���(0��1��)
        int digit = 1;

        // i��10�Ŋ�����
        for (int i = num; i >= 10; i /= 10)
        { digit++; }

        // numSprite�ɃX�v���C�g��ǉ����Ă���
        for (int i = digit; i > 0; i--)
        { numSprite.Add(mySystem.spritePlefabMana.GetNumSprite(mySystem.NthDigitVal(num, i)) ); }

        for (int i = 0; i < numSprite.Count; i++)
        {
            // ��̃I�u�W�F�̐����Ɛe�̐ݒ�
            spriteObj.Add(new GameObject());
            spriteObj[i].transform.parent = parent.transform;
            
            // Image�R���|�[�l���g����̃I�u�W�F�ɒǉ����A�擾
            Component expImgCom = spriteObj[i].AddComponent(typeof(Image));
            Image img = expImgCom.GetComponent<Image>();

            // img�X�v���C�g��[i]�̃X�v���C�g������
            img.sprite = numSprite[i];

            // img�̃T�C�Y�ƈʒu�̏������ƏC��
            img.rectTransform.sizeDelta = new Vector2(property[0], property[1]);
            img.rectTransform.localPosition = new Vector2(property[2], property[3]);
            img.rectTransform.localPosition = new Vector2(property[2] + (i * property[4]), property[3]);
        }
    }

    // ********************************************* //
    /// <summary>
    /// 2021/11/19
    /// �A�C�e����\������֐��B
    /// </summary>
    /// <param name="item">�\���������������A�C�e��</param>
    /// <param name="property">
    /// �z�u����X�v���C�g�̔z�u��̃v���p�e�B�z��
    /// [0]->Width [1]->Height [2]->��ԍ��̃��[�J���|�XX [3]->���[�J���|�XY [4]->�����Ԋu  
    /// </param>
    /// <param name="parent">�e�ɂ������I�u�W�F�N�g</param>
    public void ItemInImage(int[] item, float[] property, GameObject parent)
    {
        // �����̃X�v���C�g��������ꕨ
        List<Sprite> itemSprite = new List<Sprite>();
        // ImageComponent��t�����̃I�u�W�F
        List<GameObject> spriteObj = new List<GameObject>();

        // �X�v���C�g�𐶐�
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

        // �X�v���C�g��Image��
        for (int i = 0; i < itemSprite.Count; i++)
        {
            // ��̃I�u�W�F�̐����Ɛe�̐ݒ�
            spriteObj.Add(new GameObject());
            spriteObj[i].transform.parent = parent.transform;

            // Image�R���|�[�l���g����̃I�u�W�F�ɒǉ����A�擾
            Component expImgCom = spriteObj[i].AddComponent(typeof(Image));
            Image img = expImgCom.GetComponent<Image>();

            // img�X�v���C�g��[i]�̃X�v���C�g������
            img.sprite = itemSprite[i];

            // img�̃T�C�Y�ƈʒu�̏������ƏC��
            img.rectTransform.sizeDelta = new Vector2(property[0], property[1]);
            img.rectTransform.localPosition = new Vector2(property[2], property[3]);
            img.rectTransform.localPosition = new Vector2(property[2] + (i * property[4]), property[3]);
        }
    }

    // ********************************************* //
    /// <summary>
    /// 2021/11/19
    /// �L������\������֐��B
    /// </summary>
    /// <param name="character">�\���������������L����</param>
    /// <param name="property">
    /// �z�u����X�v���C�g�̔z�u��̃v���p�e�B�z��
    /// [0]->Width [1]->Height [2]->��ԍ��̃��[�J���|�XX [3]->���[�J���|�XY [4]->�����Ԋu  
    /// </param>
    /// <param name="parent">�e�ɂ������I�u�W�F�N�g</param>
    public void CharInImage(int[] character, float[] property, GameObject parent)
    {
        // �����̃X�v���C�g��������ꕨ
        List<Sprite> charSprite = new List<Sprite>();
        // ImageComponent��t�����̃I�u�W�F
        List<GameObject> charObj = new List<GameObject>();

        // �X�v���C�g�𐶐�
        for (int i = 0; i < character.Length; i++)
        {
            switch (character[i])
            {
                case 0: charSprite.Add(mySystem.spritePlefabMana.GetCharSprite(0)); break;
            }
        }

        // �X�v���C�g��Image��
        for (int i = 0; i < charSprite.Count; i++)
        {
            // ��̃I�u�W�F�̐����Ɛe�̐ݒ�
            charObj.Add(new GameObject());
            charObj[i].transform.parent = parent.transform;

            // Image�R���|�[�l���g����̃I�u�W�F�ɒǉ����A�擾
            Component expImgCom = charObj[i].AddComponent(typeof(Image));
            Image img = expImgCom.GetComponent<Image>();

            // img�X�v���C�g��[i]�̃X�v���C�g������
            img.sprite = charSprite[i];

            // img�̃T�C�Y�ƈʒu�̏������ƏC��
            img.rectTransform.sizeDelta = new Vector2(property[0], property[1]);
            img.rectTransform.localPosition = new Vector2(property[2], property[3]);
            img.rectTransform.localPosition = new Vector2(property[2] + (i * property[4]), property[3]);
        }
    }

    // ********************************************* //
    /// <summary>
    /// UIWaveAnimation��AddComponent����֐��B
    /// �q���I�u�W�F�N�g�ł���ΐe������΂ǂ�ɂł�Add�ł���͂������A
    /// �e(��)/�q(image)��z�肵�Ă���B
    /// </summary>
    /// <param name="parent">�e</param>
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
