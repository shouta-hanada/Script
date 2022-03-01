using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // =================== �ϐ� ==================== //
    [SerializeField]private GameObject enemy;


    // ============= Start �E Update =============== //
    void Start()
    {
        // �G�l�~�[�̐���
        Instantiate(enemy, transform.position, Quaternion.identity, transform);
    }

    void Update()
    {
        // DebugKeyCode
        if(transform.childCount == 0 &&
           Input.GetKeyDown(KeyCode.P))
        {
            // �G�l�~�[�̐���
            Instantiate(enemy, transform.position, Quaternion.identity, transform);
        }
    }
}
