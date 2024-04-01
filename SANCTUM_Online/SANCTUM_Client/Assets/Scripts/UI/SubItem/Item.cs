using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Item : UI_Base
{
    public ItemData data;
    Data.Item itemData;

    Image icon;
    //TextMeshProUGUI textName;

    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        itemData = Managers.Data.ItemDict[gameObject.name];
        icon = GetComponentsInChildren<Image>()[2];
        icon.sprite = Managers.Resource.Load<Sprite>($"Icon/{itemData.itemIcon}");

        BindEvent(gameObject, (PointerEventData data) => { ItemClick(); });
    }

    public void ItemClick()
    {
        Managers.UI.SelectItem.AddItem(itemData.itemName);
        GetComponentInParent<LevelUp>().ClosePopupUI();
        Managers.Game.Resume();
        Managers.Game.isHide = true;
    }
}