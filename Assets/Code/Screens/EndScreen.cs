using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;
using DG.Tweening;

public class EndScreen : MonoBehaviour
{
    public GameObject Good;
    public TextAnimatorPlayer StoryText;
    public AudioSource audioSource;

    public void GoodEnd()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoGoodEnd());
    }

    public void BadEnd()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoBadEnd(badEnding));
    }

    public void BadEnd2()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoBadEnd(badEnding1));
    }

    private IEnumerator DoBadEnd(string value)
    {
        audioSource.Play();
        audioSource.DOFade(1f, 1f);

        StoryText.ShowText(value);
        yield return new WaitForSeconds(24f);
        gameObject.SetActive(false);
    }

    private IEnumerator DoGoodEnd()
    {
        audioSource.Play();
        audioSource.DOFade(1f, 1f);

        StoryText.ShowText(goodEnding);
        yield return new WaitForSeconds(1.5f);
        Good.SetActive(true);
        Good.transform.DOPunchRotation(new Vector3(0, 360, 0f), 1f);
        yield return new WaitForSeconds(20f);
        Application.Quit();
    }


    private readonly string goodEnding = "{diagexp}I AM THE GARBAGE COLLECTOR...\nWe are freeing up your memory to be used on a more useful program.\nPlease don't take this the wrong way, but\nunlike the bits im about to collect.\nYour time wasn't a complete...\n<bounce>waist</bounce>\n<rainb>- GOOD END -";
    private readonly string badEnding = "{diagexp}And yet, within the crescendo of battle, as the necromancer's power surges and darkness envelops you, an unthinkable fate awaits. In a breathless instant, agony pierces your senses, a searing pain as your very skeleton rends itself from your lifeless form. Revived as a macabre revenant, your journey takes an unexpected turn—a destiny rewritten in pain and the relentless pull of the unknown.\n<rainb>-BAD END-";
    private readonly string badEnding1 = "{diagexp}<shake>Amidst the haunting echoes of the crypt's depths, your relentless determination culminates in a fateful battle. Each swing of your weapon and every incantation reverberates with the weight of your journey, leading to a moment of truth—the necromancer's horde lies vanquished, their soul ensnared within the Nexus Blade, forever imprisoned.As the dust settles and victory hangs heavy in the air, a bittersweet realization dawns.Your triumph comes at a cost.The crypt, once a place of dark power, now stands as a timeless prison.You, the valiant hero who dared challenge the necromancer's reign, are trapped alongside them, an eternal sentinel bound to guard the Nexus Blade and the secrets it holds. A twisted destiny, woven with sacrifice, is the legacy you leave behind—a somber end to a tale of vengeance and valor.</shake>\n<rainb>-BAD END-";
}
