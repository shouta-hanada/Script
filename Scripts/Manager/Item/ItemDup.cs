using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemDup : MonoBehaviour
{
    private Image plate;
    private Image item; 
    private float plPX =   0.0f, plPY = 120.0f ; // �v���[�g�̃|�W�V����
    private float plSX = 870.0f, plSY = 105.0f;  // �v���[�g�̃T�C�Y
    private float itPX = 340.0f, itPY = 0.0f ;   // �A�C�e���̃|�W�V����
    private float itSX = 100.0f, itSY = 100.0f;  // �A�C�e���̃T�C�Y
    [SerializeField]private MySystem mySystem;
    private ItemManager itemMana;

    

    public void InitItem(int num)
    {
        int maxChild = transform.childCount;
        float newPlPy = plPY;

        // ������(itemObj)�̎q���̐����擾
        // �������̍Ō�̎q�̈ʒu���擾
        // �ēx�v���[�g�̃|�W�V�������v�Z�B
        if (maxChild != 0)
        {
            Vector2 plPos = transform.GetChild(maxChild - 1).
                 GetComponent<RectTransform>().localPosition;
            newPlPy = plPos.y - plPY;
        }


        // �v���[�g�̍쐬 (obj, parent)
        // �v���[�g�̈ʒu�ݒ�
        Image pl = Instantiate(plate, transform);
        pl.rectTransform.sizeDelta = new Vector2(plSX, plSY);
        pl.rectTransform.localPosition = new Vector2(plPX, newPlPy);
        pl.sprite = mySystem.spritePlefabMana.GetItemPlateSprite(num);
        pl.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        // �A�C�e���A�C�R���̍쐬
        // �A�C�R���̈ʒu�ݒ�
        // �A�C�R���̃X�v���C�g�̕ύX
        Image it = Instantiate(item, pl.transform);
        it.rectTransform.sizeDelta = new Vector2(itSX, itSY);
        it.rectTransform.localPosition = new Vector2(itPX, itPY);
        it.sprite = mySystem.spritePlefabMana.GetItemSprite(num);
        it.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        pl.gameObject.AddComponent<ItemParameter>().itemNum = num;

        bool s = pl.gameObject.activeSelf;

        itemMana.ResizeTheNumberOfItem();
    }

    // Start is called before the first frame update
    void Start()
    {
        itemMana = GameObject.Find("Items").GetComponent<ItemManager>();
        plate = new GameObject("").AddComponent<Image>();
        item = new GameObject("").AddComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // DebugKeyCode
        if (Input.GetKeyDown(KeyCode.L))
        { InitItem(mySystem.GetRandomValue(0, 3)); } 
    }
}
