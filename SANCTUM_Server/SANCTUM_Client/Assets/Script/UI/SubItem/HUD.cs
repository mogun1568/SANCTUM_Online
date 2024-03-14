using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public enum InfoType { Time, Live, Exp, Round }
    public InfoType type;

    Slider mySlider;
    TextMeshProUGUI myText;

    void Awake()
    {
        mySlider = GetComponent<Slider>();
        myText = GetComponent<TextMeshProUGUI>();
    }

    void LateUpdate()
    {
        if (!Managers.Game.isLive)
        {
            return;
        }

        switch (type)
        {
            case InfoType.Time:
                float gameTime = Managers.Game.gameTime;
                int min = Mathf.FloorToInt(gameTime / 60);
                int sec = Mathf.FloorToInt(gameTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Live:
                int live = Managers.Game.Lives;
                myText.text = live.ToString();
                break;
            case InfoType.Exp:
                float curExp = Managers.Game.exp;
                float maxExp = Managers.Game.nextExp;
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Round:
                int RoundCount = Managers.Game.Rounds;
                myText.text = RoundCount.ToString();
                break;

        }
    }
}
