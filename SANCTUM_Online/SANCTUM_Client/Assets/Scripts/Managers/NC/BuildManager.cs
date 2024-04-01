using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    /*public static BuildManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }
        instance = this;
    }

    public GameObject selectItem;

    public GameObject buildEffect;
    public GameObject destroyEffect;

    private GameObject itemUI;
    [HideInInspector] public Node selectedNode;
    ItemData data;

    public NodeUI nodeUI;

    // node 선택 함수
    public void SelectNode(Node node)
    {
        if (selectedNode == node)   // 선택한 노드를 또 선택하면
        {
            DeselectNode();
            return;
        }

        selectedNode = node;
        itemUI = null;

        nodeUI.SetTarget(node);
    }

    // node 선택 해제 함수
    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }

    public void SelectItemToUse(GameObject item, ItemData _data)
    {
        if (itemUI == item)   // 선택한 노드를 또 선택하면
        {
            Debug.Log("Deselect Item");
            Clear();
            return;
        }

        Clear();
        data = _data;

        Debug.Log($"{data.itemName} Selected");
        itemUI = item;
        DeselectNode();
    }

    public ItemData getItemData()
    {
        return data;
    }

    public void itemUITextDecrease()
    {
        selectItem.GetComponent<SelectItem>().useItem(data.itemId);
        Clear();
    }

    public void DestroyItemUI()
    {
        itemUI.SetActive(false);
        Clear();
    }

    public void Clear()
    {
        itemUI = null;
        data = null;
    }*/
}