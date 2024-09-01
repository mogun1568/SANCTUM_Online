using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Google.Protobuf.Protocol;

public class Item : UI_Base
{
    ItemInfo _itemInfo;

    Image icon;

    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        _itemInfo = Managers.Data.ItemDict[gameObject.name];
        icon = GetComponentsInChildren<Image>()[2];
        icon.sprite = Managers.Resource.Load<Sprite>($"Icon/{_itemInfo.ItemIcon}");

        BindEvent(gameObject, (PointerEventData data) => { ItemClick(); });
    }

    public void ItemClick()
    {
        C_InvenUpdate invenUpdatePacket = new C_InvenUpdate() { PosInfo = new PositionInfo() };
        invenUpdatePacket.ItemName = gameObject.name;
        invenUpdatePacket.IsAdd = true;
        Managers.Network.Send(invenUpdatePacket);

        GetComponentInParent<LevelUp>().ClosePopupUI();
    }
}