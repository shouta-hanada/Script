using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �X�e�[�^�X�̒��ۃN���X
// �v���C���[���G�l�~�[�ɂ��K�v�ƂȂ�ϐ��Ȃǂ�ۗL
// Get��Set�𕪗����Ă���̂̓L�����ɂ���ăX�e�[�^�X�̕ϓ����̋������Ⴄ���߁B
public abstract class StatusBase : MonoBehaviour
{
    #region �ϐ�

    // �퓬���X�e�[�^�X
    protected const int m_maxLevel = 100;   // �ő僌�x��
    [SerializeField]protected int m_level;      // ���x��
    [SerializeField]protected int m_maxHp;      // �ő�HP
    [SerializeField]protected int m_currentHp;  // ���݂�HP
    [SerializeField]protected int m_attack;     // �U����
    [SerializeField]protected int m_protect;    // �h���
    [SerializeField]protected int m_agility;    // �f����

    public int level { get { return m_level; } }
    public int maxHp { get { return m_maxHp; } }
    public int currentHp
    {
        get { return m_currentHp; }
        set { m_currentHp += value; }
    }
    public int attack { get { return m_attack; } }
    public int protect { get { return m_protect; } }
    public int agility { get { return m_agility; } }



    // �T�����X�e�[�^�X
    protected float m_walk;         // �������x
    protected float m_run;          // ���鑬�x
    protected float m_currentSpeed; // ���݂̑��x
    protected float m_rot;          // ��]���x
    protected float m_turn;         // �U��������x

    public float walk { get { return m_walk; } }
    public float run { get { return m_run; } }
    public float currentSpeed { get { return m_currentSpeed; } }
    public float rot { get { return m_rot; } }
    public float turn { get { return m_turn; } }


    // �V�X�e��
    protected MySystem m_mySystem;          // �V�X�e��
    protected BattleManager m_battleMana;   // �o�g���Ǘ�
    


    // �A�j���[�V����
    protected Animator m_myAnim;    // �A�j���[�^�[

    public string activeAnim { get { return m_myAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name; } }
    public Animator myAnim { get { return m_myAnim; } }

    #endregion


    protected abstract void SetAllStatus(int id);   // StatusBase�̃X�e�[�^�X�l��S�Đݒ肷��֐�

    /// <summary>
    /// 2022/02/13
    /// �ڕW�Ɍ������Ă������U������֐�
    /// </summary>
    /// <param name="target">�ڕW�̃I�u�W�F�N�g</param>
    protected void LookAtTarget(GameObject target)
    {
        float speed = m_turn * Time.deltaTime;
        Vector3 dir = target.transform.position - transform.position;
        Quaternion q = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speed);
    }



    /// <summary>
    /// 2022/03/01
    /// �Đ����̃u�[���A�j���[�V�������I��
    /// </summary>
    protected void BoolAnimationEnd()
    { m_myAnim.SetBool(activeAnim, false); }



    protected virtual void Awake()
    {
        m_mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        m_battleMana = GameObject.Find("GameManager").GetComponent<BattleManager>();
        m_myAnim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
