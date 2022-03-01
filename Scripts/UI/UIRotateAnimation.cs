using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotateAnimation : MonoBehaviour
{
    [SerializeField] private List<int> rotateGameMode = new List<int>(); // ��]�E�\������Q�[�����[�h
    [SerializeField] private Vector3 axis;      // ��]��
    private const float rotateSpeed = 90.0f;    // ��]���x

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
        // ��]�������܂��Ă���@�Q�[�����[�h����߂��Ă���
        if(axis != null && rotateGameMode.Count > 0)
        {
            for(int i = 0; i < rotateGameMode.Count; i++)
            {
                // ���݂̃Q�[�����[�h���w��Q�[�����[�h�Ȃ�
                // ��]����for���甲����
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
