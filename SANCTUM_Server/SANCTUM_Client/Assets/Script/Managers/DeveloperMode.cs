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
        // ��� ������
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("a");
            Debug.Log(Managers.Game.nextExp);
            Debug.Log(Managers.Game.exp);
            int remainExp = Managers.Game.nextExp - Managers.Game.exp;
            Managers.Game.GetExp(remainExp);
            //Managers.UI.ShowPopupUI<LevelUp>("LevelUpUI");
        }

        // ����
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

        // �� Ȯ��
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("d");
            map.ExpendMap();
        }

        // ������ ����
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("f");
            Managers.Sound.Play("Effects/Hit3", Define.Sound.Effect);
            //GameManager.instance.soundManager.Play("Effects/Hit3", SoundManager.Sound.Effect);
            Managers.Game.Lives--;
        }

        // ���� �����(escâ�� �ȶ� ��� ���)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("g");
            Managers.Scene.sceneFader.FadeTo(Define.Scene.GamePlay);
        }

        // ����ȭ�� �̵�(escâ�� �ȶ� ��� ���)
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("h");
            Managers.Scene.sceneFader.FadeTo(Define.Scene.MainMenu);
        }
    }
}
