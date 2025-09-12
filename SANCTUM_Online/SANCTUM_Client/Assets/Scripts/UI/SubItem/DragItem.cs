using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : UI_Base
{
    GameObject silhouetteItem;
    ItemInfo itemInfo;

    private RaycastHit hit;
    private Vector3 normalPosition;
    private Vector3 normalSize;
    private Image icon;

    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        itemInfo = Managers.Data.ItemDict[gameObject.name];
        icon = GetComponentsInChildren<Image>()[6];
        icon.sprite = Managers.Resource.Load<Sprite>($"Icon/{itemInfo.ItemIcon}");

        if (itemInfo.ItemType != "WorldOnlyItem") {
            BindEvent(gameObject, (PointerEventData data) => { OnBeginDrag(); }, Define.UIEvent.BeginDrag);
            BindEvent(gameObject, (PointerEventData data) => { OnDrag(data); }, Define.UIEvent.Drag);
            BindEvent(gameObject, (PointerEventData data) => { OnEndDrag(); }, Define.UIEvent.EndDrag);
        } else
        {
            BindEvent(gameObject, (PointerEventData data) => { Onclick(); }, Define.UIEvent.Click);
        }
    }

    public void OnBeginDrag()
    {
        normalPosition = transform.position;
        normalSize = transform.localScale;
        Managers.Select.SelectItemToUse(gameObject, itemInfo);

        if (itemInfo.ItemType == "Tower")
        {
            silhouetteItem = Managers.Resource.Instantiate("ItemE_Tower");
        } else
        {
            silhouetteItem = Managers.Resource.Instantiate("Cube");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (silhouetteItem == null)
        {
            Debug.Log("SilhouetteItem is null");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        UpdateUITransform(eventData);
        UpdateSilhouetteTransform();
    }

    private void UpdateUITransform(PointerEventData eventData)
    {
        float size = Vector2.Distance(normalPosition, eventData.position);

        if (normalSize.x - size / 100 > 0)
        {
            transform.position = eventData.position;
            transform.localScale = normalSize - Vector3.one * (size / 120);
            silhouetteItem.transform.localScale = Vector3.one * (1 - (normalSize.x - size / 100));
        }
        else
        {
            transform.localScale = Vector3.zero;
            silhouetteItem.transform.localScale = Vector3.one * 1.01f;
        }
    }

    private void UpdateSilhouetteTransform()
    {
        switch (hit.transform?.tag)
        {
            case "ForestGround":
                HandleGroundPlacement();
                break;
            default:
                SetSilhouetteColor(Color.red);
                silhouetteItem.transform.position = hit.point;
                break;
        }
    }

    private void HandleGroundPlacement()
    {
        var node = hit.transform.GetComponent<Node>();
        if (itemInfo.ItemType == "Tower")
        {
            if (node != null && node.turret == null && !node.environment)
                SetSilhouetteColor(Color.green);
            else
                SetSilhouetteColor(Color.red);
        }

        silhouetteItem.transform.position = hit.transform.position + Vector3.up * hit.transform.localScale.y;
    }

    private void SetSilhouetteColor(Color color)
    {
        foreach (Renderer mat in silhouetteItem.GetComponentsInChildren<Renderer>())
        {
            mat.material.color = color;
        }
    }

    public void OnEndDrag()
    {
        transform.position = normalPosition;
        transform.localScale = normalSize;

        if (hit.transform != null && hit.transform.CompareTag("ForestGround"))
        {
            hit.transform.GetComponent<Node>().UseItem();
        }

        Managers.Select.Clear();
        Destroy(silhouetteItem);
        silhouetteItem = null;
    }

    public void Onclick()
    {
        if (itemInfo.ItemType == "WorldOnlyItem")
        {
            Managers.Select.SelectItemToUse(gameObject, itemInfo);

            C_InvenUpdate invenUpdatePacket = new C_InvenUpdate();
            invenUpdatePacket.ItemName = itemInfo.ItemName;
            Managers.Network.Send(invenUpdatePacket);
        }
    }
}