using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotateAnimation : MonoBehaviour
{
    [SerializeField] private List<int> rotateGameMode = new List<int>(); // 回転・表示するゲームモード
    [SerializeField] private Vector3 axis;      // 回転軸
    private const float rotateSpeed = 90.0f;    // 回転速度

    private MySystem mySystem;
    private SpriteRenderer mySprite;

    private void Start()
    {
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        mySprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // 回転軸が決まっている　ゲームモードが定められている
        if(axis != null && rotateGameMode.Count > 0)
        {
            for(int i = 0; i < rotateGameMode.Count; i++)
            {
                // 現在のゲームモードが指定ゲームモードなら
                // 回転してforから抜ける
                if(rotateGameMode[i] == (int)mySystem.gameMode)
                {
                    mySprite.enabled = true;
                    transform.Rotate(axis, rotateSpeed * Time.deltaTime);
                    break;
                }
                else
                { mySprite.enabled = false; }
            }
        }
        
    }
}
