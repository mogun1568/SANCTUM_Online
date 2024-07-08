using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : UI_Popup
{
    Item[] items;

    void Awake()
    {
        base.Init();

        items = GetComponentsInChildren<Item>(true);
    }

    public void Show(List<int> ItemIdxs)
    {
        Managers.Sound.Play("Effects/LevelUpLong", Define.Sound.Effect);

        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        foreach (int idx in ItemIdxs)
        {
            Item ranItem = items[idx];
            ranItem.gameObject.SetActive(true);
        }

        //C_LevelUp levelUpPacket = new C_LevelUp();
        //levelUpPacket.IsShow = true;
        //Managers.Network.Send(levelUpPacket);
    }
}
