using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectMananger
{
    
    //GameObject selectItem;

    GameObject itemUI;

    [HideInInspector] public Node selectedNode;

    Data.Item itemData;

    //ItemData data;

    //public NodeUI nodeUI;

    // node 선택 함수

    public void SelectNode(Node node, GameObject turret)
    {
        if (selectedNode == node || turret == null)   // 선택한 노드를 또 선택하거나 타워가 없는 노드를 선택하면
        {
            DeselectNode();
            return;
        }

        DeselectNode();

        selectedNode = node;
        //itemUI = null;

        NodeUI nodeUI = Managers.UI.ShowPopupUI<NodeUI>("NodeUI");
        nodeUI.SetTarget(node);
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
        //nodeUI.Hide();
    }

    public void SelectItemToUse(GameObject item, Data.Item _itemData)
    {
        /*if (itemUI == item)   // 선택한 아이템을 또 선택하면
        {
            Debug.Log("Deselect Item");
            Clear();
            return;
        }*/

        Clear();
        itemData = _itemData;

        Debug.Log($"{itemData.itemName} Selected");
        itemUI = item;

        if (Managers.UI.getPopStackTop()?.name == "NodeUI")
        {
            DeselectNode();
        }
    }

    public Data.Item getItemData()
    {
        return itemData;
    }

    public void itemUITextDecrease()
    {
        //Managers.Inven.useItem(itemData.itemName);
        itemUI.GetComponentInParent<SelectItem>().useItem(itemData.itemName);
        //selectItem.GetComponent<SelectItem>().useItem(data.itemId);
        Clear();
    }



    public void DestroyItemUI()
    {
        //Managers.Resource.Destroy(itemUI);
        itemUI.SetActive(false);
        Clear();
    }

    public void Clear()
    {
        itemUI = null;
        itemData = null;
    }
}
