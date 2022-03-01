using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 2021/10/11
/// �����̃X�N���v�g�Ŏg���֐����̒u����
/// </summary>
public class MySystem : MonoBehaviour
{
    #region �ϐ�
    // �Q�[������
    private const int FRAMERATE = 60;       // �t���[�����[�g
    public const int PARTYTYPE = 1; // ����L�����̎��
    public const int ENEMYTYPE = 1; // �G�l�~�[�̎��
    public const int ITEMTYPE = 3;  // �A�C�e���̎��
    public const int PERCENT = 100; // 100
    public const int MAXLEVEL = 100;       // �ő僌�x��



    // CVS(�X�e�[�^�X���)
    private PartyStatusInfo m_playerStatusInfo; // �v���C���[�̃X�e�[�^�X�̑f
    private EnemyStatusInfo m_enemyStatusInfo;  // �G�l�~�[�̃X�e�[�^�X�̑f
    private DropItemInfo m_dropItemInfo;        // �G�l�~�[����h���b�v����A�C�e���̑f
    
    public PartyStatusInfo pStatusInfo�@{ get { return m_playerStatusInfo; } }
    public EnemyStatusInfo eStatusInfo�@{ get { return m_enemyStatusInfo; } }
    public DropItemInfo dropItemInfo �@{ get { return m_dropItemInfo; } }
    


    // �{�^���̓��͊֘A 
    private const float m_canButtonInterval = 0.75f; // �{�^�������͕s�\�Ȏ���
    private bool m_canButton;                       // �{�^���������邩
    private float m_inputButtonTimer;               // �{�^����������Ă���̎��Ԍv��

    public bool canButton { get { return m_canButton; } }



    // �V�X�e���֘A
    private BattleManager m_battleMana;
    private SpritePlefabManager m_spritePlefabMana;
    private System.Random m_rand = new System.Random(); // �������쐬����@�B
    private GameMode m_nowGameMode;            // ���݂̃Q�[�����[�h

    public BattleManager battleMana { get { return m_battleMana; } }
    public SpritePlefabManager spritePlefabMana { get { return m_spritePlefabMana; } }
    public GameMode gameMode
    {
        get { return m_nowGameMode; }
        set { m_nowGameMode = value; }
    }

    #endregion



    #region �֗��n

    /// <summary>
    /// 2022/01/24
    /// x,z���W������2�̋��������߂�֐��B
    /// xyz���W�̋��������߂����ꍇ�́A
    /// Vector3.Distance���g���āB
    /// </summary>
    /// <param name="vec1">1�ڂ̍��W</param>
    /// <param name="vec2">2�ڂ̍��W</param>
    /// <returns>�����̐�Βl</returns>
    public float MathDistance(Vector3 vec1, Vector3 vec2, bool abs = true)
    {
        float posX = Mathf.Pow(vec1.x - vec2.x, 2);
        float posZ = Mathf.Pow(vec1.z - vec2.z, 2);

        if (abs)
        { return Mathf.Abs(Mathf.Sqrt(posX + posZ)); }
        else
        { return Mathf.Sqrt(posX + posZ); }
    }



    /// <summary>
    /// 2021/11/18
    /// �����_���Ȓl���擾����֐��B
    /// �f�t�H���g�ł�0�`100�̒l��Ԃ��B
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public int GetRandomValue(int min = 0, int max = 101)
    { return m_rand.Next(min, max); }



    /// <summary>
    /// 2022/02/17
    /// �N�C�b�N�\�[�g
    /// ��2022/02/18�ǋL
    /// �A�C�e���擾�m���Ɏg�p�ړI���������쐬��A�g�p�ł��Ȃ�(���Ă��Ӗ����Ȃ�)���Ƃ�
    /// �C�Â��g�p���Ă��Ȃ����A���ʂƂ��Ďc���B
    /// </summary>
    /// <param name="array">�\�[�g�������z��</param>
    /// <param name="left">�z��̍ŏ��̗v�f</param>
    /// <param name="right">�z��̍Ō�̗v�f</param>
    public void ArraySort(ref int[] array, int left, int right)
    {
        // �ŏ��̗p���ԍ��ƍŌ�̗p���ԍ��������Ȃ�\�[�g�͂��Ȃ��Ă����B
        if (left >= right)
        { return; }

        // �N�C�b�N�\�[�g�̊�l
        int pivot = array[left];

        int i = left;
        int j = right;

        // �\�[�g����ꏊ�������܂ŌJ��Ԃ��B
        while (true)
        {
            while (array[i] < pivot)
            { i++; }

            while (array[j] > pivot)
            { j--; }

            // ������Ă���
            if (i >= j)
            { break; }

            // ������Ă��Ȃ���while�𔲂���
            else
            {
                ValueSwap(ref array[i], ref array[j]);
                i++;
                j--;
            }
        }

        // �ċA
        ArraySort(ref array, left, i - 1);
        ArraySort(ref array, j + 1, right);
    }



