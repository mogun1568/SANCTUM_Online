using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Xml;
using System.IO;
using System.Net;

public class SelectItem : UI_Base
{
    Dictionary<string, int> _invenDict = new Dictionary<string, int>(); // 아이템 아이디와 개수를 저장하는 딕셔너리

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

    string absolutePath = "C:\\Users\\kmg3329\\Documents\\GitHub\\SANCTUM_Online\\SANCTUM_Online\\SANCTUM_Client";
    public void LoadInventory(int playerId, string pathPrefix = "Assets/Resources/Inventory")
    {
        string inventoryName = "Inventory_" + playerId.ToString();

        // Collision 관련 파일
        string text = File.ReadAllText($"{absolutePath}/{pathPrefix}/{inventoryName}.txt");
        //TextAsset textAsset = Resources.Load<TextAsset>($"Inventory/{inventoryName}");
        StringReader reader = new StringReader(text);

        // 다 비활성화
        foreach (Transform child in inventory.transform)
        {
            child.gameObject.SetActive(false);
        }

        int len = int.Parse(reader.ReadLine());
        for (int i = 0; i < len; i++)
        {
            string itemName = reader.ReadLine();
            int cnt = int.Parse(reader.ReadLine());

            CreateItemUI(itemName);
            UpdateItemUI(itemName, cnt);
        }
    }

    // 아예 전부 UI_Inven으로 옮길 수 있을지도
    void Awake()
    {
        Init();
    }

    GameObject inventory;
    public override void Init()
    {
        Managers.UI.SelectItem = GetComponent<SelectItem>();

        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        inventory = Get<GameObject>((int)GameObjects.SelectItem);
        foreach (Transform child in inventory.transform)
        {
            child.gameObject.SetActive(false);
        }

        InventoryTransform = transform.parent.GetComponent<RectTransform>();
        tok = 1;
    }

    void UpdateItemUI(string itemName, int itemCount)
    {
        // 아이템 UI를 찾아서 개수를 업데이트함
        if (Enum.TryParse(itemName + "CountText", out Texts enumValue))
        {
            GetText((int)enumValue).text = itemCount.ToString();
        }
        else
        {
            Debug.LogError($"Failed to parse enum value for itemName: {itemName}");
        }
    }

    void CreateItemUI(string itemName)
    {
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
