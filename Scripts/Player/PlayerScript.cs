using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScript : PartyStatusBase
{
    /// <summary>
    /// 2021/10/11
    /// プレイヤーを前後移動させる関数。
    /// 計算方法
    /// 現在の速度 = 指定速度 * 加速度 * 前回からのフレーム
    /// </summary>
    private void PlayerWalk()
    {
        SwitchDirWalk(m_mainCamera.gameObject);
        WalkAndRun();

         // 現在の向きに前進させる。
         Quaternion rot = transform.rotation;
         Vector3    vec = new Vector3(0.0f, 0.0f, m_currentSpeed);
         transform.position += rot * vec * Time.deltaTime;
    }


    // ============= Start ・ Update =============== //
    protected override void Start()
    {
        SetAllStatus((int)PartyID.Skelton);
        m_currentSpeed = 0.0f;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // DebugKeyCode
        if (Input.GetKeyDown(KeyCode.I))
        { m_maxHp++; }
        


        if(m_mySystem.gameMode == GameMode.Free ||
           m_mySystem.gameMode == GameMode.RunAway)
        { PlayerWalk(); }
        
        else 
        { m_currentSpeed = 0.0f; }
    }
}
