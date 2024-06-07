using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using Google.Protobuf.Protocol;

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


    Node _node;
    Turret _turret;
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

        if (_turret.Stat.Name == "Water")
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
        if (_turret == null)
        {
            if (Managers.UI.getPopStackTop()?.name == "NodeUI")
            {
                Managers.UI.ClosePopupUI();
            }

            return;
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

    public void SetTarget(Node node, Turret turret)
    {
        if (Managers.Object.MyMap.isFPM)
        {
            return;
        }

        _node = node;

        _turret = turret;

        transform.position = _node.GetBuildPosition();

        //retrunExp.text = ����ġ + "Exp"  // �߰� ����

        // �� start �Լ����� ���� ����Ǵ��� �𸣰ڴµ� �ϴ� �ش� �ڵ带 start �Լ��� �Űܼ� ������
        //if (_turret.Stat.Name == "Water")
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
        float curHP = _turret.Hp;
        float maxHP = _turret.Stat.MaxHp;

        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = curHP / maxHP;

        GetText((int)Texts.damageText).text = _turret.Stat.Attack.ToString("F0");
        GetText((int)Texts.fireRateText).text = _turret.Stat.FireRate.ToString("F1");

        sphere.localScale = new Vector3(_turret.Stat.Range * 2, sphere.localScale.y, _turret.Stat.Range * 2);
        sphere1.localScale = new Vector3(_turret.Stat.Range * 2 - 0.5f, sphere1.localScale.y, _turret.Stat.Range * 2 - 0.5f);

        if (curHP <= 0)
        {
            _turret = null;
        }
    }

    public void FirstPersonMode()
    {
        Managers.Select.DeselectNode();

        C_FirstPersonMode firstPersonModePacket = new C_FirstPersonMode();
        firstPersonModePacket.IsFPM = true;
        firstPersonModePacket.TurretId = _turret.Id;
        Managers.Network.Send(firstPersonModePacket);

        FPSUI.GetTower(_turret);
        _node.FirstPersonMode(_turret);
    }

    public void Demolite()
    {
        Managers.Select.DeselectNode();

        C_TurretDemolite turretDemolitePacket = new C_TurretDemolite();
        turretDemolitePacket.NodeId = _node.Id;
        Managers.Network.Send(turretDemolitePacket);

        //_node.DemoliteTower();
    }
}
