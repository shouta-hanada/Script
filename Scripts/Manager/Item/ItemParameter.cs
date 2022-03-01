using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemParameter : MonoBehaviour
{
    public int itemNum;
    private bool useResult;
    private MySystem mySystem;

    private const int herbPower = 5;
    private const int greatHerbPower = 20;

    public bool UseItem()
    {
        bool result = false;

        switch (itemNum)
        {
            case (int)ItemName.Herb: result = HpRecovery(herbPower); break;
            case (int)ItemName.GreatHerb: result = HpRecovery(greatHerbPower); break;
            case (int)ItemName.Smoke: result = UseSmoke(); break;
        }

        useResult = result;
        return result;
    }


    /// <summary>
    /// 2022/02/27
    /// HP回復系アイテム使用時に呼び出す関数
    /// </summary>
    /// <param name="value">回復する量</param>
    /// <returns>HPを回復できない場合falseを返す</returns>
    private bool HpRecovery(int value)
    {
        // HPが満タンか
        if (mySystem.battleMana.player.currentHp == mySystem.battleMana.player.maxHp)
        { return false; }

        // HPの回復
        mySystem.battleMana.player.currentHp += value;

        // 最大HPを超えていたら最大HPへ
        if(mySystem.battleMana.player.currentHp >= mySystem.battleMana.player.maxHp)
        { mySystem.battleMana.player.currentHp = mySystem.battleMana.player.maxHp; }

        return true;
    }



    private bool UseSmoke()
    {
        if (mySystem.gameMode != GameMode.Battle)
        { return false; }

        mySystem.gameMode = GameMode.RunAway;

        return true;
    }
    // Start is called before the first frame update
    void Start()
    {
        useResult = false;
        mySystem = GameObject.Find("GameManager").GetComponent<MySystem>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
