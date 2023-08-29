using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Febucci.UI;

public class CreateScreen : MonoBehaviour
{
    public bool TransitionReady = false;
    public TextAnimatorPlayer Title, StoryText, Profile;
    public WordSpawner WordSpawner;
    public GameObject Effect, CharacterSelection, Effect2;
    public List<Button> Selections;
    public List<TextAnimatorPlayer> SelectionText;
    public Image BackgroundEffect;
    public GridUnit _War, _Thif, _Mage, _Necro;
    public Transform SelectedPosition;
    public Button Complete, GoodEnd;
    public EndScreen EndScreen;
    public AudioSource _CreateAudio, _StartAudio;

    public void Init()
    {

        _War.OnSelected += (GridUnit unit) =>
        {
            OnClassSelected(0);
        };

        _Thif.OnSelected += (GridUnit unit) =>
        {
            OnClassSelected(1);
        };

        _Mage.OnSelected += (GridUnit unit) =>
        {
            OnClassSelected(2);
        };

        _CreationStep.RegisterEnter(CreationStep.Background, OnEnter_Background);
        _CreationStep.RegisterEnter(CreationStep.CallToAction, OnEnter_CallToAction);
        _CreationStep.RegisterEnter(CreationStep.Enemy, OnEnter_Enemy);
        _CreationStep.RegisterEnter(CreationStep.Trials, OnEnter_Trials);
        _CreationStep.RegisterEnter(CreationStep.RoadToHell, OnEnter_RoadToHell);
        _CreationStep.RegisterEnter(CreationStep.DeathOfAHero, OnEnter_DeathOfAHero);

        Selections[0].onClick.AddListener(() => { OnSelection(0); });
        Selections[1].onClick.AddListener(() => { OnSelection(1); });
        Selections[2].onClick.AddListener(() => { OnSelection(2); });

        for (int i = 0; i < Selections.Count; i++)
        {
            Selections[i].gameObject.SetActive(false);
        }

        Complete.onClick.AddListener(FinishCreation);
        Complete.gameObject.SetActive(false);

        GoodEnd.onClick.AddListener(GoodEnding);
        GoodEnd.gameObject.SetActive(false);
    }

    private void GoodEnding()
    {
        _CreateAudio.DOFade(0f, 0.3f);
        EndScreen.GoodEnd();
    }