    /// <summary>
    /// 2022/02/17
    /// �l�����ւ���֐�(�\�[�g�p)
    /// </summary>
    /// <param name="a">�l</param>
    /// <param name="b">�l</param>
    private void ValueSwap(ref int a, ref int b)
    {
        int hold = a;
        a = b;
        b = hold;
    }



    /// <summary>
    /// 2021/11/15
    /// int�^��n���Ԗڂ̒l���擾����֐�
    /// 
    /// �v�Z���̗�
    /// num = 145, digit = 2(�܂�4���擾������)
    /// digPow = 10^2 = 100
    /// (145 - (145/100) * 100) = 145 - (1 * 100) = 45 
    /// 45/(10^1) = 4
    /// 
    /// 1���ڂ��擾���鎞�́A��L�̌v�Z������0�����Ԃ��Ȃ����߁A
    /// (->((int)Mathf.Pow(10, (1 -1)))��n/0�ɂȂ�B)
    /// 1���ڂ̌v�Z���́A
    /// 145 - (145/10) * 10 = 145 - 140 = 5
    /// �ɂȂ�B
    /// </summary>
    /// <param name="num">���ׂ����l</param>
    /// <param name="digit">���ׂ�����</param>
    /// <returns></returns>
    public int NthDigitVal(int num, int digit)
    {
        // 1�������̏ꍇ0��Ԃ�
        if (digit <= 0) { return 0; }

        int val = 0;    // �l�̌��ʂ̓��ꕨ

        // 10�̒��ׂ������ׂ���
        int digPow = (int)Mathf.Pow(10, digit);

        // 1���ڂ𒲂ׂ����Ƃ�
        if (digit == 1) { val = num - (num / digPow) * digPow; }

        else
        { val = (num - (num / digPow) * digPow) / (int)Mathf.Pow(10, (digit - 1)); }

        return val;
    }

    #endregion


    /// <summary>
    /// 2021/10/29
    /// ���l��ύX����֐��B
    /// color�ւ̑���͍s���Ȃ��B(for���ŌĂяo���ƃ��l���������߁B)
    /// </summary>
    /// <param name="alphaVal">���l</param>
    /// <param name="speed">�t�F�[�h���x</param>
    /// <param name="fade">alpha�����������Ȃ�true��n��</param>
    public void FadeInOut(ref float alphaVal, float speed, bool fade)
    {
        if (!fade) { alphaVal += speed * Time.deltaTime; }
        else if (fade) { alphaVal -= speed * Time.deltaTime; }
    }

    

    /// <summary>
    /// 2021/11/11
    /// �{�^�������͉\�ɂȂ�܂ł̎��Ԃ��v������֐�
    /// </summary>
    public IEnumerator ButtonTimer()
    {
        m_canButton = false;

        while(m_inputButtonTimer < m_canButtonInterval)
        { 
            m_inputButtonTimer += Time.deltaTime;
            yield return null;
        }

        m_inputButtonTimer = 0.0f;
        m_canButton = true;
    }


    // ============= Start �E Update =============== //
    private void Awake()
    {
        Application.targetFrameRate = FRAMERATE;

        m_playerStatusInfo = new PartyStatusInfo();
        m_enemyStatusInfo = new EnemyStatusInfo();
        m_dropItemInfo = new DropItemInfo();
        m_playerStatusInfo.Init();
        m_enemyStatusInfo.Init();
        m_dropItemInfo.Init();
    }

    private void Start()
    {
        

        m_nowGameMode = GameMode.Free;
        m_battleMana = GetComponent<BattleManager>();
        m_spritePlefabMana = GetComponent<SpritePlefabManager>();
        m_canButton = true;
        m_inputButtonTimer = 0.0f;

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        { Application.Quit(); }

        if(m_nowGameMode == GameMode.Battle)
        { Cursor.visible = true; }
        else { Cursor.visible = false; }
    }
}

// =================== enum ==================== //
public enum GameMode
{
    Free,
    Battle,
    RunAway,
    Scene,
    GameOver,
    Pause,
}

public enum ItemName
{
    Herb,
    GreatHerb,
    Smoke,
    ItemMax
}

public enum CharType
{
    Player,
    Enemy,
    None
}