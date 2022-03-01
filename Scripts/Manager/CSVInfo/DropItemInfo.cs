using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DropItemInfo
{
    private static TextAsset m_csvFile;
    private static List<string[]> m_data = new List<string[]>();
    private const string fileName = "DropItemInfo";

    public int[] m_id = new int[MySystem.ITEMTYPE];         // �A�C�e���̎�ޔԍ�([�A�C�e���̎�ޕ�])
    public string[] m_name = new string[MySystem.ITEMTYPE]; // �A�C�e����
    private int[,] m_rate = new int[MySystem.ITEMTYPE, MySystem.ENEMYTYPE];    // ���藦 ([�A�C�e����,�푰��])


    public int[,] rate { get { return m_rate; } }   // rate[�A�C�e���ԍ�(���ʂȂ�2), �푰�ԍ�(�X�P���g���Ȃ�0)]�Œl�擾

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

            for (int j = 0; x < m_data[0].Length; j++)
            { m_rate[i, j] = int.Parse(m_data[y][x++]); }
        }
    }
}
