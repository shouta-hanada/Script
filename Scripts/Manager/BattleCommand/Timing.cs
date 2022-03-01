using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timing : FadeInOutBase
{
    #region �ϐ�

    // FadeInOutBase.childImg->0: BackGround, 1: Circle, 2: Target
    private const int CIRCLE = 1;   // chilImg�ɒǉ������Ƃ���Circle�I�u�W�F�N�g�̗v�f�ԍ�
    private Vector2 m_timingSize = new Vector2(500.0f, 500.0f);   // �^�C�~���O�̃T�C�Y


    private const float FADESPEAD = 5.0f;      // �t�F�[�h�X�s�[�h
    private const float MAXTIMINGTIME = 1.0f;  // �����^�C�~���O�̉�%��\���B1�Œ�
    private const float TIMINGSPEED = 0.7f;    // �^�C�~���O�����b�ŏk�����邩(n�{)
    private float m_shrinkSpeed;  // shirinkSpeed * Time.deltatime�����邱�Ƃ�1�b�Ń^�C�~���O�����ł��邽�߂̂���
    private float m_timingTimer;  // �����^�C�~���O�̉�%��\���B


    private const float EXCELLENTMIN = 0.43f;   // �G�N�Z�����g�͈͂̍ŏ��l 
    private const float EXCELLENTMAX = 0.53f;   // �G�N�Z�����g�͈͂̍ő�l
    private const float GOODMIN = 0.3f;         // �O�b�h�͈͂̍ŏ��l
    private const float GOODMAX = 0.7f;         // �O�b�h�͈͂̍ő�l


    public const int EXCELLENTHITRATE = 100;   // �G�N�Z�����g���̍U��������
    public const int GOODHITRATE = 90;         // �O�b�h���̍U��������
    public const int BADHITRATE = 60;          // �o�b�h���̍U��������
    private int m_hitRate; // ������

    public int hitRate { get { return m_hitRate; } }


    private bool m_timingEnd;     // �^�C�~���O�������I��������ǂ���

    public bool timingEnd { get { return m_timingEnd; } }

    #endregion



    #region �֐�

    /// <summary>
    /// 2022/02/27
    /// �U���^�C�~���O(���x�H)��UI��ݒ肷��֐��B
    /// </summary>
    public IEnumerator TimingRun()
    {
        // �^�C�~���O�����̎n�܂�ƁA
        // �t�F�[�h�̊J�n
        m_timingEnd = false;
        m_fadeStart = true;


        // �^�C�~���O����
        while(true)
        {
            m_timingTimer -= Time.deltaTime * TIMINGSPEED;

            CircleScaling();

            // Space���������Ƃ��A�^�C�~���O���J�n����ĉ��b���������Ŕ���
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (m_timingTimer > EXCELLENTMIN && m_timingTimer < EXCELLENTMAX)
                { m_hitRate = EXCELLENTHITRATE; }
                
                else if (m_timingTimer > EXCELLENTMAX && m_timingTimer < GOODMAX ||
                    m_timingTimer > GOODMIN && m_timingTimer < EXCELLENTMIN)
                { m_hitRate = GOODHITRATE; }
                
                else
                { m_hitRate = BADHITRATE; }

                break;
            }

            else if (m_timingTimer <= 0)
            {
                m_hitRate = 0;
                break;
            }

            yield return null;
        }

        m_fadeStart = true;
        yield return null;
        yield return new WaitWhile(() => !fadeClear);

        // �I������
        m_timingEnd = true;
        m_timingTimer = MAXTIMINGTIME;
        m_childImg[CIRCLE].rectTransform.sizeDelta = m_timingSize;
    }



    /// <summary>
    /// 2021/10/29
    /// �~���k��������֐�
    /// </summary>
    private void CircleScaling()
    {
        Vector2 sizeDelta = m_childImg[CIRCLE].rectTransform.sizeDelta;
        sizeDelta.x -= (m_shrinkSpeed * Time.deltaTime) * TIMINGSPEED;
        sizeDelta.y -= (m_shrinkSpeed * Time.deltaTime) * TIMINGSPEED;
        m_childImg[CIRCLE].rectTransform.sizeDelta = sizeDelta;
    }

    #endregion


    // ============= Start �E Update =============== //
    protected override�@void Start()
    {
        base.Start();

        m_fadeSpeed = FADESPEAD;
        m_shrinkSpeed = m_childImg[1].rectTransform.rect.width;
        m_timingTimer = MAXTIMINGTIME;
    }

    protected override void Update()
    {
        base.Update();
    }
}
