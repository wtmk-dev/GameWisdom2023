using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Febucci.UI;

public class StartScreen : MonoBehaviour
{
    public Button Start, Option, Credits, Close;
    public GameObject OptionGo, CreditsGo;

    public Button _VL, _VM, _VH, _DE, _DM, _DH;
    public TextAnimatorPlayer _V, _D;

    private void Awake()
    {
        Credits.onClick.AddListener(ShowCredits);
        Option.onClick.AddListener(ShowOption);
        Close.onClick.AddListener(Hide);

        _VL.onClick.AddListener(SetVolumeL);
        _VM.onClick.AddListener(SetVolumeM);
        _VH.onClick.AddListener(SetVolumeH);

        _DE.onClick.AddListener(SetDifficultyE);
        _DM.onClick.AddListener(SetDifficultyM);
        _DH.onClick.AddListener(SetDifficultyH);

        //SetDifficultyM();
        //SetVolumeH();
    }

    private void SetVolumeL()
    {
        AudioListener.volume = .1f;
        _V.ShowText("<wave>10%");
    }

    private void SetVolumeM()
    {
        AudioListener.volume = .5f;
        _V.ShowText("<wave>50%");
    }

    private void SetVolumeH()
    {
        AudioListener.volume = 1f;
        _V.ShowText("<wave>100%");
    }

    private void SetDifficultyE()
    {
        BattleSystem.Difficulty = 24;
        _D.ShowText("<wave>QQ");
    }

    private void SetDifficultyM()
    {
        BattleSystem.Difficulty = 14;
        _D.ShowText("<wave>DEATH AWAITS");
    }

    private void SetDifficultyH()
    {
        BattleSystem.Difficulty = 0;
        _D.ShowText("<wave>WHAT WOULD YOU LIKE YOUR TOMBSTONE TO SAY");
    }

    private void ShowCredits()
    {
        CreditsGo.gameObject.SetActive(true);
        Close.gameObject.SetActive(true);
    }

    private void ShowOption()
    {
        OptionGo.gameObject.SetActive(true);
        Close.gameObject.SetActive(true);
    }

    private void Hide()
    {
        OptionGo.gameObject.SetActive(false);
        CreditsGo.gameObject.SetActive(false);
        Close.gameObject.SetActive(false);
    }
}


