using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectMananger
{
    [HideInInspector] public Node selectedNode;
    ItemInfo _itemInfo;

    // node 선택 함수
    public void SelectNode(Node node, GameObject turret)
    {
        if (selectedNode == node)   // 선택한 노드를 또 선택하거나 타워가 없는 노드를 선택하면
        {
            DeselectNode();
            return;
        }

        DeselectNode();

        selectedNode = node;

        C_TurretUI turretUIPacket = new C_TurretUI();
        turretUIPacket.NodeId = node.Id;
        Managers.Network.Send(turretUIPacket);
    }

    // node 선택 해제 함수
    public void DeselectNode()
    {
        selectedNode = null;
        // 원래는 nodeUI 스크립트에서 했는데 nodeUI가 클로즈되고 levelUp 프리팹이 나와야 하는데 실행 순서가 반대여서 여기로 옮김
        if (Managers.UI.getPopStackTop()?.name == "NodeUI")
        {
            Managers.UI.ClosePopupUI();
        }
    }

    public void SelectItemToUse(GameObject item, ItemInfo itemInfo)
    {
        Clear();
        _itemInfo = itemInfo;

        Debug.Log($"{_itemInfo.ItemName} Selected");

        if (Managers.UI.getPopStackTop()?.name == "NodeUI")
        {
            DeselectNode();
        }
    }

    public ItemInfo getItemData()
    {
        return _itemInfo;
    }

    public void Clear()
    {
        _itemInfo = null;
    }
}
