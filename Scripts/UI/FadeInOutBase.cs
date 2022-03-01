using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutBase : MonoBehaviour
{
    #region �ϐ�

    [SerializeField]protected List<Image> m_childImg = new List<Image>();
    protected bool m_fade;          // alpha�l��1�Ȃ�true
    protected bool m_pastFade;      // 1�t���[���O��fade
    protected bool m_fadeStart;     // �t�F�[�h���J�n���邩
    protected bool m_fadeClear;     // �t�F�[�h���������Ă��邩
    protected float m_alpha;        // ���l
    protected float m_fadeSpeed;    // �t�F�[�h����X�s�[�h

    public bool fade { get { return m_fade; } }
    public bool fadeStart { set { m_fadeStart = value; } }
    public bool fadeClear { get { return m_fadeClear; } }


    protected MySystem m_mySystem;

    #endregion



    #region �֐�

    /// <summary>
    /// 2022/02/27
    /// �t�F�[�h�C���A�E�g�����s����֐��B
    /// </summary>
    private void FadeInOut()
    {
        if (!fade) { m_alpha += m_fadeSpeed * Time.deltaTime; }
        else if (fade) { m_alpha -= m_fadeSpeed * Time.deltaTime; }

        for (int i = 0; i < m_childImg.Count; i++)
        {
            Color col = m_childImg[i].color;
            col.a = m_alpha;
            m_childImg[i].color = col;
        }
    }



    /// <summary>
    /// 2022/02/27
    /// Img��Ray�ɂ�铖���蔻���L���ɂ��邩�ۂ�
    /// </summary>
    /// <param name="enabled">true�œ����蔻�肪�t��</param>
    private void RaycastTargetEnabled(bool enabled)
    {
        for (int i = 0; i < m_childImg.Count; i++)
        { m_childImg[i].raycastTarget = enabled; }
    }



    /// <summary>
    /// 2022/02/27
    /// childImg�̗v�f���𑝂₷�֐�
    /// </summary>
    /// <param name="pastNum">���₷�O�̃V�[����̎q�̍ő吔</param>
    /// <param name="addNum">���݂̃V�[����̎q�̍ő吔</param>
    public void AddChildImg(int pastNum, int addNum)
    {
        for (int i = pastNum; i < pastNum + addNum; i++)
        {
            // �V�����ǉ����ꂽImage��GetComponent
            m_childImg.Add(transform.GetChild(i).GetComponent<Image>());
        }
    }



    /// <summary>
    /// 2022/02/28
    /// childImg�̒��͂�������
    /// [0]oyaObj
    /// [1]koObj
    /// [2]koObj_2
    /// [3]oyaObj_2
    /// [4]koObj
    /// [5]koObj_2
    /// �̂悤�ɂȂ��Ă���
    /// </summary>
    /// <param name="obj">List����O�������I�u�W�F�N�g</param>
    /// <param name="onChild">�I�u�W�F�N�g�ɂ��Ă�q�̐�(0�ł�����)</param>
    public void RemovedChildImg(Image obj, int onChild)
    {
        // �����[�u����\��̗v�f�ԍ�
        int removeNum = 0;

        // ���������I�u�W�F�N�g�̗v�f�ԍ����擾
        foreach (Image i in m_childImg)
        {
            // �����[�u�\���Image�ƈ�v�����甲����
            if (obj == i)
            { break; }

            removeNum++;
        }

        // ���������I�u�W�F�N�g�Ƃ��̎q���������Ă����B
        // �q�͐eImg�̎��̗v�f�ɂȂ�B
        for (int i = 0; i < onChild+1; i++)
        { m_childImg.RemoveAt(removeNum); }
    }

    #endregion



    // ============= Start �E Update =============== //
    private void Awake()
    {
        m_mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        GetComponentsInChildren(m_childImg);
    }



    protected virtual void Start()
    {
        // �������E������
        m_pastFade = m_fade = false;
        m_fadeStart = false;
        m_alpha = 0.0f;
        m_fadeSpeed = 10.0f;
        for(int i = 0; i < m_childImg.Count; i++)
        {
            Color col = m_childImg[i].color;
            col.a = m_alpha;
            m_childImg[i].color = col;
        }
    }



    protected virtual void Update()
    {
        // �s�����Ȃ�true
        if(m_alpha >= 1) { m_fade = true; RaycastTargetEnabled(true); }
        else if(m_alpha <= 0) { m_fade = false; RaycastTargetEnabled(false); }

        // fadeStart���Ă΂�āA�t�F�[�h��Ԃ��؂�ւ���ĂȂ���΃t�F�[�h
        if(m_fadeStart && m_pastFade == fade)
        { m_fadeClear = false; FadeInOut(); }
        
        else
        {
            m_fadeStart = false;
            m_pastFade = fade;
            m_fadeClear = true;
        }
    }
}
