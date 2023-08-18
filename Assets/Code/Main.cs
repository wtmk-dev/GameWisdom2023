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

    void Awake()
    {
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
    }

    void Update()
    {
        if(CurrentGameScreen == Game)
        {
            _BattleSystem.Update();
        }

        if(Input.GetKey(KeyCode.Space))
        {
            _PC.CombatModel.BattleState = UnitBattleState.Waiting;
        }

        if(_PC.CombatModel.BattleState == UnitBattleState.Ready)
        {
            _ActionBar.gameObject.SetActive(true);
        }
        else
        {
            _ActionBar.gameObject.SetActive(false);
        }
    }

    private void OnPlayerSelected(GridUnit unit)
    {
        
    }

    private GridUnit _PC;
    private BattleSystem _BattleSystem;
    private UnitFactory _UnitFactory;
    private int CurrentGameScreen = -1, Init = 0, Game = 1, Credits = 2;
}
