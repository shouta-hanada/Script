using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DropItemInfo
{
    private static TextAsset m_csvFile;
    private static List<string[]> m_data = new List<string[]>();
    private const string fileName = "DropItemInfo";

    public int[] m_id = new int[MySystem.ITEMTYPE];         // アイテムの種類番号([アイテムの種類分])
    public string[] m_name = new string[MySystem.ITEMTYPE]; // アイテム名
    private int[,] m_rate = new int[MySystem.ITEMTYPE, MySystem.ENEMYTYPE];    // 入手率 ([アイテム数,種族数])


    public int[,] rate { get { return m_rate; } }   // rate[アイテム番号(煙玉なら2), 種族番号(スケルトンなら0)]で値取得

    /// <summary>
    /// 2022/02/28
    /// CSVファイルを読み込む関数
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
