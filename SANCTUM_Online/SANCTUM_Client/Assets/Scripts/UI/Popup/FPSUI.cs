using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;

public class FPSUI : UI_Popup
{
    static public TowerControl towerControl;

    enum GameObjects
    {
        HpBar
    }
    enum Texts
    {
        HP
    }
    enum Images
    {
        Icon
    }

    void OnEnable()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
 
        GetImage((int)Images.Icon).sprite = Managers.Resource.Load<Sprite>($"Icon/{towerControl.itemData.itemIcon}");
    }

    void Update()
    {
        if (!Managers.Game.isLive)
        {
            return;
        }

        if (towerControl)
        {
            ChangeInfo();
        }
    }

    void ChangeInfo()
    {
        float curHP = towerControl._stat.HP;
        float maxHP = towerControl._stat.MaxHp;

        GetObject((int)GameObjects.HpBar).GetComponent<Slider>().value = curHP / maxHP;
        GetText((int)Texts.HP).text = towerControl._stat.HP.ToString("F0") + "/100";
    }

    static public void GetTower(TowerControl _tower)
    {
        towerControl = _tower;
    }
}
