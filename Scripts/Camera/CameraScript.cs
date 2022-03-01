using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // =================== 変数 ==================== //
    private GameObject targetChar; // 通常時に映すキャラ
    private MySystem mySystem;     // システム
    private CameraAnim cameraAnim; // カメラのアニメーション用
    
    private BattleManager battleMana;

    // 追跡関係
    private const float height = 1.5f;          // カメラの高さ
    private const float distance = 3.0f;        // 最短距離
    private const float trackingRange = 0.3f;   // 追跡範囲
    private float playerCurrentSpeed; 
    [SerializeField] float nowDis;              // 現在の目標との距離

    // メインカメラ関係
    private const float fieldOfView = 60.0f;
    private const float maxFieldOfView = 90.0f;
    private const float addFieldOfView = 5.0f;

    private const float rotSpeed = 135;

    // ================= function ================== //
    /// <summary>
    /// 2021/12/10
    /// 目標(targetChar)を追跡する関数。
    /// </summary>
    private void ReflectChar()
    {
        Vector3 myPos = transform.position;
        Vector3 tPos = targetChar.transform.position;
        nowDis = mySystem.MathDistance(myPos, tPos, false);

        // 衛星回転
        transform.RotateAround( tPos, Vector3.up, Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime);

        
        // 目標凝視
        Vector3 tPosY = new Vector3(tPos.x, tPos.y + height, tPos.z);
        transform.LookAt(tPosY);

        // 目標地点の高さ - 現在の高さ
        float myPosY = ((tPos.y + height) - myPos.y) * Time.deltaTime;

        if (nowDis > distance + trackingRange)
        {
            transform.position += transform.rotation * new Vector3(0.0f, myPosY, playerCurrentSpeed * Time.deltaTime);
        }
        else if (nowDis < distance - trackingRange)
        {
            transform.position += transform.rotation * new Vector3(0.0f, myPosY, -playerCurrentSpeed * Time.deltaTime);

        }
    }

    // ********************************************* //
    /// <summary>
    /// 2021/12/12
    /// バトル開始時のカメラの動き
    /// </summary>
    /// <returns></returns>
    public bool BattleRotate(Vector3 party, Vector3 enemy)
    {
        // プレイヤー側とエネミー側のベクトルからその中間を取得
        // カメラの中心を移動
        Vector3 center = ((party + enemy) / 2) + new Vector3(0.0f, height, 0.0f);
        transform.position = Vector3.Lerp(transform.position, center, Time.deltaTime);

        // バトルが始まった状態なら
        if (cameraAnim.BattleStart)
        { 
            cameraAnim.SetBool("BattleStart", true);
            return false;
        }

        else
        {
            cameraAnim.SetBool("BattleStart", false); 

            // コマンド選択中
            if(!cameraAnim.BattleNow && battleMana.battleMode == BattleMode.Command)
            {
                cameraAnim.SetBool("CameraBattleNowRotate_0", true);
                cameraAnim.BattleNow = true;
            }

            return true;
        }
    }

    // ============= Start ・ Update =============== //
    void Start()
    {
        targetChar = GameObject.Find("Player");
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        battleMana = GameObject.Find("GameManager").GetComponent<BattleManager>();
        cameraAnim = GetComponentInChildren<CameraAnim>();

        transform.position = new Vector3(0.0f, targetChar.transform.position.y + height, -3.0f);
    }

    void Update()
    {
        playerCurrentSpeed = battleMana.player.currentSpeed;

        // 視野角
        if (playerCurrentSpeed >=battleMana.player.run && 
            Camera.main.fieldOfView < maxFieldOfView)
        { Camera.main.fieldOfView += (playerCurrentSpeed * addFieldOfView) * Time.deltaTime; }
        
        else if(playerCurrentSpeed <= 0 && Camera.main.fieldOfView > fieldOfView)
        { Camera.main.fieldOfView = Camera.main.fieldOfView - fieldOfView * Time.deltaTime; }

        // 映すターゲットが居れば
        if (targetChar)
        {
            if(mySystem.gameMode == GameMode.Free ||
               mySystem.gameMode == GameMode.RunAway)
            { 
                ReflectChar();
            }
        }
        else
        {
            targetChar = GameObject.Find("Player");
            if(!targetChar) { mySystem.gameMode = GameMode.GameOver; }
        }

        if (mySystem.gameMode != GameMode.Battle) { }
    }
    
    
}
