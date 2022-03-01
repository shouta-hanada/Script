using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // =================== �ϐ� ==================== //
    private GameObject targetChar; // �ʏ펞�ɉf���L����
    private MySystem mySystem;     // �V�X�e��
    private CameraAnim cameraAnim; // �J�����̃A�j���[�V�����p
    
    private BattleManager battleMana;

    // �ǐՊ֌W
    private const float height = 1.5f;          // �J�����̍���
    private const float distance = 3.0f;        // �ŒZ����
    private const float trackingRange = 0.3f;   // �ǐՔ͈�
    private float playerCurrentSpeed; 
    [SerializeField] float nowDis;              // ���݂̖ڕW�Ƃ̋���

    // ���C���J�����֌W
    private const float fieldOfView = 60.0f;
    private const float maxFieldOfView = 90.0f;
    private const float addFieldOfView = 5.0f;

    private const float rotSpeed = 135;

    // ================= function ================== //
    /// <summary>
    /// 2021/12/10
    /// �ڕW(targetChar)��ǐՂ���֐��B
    /// </summary>
    private void ReflectChar()
    {
        Vector3 myPos = transform.position;
        Vector3 tPos = targetChar.transform.position;
        nowDis = mySystem.MathDistance(myPos, tPos, false);

        // �q����]
        transform.RotateAround( tPos, Vector3.up, Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime);

        
        // �ڕW�Î�
        Vector3 tPosY = new Vector3(tPos.x, tPos.y + height, tPos.z);
        transform.LookAt(tPosY);

        // �ڕW�n�_�̍��� - ���݂̍���
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
    /// �o�g���J�n���̃J�����̓���
    /// </summary>
    /// <returns></returns>
    public bool BattleRotate(Vector3 party, Vector3 enemy)
    {
        // �v���C���[���ƃG�l�~�[���̃x�N�g�����炻�̒��Ԃ��擾
        // �J�����̒��S���ړ�
        Vector3 center = ((party + enemy) / 2) + new Vector3(0.0f, height, 0.0f);
        transform.position = Vector3.Lerp(transform.position, center, Time.deltaTime);

        // �o�g�����n�܂�����ԂȂ�
        if (cameraAnim.BattleStart)
        { 
            cameraAnim.SetBool("BattleStart", true);
            return false;
        }

        else
        {
            cameraAnim.SetBool("BattleStart", false); 

            // �R�}���h�I��
            if(!cameraAnim.BattleNow && battleMana.battleMode == BattleMode.Command)
            {
                cameraAnim.SetBool("CameraBattleNowRotate_0", true);
                cameraAnim.BattleNow = true;
            }

            return true;
        }
    }

    // ============= Start �E Update =============== //
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

        // ����p
        if (playerCurrentSpeed >=battleMana.player.run && 
            Camera.main.fieldOfView < maxFieldOfView)
        { Camera.main.fieldOfView += (playerCurrentSpeed * addFieldOfView) * Time.deltaTime; }
        
        else if(playerCurrentSpeed <= 0 && Camera.main.fieldOfView > fieldOfView)
        { Camera.main.fieldOfView = Camera.main.fieldOfView - fieldOfView * Time.deltaTime; }

        // �f���^�[�Q�b�g�������
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
