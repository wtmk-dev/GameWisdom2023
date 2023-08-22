using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject _Start, _Create, _Game, _Credits;
    void Awake()
    {
        _StartScreen = _Start.GetComponent<StartScreen>();
        _StartScreen.Start.onClick.AddListener(OnStartGame);
        _UnitFactory = GetComponent<UnitFactory>();
        _Grid = GetComponent<WTMK.Mechanics.Grid>();
        _Grid.Init();
    }

    void Start()
    {
        var clone = Instantiate<GameObject>(_Unit_Prefab);

        _PC = clone.GetComponent<GridUnit>();
        _PC.Init(100, 5f, _Player);

        _PC.OnSelected += OnPlayerSelected;

        _Grid.SetPC(_PC);

        var key = (4, 1);
        _PC.DoMove(_Grid.Map[key]);

        _BattleSystem = new BattleSystem(_PC, _Grid, _UnitFactory);

        _StartScreen.gameObject.SetActive(true);
        CurrentGameScreen = Init;
    }

    void Update()
    {
        if(CurrentGameScreen == Game)
        {
            _BattleSystem.Update();
        }
    }

    private void OnPlayerSelected(GridUnit unit)
    {
        
    }

    private void OnStartGame()
    {
        _StartScreen.gameObject.SetActive(false);
        _Create.gameObject.SetActive(true);

        CurrentGameScreen = Create;
    }

    private GridUnit _PC;
    private BattleSystem _BattleSystem;
    private UnitFactory _UnitFactory;
    private StartScreen _StartScreen;
    private int CurrentGameScreen = -1, Init = 0, Game = 1, Create = 2, Credits = 3;
}
