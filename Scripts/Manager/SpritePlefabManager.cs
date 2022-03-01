using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������鎞�ȂǂɎg���X�v���C�g(�摜)���܂Ƃ߂Ă����N���X
// num���v�f���ȏ�̏ꍇ�ǂ���ɂ���G���[�����珑���Ă��Ȃ��B
// �Ăяo��->mySystem.spritePlefabMana.Get����(num)
public class SpritePlefabManager : MonoBehaviour
{
    [SerializeField] private Sprite[] itemSprite = new Sprite[(int)ItemName.ItemMax]; // �A�C�e���̃X�v���C�g(�A�C�e������)
    public Sprite GetItemSprite(int num) { return itemSprite[num]; }

    [SerializeField] private Sprite[] itemPlateSprite = new Sprite[(int)ItemName.ItemMax];  // �A�C�e���̖��O���`���ꂽ�X�v���C�g
    public Sprite GetItemPlateSprite(int num) { return itemPlateSprite[num]; }          

    [SerializeField] private Sprite[] charSprite = new Sprite[MySystem.PARTYTYPE];  // �L�����̉摜
    public Sprite GetCharSprite(int num) { return charSprite[num]; }

    [SerializeField] private Sprite[] numSprite = new Sprite[10];  // �l�̃X�v���C�g(0~9)
    public Sprite GetNumSprite(int num) { return numSprite[num]; }
}
