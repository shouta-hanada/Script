using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �A�C�e���{�b�N�X���Ǘ�(�t�F�[�h�������s�������ł͂���)
// ���A�C�e���N���X�̊T�v
// DisplaySelectItem >> �A�C�e�����I�����ꂽ�Ƃ��̏���
// ItemDup >> �A�C�e���𕡐����鏈��
// ItemManager >> �A�C�e���̐�, �X�N���[��, �t�F�[�h���s���B
public class ItemBoxManager : FadeInOutBase
{
    // =================== �ϐ� ==================== //
    private bool call;
    public bool Call
    { 
        get { return call; } 
    }
    private ItemManager itemMana; // �A�C�e���ꗗ�̊Ǘ�(���������p)
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
    /// ���^�[���{�^����\����\��������֐�
    /// �{�^�����͂ɑΉ������邽�߂ɂ�₱�����������ɂȂ��Ă���B
    /// </summary>
    /// <param name="mode">�\������Ƃ���true</param>
    public void ReturnButtonAppear(bool mode)
    {
        // �{�^���œ��͂��ꂽ�Ƃ�
        if(m_mySystem.gameMode == GameMode.Battle &&
           battleMana.battleCommand == BattleCommand.Item && !mode)
        {
            returnButtonAppear = mode;
            itemSelect = true;
        }
        
        // mode�̏����Ńx�N�g���ύX(�ȗ�if)
        Vector2 vec = (mode) ? returnButtonAppearPos : returnButtonUnAppearPos;

        Vector2 reVec = returnButtonTrans.localPosition;

        // �w��x�N�g���܂ł̋�����0.1f�ȏ゠���
        if(Vector2.Distance(reVec, vec) > 0.1f)
        {
           returnButtonTrans.localPosition = Vector2.MoveTowards(
           returnButtonTrans.localPosition,
           vec,
           Time.deltaTime * appearSpeed);
        }
    }


    // ============= Start �E Update =============== //
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

        // �o�g�����ɃA�C�e���R�}���h�I�����ĂȂ����ɕ\������Ă�����
        else if (m_mySystem.gameMode == GameMode.Battle && 
                 m_mySystem.battleMana.battleCommand != BattleCommand.Item && 
                 fadeClear && call)
        { 
            itemMana.Finalized();
            call = false;
        }



        // �o�g�����̃A�C�e�����f����
        if(m_mySystem.battleMana.battleCommand == BattleCommand.Item &&
           !itemSelect && Input.GetKeyDown(KeyCode.Tab))
        { itemSelect = true; }

        ReturnButtonAppear(returnButtonAppear); 
    }
}
