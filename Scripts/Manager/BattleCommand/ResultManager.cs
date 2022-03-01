using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    // =================== �ϐ� ==================== //
    private MySystem mySystem;          // �V�X�e��
    private BattleManager battleMana;   // �o�g���Ǘ�
    private SpriteManager spriteMana;   // �X�v���C�g�Ǘ�
    private PartyStatusBase player;     // �v���C���[
    private EnemyBaseScript enemy;      // �G�l�~�[

    private GameObject resultParent;                    // ���U���g�S�̂̐e
    private List<Image> results = new List<Image>();    // ���U���g�̎q��Image

    private const float fadeSpeed = 1.0f;               // �t�F�[�h�X�s�[�h
    private float resultAlpha;                          // results�̃��l
    private bool fade;                                  // alpha�l��0�̂Ƃ�false�ɂȂ�
    private bool fadeComp;                              // �t�F�[�h������������

    private const int maxGetItemNum = 5;

    private bool resultOnce;            // ���U���g�����J�n����񂾂����s

    // EXP
    [SerializeField] private GameObject expValParent;   // ���������o���l�X�v���C�g(Image)�I�u�W�F�̐e
    // Width, Height, ��ԍ��̃��[�J���|�XX, ���[�J���|�XY, �����Ԋu  
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
    /// �o�g���̃��U���g���������s����֐�
    /// </summary>
    public IEnumerator ResultFunc()
    {
        // �Ăяo���ꂽ�Ƃ��ɕϐ�����ł����
        if (!player) { player = battleMana.player; }
        if (!enemy) { enemy = battleMana.enemy; }

        if (!fade)
        { StartCoroutine(FadeInOutResult(true)); }

        // �v���C���[�����݂��Ă��銎��
        // �v���C���[��HP��0�ȉ��Ȃ�
        if (player && player.currentHp <= 0)
        { 
            // �G���^�[�L�[�������ꂽ�甲����
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

        // �֐������Ĉ��ڂȂ�
        if (!resultOnce)
        {
            returnItem = enemy.item;
            player.currentExp += exp = enemy.exp;
            Destroy(enemy.gameObject);
            resultOnce = true;
        }

        // �t�F�[�h���������Ă���expValParent�Ɏq�����Ȃ����UI�̕\��
        // ��fadeComp��������2�񐶐������B
        else if(fadeComp && expValParent.transform.childCount == 0)
        {
            spriteMana.NumInImage(exp, expValProperty, expValParent);
            spriteMana.StartCoroutine(spriteMana.SpriteAddWave(expValParent));

            spriteMana.NumInImage(battleMana.turnNum, turnValProperty, turnValParent);
            spriteMana.StartCoroutine(spriteMana.SpriteAddWave(turnValParent));


            // �A�C�e�����擾����\��Ȃ�B
            if (returnItem != null)
            {
                spriteMana.ItemInImage(returnItem, itemImgProperty, itemParent);
                spriteMana.StartCoroutine(spriteMana.SpriteAddWave(itemParent));


                for (int i = 0; i < returnItem.Length; i++)
                {
                    // returnItem[i]�̃A�C�e����1�ǉ�(=�ɂ��Ă邯�ǁA����+=)
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

        // Enter�L�[�������ꂽ��
        // �o�g�����[�h�A�o�g���R�}���h��������
        // �Q�[�����[�h���t���[�ɂ��āA�R���[�`���𔲂���B
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
    /// ���U���g�̃C���[�W���t�F�[�h�C���t�F�[�h�A�E�g����֐�
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
    /// �v���C���[�����񂾂Ƃ�(HP0)�̎��̏���
    /// ���̃p�[�e�B�[�̃L���������񂾂Ƃ��̏����͕ʂɍ��\��B
    /// (���̃L����������Ă��Ȃ�����A���؂��ł��Ȃ��B)
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
    /// ���U���g��ʏI�����O����(������)������֐��B
    /// </summary>
    private void ResultEnd()
    {
        // �s�����̊�
        // �����ɂ���
        StartCoroutine(FadeInOutResult(false));

        // �\������UI�̍폜
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

    // ============= Start �E Update =============== //
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
