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
        // evt.gameObject는 RectTransform을 가지고 있는데 evt.gameObject.transform이 가능한 이유는 RectTransform이 transform을 상속하고 있기 때문
        // BindEvent 함수가 실행되는 순간 UI_EventHandler 컴포넌트가 생겨나서 드레그가 가능해지는 거임
        // 람다식은 해당 핸들러 이벤트에 등록이 된다고 생각하면 됨 ex) 이 경우에는 드레그를 할때마다 람다식도 실행이 됨
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
