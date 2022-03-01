using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �G�l�~�[�̊�{���̌p����
/// MonoBehaviour�ɑS�G�l�~�[�����ł��낤
/// �X�e�[�^�X���ǉ�����邾���B
/// </summary>
public abstract class EnemyBaseScript : StatusBase
{
    #region �ϐ�
    protected int m_id;
    protected int m_exp;
    protected float m_searchRange;  // ���G�͈�
    protected float m_battleRange;  // �퓬�͈�
    protected int m_itemGetRate;    // �A�C�e���擾�m��
    protected int m_maxNumItem;     // �ő�擾�A�C�e����
    protected int[] m_item;         // �擾�����A�C�e��

    // �o�g�����̋���
    protected bool m_activeAction;  // �G�l�~�[���A�N�V�������N�����Ă��邩(�N�����Ă����true)

    protected NavMeshAgent m_agent;       // AI
    protected EnemyStatusInfo info;

    public int exp { get { return m_exp; } }
    public int[] item { get { return m_item; } }
    public NavMeshAgent Agent
    {
        get { return m_agent; }
    }

    #endregion



    #region �X�e�[�^�X�n
    /// <summary>
    /// 2022/12/15
    /// �G�l�~�[�̑S�Ă̊�b�X�e�[�^�X��ݒ肷��֐�
    /// level = �v���C���[�̃��x�� + �Z�`�Z�̒l (�Œ�1)
    /// hp = level * �Z�`�Z�̒l (�Œ�1)
    /// at = level * �Z�`�Z�̒l (�Œ�1)
    /// pr = attack * �Z.�Z�`�Z.�Z�̒l�i�Œ�1,�l�̌ܓ��j
    /// ag = level * �Z.�Z�`�Z.�Z�̒l�i�Œ�1,�l�̌ܓ��j
    /// exp = level ; �Z�`�Z�̒l�i�Œ�0,�l�̌ܓ��j
    /// </summary>
    /// <param name="enemyId">���̃G�l�~�[�ԍ�</param>
    protected override void SetAllStatus(int id)
    {
        info = m_mySystem.eStatusInfo;

        #region m_level
        m_level = m_mySystem.battleMana.player.level + m_mySystem.GetRandomValue(info.lvMag[id, 0], info.lvMag[id, 1]);
        if (m_level <= 0)
        { m_level = 1; }
        #endregion
        m_currentHp = m_maxHp = m_level * m_mySystem.GetRandomValue(info.hpMag[id, 0], info.hpMag[id, 1]);
        m_attack = m_level * m_mySystem.GetRandomValue(info.atMag[id, 0], info.atMag[id, 1]);
        m_protect = Mathf.RoundToInt((float)m_attack * (float)(m_mySystem.GetRandomValue(info.prMag[id, 0], info.prMag[id, 1]) / 10)); // ����10
        m_agility = Mathf.RoundToInt((float)m_level * (float)(m_mySystem.GetRandomValue(info.agMag[id, 0], info.agMag[id, 1]) / 10)); // ����10
        m_exp = m_level * m_mySystem.GetRandomValue(info.expMag[id, 0], info.expMag[id, 1]);

        m_walk = info.walk[id];
        m_run = info.run[id];
        m_rot = info.rot[id];
        m_turn = info.turn[id];
        m_searchRange = info.searchRange[id];
        m_battleRange = info.battleRange[id];

        #region ��{�K�{�X�e�[�^�X��0�ɂȂ��Ȃ��悤�ɂ��邯�ǃ`�F�b�N
        if (m_maxHp <= 0)
        { m_maxHp = 1; }

        if (m_attack <= 0)
        { m_attack = 1; }

        if (m_protect <= 0)
        { m_protect = 1; }

        if (m_agility <= 0)
        { m_agility = 1; }

        if (m_exp < 0)
        { m_exp = 0; }
        #endregion


    }



    /// <summary>
    /// 2022/02/09
    /// </summary>
    /// <returns>�擾�A�C�e����</returns>
    private int GetItemNum()
    {
        // �A�C�e���擾�����傫����Ύ擾�A�C�e��0
        if (m_itemGetRate < m_mySystem.GetRandomValue())
        { return 0; }


        // �擾�A�C�e����
        int getItemNum = 0;


        // �ő�擾�A�C�e������
        // for���Ђ����璊�I(�A�C�e���擾�\�ł�0�̉\���͂���)
        for (int i = 0; i < m_maxNumItem; i++)
        {
            if (m_itemGetRate > m_mySystem.GetRandomValue())
            {
                // �擾�\�A�C�e����++
                getItemNum++;
                continue;
            }

            break;
        }

        // �擾�A�C�e����
        return getItemNum;
    }



    /// <summary>
    /// 2022/02/18
    /// �A�C�e�����擾����֐�
    /// </summary>
    private void GetItem()
    {
        // �A�C�e���擾��
        m_item = new int[GetItemNum()];

        // �擾�\��A�C�e���ň�ԒႢ�m���̃A�C�e���ԍ�
        int rateComparison = 0;
        int randVal = 0;



        for (int i = 0; i < m_item.Length; i++)
        {
            randVal = m_mySystem.GetRandomValue(1, MySystem.PERCENT + 1);

            // �S�A�C�e����
            for (int j = 0; j < MySystem.ITEMTYPE; j++)
            {
                // j�Ԗڂ̃A�C�e���̊m���������ȏ� && j�Ԗڂ̃A�C�e�������̃A�C�e���̊m����菬�������
                if (m_mySystem.dropItemInfo.rate[j, m_id] >= randVal &&
                    m_mySystem.dropItemInfo.rate[rateComparison, m_id] >= m_mySystem.dropItemInfo.rate[j, m_id])
                {
                    // ���Œ�m����j�Ԗڂ̃A�C�e���̊m���������Ȃ痐������2��"����؂�Ȃ�������"�R���e�B�j���[
                    if (m_mySystem.dropItemInfo.rate[rateComparison, m_id] == m_mySystem.dropItemInfo.rate[j, m_id] &&
                       randVal / 2 != 0)
                    { continue; }

                    rateComparison = j;
                }
            }

            // �ŏI�I�Ɉ�ԒႩ�����A�C�e��(�ԍ�)���擾
            m_item[i] = rateComparison;
            rateComparison = 0;
            randVal = 0;
        }
    }

    #endregion


    /// <summary>
    /// 2022/02/09
    /// �v���C���[��ǔ����ă��[�h��ς���֐�
    /// </summary>
    protected void SearchPlayersInSight(float searchRange, float battleRange)
    {
        Vector3 pPos = m_battleMana.player.transform.position;
        Vector3 ePos = transform.position;
        float dis = Vector3.Distance(pPos, ePos);

        // �ǐՔ͈͂Ȃ�
        if(dis < searchRange)
        {
            m_agent.destination = pPos;
            m_agent.speed = m_walk;
        }
        else
        { m_agent.speed = 0.0f; }

        // �o�g���͈͂Ȃ�
        if(dis < battleRange)
        {
            m_agent.speed = 0.0f;
            m_mySystem.gameMode = GameMode.Battle;
            m_battleMana.enemy = this;
        }
    }

    
    // ********************************************* //

    /// <summary>
    ///  2021/12/06
    /// �G�l�~�[�̃A�N�V����(�R�}���h)����
    /// </summary>
    protected abstract IEnumerator ActionStart();




    // ============= Start �E Update =============== //
    protected override void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();

        SetAllStatus(m_id);
        m_activeAction = false;
    }

    protected override void Update()
    {
    }
}
