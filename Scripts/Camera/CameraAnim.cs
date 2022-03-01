using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnim : MonoBehaviour
{
    private Animator myAnim;
    private bool battleStart;
    public bool BattleStart { get { return battleStart; } }

    private bool battleNow;
    public bool BattleNow 
    { 
        get { return battleNow; } 
        set { battleNow = value; }
    }
    private MySystem mySystem;

    public void CallBackBattleStart() { battleStart = false; }
    public void CallBackBattleNow() { battleNow = false; }

    public void SetBool(string key, bool value)
    { myAnim.SetBool(key, value); }

    private void Start()
    {
        battleStart = true;
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
        myAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(mySystem.gameMode != GameMode.Battle)
        { 
            battleStart = true;
            battleNow = false;

            if(!myAnim.GetCurrentAnimatorStateInfo(0).IsName("State"))
            {
                var clipInfo = myAnim.GetCurrentAnimatorClipInfo(0);
                SetBool(clipInfo[0].clip.name, false);
            }
            
        }
    }

}
