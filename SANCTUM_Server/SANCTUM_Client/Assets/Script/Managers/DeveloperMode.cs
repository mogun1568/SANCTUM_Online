using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveloperMode : MonoBehaviour
{
    [SerializeField] NewMap map;

    void Start()
    {
        
    }

    void Update()
    {
        // 즉시 레벨업
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("a");
            Debug.Log(Managers.Game.nextExp);
            Debug.Log(Managers.Game.exp);
            int remainExp = Managers.Game.nextExp - Managers.Game.exp;
            Managers.Game.GetExp(remainExp);
            //Managers.UI.ShowPopupUI<LevelUp>("LevelUpUI");
        }

        // 정지
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("s");
            if (Managers.Game.isLive)
            {
                Managers.Game.Stop();
            }
            else
            {
                Managers.Game.Resume();
            }

        }

        // 맵 확장
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("d");
            map.ExpendMap();
        }

        // 라이프 조절
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("f");
            Managers.Sound.Play("Effects/Hit3", Define.Sound.Effect);
            //GameManager.instance.soundManager.Play("Effects/Hit3", SoundManager.Sound.Effect);
            Managers.Game.Lives--;
        }

        // 게임 재시작(esc창이 안뜰 경우 대비)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("g");
            Managers.Scene.sceneFader.FadeTo(Define.Scene.GamePlay);
        }

        // 메인화면 이동(esc창이 안뜰 경우 대비)
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("h");
            Managers.Scene.sceneFader.FadeTo(Define.Scene.MainMenu);
        }
    }
}