    public void StartTransition(GridUnit unit)
    {
        _Player = unit;
        _StartAudio.DOFade(0f, 1f);

        Title.ShowText("{horiexp}<shake>Will the pain stop?</shake>{/horiexp}");        
        BackgroundEffect.DOFade(0f, 6f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            StartCoroutine(DoTransition());
        });
    }

    private List<float> _TransitonWaits = new List<float>() { 1.6f, 3.2f, 6f, 3f, 2f }; 
    private GridUnit _Player;
    private IEnumerator DoTransition()
    {
        WordSpawner.StartSpawn();
        Title.ShowText("{horiexp}<pend>You wouldn't want it to.</pend>{/horiexp}");
        yield return new WaitForSeconds(_TransitonWaits[0]);

        BackgroundEffect.DOFade(.2f, 0f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            Effect.gameObject.SetActive(true);
        });
        yield return new WaitForSeconds(_TransitonWaits[1]);
        StoryText.ShowText("{diagexp}<bounce>Amidst the remnants of the shattered reality, " +
            "three figures materialize—A battle-worn Warrior, a shadowy Thief, and an enigmatic Mage.</bounce>{/diagexp}");
        yield return new WaitForSeconds(_TransitonWaits[2]);
        CharacterSelection.SetActive(true);
        
        yield return new WaitForSeconds(_TransitonWaits[4]);
        Title.ShowText("{rdir}<incr>REMEMBER!?</incr>{/rdir}");

        _CreateAudio.Play();
        _CreateAudio.DOFade(1f, 0.6f);        
    }

    bool hasSelected;
    public GridUnit t = null;
    private void OnClassSelected(int selection)
    {
        if(hasSelected)
        {
            return;
        }

        hasSelected = true;

        string c = "";

        if (selection == 0)
        {
            _Thif.gameObject.SetActive(false);
            _Mage.gameObject.SetActive(false);

            t = _War;
            c = "WARRIOR";

            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Block));
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.WhirlWind));
        }

        if (selection == 1)
        {
            _War.gameObject.SetActive(false);
            _Mage.gameObject.SetActive(false);

            t = _Thif;
            c = "THIEF";

            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.TrickAttack));
            _Player.SetCounter(_AbilityFactory.GetCounter(CounterType.Dodge));
        }

        if (selection == 2)
        {
            _War.gameObject.SetActive(false);
            _Thif.gameObject.SetActive(false);

            t = _Mage;
            c = "MAGE";

            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.FireBall));
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Teleport));
        }

        StoryText.ShowText("{diagexp}<bounce>The past beckons, and you are drawn into the depths of their memories, " +
            "where the foundation of their destiny lies, waiting to be etched in shadow and light.</bounce>{/diagexp}");

        Title.ShowText("{fade}" + c + "{/fade}");

        t.transform.DOMove(SelectedPosition.position, 3.2f).SetEase(Ease.InOutFlash);

        BackgroundEffect.DOFade(0f, 3.2f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            Effect.SetActive(false);
            Effect2.SetActive(true);
        });

        StartCoroutine(DoStartStory());
    }

    private float _StartStoryWaint = 12f;
    private IEnumerator DoStartStory()
    {
        yield return new WaitForSeconds(0f);
        _CreationStep.StateChange(CreationStep.Background);
    }

    private IEnumerator _Coroutine;
    private StateActionMap<CreationStep> _CreationStep = new StateActionMap<CreationStep>();
    private string _ProfileString;

    private void ToggleSelection(bool isActive)
    {
        for (int i = 0; i < Selections.Count; i++)
        {
            Selections[i].gameObject.SetActive(isActive);
        }
    }

    private void OnSelection(int option)
    {
        if (_CreationStep.CurrentState == CreationStep.Background)
        {
            ToggleSelection(false);

            if (option == 0)
            {
                _ProfileString += Noble_Ancestry;
                _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Potion));
                _Player.SetPassive(_AbilityFactory.GetPassive(PassiveType.Noble));
                
            } else if (option == 1)
            {
                _ProfileString += Orphaned_Existence;
                _Player.SetPassive(_AbilityFactory.GetPassive(PassiveType.Orphan));
            }
            else if (option == 2)
            {
                _ProfileString += Disciplined_Pursuit;
                _Player.SetPassive(_AbilityFactory.GetPassive(PassiveType.Disciplined));
            }

            WordSpawner.StopSpawn();
            StartCoroutine(TransitionState(CreationStep.CallToAction));
        }

        if (_CreationStep.CurrentState == CreationStep.CallToAction)
        {
            ToggleSelection(false);

            _ProfileString += "\n";

            if (option == 0)
            {
                _ProfileString += SiblingsSteel;
                _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Sword));
            }
            else if (option == 1)
            {
                _ProfileString += FatesEmbrace;
                _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Dagger));
            }
            else if (option == 2)
            {
                _ProfileString += HungryForKnowlage;
                _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Staff));
            }

            Effect2.SetActive(false);
            t.Default(option);
            StartCoroutine(TransitionState(CreationStep.Enemy));
        }

        if (_CreationStep.CurrentState == CreationStep.Enemy)
        {
            ToggleSelection(false);
            _ProfileString += "\n" + Enemy;

            StartCoroutine(TransitionState(CreationStep.Trials));
        }

        if (_CreationStep.CurrentState == CreationStep.Trials)
        {
            if(_Trials == 0)
            {
                var vengace = _AbilityFactory.GetActivateWhenReady(ActivateWhenReadyType.Vengance);
                _Player.SetActivateWhenReady(vengace);
            }
            else if (_Trials == 1)
            {
                //AddTomb(option);
            }
            else if (_Trials == 2)
            {
                //AddRune(option);
            }

            ToggleSelection(false);

            if (_Trials < 2)
            {
                StartCoroutine(TransitionState(CreationStep.Trials));
            }
            else
            {
                StartCoroutine(TransitionState(CreationStep.RoadToHell));
            }
            _Trials++;
        }

        if (_CreationStep.CurrentState == CreationStep.DeathOfAHero)
        {
            ToggleSelection(false);

            _ProfileString += "\n";

            if(option == 0)
            {
                _ProfileString += Embrace;
            }else if(option == 1)
            {
                _ProfileString += Yield;
            }
            else if (option == 2)
            {
                _ProfileString += Become;
            }

            StartCoroutine(TellPlayerStory());
        }
    }

    private IEnumerator TellPlayerStory()
    {
        StoryText.ShowText(_ProfileString);
        t.gameObject.SetActive(true);
        Complete.gameObject.SetActive(true);
        GoodEnd.gameObject.SetActive(true);
        yield return null;
    }

    private void FinishCreation()
    {
        _CreateAudio.DOFade(0f, 0.3f);

        t.gameObject.SetActive(false);
        Complete.gameObject.SetActive(false);
        GoodEnd.gameObject.SetActive(false);
        TransitionReady = true;
    }

    private void AddTomb(int option)
    {
        if(option == 0)
        {
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Soulshatter));
        }else if(option == 1)
        {
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Shadows));
        }
        else if (option == 2)
        {
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.Hex));
        }
    }

    private void AddRune(int option)
    {
        if (option == 0)
        {
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.RuneOfPower));
        }
        else if (option == 1)
        {
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.RuneOfCourage));
        }
        else if (option == 2)
        {
            _Player.SetAbility(_AbilityFactory.GetAbility(AbilityType.RuneOfWisdom));
        }
    }

    private float _TransitionStateTime = 3f;
    private IEnumerator TransitionState(CreationStep state)
    {
        yield return new WaitForSeconds(0);
        _CreationStep.StateChange(state);
    }

    private void OnEnter_Background()
    {
        var pos = StoryText.transform.position;
        StoryText.transform.position = new Vector3(pos.x, pos.y, 0);

        Title.ShowText("{fade} BACKGROUND {fade}");
        StoryText.ShowText("As life's threads converge, fragments of your origins stir. " +
                           "path—born of nobility's whispers, " +
                           "raised in the echoes of abandonment, " +
                           "or nurtured within the halls of discipline. " +
                           "Your past shapes the echoes of your existence.");

        ToggleSelection(true);

        SelectionText[0].ShowText("Noble Ancestry");
        SelectionText[1].ShowText("Orphaned Existence");
        SelectionText[2].ShowText("Disciplined Pursuit");
    }

    private void OnEnter_CallToAction()
    {
        Title.ShowText("{fade} THE CALL {fade}");
        StoryText.ShowText("A moment etched in time, an event that fractured your existence. " +
                           "Amidst spectral echoes, recall the call to arms—an invitation to face the inevitable. " +
                           "Darkness rises, shadows converge. The past and present entwine, " +
                           "guiding you through the mist of memory toward your fated reckoning.");

        ToggleSelection(true);

        SelectionText[0].ShowText("Sibling's Steel");
        SelectionText[1].ShowText("Fate's Embrace");
        SelectionText[2].ShowText("Hunger for Knowledge");
    }

    private void OnEnter_Enemy()
    {
        Title.ShowText("{fade} YOUR ENEMY {fade}");
        StoryText.ShowText("{fade}From the depths emerges your nemesis, veiled in enigma. An entity unknown, their identity a riddle, a mirror to your own journey. What darkness calls forth your opposing force?");

        ToggleSelection(true);
        _Necro.gameObject.SetActive(true);

        SelectionText[0].ShowText("Sinister Necromancer");
        SelectionText[1].ShowText("Sinister Necromancer");
        SelectionText[2].ShowText("Sinister Necromancer");
    }

    private int _Trials = 0;
    private readonly string _Trials_0 = "Among the haunting crypts, you confronted the necromancer's minions—servants of death with an unearthly hunger. Your only friend fell to their dark magic, only to rise as an abomination. Narrowly escaping with your life, you unlocked a power within you—a spark of magic, a latent strength. This encounter etched a thirst for vengeance into your soul";
    private readonly string _Trials_1 = "Years later, within the shadowed halls of an occult temple, you confronted the necromancer's disciples—adepts of the dark arts. The temple's air pulsed with the echoes of forbidden rituals, revealing a sinister truth—you must imbue your weapon with the arcane nexus's power to banish the necromancer. Three tombs lay before you, each resonating with necromantic power. Choose your path and claim your newfound abilities.";
    private readonly string _Trials_2 = "As years of pursuit led you to the brink of ultimate confrontation, the whispering winds of arcane knowledge guided you toward the arcane nexus—a place shrouded in mystic power and guarded by the necromancer's final acolyte. The path to the nexus remained veiled, requiring you to unravel hidden glyphs and esoteric symbols. Guided by your choices and honed abilities, you unlocked the door to the ethereal heart of power—a gateway to your final destiny";

    private void OnEnter_Trials()
    {
        ToggleSelection(true);

        var t = "";
        if (_Trials == 0)
        {
            Title.ShowText("{fade}  Crypt Ambush {fade}");

            t = _Trials_0;

            SelectionText[0].ShowText("Vengeance's Resilience");
            SelectionText[1].ShowText("Vengeance's Resilience");
            SelectionText[2].ShowText("Vengeance's Resilience");

        }
        else if (_Trials == 1)
        {
            Title.ShowText("{fade}  Occult Ambush {fade}");

            t = _Trials_1;

            SelectionText[0].ShowText("Tomb of Soulshatter");
            SelectionText[1].ShowText("Tomb of Ephemeral Shadows");
            SelectionText[2].ShowText("Tomb of Withering Hex");
        } else if (_Trials == 2)
        {
            Title.ShowText("{fade}  Unveiling the Nexus {fade}");

            t = _Trials_2;

            SelectionText[0].ShowText("Nexus Rune of Power");
            SelectionText[1].ShowText("Nexus Rune of Courage");
            SelectionText[2].ShowText("Nexus Rune of Wisdom");
        }

        StoryText.ShowText(t);
    }

    private void OnEnter_RoadToHell()
    {
        Title.ShowText("{fade} ROAD TO HELL {fade}");
        _Necro.gameObject.SetActive(false);
        StartCoroutine(SequenceRoadToHellText());
    }

    private List<float> _RoadToHellTimes = new List<float>() { 24f, 24f };
    private IEnumerator SequenceRoadToHellText()
    {
        StoryText.ShowText("{fade}With the echoes of your journey converging, you stand at the crossroads of destiny, poised on the precipice of the necromancer's castle—the final battlefield of your relentless pursuit. Yet, before the clash that will shape fate's tapestry, you encounter a blind seer whose gaze pierces the veil of reality.Their fingers bestow upon you an amulet—the Twin Phoenix Amulet—relic of a land untamed by the shackles of reality, where echoes of power and eternity intertwine. 'The amulet's heart beats with the rhythm of duality, ' the seer murmurs, 'twins in unity, fire and ash, life and death.");
        yield return new WaitForSeconds(22f);
        StoryText.ShowText("{fade}The amulet's weight is more than mere gold and gems—it carries the weight of ancient prophecies and forgotten truths. As your fingers touch its cool surface, you feel the heartbeat of two souls, twining like serpents of power. The seer's words linger, hinting at its connection to the looming battle and your own fate. Armed with the amulet's enigmatic power, you set forth, stepping into the shadow of the necromancer's castle, where reality and nightmares merge—a testament to the trials you've conquered and the choices that have shaped you.");
        yield return new WaitForSeconds(22f);
        _CreationStep.StateChange(CreationStep.DeathOfAHero);
    }

    private void OnEnter_DeathOfAHero()
    {
        Title.ShowText("{fade}  {fade}");
        StartCoroutine(SequenceDeathOfAHero());
    }

    private List<float> _DeathOfAHeroTimes = new List<float>() { 24f, 24f, 24f, 24f };
    private IEnumerator SequenceDeathOfAHero()
    {
        StoryText.ShowText("{fade}Stepping into the crypt's shadows, you venture deeper—a lone figure embracing destiny's call. The air grows heavy with the scent of ancient dust and the palpable tension of your purpose. Each step carries the weight of battles fought, choices made, and the ghosts of companions lost along the way. The echoes of your past resonate like the final notes of a symphony as you navigate the crypt's twists and turns, drawn ever closer to the heart of darkness.");
        yield return new WaitForSeconds(24f);
        StoryText.ShowText("{fade}Torches flicker like fading stars, guiding you through the crypt's labyrinthine corridors. Unearthly whispers and the distant echoes of your own heartbeats create an eerie melody that accompanies your descent. At last, the crypt yields to a grand chamber—a nexus of arcane energy, cloaked in a shroud of ethereal mist. There, amidst a symphony of shadows, stands the necromancer, their power palpable, their intentions dark.");
        yield return new WaitForSeconds(24f);
        StoryText.ShowText("{fade}The clash ensues—a dance of blades, incantations, and eldritch forces that reshape reality. With each strike, the necromancer's power and mastery over death become more evident. Yet, your resolve remains unbroken. As the battle rages on, the necromancer's dark magic begins to sap your strength. Wounds accumulate, and your movements grow sluggish. In the heart of the battle, the Twin Phoenix Amulet's ethereal glow intensifies, resonating with the rhythm of your struggle.");
        yield return new WaitForSeconds(24f);
        StoryText.ShowText("{fade}As the weight of the amulet presses against your chest, an enigmatic power awakens within—the very power the blind seer spoke of. The amulet, a conduit between life and death, pulses with an energy you've never felt before. Its resonance mingles with your heartbeat, weaving a symphony that harmonizes with your very soul.");

        ToggleSelection(true);

        SelectionText[0].ShowText("Embrace the Abyss");
        SelectionText[1].ShowText("Yield to the Shadows");
        SelectionText[2].ShowText("Become the Hero");
    }

    private AbilityFactory _AbilityFactory = new AbilityFactory();
    private GridUnit _GridUnit;

    private readonly string Noble_Ancestry = "Memories of opulent halls and whispered secrets cling to you. Once a scion of nobility, you were raised amidst wealth and power.";
    private readonly string Orphaned_Existence = "Eyes haunted by distant memories, you recall the loneliness of your orphaned past. Each echoing footstep was a testament to survival.";
    private readonly string Disciplined_Pursuit = "Discipline forged your character, your journey an embodiment of relentless pursuit. The halls of learning and trials of training sculpted your identity";
    private readonly string SiblingsSteel = "Amidst ancestral halls, your eyes met those of your sibling. A rivalry ignited, their sword clashing against yours in a dance of sparks. The blades symbolized your shared strength, a pact bound in rivalry and blood. Each swing of the sword echoes with your connection, driving you forward.";
    private readonly string FatesEmbrace = "Beneath the moon's pale light, you stumbled upon a figure cloaked in shadows. Their daggers moved with deadly grace, each strike an offering of fate's design. As they beckoned you to join the dance, your journey entwined with destiny. The daggers' embrace whispers secrets of concealed paths";
    private readonly string HungryForKnowlage = "In the mundane trappings of life, an ordinary staff caught your eye. Yet, as your hands grasped its form, an insatiable thirst for knowledge was ignited. You delved into arcane tomes and ancient scrolls, the staff your conduit to forgotten truths. A lust for power pushes you toward depths uncharted.";
    private readonly string Enemy = "From the depths of your past, an entity emerges—a sinister necromancer cloaked in shadows. Once an ally, they delved into the dark arts and were consumed by a lust for power. A betrayal darker than death itself, they tore the fabric of trust, leaving a trail of shattered loyalties. With a staff that channels forbidden necromancy, their motive is as vile as their methods—undeniable supremacy over life and death. A sinister legacy of betrayal and forbidden lore intertwines with your path, drawing you both toward an inescapable confrontation. As the threads of fate unravel, this necromancer's sinister scheme looms, seeking to bend the balance of life and death to their twisted will.";
    private readonly string Embrace = "The battle rages, your strength wanes, and the pull of the amulet grows stronger. The specter of death lingers, tempting you to surrender to its embrace, to let go and embrace a new existence.";
    private readonly string Yield = "Amidst the onslaught, the shadows seem welcoming, promising rest and the serenity of oblivion. Perhaps it's time to yield to the darkness, to meld with the shadows and cease the struggle.";
    private readonly string Become = "Though weakened, the embers of your heroism still burn. The amulet's glow mirrors your tenacity, and the prospect of surrendering remains distant. To give into death now would be to abandon the battles fought, the choices made.";
}

public enum CreationStep
{
    Background,
    CallToAction,
    Enemy,
    Trials,
    RoadToHell,
    DeathOfAHero
}
