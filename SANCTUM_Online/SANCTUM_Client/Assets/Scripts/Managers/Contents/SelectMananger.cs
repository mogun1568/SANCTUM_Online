using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectMananger
{
    [HideInInspector] public Node selectedNode;
    ItemInfo _itemInfo;

    // node ���� �Լ�
    public void SelectNode(Node node, GameObject turret)
    {
        if (selectedNode == node)   // ������ ��带 �� �����ϰų� Ÿ���� ���� ��带 �����ϸ�
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

    // node ���� ���� �Լ�
    public void DeselectNode()
    {
        selectedNode = null;
        // ������ nodeUI ��ũ��Ʈ���� �ߴµ� nodeUI�� Ŭ����ǰ� levelUp �������� ���;� �ϴµ� ���� ������ �ݴ뿩�� ����� �ű�
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
