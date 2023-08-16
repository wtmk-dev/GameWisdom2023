using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridPlayer : MonoBehaviour
{
    public event Action<GridUnit> OnMoveBallToUnit;
    public event Action<GridTile> OnUnitClickNotOnTurn;
    public event Action OnBounceBall;

    [SerializeField]
    private bool _IsLeftPlayer;
    [SerializeField]
    private List<GridAction> _Actions;
    [SerializeField]
    private GameObject _ActionSelection;
    public List<GridUnit> _UnitsOnGrid;
    public bool IsTurn { get; private set; }
    public bool HasDeployed { get; private set; }
    public bool HasActivated { get; private set; }
    public bool IsSelectingAction { get => _ActionSelection.activeInHierarchy; }

    public void ResolveUnitClickNotOnTurn(GridTile gridTile)
    {
        Debug.Log(_CurrentState);
        Debug.Log(gridTile.GridPosition.x);
        Debug.Log(gridTile.GridPosition.y);

        if(_CurrentState == TurnState.Throw)
        {
            OnClick_Tile(gridTile);
        }
    }

    public void StartOfRound()
    {
        HasDeployed = false;
        HasActivated = false;
    }

    public void TakeTurn(int clock)
    {
        Debug.Log($"is left player {_IsLeftPlayer}");

        IsTurn = true;
        HasActivated = false;
        _CurrentClock = clock;
        _StateActionMap.StateChange(TurnState.Active);
    }

    public void EndTurn()
    {
        IsTurn = false;
    }
    
    public void SetUp(GridTile basePos, GridTile champPos, GridBall ball, Dictionary<(int, int), GridTile> tileMap)
    {
        _UnitsOnGrid = new List<GridUnit>();

        _TileMap = tileMap;
        _Ball = ball;

        //RegisterBallEvents();

        /*
        _Champion.Life = 9;
        _Champion.CurrentPosition = champPos;
        _Champion.transform.position = champPos.transform.position;
        champPos.IsOccupied = true;

        _Base.CurrentPosition = basePos;
        _Base.transform.position = basePos.transform.position;
        basePos.IsOccupied = true;

        _UnitsOnGrid.Add(_Champion);

        RegisterGridUnit(_Champion);
        ResetGridUnit(_Champion);

        for (int i = 0; i < _Pool.Count; i++)
        {
            RegisterGridUnit(_Pool[i]);
            ResetGridUnit(_Pool[i]);
        }
        */

        foreach(var tile in _TileMap)
        {
            tile.Value.OnClick += OnClick_Tile;
        }

        IsTurn = false;
    }

    public bool HasActiveUnit(int turn)
    {
        bool hasActive = false;
        for(int i = 0; i < _UnitsOnGrid.Count; i++)
        {
            //if(_UnitsOnGrid[i].Clock <= turn)
            {
                hasActive = true;
                break;
            }
        }
        return hasActive;
    }

    private TurnState _CurrentState => _StateActionMap.CurrentState;
    private StateActionMap<TurnState> _StateActionMap = new StateActionMap<TurnState>();
    private int _CurrentClock;
    private GridUnit _SelectedUnit;
    private List<GridTile> _AvaliableMove;
    private List<(int, int)> _DeployZone;
    private Dictionary<(int, int), GridTile> _TileMap;
    private GridBall _Ball;
    private Direction _CurrentDirection;

    private void Awake()
    {
        //_Base.IsLeftPlayer = _IsLeftPlayer;
        //_Champion.IsLeftPlayer = _IsLeftPlayer;

        //for(int i = 0; i < _Pool.Count; i++)
        {
            //_Pool[i].IsLeftPlayer = _IsLeftPlayer;
        }

        IsTurn = false;

        _StateActionMap.RegisterEnter(TurnState.Throw, OnEnter_Throw);
        _StateActionMap.RegisterEnter(TurnState.Deploy, OnEnter_Deploy);
        _StateActionMap.RegisterEnter(TurnState.PickUp, OnEnter_PickUp);
        _StateActionMap.RegisterEnter(TurnState.Move, OnEnter_Move);
        _StateActionMap.RegisterEnter(TurnState.ActionSelection, OnEnter_ActionSelection);
        _StateActionMap.RegisterEnter(TurnState.Reaction, OnEnter_Reaction);

        for (int i = 0; i < _Actions.Count; i++)
        {
            _Actions[i].gameObject.SetActive(false);
            RegisterAction(_Actions[i]);
        }
    }

    private void UpdateOccupied(GridTile to, GridTile from)
    {
        to.IsOccupied = true;
        from.IsOccupied = false;
    }

    private void OnClick_Tile(GridTile tile)
    {
        if (!IsTurn)
        {
            return;
        }

        if (_StateActionMap.CurrentState == TurnState.Move)
        {
            if (_AvaliableMove != null && _AvaliableMove.Contains(tile))
            {
                //_SelectedUnit.Clock++;
                _SelectedUnit.DoMove(tile, ResolveMove);

                var from = _SelectedUnit.CurrentPosition;
                _SelectedUnit.CurrentPosition = tile; //to
                UpdateOccupied(tile, from);

                //_Base.RefreshGrid(_TileMap);
            }
        }
        if (_StateActionMap.CurrentState == TurnState.Deploy)
        {
            if (_DeployZone.Contains(((int)tile.GridPosition.x, (int)tile.GridPosition.y)))
            {
                HasDeployed = true;
              //  _SelectedUnit.Clock = _CurrentClock + 1;
                _SelectedUnit.Life = 6;

                //_Pool.Remove(_SelectedUnit);
                _UnitsOnGrid.Add(_SelectedUnit);

                tile.IsOccupied = true;

                //_Base.RefreshGrid(_TileMap);

                _SelectedUnit.CurrentPosition = tile;
                _SelectedUnit.DoMove(tile, ResolveMove);
            }
        }
        if (_StateActionMap.CurrentState == TurnState.Throw)
        {
            Debug.Log("Throw ball");
            //_SelectedUnit.Clock += 2;

            //_Base.RefreshGrid(_TileMap);
            _CurrentDirection = _Ball.GetDirection(_SelectedUnit.CurrentPosition, tile);

            //_SelectedUnit.StartShotIndicator();
            _StateActionMap.StateChange(TurnState.Throwing);
        }
    }

    private void OnClick_Action(GridAction action)
    {
        Debug.Log(action.Type);
        _ActionSelection.gameObject.SetActive(false);

        if (action.Type == ActionType.Move)
        {
            StateChange_Move();
        }
        else if (action.Type == ActionType.PickUp)
        {
            StateChange_Pick();
        }
        else if (action.Type == ActionType.Throw)
        {
            StateChange_Throw();
        }else if(action.Type == ActionType.Catch)
        {
            StateChange_Catch();
        }
        else if (action.Type == ActionType.Block)
        {
            StateChange_Block();
        }
    }

    private void OnSelected_GridUnit(GridUnit gridUnit)
    {
        if (!IsTurn)
        {
            if(_UnitsOnGrid.Contains(gridUnit))
            {
                OnUnitClickNotOnTurn?.Invoke(gridUnit.CurrentPosition);
            }
            return;
        }
       
        if (_CurrentState == TurnState.Active)
        {
            _SelectedUnit = gridUnit;

            if (_UnitsOnGrid.Contains(gridUnit))
            {
                if (HasActivated)
                {
                    return;
                }

                //if (gridUnit.Clock <= _CurrentClock)
                {
                    _StateActionMap.StateChange(TurnState.ActionSelection);
                }
            }
            else
            {
                ShowDeoplyZone(gridUnit);
            }
        }
        else if (_CurrentState == TurnState.End)
        {

        }
        else if (_CurrentState == TurnState.Throw)
        {
            if(_UnitsOnGrid.Contains(gridUnit))
            {
                OnClick_Tile(gridUnit.CurrentPosition);
            }
        }
    }

    private void StateChange_Throw()
    {
        _StateActionMap.StateChange(TurnState.Throw);
    }

    private void StateChange_Move()
    {
        _StateActionMap.StateChange(TurnState.Move);
    }

    private void StateChange_Pick()
    {
        _StateActionMap.StateChange(TurnState.PickUp);
    }

    private void StateChange_Catch()
    {
        //_SelectedUnit.StartCatchIndicator();
    }

    private void StateChange_Block()
    {
        //_SelectedUnit.StartBlockIndicator();
    }

    private void HighlightTiles()
    {
        var avaliableMoves = new List<GridTile>();

        /*
        var avaliblePositions = _Base.GridTiles(_SelectedUnit.CurrentPosition);

        for (int i = 0; i < avaliblePositions.Count; i++)
        {
            try
            {
                var moveTile = _TileMap[avaliblePositions[i]];

                if (moveTile != null)
                {
                    avaliableMoves.Add(moveTile);
                }

            }
            catch (KeyNotFoundException knf)
            {
                Debug.Log(knf.Message);
            }
        }
        */

        _AvaliableMove = avaliableMoves;

        for (int i = 0; i < avaliableMoves.Count; i++)
        {
            avaliableMoves[i].Highlight();
        }
    }

    #region OnEnter
    private void OnEnter_Throw()
    {
        //_SelectedUnit.Life -= ArcadeConstants.THROW_COST;
        HighlightTiles();
    }

    private void OnEnter_BlockTileSelection()
    {
        //_SelectedUnit.Life -= ArcadeConstants.BLOCK_COST;
        HighlightTiles();
    }

    private void OnEnter_PickUp()
    {
        /*
        _SelectedUnit.Clock++;

        _Ball.Fade();
        _SelectedUnit.PickUpBall();

        TranstionEndOrActive();
        */
    }

    private void OnEnter_Move()
    {
        var pos = _SelectedUnit.CurrentPosition.GridPosition;
        var key = ((int)pos.x, (int)pos.y);
        var tile = _TileMap[key];

        var avaliableMoves = new List<GridTile>();
        /*
        var avaliblePositions = _Base.GridTiles(tile);

        for (int i = 0; i < avaliblePositions.Count; i++)
        {
            try
            {
                var moveTile = _TileMap[avaliblePositions[i]];

                if (!moveTile.IsOccupied)
                {
                    avaliableMoves.Add(moveTile);
                }

            }
            catch (KeyNotFoundException knf)
            {
                Debug.Log(knf.Message);
            }
        }
        */

        _AvaliableMove = avaliableMoves;

        for (int i = 0; i < avaliableMoves.Count; i++)
        {
            avaliableMoves[i].Select();
        }
    }

    private void OnEnter_Deploy()
    {
        /*
        (int, int) key = ((int)_Base.CurrentPosition.GridPosition.x, (int)_Base.CurrentPosition.GridPosition.y);
        List<(int, int)> deployZone = new List<(int, int)>();

        if (_IsLeftPlayer)
        {
            int col = key.Item1 + 1;
            deployZone.Add((col, 0));
            deployZone.Add((col, 1));
            deployZone.Add((col, 2));

            int col2 = key.Item1 + 2;
            deployZone.Add((col2, 0));
            deployZone.Add((col2, 1));
            deployZone.Add((col2, 2));
        }
        else
        {
            int col = key.Item1 - 1;
            deployZone.Add((col, 0));
            deployZone.Add((col, 1));
            deployZone.Add((col, 2));

            int col2 = key.Item1 - 2;
            deployZone.Add((col2, 0));
            deployZone.Add((col2, 1));
            deployZone.Add((col2, 2));
        }

        foreach(var tile in _TileMap)
        {
            if(tile.Value.IsOccupied)
            {
                var gTile = tile.Value;
                (int, int) pos = ((int)gTile.GridPosition.x, (int)gTile.GridPosition.y);

                if (deployZone.Contains(pos))
                {
                    deployZone.Remove(pos);
                }
            }
        }

        if (deployZone.Count < 0)
        {
            Debug.Log("You can't deploy right now");
            return;
        }

        for (int i = 0; i < deployZone.Count; i++)
        {
            _TileMap[deployZone[i]].Select();
        }

        _DeployZone = deployZone;
        */
    }

    private void OnEnter_ActionSelection()
    {
        /*
        if(_UnitsOnGrid.Contains(_SelectedUnit))
        {
            Debug.Log("grid unit");
            _ActionSelection.gameObject.SetActive(true);

            for (int i = 0; i < _Actions.Count; i++)
            {
                _Actions[i].gameObject.SetActive(false);
            }

            if(_SelectedUnit.HasBall)
            {
                _Actions[0].gameObject.SetActive(true);
                _Actions[0].Type = ActionType.Move;

                _Actions[1].gameObject.SetActive(true);
                _Actions[1].Type = ActionType.Throw;
            }
            else if(_SelectedUnit.CurrentPosition == _Ball.CurrentPosition)
            {
                _ActionSelection.gameObject.SetActive(true);

                _Actions[0].gameObject.SetActive(true);
                _Actions[0].Type = ActionType.PickUp;
            }
            else
            {
                _Actions[0].gameObject.SetActive(true);
                _Actions[0].Type = ActionType.Move;
            }
        }
        */
    }

    private void OnEnter_Reaction()
    {
        bool possession = false;

        for(int i = 0; i < _UnitsOnGrid.Count; i++)
        {
            //if(_UnitsOnGrid[i].HasBall)
            {
                possession = true;
                break;
            }
        }

        _ActionSelection.gameObject.SetActive(true);

        if (possession)
        {
            _Actions[0].gameObject.SetActive(true);
            _Actions[0].Type = ActionType.Move;

            _Actions[1].gameObject.SetActive(true);
            _Actions[1].Type = ActionType.Throw;

            _Actions[2].gameObject.SetActive(true);
            _Actions[2].Type = ActionType.None;
        }
        else
        {
            _Actions[0].gameObject.SetActive(true);
            _Actions[0].Type = ActionType.Block;

            _Actions[1].gameObject.SetActive(true);
            _Actions[1].Type = ActionType.Catch;

            _Actions[2].gameObject.SetActive(true);
            _Actions[2].Type = ActionType.None;
        }
    }
    #endregion

    private void ResolveMove()
    {
        if (_CurrentState != TurnState.Deploy)
        {
            HasActivated = true;
        }
    }

    private void TranstionEndOrActive()
    {
        if (HasActivated && HasDeployed)
        {
            _StateActionMap.StateChange(TurnState.End);
        }
        else
        {
            _StateActionMap.StateChange(TurnState.Active);
        }
    }

    private void RegisterGridUnit(GridUnit gridUnit)
    {
        gridUnit.OnSelected += OnSelected_GridUnit;
    }

    private void ShowDeoplyZone(GridUnit gridUnit)
    {
        if(!HasDeployed && IsTurn)
        {
            //if (_Pool.Contains(gridUnit))
            {
                _StateActionMap.StateChange(TurnState.Deploy);
            }
        }
    }

    private void RegisterAction(GridAction action)
    {
        action.OnActionComplete += OnClick_Action;
    }
}

public enum TurnState
{
    End,
    Active,
    ActionSelection,
    Deploy,
    Move,
    Action,
    Reaction,
    PickUp,
    Throw,
    Throwing
}
