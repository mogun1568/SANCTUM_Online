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
    //public TextMeshProUGUI retrunExp; // �߰� ����

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

        RectTransform uiElementRectTransform = GetComponentInChildren<RectTransform>();  // UI ����� RectTransform ������Ʈ ��������

        Vector3 anchoredPosition3D = uiElementRectTransform.anchoredPosition3D;  // UI ����� ��Ŀ�� ��ġ ��������
        Vector3 screenPosition = uiElementRectTransform.TransformPoint(anchoredPosition3D);  // ȭ�� ��ǥ��� ��ȯ

        // screenPosition�� ����Ͽ� UI ����� ȭ�� ��ǥ�� ��ġ Ȱ��
        Vector3 uiScreenPoint = screenPosition;  // UI�� ȭ�� ��ǥ�� ��ġ

        Vector3 uiNormal = Camera.main.transform.forward;  // UI ����� ���� ����
        Vector3 cameraPlaneNormal = -Camera.main.transform.forward;  // ī�޶� ����� ���� ����

        Plane uiPlane = new Plane(uiNormal, uiScreenPoint);  // UI ��� ����
        Plane cameraPlane = new Plane(cameraPlaneNormal, Camera.main.transform.position);  // ī�޶� ��� ����

        float distance = Mathf.Abs(cameraPlane.GetDistanceToPoint(uiScreenPoint));

        uiElementRectTransform.localScale = new Vector3(distance * 0.003f, distance * 0.003f, 0f);  // ũ�� ����

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

        //retrunExp.text = ����ġ + "Exp"  // �߰� ����

        // �� start �Լ����� ���� ����Ǵ��� �𸣰ڴµ� �ϴ� �ش� �ڵ带 start �Լ��� �Űܼ� ������
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
