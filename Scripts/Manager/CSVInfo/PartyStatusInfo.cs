using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PartyStatusInfo
{
    private static TextAsset m_csvFile;
    private static List<string[]> m_data = new List<string[]>();
    private const string fileName = "PartyStatusInfo";

    private int[] m_id = new int[MySystem.PARTYTYPE];           // �G�l�~�[�̎푰�ʔԍ�([�푰��])
    private string[] m_name = new string[MySystem.PARTYTYPE];   // �G�l�~�[�̖��O
    private int[] m_lv = new int[MySystem.PARTYTYPE];       // �p�[�e�B�Q�����̍Œ჌�x��
    private int[] m_hpMag = new int[MySystem.PARTYTYPE];    // Hp
    private int[] m_atMag = new int[MySystem.PARTYTYPE];    // �U����
    private int[] m_prMag = new int[MySystem.PARTYTYPE];    // �h���
    private int[] m_agMag = new int[MySystem.PARTYTYPE];    // ���΂₳
    private int[] m_expMag = new int[MySystem.PARTYTYPE];   // �o���l
    private float[] m_walk = new float[MySystem.PARTYTYPE];     // �������x
    private float[] m_run = new float[MySystem.PARTYTYPE];      // ���鑬�x
    private float[] m_rot = new float[MySystem.PARTYTYPE];      // ��]���x
    private float[] m_turn = new float[MySystem.PARTYTYPE];     // �U��������x

    public int[] id { get { return m_id; } }
    public string[] name { get { return m_name; } }
    public int[] level { get { return m_lv; } }
    public int[] hpMag { get { return m_hpMag; } }
    public int[] atMag { get { return m_atMag; } }
    public int[] prMag { get { return m_prMag; } }
    public int[] agMag { get { return m_agMag; } }
    public int[] expMag { get { return m_expMag; } }
    public float[] walk { get { return m_walk; } }
    public float[] run { get { return m_run; } }
    public float[] rot { get { return m_rot; } }
    public float[] turn { get { return m_turn; } }

    /// <summary>
    /// 2022/02/28
    /// CSV�t�@�C����ǂݍ��ފ֐�
    /// </summary>
    /// <param name="fileName"></param>
    private static void CsvReader()
    {
        m_csvFile = Resources.Load("CSV/" + fileName) as TextAsset;
        StringReader reader = new StringReader(m_csvFile.text);
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            m_data.Add(line.Split(','));
        }
    }


    public void Init()
    {
        CsvReader();

        for (int y = 1; y < m_data.Count; y++)
        {

            int i = y - 1;
            int x = 0;

            m_id[i] = int.Parse(m_data[y][x++]);
            m_name[i] = m_data[y][x++];
            m_lv[i] = int.Parse(m_data[y][x++]);
            m_hpMag[i] = int.Parse(m_data[y][x++]);
            m_atMag[i] = int.Parse(m_data[y][x++]);
            m_prMag[i] = int.Parse(m_data[y][x++]);
            m_agMag[i] = int.Parse(m_data[y][x++]);
            m_expMag[i] = int.Parse(m_data[y][x++]);

            m_walk[i] = float.Parse(m_data[y][x++]);
            m_run[i] = float.Parse(m_data[y][x++]);
            m_rot[i] = float.Parse(m_data[y][x++]);
            m_turn[i] = float.Parse(m_data[y][x++]);
        }
    }
}
