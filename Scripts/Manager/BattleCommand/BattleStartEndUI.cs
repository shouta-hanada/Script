using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStartEndUI : FadeInOutBase
{
    private BattleManager battleMana;

    [SerializeField]private bool once;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        battleMana = GameObject.Find("GameManager").GetComponent<BattleManager>();

        once = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(this.name == "BattleStart")
        {
            if (m_mySystem.gameMode == GameMode.Battle &&
                battleMana.BattleStart && !once)
            { 
                fadeStart = true;
                once = true;
            }

            else if (!battleMana.BattleStart && once)
            { 
                fadeStart = true;
                once = false;
            }
        }

        else if(this.name == "RunAway")
        {
            if (m_mySystem.gameMode == GameMode.RunAway &&
                !once)
            {
                fadeStart = true;
                once = true;
            }

            else if(m_mySystem.gameMode != GameMode.RunAway &&
                    once)
            {
                fadeStart = true;
                once = false;
            }
        }
    }
}
