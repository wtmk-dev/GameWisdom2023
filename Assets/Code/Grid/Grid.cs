using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public Dictionary<(int, int), GridTile> GridMap => _GridMap;

    public void Init()
    {
        _View.SetActive(true);

        foreach (var tile in _GridTiles)
        {
            var pos = ((int)tile.GridPosition.x, (int)tile.GridPosition.y);

            if (!_GridMap.ContainsKey(pos))
            {
                _GridMap.Add(pos, tile);
            }
            else
            {
                Debug.LogError("Check Grid Tiles :: tile already in map");
            }
        }

        _GridBattle.RegisterEnter(GridState.SetUp, OnEnter_SetUp);
        _GridBattle.RegisterEnter(GridState.StartOfRound, OnEnter_StartOfRound);
        _GridBattle.RegisterEnter(GridState.TakeTurn, OnEnter_TakeTurn);
        _GridBattle.RegisterEnter(GridState.CheckForActivation, OnEnter_CheckForActivation);
        _GridBattle.RegisterEnter(GridState.EndOfRound, OnEnter_EndOfRound);
        _GridBattle.RegisterEnter(GridState.Interrupt, OnEnter_Interrupt);

        _GridBattle.StateChange(GridState.SetUp);

        _EndTrun.onClick.AddListener(OnClick_EndTurn);
    }

    [SerializeField]
    private TextAnimatorPlayer _Clock;
    [SerializeField]
    private TextAnimatorPlayer _Score;
    [SerializeField]
    private GridBall _Ball;
  
    [SerializeField]
    private GameObject _View;
    [SerializeField]
    private List<GridTile> _GridTiles;
    [SerializeField]
    private GridPlayer _LeftPlayer, _RightPlayer;
    [SerializeField]
    private Button _EndTrun;
    [SerializeField]
    private GameObject _LeftPossetion, _RightPosition, _LeftTurn, _RightTurn;

    private Dictionary<(int, int), GridTile> _GridMap = new Dictionary<(int, int), GridTile>();
    private StateActionMap<GridState> _GridBattle = new StateActionMap<GridState>();

    private void OnEnter_SetUp()
    {
        _LeftPossetion.SetActive(false);
        _LeftTurn.SetActive(false);

        _RightPosition.SetActive(false);
        _RightTurn.SetActive(false);

        var lBase = _GridMap[(0, 1)];
        var lChamp = _GridMap[(3, 1)];

        var rBase = _GridMap[(8, 1)];
        var rChamp = _GridMap[(5, 1)];

        _Turn = -1;
        //pick first player
        //set losing player as current player
        _PreviousPlayer = _LeftPlayer; 
        _CurrentPlayer = _RightPlayer;

        var ballPos = _GridMap[(5, 0)];
        
        _Ball.Init(_GridMap);

        _Ball.DoMove(ballPos, () => 
        {
            _GridBattle.StateChange(GridState.StartOfRound);
        });

        _LeftPlayer.SetUp(lBase, lChamp, _Ball, _GridMap);
        _RightPlayer.SetUp(rBase, rChamp, _Ball, _GridMap);

        GridUnit.OnBallPickedUp += OnBallPickedUp;

        _LeftPlayer.OnMoveBallToUnit += MoveBallToUnit;
        _RightPlayer.OnMoveBallToUnit += MoveBallToUnit;

        _LeftPlayer.OnUnitClickNotOnTurn += _RightPlayer.ResolveUnitClickNotOnTurn;
        _RightPlayer.OnUnitClickNotOnTurn += _LeftPlayer.ResolveUnitClickNotOnTurn;

        _LeftPlayer.OnBounceBall += _Ball.Bounce;
        _RightPlayer.OnBounceBall += _Ball.Bounce;
    }

    private void MoveBallToUnit(GridUnit unit)
    {
        _Ball.DoMove(unit.CurrentPosition);
    }

    private void OnBallPickedUp(GridUnit gridUnit)
    {
        if(_LeftPlayer._UnitsOnGrid.Contains(gridUnit))
        {
            _LeftPossetion.gameObject.SetActive(true);
            _RightPosition.gameObject.SetActive(false);
        }else if(_RightPlayer._UnitsOnGrid.Contains(gridUnit))
        {
            _LeftPossetion.gameObject.SetActive(false);
            _RightPosition.gameObject.SetActive(true);
        }
    }

    private void OnEnter_StartOfRound()
    {
        _Turn++;
        _Clock.ShowText($"TURN \n {_Turn}");
        _Score.ShowText($"{_LeftScore} X {_RightScore}");

        _LeftPlayer.StartOfRound();
        _RightPlayer.StartOfRound();

        SwapCurrentPlayer();

        _GridBattle.StateChange(GridState.CheckForActivation);
    }

    private void SwapCurrentPlayer()
    {
        if (_CurrentPlayer.IsSelectingAction)
        {
            Debug.Log("Player is selecting an action");
            return;
        }

        var current = _PreviousPlayer;
        _PreviousPlayer = _CurrentPlayer;

        _CurrentPlayer = current;

        _PreviousPlayer.EndTurn();
        _CurrentPlayer.TakeTurn(_Turn);
    }

    private void OnEnter_CheckForActivation()
    {
        if(_CurrentPlayer.HasActivated || !_CurrentPlayer.HasActiveUnit(_Turn))
        {
            _GridBattle.StateChange(GridState.EndOfRound);
        }
        else
        {
            _GridBattle.StateChange(GridState.TakeTurn);
        }
    }

    private void OnEnter_TakeTurn()
    {
        if(_CurrentPlayer == _RightPlayer)
        {
            _LeftTurn.SetActive(false);
            _RightTurn.SetActive(true);
        }else
        {
            _LeftTurn.SetActive(true);
            _RightTurn.SetActive(false);
        }

        Debug.Log($"{_CurrentPlayer.gameObject.name} Player will take turn");
    }

    private void OnEnter_EndOfRound()
    {
        if(_PreviousPlayer.HasActiveUnit(_Turn))
        {
            _PreviousPlayer.TakeTurn(_Turn);

            SwapCurrentPlayer();
            _GridBattle.StateChange(GridState.CheckForActivation);
        }else if(_CurrentPlayer.HasActivated)
        {
            _GridBattle.StateChange(GridState.StartOfRound);
        }
    }

    private void OnEnter_Interrupt()
    {

    }

    private void OnClick_EndTurn()
    {
        Debug.Log("Player End turn");
        _GridBattle.StateChange(GridState.CheckForActivation);
    }

    //model
    private int _Turn, _LeftScore, _RightScore;

    private GridPlayer _PreviousPlayer;
    private GridPlayer _CurrentPlayer;
}

public enum GridState
{
    SetUp,
    StartOfRound,
    CheckForActivation,
    TakeTurn,
    EndOfRound,
    Interrupt
}
