using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 参考:https://wisteria-sophy.com/663/unity_basis9/#toc4


public class EnemyStatusInfo
{
    private static TextAsset m_csvFile;
    private static List<string[]> m_data = new List<string[]>();
    private const string fileName = "EnemyStatusInfo";

    private int[] m_id = new int[MySystem.ENEMYTYPE];           // エネミーの種族別番号([種族分])
    private string[] m_name = new string[MySystem.ENEMYTYPE];   // エネミーの名前
    private int[,] m_lvMag = new int[MySystem.ENEMYTYPE, 2];    // レベル([種族分,最低最大(2)])
    private int[,] m_hpMag = new int[MySystem.ENEMYTYPE, 2];    // Hp
    private int[,] m_atMag = new int[MySystem.ENEMYTYPE, 2];    // 攻撃力
    private int[,] m_prMag = new int[MySystem.ENEMYTYPE, 2];    // 防御力
    private int[,] m_agMag = new int[MySystem.ENEMYTYPE, 2];    // すばやさ
    private int[,] m_expMag = new int[MySystem.ENEMYTYPE, 2];   // 経験値
    private float[] m_walk = new float[MySystem.ENEMYTYPE];     // 歩く速度(navMesh)
    private float[] m_run = new float[MySystem.ENEMYTYPE];      // 走る速度(navMesh)
    private float[] m_rot = new float[MySystem.ENEMYTYPE];      // 回転速度(navMesh)
    private float[] m_turn = new float[MySystem.ENEMYTYPE];     // 振り向き速度(navMesh)
    private float[] m_searchRange = new float[MySystem.ENEMYTYPE];  // 探索範囲
    private float[] m_battleRange = new float[MySystem.ENEMYTYPE];  // 戦闘範囲

    public int[] id { get { return m_id; } }
    public string[] name { get { return m_name; } }
    public int[,] lvMag { get { return m_lvMag; } } // lvMag[種族番号,最小値なら0]で値取得
    public int[,] hpMag { get { return m_hpMag; } }
    public int[,] atMag { get { return m_atMag; } }
    public int[,] prMag { get { return m_prMag; } }
    public int[,] agMag { get { return m_agMag; } }
    public int[,] expMag { get { return m_expMag; } }
    public float[] walk { get { return m_walk; } }
    public float[] run { get { return m_run; } }
    public float[] rot { get { return m_rot; } }
    public float[] turn { get { return m_turn; } }
    public float[] searchRange { get { return m_searchRange; } }
    public float[] battleRange { get { return m_battleRange; } }



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
            m_lvMag[i, 0] = int.Parse(m_data[y][x++]);
            m_lvMag[i, 1] = int.Parse(m_data[y][x++]);
            m_hpMag[i, 0] = int.Parse(m_data[y][x++]);
            m_hpMag[i, 1] = int.Parse(m_data[y][x++]);
            m_atMag[i, 0] = int.Parse(m_data[y][x++]);
            m_atMag[i, 1] = int.Parse(m_data[y][x++]);
            m_prMag[i, 0] = int.Parse(m_data[y][x++]);
            m_prMag[i, 1] = int.Parse(m_data[y][x++]);
            m_agMag[i, 0] = int.Parse(m_data[y][x++]);
            m_agMag[i, 1] = int.Parse(m_data[y][x++]);
            m_expMag[i, 0] = int.Parse(m_data[y][x++]);
            m_expMag[i, 1] = int.Parse(m_data[y][x++]);

            m_walk[i] = float.Parse(m_data[y][x++]);
            m_run[i] = float.Parse(m_data[y][x++]);
            m_rot[i] = float.Parse(m_data[y][x++]);
            m_turn[i] = float.Parse(m_data[y][x++]);
            m_searchRange[i] = float.Parse(m_data[y][x++]);
            m_battleRange[i] = float.Parse(m_data[y][x++]);
        }
    }
}
