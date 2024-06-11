using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using UnityEngine;

public class MyMapController : NewMap
{
    public int _countLevelUp;
    bool isPractice;
    public bool isHide;

    public Camera _mainCamera;

    protected override void Init()
    {
        base.Init();

        _countLevelUp = 0;
        _mainCamera = Camera.main;

        int cameraDefault = MapDefaultSize * 2 - 24;
        _mainCamera.transform.position = Pos + new Vector3(cameraDefault, 40, cameraDefault);
    }

    public override void Update()
    {
        //Debug.Log($"{_countLevelUp}, {isFPM}, {isPractice}");
        if (_countLevelUp > 0 && !IsFPM && !isPractice)
        {
            isPractice = true;
            // ���� �߰��� _countLevelUp�� ���ŵǸ� �������� �����
            // �׳� �������� ������ �ϴ� ���� �´���
            StartlevelUpCoroutine();
            _countLevelUp = 0;
        }
    }

    public void StartlevelUpCoroutine()
    {
        StartCoroutine(WaitForItemSelection());
    }

    IEnumerator WaitForItemSelection()
    {
        Debug.Log(_countLevelUp);

        while (_countLevelUp > 0)
        {
            if (Managers.UI.getPopStackTop()?.name == "NodeUI")
            {
                Managers.UI.ClosePopupUI();
            }
            Managers.UI.ShowPopupUI<LevelUp>("LevelUpUI");
            _countLevelUp--;

            // Hide() �Լ��� ����Ǹ� �Ѿ���� �ؾߵ�
            isHide = false;
            yield return new WaitUntil(() => isHide);
        }

        isPractice = false;
    }
}
