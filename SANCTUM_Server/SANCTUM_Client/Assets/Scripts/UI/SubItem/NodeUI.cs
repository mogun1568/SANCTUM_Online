using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;

public class NodeUI : UI_Popup
{
    enum GameObjects
    {
        HpBar,
        FirstPersonMode,
        Demolition,
        Sphere,
        Sphere1
    }

    enum Texts
    {
        damageText,
        fireRateText
    }

    Transform sphere;
    Transform sphere1;

    GameObject fPMButton;
    GameObject DelButton;


    Node target;
    TowerControl towerControl;
    //public TextMeshProUGUI retrunExp; // 추가 예정

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 45, 0);

        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        fPMButton = GetObject((int)GameObjects.FirstPersonMode);
        DelButton = GetObject((int)GameObjects.Demolition);
        sphere = GetObject((int)GameObjects.Sphere).transform;
        sphere1 = GetObject((int)GameObjects.Sphere1).transform;

        BindEvent(fPMButton, (PointerEventData data) => { FirstPersonMode(); }, Define.UIEvent.Click);
        BindEvent(DelButton, (PointerEventData data) => { Demolite(); }, Define.UIEvent.Click);

        if (towerControl.itemData.itemName == "Water")
        {
            fPMButton.SetActive(false);
        }
        else
        {
            fPMButton.SetActive(true);
        }
    }
    void Update()
    {
        if (target.turret == null)
        {
            if (Managers.UI.getPopStackTop()?.name == "NodeUI")
            {
                Managers.UI.ClosePopupUI();
            }
        }

        RectTransform uiElementRectTransform = GetComponentInChildren<RectTransform>();  // UI 요소의 RectTransform 컴포넌트 가져오기

        Vector3 anchoredPosition3D = uiElementRectTransform.anchoredPosition3D;  // UI 요소의 앵커드 위치 가져오기
        Vector3 screenPosition = uiElementRectTransform.TransformPoint(anchoredPosition3D);  // 화면 좌표계로 변환

        // screenPosition을 사용하여 UI 요소의 화면 좌표계 위치 활용
        Vector3 uiScreenPoint = screenPosition;  // UI의 화면 좌표계 위치

        Vector3 uiNormal = Camera.main.transform.forward;  // UI 평면의 법선 벡터
        Vector3 cameraPlaneNormal = -Camera.main.transform.forward;  // 카메라 평면의 법선 벡터

        Plane uiPlane = new Plane(uiNormal, uiScreenPoint);  // UI 평면 생성
        Plane cameraPlane = new Plane(cameraPlaneNormal, Camera.main.transform.position);  // 카메라 평면 생성

        float distance = Mathf.Abs(cameraPlane.GetDistanceToPoint(uiScreenPoint));

        uiElementRectTransform.localScale = new Vector3(distance * 0.003f, distance * 0.003f, 0f);  // 크기 조정

        float x = distance * 0.2f;
        x = Mathf.Clamp(x, 6f, 15f);
        float y = x * 0.5f;
        uiElementRectTransform.anchoredPosition3D = new Vector3(0f, x, y);

        ChangeInfo();
    }

    public void SetTarget(Node _target)
    {
        if (Managers.Game.isFPM)
        {
            return;
        }

        target = _target;

        transform.position = target.GetBuildPosition();

        towerControl = target.turret.GetComponent<TowerControl>();

        //retrunExp.text = 경헙치 + "Exp"  // 추가 예정

        // 왜 start 함수보다 먼저 실행되는지 모르겠는데 일단 해당 코드를 start 함수로 옮겨서 수정함
        //if (towerControl.itemData.itemName == "Water")
        //{
        //    fPMButton.SetActive(false);
        //}
        //else
        //{
        //    fPMButton.SetActive(true);
        //}
    }

    void ChangeInfo()
    {
        float curHP = towerControl._stat.HP;
        float maxHP = 100f;

        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = curHP / maxHP;

        GetText((int)Texts.damageText).text = towerControl._stat.BulletDamage.ToString("F0");
        GetText((int)Texts.fireRateText).text = towerControl._stat.FireRate.ToString("F1");

        sphere.localScale = new Vector3(towerControl._stat.Range * 2, sphere.localScale.y, towerControl._stat.Range * 2);
        sphere1.localScale = new Vector3(towerControl._stat.Range * 2 - 0.5f, sphere1.localScale.y, towerControl._stat.Range * 2 - 0.5f);

        if (curHP <= 0)
        {
            target.turret = null;
        }
    }

    public void FirstPersonMode()
    {
        Managers.Select.DeselectNode();
        FPSUI.GetTower(towerControl);
        target.FirstPersonMode();
    }

    public void Demolite()
    {
        Managers.Select.DeselectNode();
        target.DemoliteTower();
    }
}
