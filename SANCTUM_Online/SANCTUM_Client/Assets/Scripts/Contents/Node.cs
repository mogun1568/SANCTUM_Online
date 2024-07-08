using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Node : BaseController
{
    // DragItem���� ���ٴµ� Ȯ�� �ʿ�
    [HideInInspector] public GameObject turret;
    [HideInInspector] public bool environment;

    void OnMouseDown()
    {
        if (Managers.Game.GameIsOver)
        {
            return;
        }

        if (!Managers.Game.isPopup)
        {
            return;
        }

        Managers.Select.SelectNode(this, turret);
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + new Vector3(0f, transform.localScale.y, 0f);
    }

    public void UseItem()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        ItemInfo itemInfo = Managers.Select.getItemData();
        CheckUpdatedInven(itemInfo.ItemName);
    }

    void CheckUpdatedInven(string name)
    {
        C_InvenUpdate invenUpdatePacket = new C_InvenUpdate() { PosInfo = new PositionInfo() };
        invenUpdatePacket.ItemName = name;
        invenUpdatePacket.IsAdd = false;
        invenUpdatePacket.NodeId = Id;
        invenUpdatePacket.PosInfo.PosX = transform.position.x;
        invenUpdatePacket.PosInfo.PosY = transform.position.y + transform.localScale.y;
        invenUpdatePacket.PosInfo.PosZ = transform.position.z;

        Managers.Network.Send(invenUpdatePacket);
    }

    public void FirstPersonMode(Turret turret)
    {
        Debug.Log("First Person Mode");
        Managers.Object.MyMap.IsFPM = true;
        turret.IsFPM = true;

        //��Ȱ��ȭ�� ������Ʈ�� �׳� GetComponent�� ��ã�� GetComponents<>(true)�� �迭�� ã�Ƽ� ����ؾ� ��
        turret.GetComponentsInChildren<Camera>(true)[0].gameObject.SetActive(true);

        Managers.Game.invenUI.SetActive(false);
        Managers.UI.ShowPopupUI<FPSUI>("FPSModeUI");
    }
}