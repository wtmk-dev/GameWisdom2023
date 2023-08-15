using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridBall : GridObject
{
    public event Action OnThrowSuccessful;
    public event Action OnThrowIntrupt;

    public void Init(Dictionary<(int, int), GridTile> tileMap)
    {
        _TileMap = tileMap;
        _BallState.RegisterEnter(BallState.Thrown, OnEnter_Thown);
        _BallState.RegisterEnter(BallState.Bounce, OnEnter_Bounce);
    }

    public void Throw(Direction direction, int power)
    {
        _Trajectory = direction;
        _Power = power;

        _BallState.StateChange(BallState.Thrown);
    }

    public void Bounce()
    {
        _BallState.StateChange(BallState.Bounce); 
    }

    public (int, int) GetNextMoveByDirection(Direction direction, Vector2 gridPos) 
    {
        var nextMove = ((int)gridPos.x,(int)gridPos.y);
        switch(direction)
        {
            case Direction.NorthWest:
                nextMove.Item1--;
                nextMove.Item2--;
                break;
            case Direction.North:
                nextMove.Item2--;
                break;
            case Direction.NorthEast:
                nextMove.Item1++;
                nextMove.Item2--;
                break;
            case Direction.East:
                nextMove.Item1++;
                break;
            case Direction.SouthEast:
                nextMove.Item1++;
                nextMove.Item2++;
                break;
            case Direction.South:
                nextMove.Item2++;
                break;
            case Direction.SouthWest:
                nextMove.Item1--;
                nextMove.Item2++;
                break;
            case Direction.West:
                nextMove.Item1--;
                break;
        }


        return nextMove;
    }   

    public Direction GetDirection(GridTile thrower, GridTile selected)
    {
        if(thrower.GridPosition.x > selected.GridPosition.x &&
           thrower.GridPosition.y > selected.GridPosition.y)
        {
            return Direction.NorthWest;
        }else if(thrower.GridPosition.x < selected.GridPosition.x &&
                 thrower.GridPosition.y < selected.GridPosition.y)
        {
            return Direction.SouthEast;
        }else if(thrower.GridPosition.x < selected.GridPosition.x &&
                 thrower.GridPosition.y > selected.GridPosition.y)
        {
            return Direction.NorthEast;

        }else if (thrower.GridPosition.x > selected.GridPosition.x &&
                  thrower.GridPosition.y < selected.GridPosition.y) 
        {
            return Direction.SouthWest;
        }else if(thrower.GridPosition.x > selected.GridPosition.x)
        {
            return Direction.West;
        }
        else if (thrower.GridPosition.x < selected.GridPosition.x)
        {
            return Direction.East;
        }
        else if (thrower.GridPosition.y > selected.GridPosition.y)
        {
            return Direction.North;
        }
        else if (thrower.GridPosition.y < selected.GridPosition.y)
        {
            return Direction.South;
        }

        return Direction.West;
    }

    public override void DoMove(GridTile tile, Action callback = null)
    {
        transform.DOMove(tile.transform.position, 0.3f).OnComplete(() =>
        {
            CurrentPosition = tile;
            
            if (callback != null)
            {
                callback?.Invoke();
            }
        });
    }

    private Dictionary<(int, int), GridTile> _TileMap;
    private StateActionMap<BallState> _BallState = new StateActionMap<BallState>();
    private Direction _Trajectory;
    private int _Power;

    private void OnEnter_Thown()
    {
        if(_Power > 0)
        {
            var goToPos = GetNextMoveByDirection(_Trajectory, CurrentPosition.GridPosition);
            if(_TileMap.ContainsKey(goToPos))
            {
                var goToTile = _TileMap[goToPos];
                DoMove(goToTile, ()=> OnThrowComplete(goToTile) );
            }
            else
            {
                _BallState.StateChange(BallState.Bounce);
            }    
        }
    }

    private void OnThrowComplete(GridTile gotToTile)
    {
        _Power--;
        Debug.Log("Throw Power: " + _Power);

        if (gotToTile.IsOccupied)
        {
            OnThrowIntrupt?.Invoke();
        }
        else
        {
            if (_Power > 0)
            {
                _BallState.StateChange(BallState.Thrown);
            }
            else
            {
                if (!CurrentPosition.IsOccupied)
                {
                    RefreshGrid(_TileMap);
                    OnThrowSuccessful?.Invoke();
                }
                else
                {

                }
            }
        }
    }

    private int _BounceIndex = 0;
    private bool _IsBounceActive = false;
    private List<GridTile> _BounceTiles;
    private IEnumerator _BounceCorutine;
    private void OnEnter_Bounce()
    {
        var bounceTargets = GridTiles(CurrentPosition);
        var avaliableTargets = new List<GridTile>();

        for(int i = 0; i < bounceTargets.Count; i++)
        {
            var pos = bounceTargets[i];
            if(_TileMap.ContainsKey(pos))
            {
                avaliableTargets.Add(_TileMap[pos]);
            }
        }

        if(_BounceCorutine != null)
        {
            StopCoroutine(_BounceCorutine);
            _BounceCorutine = null;
        }

        _BounceCorutine = DoBounceTileSelection(avaliableTargets);
        _IsBounceActive = true;

        StartCoroutine(_BounceCorutine);
    }

    private IEnumerator DoBounceTileSelection(List<GridTile> avaliableTargets)
    {
        _BounceTiles = avaliableTargets;

        while (_IsBounceActive)
        {
            _BounceIndex = 0;
            for (int i = 0; i < avaliableTargets.Count; i++)
            {
                _BounceIndex = i;

                avaliableTargets[i].BounceTarget();
                yield return new WaitForSeconds(0.15f);
                avaliableTargets[i].Refresh();
            }

            yield return new WaitForSeconds(0.01f);

            for (int i = avaliableTargets.Count -1; i > avaliableTargets.Count; i--)
            {
                _BounceIndex = i;

                avaliableTargets[i].BounceTarget();
                yield return new WaitForSeconds(0.15f);
                avaliableTargets[i].Refresh();
            }
        }

    }

    private void Update()
    {
        _BallState.DoUpdate();

        if (!_IsBounceActive)
        {
            return;
        }

        if (Input.anyKey)
        {
            _IsBounceActive = false;
            if (_BounceCorutine != null)
            {
                StopCoroutine(_BounceCorutine);

                if(_Power > 0)
                {
                    _Trajectory = GetDirection(CurrentPosition, _BounceTiles[_BounceIndex]); 
                    Throw(_Trajectory, _Power);
                }
            }
        }
    }
}

public enum BallState
{
    Thrown,
    Bounce
}

public enum Direction
{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest
}
