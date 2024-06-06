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
    public bool isHide, isFPM;

    protected override void Init()
    {
        base.Init();

        _countLevelUp = 0;

        int cameraDefault = MapDefaultSize * 2 - 24;
        Camera.main.transform.position = Pos + new Vector3(cameraDefault, 40, cameraDefault);
    }

    public override void Update()
    {
        //Debug.Log($"{_countLevelUp}, {isFPM}, {isPractice}");
        if (_countLevelUp > 0 && !isFPM && !isPractice)
        {
            isPractice = true;
            // ���� �߰��� _countLevelUp�� ���ŵǸ� �������� �����
            // �׳� �������� ������ �ϴ� ���� �´���
            StartCoroutine(WaitForItemSelection(_countLevelUp));
            _countLevelUp = 0;
        }
    }

    IEnumerator WaitForItemSelection(int countLevelUp)
    {
        while (countLevelUp > 0)
        {
            if (Managers.UI.getPopStackTop()?.name == "NodeUI")
            {
                Managers.UI.ClosePopupUI();
            }
            Managers.UI.ShowPopupUI<LevelUp>("LevelUpUI");
            countLevelUp--;

            // Hide() �Լ��� ����Ǹ� �Ѿ���� �ؾߵ�
            isHide = false;
            yield return new WaitUntil(() => isHide);
        }

        isPractice = false;
    }
}
