using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Setting : UI_Popup
{
    AudioMixer audioMixer;

    Slider soundSlider;
    Slider mouseSlider;

    static float soundValue = 0.7f;
    static float mouseValue = 100;

    enum GameObjects
    {
        SoundSlider,
        MouseSlider,
    };

    enum Buttons
    {
        Button_Close
    };

    void Awake()
    {
        base.Init();

        audioMixer = Resources.Load<AudioMixer>("Sounds/NewAudioMixer");

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        soundSlider = GetObject((int)GameObjects.SoundSlider).GetComponent<Slider>();
        mouseSlider = GetObject((int)GameObjects.MouseSlider).GetComponent<Slider>();

        IntialSetting();

        soundSlider.onValueChanged.AddListener(Sound);
        mouseSlider.onValueChanged.AddListener(Mouse);

        BindEvent(GetButton((int)Buttons.Button_Close).gameObject, (PointerEventData data) => { Close(); }, Define.UIEvent.Click);

        Managers.Game.isSettingUI = true;
    }

    public void IntialSetting()
    {
        soundSlider.value = soundValue;
        mouseSlider.value = mouseValue;

        audioMixer.SetFloat("Master", Mathf.Log10(soundSlider.value) * 20);
        FirstPersonCamera.mouseSensitivitiy = mouseSlider.value;
    }


    void Sound(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
    }

    void Mouse(float value)
    {
        FirstPersonCamera.mouseSensitivitiy = value;
    }

    public void Close()
    {
        Managers.Sound.Play("Effects/UiClickLow", Define.Sound.Effect);

        ChangeScene();

        Managers.Game.isSettingUI = false;

        Managers.UI.ClosePopupUI();
    }

    public void ChangeScene()
    {
        soundValue = soundSlider.value;
        mouseValue = mouseSlider.value;
    }
}
