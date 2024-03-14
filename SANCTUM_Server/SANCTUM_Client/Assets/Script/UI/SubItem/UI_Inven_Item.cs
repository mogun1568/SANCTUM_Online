using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        //temNameText,
    }

    string _name;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        //Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<TextMeshProUGUI>().text = _name;

        //Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => { Debug.Log($"Item Cilck! {_name}"); });

        GameObject go = Get<GameObject>((int)GameObjects.ItemIcon).gameObject;
        // evt.gameObject�� RectTransform�� ������ �ִµ� evt.gameObject.transform�� ������ ������ RectTransform�� transform�� ����ϰ� �ֱ� ����
        // BindEvent �Լ��� ����Ǵ� ���� UI_EventHandler ������Ʈ�� ���ܳ��� �巹�װ� ���������� ����
        // ���ٽ��� �ش� �ڵ鷯 �̺�Ʈ�� ����� �ȴٰ� �����ϸ� �� ex) �� ��쿡�� �巹�׸� �Ҷ����� ���ٽĵ� ������ ��
        BindEvent(go, (PointerEventData data) => { move(go, data); }, Define.UIEvent.Drag);
    }

    void move(GameObject go, PointerEventData data)
    {
        go.transform.position = data.position;
        go.transform.localScale *= 0.95f;
    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
