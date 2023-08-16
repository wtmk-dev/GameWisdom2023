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

    void Awake()
    {
        _Grid = GetComponent<WTMK.Mechanics.Grid>();
        _Grid.Init();
    }

    void Start()
    {
        var clone = Instantiate<GameObject>(_Unit_Prefab);
        _PC = clone.GetComponent<GridUnit>();
        _PC.CombatModel = new CombatModel();

        _PC.OnSelected += OnPlayerSelected;

        _Grid.SetPC(_PC);

        var key = (4, 1);
        _PC.DoMove(_Grid.Map[key]);

        _BattleSystem = new BattleSystem(_Grid, _PC, _UnitFactory);
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
        Debug.Log("unit");
    }

    private GridUnit _PC;
    private BattleSystem _BattleSystem;
    private UnitFactory _UnitFactory;
    private int CurrentGameScreen = -1, Init = 0, Game = 1, Credits = 2;
}
