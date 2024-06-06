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
            // 실행 중간에 _countLevelUp이 갱신되면 어케할지 고민중
            // 그냥 서버에서 관리를 하는 것이 맞는지
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

            // Hide() 함수가 실행되면 넘어가도록 해야됨
            isHide = false;
            yield return new WaitUntil(() => isHide);
        }

        isPractice = false;
    }
}
