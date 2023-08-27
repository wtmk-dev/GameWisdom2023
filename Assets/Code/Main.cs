using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Main : MonoBehaviour
{
    private WTMK.Mechanics.Grid _Grid;

    [SerializeField]
    private GameObject _Unit_Prefab;
    [SerializeField]
    private GridUnitStatus _Target, _Player;
    [SerializeField]
    private UnitActionBar _ActionBar;
    [SerializeField]
    private GameObject _Start, _Create, _Game, _Credits, _Tutorial;
    [SerializeField]
    private GridUnit _PC;

    void Awake()
    {
        _StartScreen = _Start.GetComponent<StartScreen>();
        _CreateScreen = _Create.GetComponent<CreateScreen>();
        _GameScreen = _Game.GetComponent<GameScreen>();
        _TutorialScreen = _Tutorial.GetComponent<TutorialScreen>();

        _StartScreen.Start.onClick.AddListener(OnStartGame);
        _UnitFactory = GetComponent<UnitFactory>();
        _Grid = GetComponent<WTMK.Mechanics.Grid>();
        _Grid.Init();
    }

    void Start()
    {
        _PC.Init(100, 5f, _Player);
        _Grid.SetPC(_PC);
        StartCoroutine(SequenceBuild());
    }

    private IEnumerator SequenceBuild()
    {        
        
        var key = (4, 1);
        _PC.DoMove(_Grid.Map[key],()=> 
        {
            _BattleSystem = new BattleSystem(_PC, _Grid, _UnitFactory, _TutorialScreen);

            _StartScreen.gameObject.SetActive(true);

            CurrentGameScreen = Init;

            _CreateScreen.Init();

            _Create.SetActive(false);
            _Credits.SetActive(false);
            _Tutorial.SetActive(false);

        });

        yield return new WaitForEndOfFrame();

    }

    void Update()
    {
        if(CurrentGameScreen == Game)
        {
            _BattleSystem.Update();
        }

        if(_CreateScreen.TransitionReady)
        {
            _CreateScreen.TransitionReady = false;
            _Create.SetActive(false);
            TransitionGame();
        }
    }

    private void OnPlayerSelected(GridUnit unit)
    {
        
    }

    private void OnStartGame()
    {
        _StartScreen.gameObject.SetActive(false);
        TranstionCreate();
    }

    private void TransitionGame()
    {
        _Game.SetActive(true);
        CurrentGameScreen = Game;
        _PC.Body.sprite = _CreateScreen.t.Body.sprite;
        _PC.Default(_CreateScreen.t.Option);
        _GameScreen.StartTransition(_PC, _BattleSystem);
    }

    private void TranstionCreate()
    {
        _Create.gameObject.SetActive(true);
        CurrentGameScreen = Create;
        _CreateScreen.StartTransition(_PC);
    }

    private BattleSystem _BattleSystem;
    private UnitFactory _UnitFactory;
    private GameScreen _GameScreen;
    private StartScreen _StartScreen;
    private CreateScreen _CreateScreen;
    private TutorialScreen _TutorialScreen;
    private int CurrentGameScreen = -1, Init = 0, Game = 1, Create = 2, Credits = 3;
}
