using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // =================== 変数 ==================== //
    [SerializeField]private GameObject enemy;


    // ============= Start ・ Update =============== //
    void Start()
    {
        // エネミーの生成
        Instantiate(enemy, transform.position, Quaternion.identity, transform);
    }

    void Update()
    {
        // DebugKeyCode
        if(transform.childCount == 0 &&
           Input.GetKeyDown(KeyCode.P))
        {
            // エネミーの生成
            Instantiate(enemy, transform.position, Quaternion.identity, transform);
        }
    }
}
