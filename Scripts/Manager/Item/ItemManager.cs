using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    #region �ϐ�

    private ItemBoxManager itemBoxMana;
    private MySystem mySystem;

    [SerializeField]private List<Image> childImg = new List<Image>();     // �q��Image
    [SerializeField] private int maxChildCount;
    private const float maxItemVecY = 121.0f;       // �\���A�C�e���̍ő�x�N�g��Y
    private const float minItemVecY = -121.0f;      // �\���A�C�e���̍Œ�x�N�g��Y
    private const float selectItemVecX = -100.0f;   // �I������Ă�A�C�e����X�l
    private Vector3 upDownVec = new Vector3(0.0f, 120.0f, 0.0f);    // �㉺������Ƃ��̒ǉ��x�N�g��

    // �L�[���͊֘A
    private const float overTimer = 0.5f;     // ���������肳��鎞��
    private const float scrollWait = 0.1f; // �������ŃX�N���[�����鎞�̑҂�����
    private float inputTimer;   // �L�[����͂��Ă���̎���
    private bool inputKey;      // �L�[�������͂�����
    private bool longInputKey;  // ���������Ă��邩

    // �I���A�C�e���֘A
    private bool hasItem;
    public bool HasItem { get { return hasItem; } }

    private const int reflectedItemNum = 3;    // �f���A�C�e����
    private int bottomOfTopItem; // ��ԏ�̈�ԉ��̃A�C�e��(0 + reflectedItemNum)
    private int topOfBottomItem; // ��ԉ��̈�ԏ�̃A�C�e��
    [SerializeField]private int selectItemNum;  // �I�����ꂽ�A�C�e���ԍ�

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
    [SerializeField] private Slider sideSlider; // �I�𒆂̃A�C�e���̂����悻�̈ʒu

    #endregion 



    #region �֐�

    /// <summary>
    /// 2021/12/15
    /// �A�C�e�����\���͈͓��ł���Ε\������֐��B
    /// </summary>
    private void ReflectedRange()
    {
        for (int i = 0; i < childImg.Count; i++)
        {
            if(!childImg[i].gameObject.activeSelf)
            { childImg[i].gameObject.SetActive(true); }

            // �͈͓��ł���Ε\��
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
    /// �I�����ꂽ�A�C�e�����Ƃтł�֐�
    /// </summary>
    private void SelectItem()
    {
        for (int i = 0; i < childImg.Count; i++)
        {
            Vector3 myPos = childImg[i].rectTransform.localPosition;

            // i���I�����ꂽ�A�C�e���Ȃ�I�������Ƃ���x�l���w��
            // �����łȂ����0
            if (i == selectItemNum)
            { myPos.x = selectItemVecX; }
            
            else
            { myPos.x = 0; }

            childImg[i].rectTransform.localPosition = myPos;
        }
    }



    /// <summary>
    /// 2021/12/16
    /// ��ԏ�ƈ�ԉ��̃A�C�e���̈ʒu�𒲂ׂ�֐��B
    /// </summary>
    /// <param name="firstOrLast">��ԏ�̃A�C�e���𒲂ׂ����Ƃ���true</param>
    /// <returns>�擪�̃A�C�e�����\���͈͓����邢�́A
    /// �����̃A�C�e�����\���͈͓��łȂ����true��Ԃ�</returns>
    private bool CheckPosOfFirstAndLastItems(bool firstOrLast)
    {
        // ��ԏ�𒲂ׂ�
        if(firstOrLast &&
           childImg[0].rectTransform.localPosition.y < maxItemVecY &&
           childImg[0].rectTransform.localPosition.y > minItemVecY)
        { return false; }

        // ��ԉ��𒲂ׂ�
        else if (!firstOrLast &&
            childImg[childImg.Count-1].rectTransform.localPosition.y > minItemVecY &&
            childImg[childImg.Count - 1].rectTransform.localPosition.y < maxItemVecY)
        { return false; }


        return true;
    }



    /// <summary>
    /// 2022/01/14
    /// �r���̃A�C�e���ԍ��ƈʒu�𒲂ׂ�֐��B
    /// CheckPosOfFirstAndLastItems��true�̂Ƃ��Ɏg�����ƂɂȂ�Ǝv���B
    /// </summary>
    /// <param name="upOrDown">��ɃX�N���[������(selectNum��0�ɋ߂Â�����)�Ƃ���true</param>
    /// <returns>��ɃX�N���[������Ƃ��A�I������Ă���A�C�e�����\�����ԏ�Ȃ�true��Ԃ�</returns>
    private bool CheckItemNumberAndPositionOnTheWay(bool upOrDown)
    {
        int numOfPass = 0; // �ʉ߂�����
        for (int i = 0; i < childImg.Count; i++)
        {
            // childImg[i]���\���͈͂Ȃ�
            if (childImg[i].rectTransform.localPosition.y <= maxItemVecY &&
                childImg[i].rectTransform.localPosition.y >= minItemVecY)
            {
                // ��ɃX�N���[�� && ��񂵂��ʉ߂��Ă��Ȃ� && �I������Ă���A�C�e���Ȃ�
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
    /// �A�C�e���̈ʒu��SelectItemNum��ύX����֐�
    /// </summary>
    /// <param name="vec">�ǉ��œ����������x�N�g��</param>
    /// <param name="addSelectNum">��ɃX�N���[������Ƃ���1�A����-1</param>
    private void ScrollItems(Vector3 vec, int addSelectNum)
    {
        // �I���A�C�e���ԍ����A�C�e���̍Ō㖢�����ǉ�����̂����̒l�Ȃ�(���X�N���[��)
        // �I���A�C�e���ԍ���1�ȏ㊎�ǉ�����̂����̒l�Ȃ�(��X�N���[��)
        bool canScroll = selectItemNum < childImg.Count - 1 && addSelectNum > 0 ||
                         selectItemNum > 0 && addSelectNum < 0;

        // ���͎��Ԃ𑫂�
        inputTimer += Time.deltaTime;

        // ����������Ă��� && ���͎��Ԃ��X�N���[�����鎞�ɑ҂��Ԉȏ�
        if (longInputKey && inputTimer >= scrollWait &&
            canScroll)
        {
            for (int i = 0; i < childImg.Count; i++)
            { childImg[i].rectTransform.localPosition += vec; }
            selectItemNum += addSelectNum;
            inputTimer = 0.0f;
        }

        // �L�[�����͂���Ă��Ȃ���ԂȂ�
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

        // �I�������A�C�e����������яo������
        // �A�C�e�����\��(�\��)������
        SelectItem();
        ReflectedRange();
    }



    /// <summary>
    /// 2021/12/17
    /// SelectItemNum��ύX����֐�
    /// </summary>
    /// <param name="addSelectNum">���ɃX�N���[���������Ƃ���-1��n���B��ɃX�N���[���������Ƃ���1��n���B</param>
    private void ScrollItems(int addSelectNum)
    {
        // �I���A�C�e���ԍ����A�C�e���̍Ō㖢�����ǉ�����̂����̒l�Ȃ�(���X�N���[��)
        // �I���A�C�e���ԍ���1�ȏ㊎�ǉ�����̂����̒l�Ȃ�(��X�N���[��)
        bool canScroll = selectItemNum < childImg.Count - 1 && addSelectNum > 0 ||
                         selectItemNum > 0 && addSelectNum < 0;

        // ���͎��Ԃ𑫂�
        inputTimer += Time.deltaTime;

        // ����������Ă��� && ���͎��Ԃ��X�N���[�����鎞�ɑ҂��Ԉȏ�
        if (longInputKey && inputTimer >= scrollWait)
        {
            if (canScroll)
            { selectItemNum += addSelectNum; }
            inputTimer = 0.0f;
        }

        // �����L�[����͂���Ă��Ȃ��Ƃ�
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
    /// �r������̃A�C�e���̈ʒu��SelectItemNum��ύX����֐�
    /// </summary>
    /// <param name="vec">�ǉ��œ����������x�N�g��</param>
    /// <param name="valueInMiddle">childImg�̓r���̗v�f(�ԍ�(SelectItemNum))</param>
    /// <param name="mode">��ɓ������Ƃ���true�A����false</param>
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
    /// �A�C�e�����̃T�C�Y�ύX
    /// ��₱�������ǁA
    /// childCount = 1��obj�� GetChild(0)������A
    /// maxchildCount��0(�q�Ȃ�)��0�ȏ�ɂȂ����ꍇ
    /// i��0������AGetChild(0)���擾�̂悤�Ȍ`�ɂȂ�B
    /// (������₷���I�ɂ�childImg��Clear���āA�S�Ď擾�̕����������ǁA
    /// �A�C�e������������ɂꏈ�����d���Ȃ�͍̂���B)
    /// </summary>
    public void ResizeTheNumberOfItem()
    {
        // ���݂̎q�̐����O�t���[���̍ő�q�����傫�����
        if(transform.childCount > maxChildCount)
        { itemBoxMana.AddChildImg(maxChildCount, transform.childCount); }

        hasItem = true;
        maxChildCount = transform.childCount;
        bottomOfTopItem = reflectedItemNum - 1;
        topOfBottomItem = childImg.Count - reflectedItemNum;
    }



    /// <summary>
    /// 2021/12/23
    /// �A�C�e���{�b�N�X���J�����Ƃ��̃A�C�e���̏����������B
    /// </summary>
    public void Initialized()
    {
        selectItemNum = 0;
        SelectItem();

        // �ŏ��ɕ\�������A�C�e�����A�N�e�B�u
        // ���̑��͔�A�N�e�B�u
        // ��A�N�e�B�u�̃A�C�e���̓X�N���[�������Ƃ��ɃA�N�e�B�u�ɂȂ�B
        for (int i = 0; i < childImg.Count; i++)
        {
            if (i < reflectedItemNum)
            { childImg[i].gameObject.SetActive(true); }
            else 
            { childImg[i].gameObject.SetActive(false); }
        }

        // ��ԏ�̃A�C�e���̃|�W�V��������ʒu�ɂ��Ȃ���
        // �A�C�e���S�̂�-120����(��ԏ�̃A�C�e����Y�����o�O�ȊO��0�ȉ��ɂȂ邱�Ƃ͂Ȃ�)
        while (childImg[0].rectTransform.localPosition.y >= maxItemVecY)
        {
            for (int i = 0; i < childImg.Count; i++)
            { childImg[i].rectTransform.localPosition += -upDownVec; }
        }
    }



    /// <summary>
    /// 2021/12/23
    /// �A�C�e���{�b�N�X�����Ƃ��̃A�C�e���̏I������
    /// </summary>
    public void Finalized()
    {
        for (int i = 0; i < childImg.Count; i++)
        {
            // �\������Ă���A�C�e���ȊO�̃��l��0�ɂ���B
            if(childImg[i].rectTransform.localPosition.y >= maxItemVecY &&
                childImg[i].rectTransform.localPosition.y <= minItemVecY)
            {
                childImg[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                childImg[i].rectTransform.GetChild(0).GetComponent<Image>().color =
                    new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            
            // ���l��0�ȉ��̃A�C�e�����A�N�e�B�u�ɂ���B
            if(childImg[i].color.a <= 0.0f)
            {  childImg[i].gameObject.SetActive(false); }
        }
    }

    #endregion

    // ============= Start �E Update =============== //
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
        // �A�C�e���{�b�N�X���̃X���C�_�[
        // �񐔂��ۂ�
        if(!float.IsNaN((float)selectItemNum / ((float)childImg.Count - 1)))
        { sideSlider.value = (float)selectItemNum / ((float)childImg.Count - 1); }

        #region �{�^�����͏���
        // �����������Ƃ�
        if (inputTimer >= overTimer)
        { longInputKey = true; }

        // W�L�[��S�L�[���グ����
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            inputKey = false;
            longInputKey = false;
            inputTimer = 0.0f;
        }

        // �A�C�e���{�b�N�X���\������Ă���Ƃ���������
        if(itemBoxMana.Call && itemBoxMana.fade && itemBoxMana.fadeClear)
        {
            ReflectedRange();

            // ��ɃX�N���[��
            // �I���A�C�e���ԍ��������ړ�
            // w && �I���A�C�e�����\���A�C�e���̈�ԏ�̂Ƃ�
            // w && �A�C�e���̐擪�������Ă���Ƃ�
            // w && �A�C�e���̖����������Ă���Ƃ� && �I���A�C�e�����\���A�C�e���̈�ԏ�̂Ƃ�
            if (Input.GetKey(KeyCode.W) && !CheckItemNumberAndPositionOnTheWay(true) ||
                Input.GetKey(KeyCode.W) && !CheckPosOfFirstAndLastItems(true) ||
                Input.GetKey(KeyCode.W) && !CheckPosOfFirstAndLastItems(false) &&
                !CheckItemNumberAndPositionOnTheWay(true))
            { ScrollItems(-1); }

            // �A�C�e���S�̂��ړ�
            else if (Input.GetKey(KeyCode.W))
            { ScrollItems(-upDownVec, -1);}

            // * * * * * * * * * * * * * * * * * * * * * * * //

            // ���ɃX�N���[������B
            else if (Input.GetKey(KeyCode.S) && !CheckItemNumberAndPositionOnTheWay(false) ||
                     Input.GetKey(KeyCode.S) && !CheckPosOfFirstAndLastItems(false) ||
                     Input.GetKey(KeyCode.S) && !CheckPosOfFirstAndLastItems(true) &&
                     selectItemNum < bottomOfTopItem)
            { ScrollItems(1); }

            else if (Input.GetKey(KeyCode.S) && CheckPosOfFirstAndLastItems(false))
            { ScrollItems(upDownVec, 1); }

            // * * * * * * * * * * * * * * * * * * * * * * * //

            // �A�C�e���̎g�p
            if (Input.GetKeyDown(KeyCode.Return) && maxChildCount != 0 &&
                childImg[selectItemNum].gameObject.GetComponent<ItemParameter>().UseItem())
            {
                // �폜����摜(�A�C�e��)
                GameObject goodbye = childImg[selectItemNum].gameObject;
                int dChildCount = childImg[selectItemNum].transform.childCount;
                
                // �������O�̃A�C�e���ԍ�
                int pastSelect = selectItemNum;

                // List����폜
                itemBoxMana.RemovedChildImg(childImg[selectItemNum], dChildCount);
                childImg.RemoveAt(selectItemNum);
                
                // �Q�[������폜
                Destroy(goodbye);
                maxChildCount = transform.childCount;
                

                // �A�C�e�����Ȃ��Ȃ����ꍇ�������Ȃ�
                if(childImg.Count == 0)
                { 
                    selectItemNum = 0;
                    hasItem = false;
                }

                // �擪�Ɩ����̃A�C�e���������Ă���
                else if(!CheckPosOfFirstAndLastItems(true) &&
                   !CheckPosOfFirstAndLastItems(false))
                {
                    if (selectItemNum + 1 == pastSelect)
                    { SelectItem(); }
                    else
                    { ScrollItems(upDownVec, selectItemNum, true); }
                }

                // �擪�������Ă��Ė����̃A�C�e���������Ă��Ȃ�
                else if(!CheckPosOfFirstAndLastItems(true) &&
                         CheckPosOfFirstAndLastItems(false))
                { ScrollItems(upDownVec, selectItemNum, true); }

                // �擪�������Ȃ��Ė����̃A�C�e��������
                else if(CheckPosOfFirstAndLastItems(true) &&
                       !CheckPosOfFirstAndLastItems(false))
                {
                    // �������g�����ꍇ
                    if (selectItemNum+1 == pastSelect)
                    { ScrollItems(-upDownVec, selectItemNum, false); }

                    // �����ȊO���g�����ꍇ
                    else
                    { ScrollItems(-upDownVec, selectItemNum-1, false); }
                }

                // �擪�������������Ȃ�
                else
                { ScrollItems(upDownVec, selectItemNum, true); }

                itemBoxMana.itemSelect = true;
            }
        }
        #endregion
    }
}