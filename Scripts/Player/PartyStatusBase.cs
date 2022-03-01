using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PartyStatusBase : StatusBase
{
    #region �ϐ�

    protected int m_id;
    protected int m_maxExp;      // �o���l�̓��ꕨ
    protected int m_currentExp;  // �o���l
    protected int m_hitRate;     // ������

    private const float m_sliderSpeed = 0.5f; // �X���C�_�[�̑������x

    private bool m_guard; // �K�[�h���ł�����

    private Slider m_hpSlider;  // HP�̃X���C�_�[
    private Slider m_expSlider; // �o���l�̃X���C�_�[
    private Text m_levelText;   // �X�e�[�^�XUI

    protected Camera m_mainCamera;  // �J����

    private PartyStatusInfo info;

    public int currentExp
    {
        get { { return m_currentExp; } }
        set { { m_currentExp += value; } }
    }
    public int hitRate { get { return m_hitRate; } }
    public bool guard
    { 
        get { return m_guard; } 
        set { m_guard = value; }
    }

    #endregion



    #region �֐�

    #region �ړ��n

    /// <summary>
    /// 2021/11/22
    /// �J��������ɕ���������؂�ւ���֐��B
    /// </summary>
    /// <param name="camera">�ڕW</param>
    protected void SwitchDirWalk(GameObject camera)
    {
        float speed = m_turn * Time.deltaTime;
        Vector3 dir;
        Quaternion target = transform.rotation;
        Vector3 camV = camera.transform.forward;

        // �����̐ݒ� ��������̐ݒ�
        if (Input.GetKey(KeyCode.W))
        {
            dir = new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(dir);
        }

        if (Input.GetKey(KeyCode.S))
        {
            dir = new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(-dir);
        }

        if (Input.GetKey(KeyCode.A))
        {
            dir = Quaternion.Euler(0, 90, 0) * new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(-dir);
        }

        if (Input.GetKey(KeyCode.D))
        {
            dir = Quaternion.Euler(0, 90, 0) * new Vector3(camV.x, 0, camV.z);
            target = Quaternion.LookRotation(dir);
        }


        // ���̃X�s�[�h�Ńv���C���[��Y����]��ύX
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, speed);
    }



    /// <summary>
    /// 2021/11/22
    /// �����A����̋������s���֐��B
    /// W + Shift�̎��͑���B
    /// W,A,S,D��������Ă���Ƃ��͕����B
    /// WASD��������Ă��Ȃ��A�����Ă���Ԃ͌����B
    /// </summary>
    protected void WalkAndRun()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) ||
           Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.RightShift))
        {
            if (m_currentSpeed <= m_run)
            { m_currentSpeed += m_run * Time.deltaTime; }
        }

        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (m_currentSpeed <= m_walk)
            { m_currentSpeed += m_walk * Time.deltaTime; }
        }

        else
        {
            if (Mathf.Abs(m_currentSpeed) > 0.1f)
            { m_currentSpeed = -m_run * Time.deltaTime; }
            else
            { m_currentSpeed = 0.0f; }
        }
    }

    #endregion



    #region �X�e�[�^�X�n
    protected override void SetAllStatus(int id)
    {
        info = m_mySystem.pStatusInfo;

        if(m_level < info.level[id])
        { m_level = info.level[id]; }
        m_currentHp = m_maxHp = m_level * info.hpMag[id];
        m_attack = m_level * info.atMag[id];
        m_protect = Mathf.RoundToInt((float)m_attack * (float)info.prMag[id] / 10); // ����10
        m_agility = Mathf.RoundToInt((float)m_level * (float)info.agMag[id] / 10); // ����10
        m_maxExp = m_level * info.expMag[id];

        m_walk = info.walk[id];
        m_run = info.run[id];
        m_rot = info.rot[id];
        m_turn = info.turn[id];

        #region ��{�K�{�X�e�[�^�X��0�ɂȂ��Ȃ��悤�ɂ��邯�ǃ`�F�b�N
        if (m_maxHp <= 0)
        { m_maxHp = 1; }

        if (m_attack <= 0)
        { m_attack = 1; }

        if (m_protect <= 0)
        { m_protect = 1; }

        if (m_agility <= 0)
        { m_agility = 1; }
        #endregion
    }



    /// <summary>
    /// 2022/02/13
    /// �o���l���ő�ɂȂ��Ă��邩�m�F����֐��B
    /// ���x���A�b�v��̃X�e�[�^�X�ݒ���s���B
    /// </summary>
    protected void CheckLevelUp()
    {
        if(m_currentExp >= m_maxExp && m_level < m_maxLevel)
        {
            m_expSlider.value = 0.0f;
            m_currentExp = m_currentExp - m_maxExp;
            SetAllStatus(m_id);
        }
    }



    /// <summary>
    /// 2022/02/13
    /// Slider��value���ϓ�����֐�
    /// </summary>
    /// <param name="slider">�ϓ���������Slider</param>
    /// <param name="nume">�ϓ���̒l</param>
    /// <param name="deno">�l�̍ő�l(����)</param>
    private void TransSliderVal
        (Slider slider, int nume , int deno)
    {
        float sVal = slider.value;
        float dVal = (float)nume / (float)deno;
        float dSpeed = m_sliderSpeed * Time.deltaTime;

        slider.value = Mathf.MoveTowards(sVal, dVal, dSpeed);
    }

    #endregion



    #endregion

    // ============= Start �E Update =============== //
    protected override void Start()
    {
        m_mainCamera = Camera.main;
        m_hpSlider = transform.Find("ThisCanvas/HP").GetComponent<Slider>();
        m_expSlider = transform.Find("ThisCanvas/EXP").GetComponent<Slider>();
        m_levelText = transform.Find("ThisCanvas/Lv").GetComponent<Text>(); 


        m_hpSlider.value = currentHp;
        m_expSlider.value = currentExp;
    }

    protected override void Update()
    {
        TransSliderVal(m_hpSlider, m_currentHp, m_maxHp);
        TransSliderVal(m_expSlider, m_currentExp, m_maxExp);

        CheckLevelUp();

        m_levelText.text = "Lv." + m_level.ToString();

        m_myAnim.SetFloat("Walk", m_currentSpeed/m_run);
    }


    // �p�[�e�B�ɉ����L�����̎푰(�X�e�[�^�XID�p)
    protected enum PartyID
    {
        Skelton,
    }
}
