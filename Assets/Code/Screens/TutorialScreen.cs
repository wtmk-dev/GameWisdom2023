using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialScreen : MonoBehaviour
{
    public Button Confirm;
    public Sprite Necro;
    public EndScreen EndScreen;
    public Canvas Canvas;
    public AudioSource _BattleAudio, _EndAudio;

    public void SetActive(bool isActive)
    {
        Canvas.gameObject.SetActive(isActive);
    }

    public void StartBadEndTransition()
    {
        EndScreen.gameObject.SetActive(true);
        StartCoroutine(BadEnd());
    }

    public void StartBadEnd2Transition()
    {
        EndScreen.gameObject.SetActive(true);
        StartCoroutine(BadEnd2());
    }

    private IEnumerator BadEnd()
    {
        _BattleAudio.DOFade(0f, 0.3f);
        _EndAudio.DOFade(1f, 0.3f);
        _EndAudio.Play();
        EndScreen.BadEnd();
        yield return new WaitForSeconds(20f);
        EndScreen.gameObject.SetActive(false);
    }

    private IEnumerator BadEnd2()
    {

        _BattleAudio.DOFade(0f, 0.3f);
        _EndAudio.DOFade(1f, 0.3f);
        EndScreen.BadEnd2();
        yield return new WaitForSeconds(20f);
        EndScreen.gameObject.SetActive(false);
    }
}
