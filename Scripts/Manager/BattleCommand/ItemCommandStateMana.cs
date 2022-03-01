using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCommandStateMana : MonoBehaviour
{
    private Button itemButton;
    private ItemManager itemMana;

    // Start is called before the first frame update
    void Start()
    {
        itemButton = GetComponent<Button>();
        itemMana = GameObject.Find("Items").GetComponent<ItemManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(itemMana.MaxChildCount != 0)
        {
            ColorBlock col = itemButton.colors;
            col.normalColor = new Color(1.0f, 1.0f, 1.0f);
            itemButton.colors = col;

            itemButton.interactable = true;
        }

        else
        {
            ColorBlock col = itemButton.colors;
            col.normalColor -= new Color(0.5f, 0.5f, 0.5f);
            itemButton.colors = col;

            itemButton.interactable = false;
        }
    }
}
