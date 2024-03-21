using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Xml;

public class SelectItem : UI_Base
{
    Dictionary<string, int> invenDict = new Dictionary<string, int>(); // ������ ���̵�� ������ �����ϴ� ��ųʸ�

    RectTransform InventoryTransform;
    //[SerializeField] float ScrlSpd = 1.75f;
    float tok;
    private bool isLerping = false;
    Vector3 max = new Vector3(0f, 100f, 0f);
    Vector3 min = new Vector3(0f, 0, 0f);

    enum GameObjects
    {
        SelectItem,
        StandardTower,
        Fire,
        Ice,
        Water,
        Lightning,
        Dirt,
        DamageUp,
        RangeUp,
        FireRateUp,
        LifeRecovery
    }

    enum Texts
    {
        StandardTowerCountText,
        FireCountText,
        IceCountText,
        WaterCountText,
        LightningCountText,
        DirtCountText,
        DamageUpCountText,
        RangeUpCountText,
        FireRateUpCountText,
        LifeRecoveryCountText
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Managers.UI.SelectItem = GetComponent<SelectItem>();

        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GameObject inventory = Get<GameObject>((int)GameObjects.SelectItem);
        foreach (Transform child in inventory.transform)
        {
            //Managers.Resource.Destroy(child.gameObject);
            child.gameObject.SetActive(false);
        }

        // ���� �κ��丮 ������ �����ؼ�
        for (int i = 0; i < 5; i++)
        {
            AddItem("StandardTower");

            //GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(inventory.transform).gameObject;
            //UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();
            //invenItem.SetInfo($"Sword{i}");
        }

        InventoryTransform = transform.parent.GetComponent<RectTransform>();
        tok = 1;
    }

    public void FirstAddItem()
    {
        // ���� �κ��丮 ������ �����ؼ�
        for (int i = 0; i < 3; i++)
        {
            AddItem("StandardTower");

            //GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(inventory.transform).gameObject;
            //UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();
            //invenItem.SetInfo($"Sword{i}");
        }
    }
    public void AddItem(string itemName)
    {
        if (invenDict.ContainsKey(itemName))
        {
            // �̹� �ش� �������� �κ��丮�� �ִ� ���
            invenDict[itemName]++; // ������ ������ 1 ������Ŵ
            UpdateItemUI(itemName, invenDict[itemName]); // ������ UI ������Ʈ
        }
        else
        {
            // ���ο� �������� ȹ���� ���
            invenDict.Add(itemName, 1); // �������� ��ųʸ��� �߰��ϰ� ������ 1�� �ʱ�ȭ��
            //CreateItemUI(item); // ������ UI ����
            CreateItemUI(itemName);
        }
    }

    public void useItem(string itemName)
    {
        invenDict[itemName]--;
        if (invenDict[itemName] == 0)
        {
            invenDict.Remove(itemName);
            Managers.Select.DestroyItemUI();
        }
        else
        {
            UpdateItemUI(itemName, invenDict[itemName]);
        }
    }

    void UpdateItemUI(string itemName, int itemCount)
    {
        // ������ UI�� ã�Ƽ� ������ ������Ʈ��
        if (Enum.TryParse(itemName + "CountText", out Texts enumValue))
        {
            GetText((int)enumValue).text = itemCount.ToString();
        }
        else
        {
            Debug.LogError($"Failed to parse enum value for itemName: {itemName}");
        }

        //Transform itemUI = transform.Find(itemName);
        //TextMeshProUGUI itemText = itemUI.GetComponentInChildren<TextMeshProUGUI>();
        //itemText.text = itemCount.ToString();
    }

    void CreateItemUI(string itemName)
    {
        //GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(transform, itemName).gameObject;
        //UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();

        if (Enum.TryParse(itemName, out GameObjects enumValue))
        {
            if (GetObject((int)enumValue) == null)
            {
                Debug.Log("null");
            }
            Transform itemUI = GetObject((int)enumValue).transform;
            itemUI.SetAsLastSibling();
            itemUI.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"Failed to parse enum value for itemName: {itemName}");
        }
        //Transform itemUI = transform.Find(itemName.ToString());
        //itemUI.SetAsLastSibling();
        //itemUI.gameObject.SetActive(true);
    }

    public IEnumerator StartLerpUp()
    {
        isLerping = true;
        float lerpTime = 0;
        float lerpDuration = .5f;
        tok = 1;
        while (lerpTime < lerpDuration)
        {
            lerpTime += Time.deltaTime;
            InventoryTransform.localPosition += Vector3.Lerp(min, max, 2f * Time.deltaTime);
            yield return null;
        }
        isLerping = false;
    }
    public IEnumerator StartLerpDown()
    {
        isLerping = true;
        float lerpTime = 0;
        float lerpDuration = .5f;
        tok = 0;
        while (lerpTime < lerpDuration)
        {
            lerpTime += Time.deltaTime;
            InventoryTransform.localPosition -= Vector3.Lerp(min, max, 2f * Time.deltaTime);
            yield return null;
        }
        isLerping = false;
    }

    public void ScrollBar()
    {
        if (!isLerping)
        {
            if (tok == 0)
            {
                StartCoroutine(StartLerpUp());
            }
            else
            {
                StartCoroutine(StartLerpDown());
            }
        }
    }
}
