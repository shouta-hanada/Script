using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeltonScript : EnemyBaseScript
{
    // =================== �ϐ� ==================== //

    private readonly int[] m_itemRate = new int[3]
    {
        60, // �� 
        30, // ������
        10  // ����
    };  // �擾�A�C�e���̓��藦
    private enum ItemName { Herb, GreateHerb, Smoke }
    protected enum ActionPattern
    {
        Attack,
        Protect,
        Miss,
        None
    }


    // ================= function ================== //



    /// <summary>
    /// 2022/02/27
    /// int�l����ActionPattern�̒l�ɕϊ�����֐�
    /// </summary>
    /// <param name="num">ActionPattern���̒l</param>
    /// <returns>�����̒l���R�}���h�l�łȂ����None��Ԃ�</returns>
    private ActionPattern ConvertIntToEnumActionPattern(int num)
    {
        // int����string�ɕϊ�
        foreach (int i in System.Enum.GetValues(typeof(ActionPattern)))
        {
            // ������ꂽ�R�}���h�ԍ���Enum�̃R�}���h�ԍ�����v
            // �R�}���h���̑���ƃ}�l�[�W���[�̕ϐ��̒l��ύX
            if (num == i)
            { return (ActionPattern)System.Enum.ToObject(typeof(ActionPattern), i); }
        }

        return ActionPattern.None;
    }

    /// <summary>
    /// 2022/02/27
    /// </summary>
    protected override IEnumerator ActionStart()
    {
        m_activeAction = true;

        // �v���C���[�̃^�C�~���O�������I�����1�b���Ă�����s
        yield return new WaitForSeconds(1f);

        // ����������
        int hitRate = m_mySystem.GetRandomValue(0, (int)ActionPattern.None);
        string commandName = ConvertIntToEnumActionPattern(hitRate).ToString();
        ActionPattern command = ConvertIntToEnumActionPattern(hitRate);



        // �A�j���[�V�����J�n
        // �A�j���[�V�����I���܂őҋ@
        // �_���[�W�v�Z
        myAnim.SetBool(commandName, true);
        yield return new WaitWhile(() => m_battleMana.player.activeAnim == commandName);

        Debug.Log(commandName);

        // �R�}���h�̎��s
        switch (command)
        {
            case ActionPattern.Attack: m_battleMana.Attack((int)CharType.Enemy, attack); break;
            case ActionPattern.Protect: m_battleMana.Protect((int)CharType.Enemy, protect); break;
            case ActionPattern.Miss: m_battleMana.Miss(); break;
        }


        // �R�}���h��J�E���^�[���\�ȏꍇ�J�E���^�[
        if (m_battleMana.counter == CharType.Enemy)
        {
            myAnim.SetBool(ActionPattern.Attack.ToString(), true);
            yield return new WaitWhile(() => activeAnim == BattleCommand.Attack.ToString());

            m_battleMana.Counter();
        }

        m_activeAction = false;
    }

    // ����G�l�~�[����̍U���Ȃǂ͂Ȃ�����A
    // EnemyBase��Start��Update���ĂԂ���
    // ============= Start �E Update =============== //
    protected override void Start()
    {
        m_id = 0;
        m_itemGetRate = 50;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // �R�}���h���s������
        // �^�C�~���O���I��
        if(m_battleMana.battleMode == BattleMode.Execution &&
           m_battleMana.timing.timingEnd && !m_activeAction)
        { StartCoroutine(ActionStart());  }

        // �Q�[�����[�h���t���[�Ȃ�T��
        // �����łȂ����NavMesh�̓������~�B
        if (m_mySystem.gameMode == GameMode.Free)
        { SearchPlayersInSight(m_searchRange, m_battleRange); }
        else
        { m_currentSpeed = 0.0f; }

        // NavMesh�ňړ����Ă���΃A�j���[�V����
        m_myAnim.SetFloat("Walk", m_currentSpeed / m_run);
    }
}
