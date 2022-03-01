using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region �ϐ�

    // �o�g���Ǘ��n
    public const int COMMANDNUM = (int)BattleCommand.None; // �퓬�R�}���h�� 
    private const int BATTLECHAR = 2;           // �����ɐ퓬�̏�ɗ��L�����̐�
    private const float MAXRUNAWAYTIME = 3.0f;  // ���������G�l�~�[���ҋ@���鎞��
    

    private int m_turnNum;            // �^�[����
    private bool m_battleStart;       // �o�g���̏��߂�
    private float m_runAwayTimer;     // �����Ă���̎���

    public int turnNum
    {
        get { return m_turnNum; }
        set { m_turnNum += value; }
    }
    public bool BattleStart { get { return m_battleStart; } }


    // �_���[�W�v�Z�n
    private int m_callAttack;  // �U�����Ă΂ꂽ�񐔂��L�^
    private int m_callProtect; // �h�䂪�Ă΂ꂽ�񐔂��L�^
    private int m_callMiss;    // �~�X���Ă΂ꂽ�񐔂��L�^


    private bool m_damageCalcuration; // �_���[�W�v�Z��(Attack + Protect�̏ꍇ�J�E���^�[���I�������false)
    private bool m_oneMoreCommand;    // ������x�R�}���h�����s���邩

    private int m_playerAssign;       // �v���C���[�̕t���l
    private int m_enemyAssign;    // �G�l�~�[�̕t���l
    private CharType m_counter;

    public bool damageCalcuration { get { return m_damageCalcuration; } }
    public bool oneMoreCommand { get { return m_oneMoreCommand; } }
    public CharType counter { get { return m_counter; } }

    

    // �V�X�e���n
    private MySystem m_mySystem;            // �V�X�e��
    private Command m_commandScript;        // �R�}���h
    private Timing m_timing;                // �^�C�~���O
    private ResultManager m_resultMana;     // ���U���g
    private CameraScript m_cameraScript;    // �J����
    private EnemyBaseScript m_battleEnemy;  // �G
    private PartyStatusBase m_battlePlayer; // �v���C���[
    private BattleMode m_nowBattleMode;       // ���݂̃o�g�����[�h(�I�𒆂����s������)
    private BattleCommand m_nowBattleCommand; // ���ݑI�𒆂̃R�}���h(�U�����h�䂩)

    public MySystem mySystem { get { return m_mySystem; } }
    public Timing timing { get { return m_timing; } }
    public EnemyBaseScript enemy
    {
        get { return m_battleEnemy; }
        set { m_battleEnemy = value; }
    }
    public PartyStatusBase player
    {
        get { return m_battlePlayer; }
        set { m_battlePlayer = value; }
    }
    public BattleMode battleMode
    {
        get { return m_nowBattleMode; }
        set { m_nowBattleMode = value; }
    }
    public BattleCommand battleCommand
    {
        get { return m_nowBattleCommand; }
        set { m_nowBattleCommand = value; }
    }

    #endregion



    #region �֐�

    /// <summary>
    /// 2021/10/14
    /// �o�g�����[�h�ɑJ�ڂ����Ƃ��̏���
    /// </summary>
    public void BattleModeFunc()
    {
        player.transform.LookAt(enemy.transform);
        enemy.transform.LookAt(player.transform);

        Vector3 partyPos = player.transform.position;
        Vector3 enemyPos = enemy.transform.position;

        // �J�������o�g�����̈ʒu�ɒ�������(battleStart�ɑ��)
        if (m_cameraScript.BattleRotate(partyPos, enemyPos))
        {
            m_battleStart = false;
            m_commandScript.BattleCommandFunc(); 
        }
        else
        {
            m_battleStart = true;
        }

        if(battleMode == BattleMode.Result) { m_resultMana.ResultFunc(); }
    }

    #region �o�g���R�}���h���s�n

    /// <summary>
    /// 2022/02/21
    /// �U���R�}���h���s���ɌĂԊ֐�
    /// </summary>
    /// <param name="charType">�������G��</param>
    /// <param name="assign">�U����</param>
    public void Attack(int charType, int assign)
    {
        m_callAttack++;

        // �L�����d����
        switch (charType)
        {
            case (int)CharType.Player : m_playerAssign = assign; break;
            case (int)CharType.Enemy : m_enemyAssign = assign; break;
        }

        // �ǂ����������R�}���h���g������
        if(m_callAttack == BATTLECHAR)
        {
            // ���ꂼ��Ɍv�Z���ʂ̃_���[�W��t�^
            m_battleEnemy.currentHp -= m_playerAssign;
            m_battlePlayer.currentHp -= m_enemyAssign;

            // �ǂ�����������ł���ΘA���R�}���h�͖���
            if (m_battleEnemy.currentHp <= 0 || m_battlePlayer.currentHp <= 0)
            { m_oneMoreCommand = false; }

            else { m_oneMoreCommand = true; }

            m_damageCalcuration = false;
            m_callAttack = 0;


            m_callAttack = m_callProtect = m_callMiss = 0;
            m_enemyAssign = m_playerAssign = 0;
        }

        else
        { Other(); }
    }



    /// <summary>
    /// 2022/02/21
    /// �h��R�}���h���s���ɌĂԊ֐�
    /// </summary>
    /// <param name="charType">�������G��</param>
    /// <param name="assign">�h���</param>
    public void Protect(int charType, int assign)
    {
        m_callProtect++;

        // �L�����d����
        switch (charType)
        {
            case (int)CharType.Player: 
                m_playerAssign = assign;
                m_counter = (int)CharType.Player;
                break;

            case (int)CharType.Enemy: 
                m_enemyAssign = assign;
                m_counter = CharType.Enemy;
                break;
        }

        // �ǂ����������R�}���h��I�����Ă���Ƃ�
        if (m_callProtect == BATTLECHAR)
        { Initialized(); }

        else
        { Other(); }
    }



    /// <summary>
    /// 2022/02/21
    /// �~�X���s���ɌĂԊ֐�
    /// �U���͓��̓R�}���h�I�����ɖ���0���n����邩��
    /// ������Ȃ��Ă悢�B
    /// </summary>
    public void Miss()
    {
        m_callMiss++;

        if(m_callMiss == BATTLECHAR)
        { Initialized(); }

        else
        { Other(); }
    }



    /// <summary>
    /// 2022/02/28
    /// �J�E���^�[���s���ɌĂԊ֐�
    /// </summary>
    public void Counter()
    {
        if(m_counter == CharType.Player)
        { m_battleEnemy.currentHp -= DamageCalculation(m_playerAssign, 0); }

        else
        { m_battlePlayer.currentHp -= DamageCalculation(m_enemyAssign, 0); }
       
        

        Initialized();
    }


    /// <summary>
    /// 2022/02/21
    /// �������Ⴄ�R�}���h��I�������Ƃ�
    /// �Е����R�}���h�����s�ł��Ă��Ȃ��ꍇ�A�ʂ�߂���B
    /// </summary>
    /// <param name="charType">�������G��</param>
    /// <param name="assign">�h���</param>
    private void Other()
    {
        // �U���Ɩh�䂪�Ă΂�Ă�����܂���
        // �U���ƃ~�X���Ă΂�Ă�����
        // (�Е���)
        if ((m_callAttack + m_callProtect) == BATTLECHAR ||
           (m_callAttack + m_callMiss) == BATTLECHAR)
        {
            m_battlePlayer.currentHp -= DamageCalculation(m_enemyAssign, m_playerAssign);
            m_battleEnemy.currentHp -= DamageCalculation(m_playerAssign, m_enemyAssign);

            if(m_callProtect == 0)
            { Initialized(); }
        }

        else if ((m_callProtect + m_callMiss) == BATTLECHAR)
        { Initialized(); } 
    }



    /// <summary>
    /// 2022/02/21
    /// �v�Z�������Â炢����֐���
    /// �h��ɂ���ă_���[�W��0�Ȃ�Œ�1
    /// </summary>
    /// <param name="attack">�U�����鑤�̍U����</param>
    /// <param name="protect">�h�䂷�鑤�̖h���</param>
    /// <returns>�v�Z����</returns>
    private int DamageCalculation(int attack, int protect)
    {
        // �h�䂵�Ă��Ȃ��Ƃ�
        if (protect >= 0) { return attack; }

        // �U���͂��h��͂��������Ƃ�
        else if ((attack + protect) <= 1) { return 1; }

        // �U���͂Ɩh��͂𑫂���2�ȏ�̎�
        else { return attack + protect; }
    }

    #endregion

    // ********************************************* //
    /// <summary>
    /// 2021/12/12
    /// �U����h�䂵���Ƃ��ɌĂяo���֐��B
    /// </summary>
    /// <param name="genus">�Ăяo�����Ƃ̎푰</param>
    /// <param name="protect">�Ăяo�����Ƃ̖h���</param>
    public void ActionAgain(int charType, int protect)
    {
        // �G�l�~�[�̃��x�����Ⴂ�Ɩh��͂�0�̉\�������邩��m�F
        protect += (protect == 0) ? 1 : 0;

        // �v���C���[����������G�l�~�[�Ƀ_���[�W
        switch(charType)
        {
            case (int)CharType.Player: m_battleEnemy.currentHp -= protect; break;
            case (int)CharType.Enemy: m_battlePlayer.currentHp -= protect; break;
        }

        Initialized();
    }




    // ********************************************* //
    /// <summary>
    /// 2022/02/27
    /// �_���[�W�v�Z�Ŏg�p�����ϐ��Ȃǂ̏�����
    /// </summary>
    private void Initialized()
    {
        m_nowBattleMode = BattleMode.Command;
        m_nowBattleCommand = BattleCommand.None;

        m_callAttack = m_callProtect = m_callMiss = 0;
        m_enemyAssign = m_playerAssign = 0;
        
        
        m_turnNum = 0;
        m_damageCalcuration = false;
        m_oneMoreCommand = false;
        m_counter = CharType.None;
    }

    #endregion



    // ============= Start �E Update =============== //
    void Start()
    {
        m_nowBattleMode = BattleMode.Command;
        m_nowBattleCommand = BattleCommand.None;

        m_mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        m_commandScript = GameObject.Find("Command").GetComponent<Command>();
        m_timing = GameObject.Find("Timing").GetComponent<Timing>();
        m_cameraScript = GameObject.Find("CameraLoot").GetComponent<CameraScript>();
        m_resultMana = GameObject.Find("GameManager").GetComponent<ResultManager>();
        m_battlePlayer = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    void Update()
    {
        // �Q�[�����[�h���o�g������
        // �o�g�����[�h�����U���g�ł͂Ȃ��B
        if(mySystem.gameMode == GameMode.Battle &&
           m_nowBattleMode != BattleMode.Result)
        {
            if (m_battleEnemy.currentHp <= 0 || m_battlePlayer.currentHp <= 0) { m_nowBattleMode = BattleMode.Result; }
            BattleModeFunc();
        }
        


        if(m_nowBattleMode == BattleMode.Result) { StartCoroutine(m_resultMana.ResultFunc()); m_battleStart = true; }

        else if(m_mySystem.gameMode == GameMode.RunAway) 
        {
            Initialized();
            m_battleStart = true;

            // �R�}���h�œ������ꍇ�w�莞�ԂɂȂ�܂ŁAEnemy�̍s�����~�߂�B
            m_runAwayTimer += Time.deltaTime; 
            if(m_runAwayTimer >= MAXRUNAWAYTIME)
            { m_mySystem.gameMode = GameMode.Free; m_runAwayTimer = 0.0f; }
        }
    }
}

// =================== enum ==================== //
public enum BattleMode
{
    Command,        // �R�}���h�I��
    Execution,      // �Z���s��
    Result,         // �o�g���I��������
    None,           // ���蓖�ĂȂ�
}

public enum BattleCommand
{
    Attack,     // �U���R�}���h
    Protect,    // �h��R�}���h
    Item,       // �A�C�e���R�}���h
    RunAway,    // ������R�}���h
    None,       // ���蓖�ĂȂ�
}