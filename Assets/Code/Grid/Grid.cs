using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;
using UnityEngine.UI;

namespace WTMK.Mechanics
{
    public class Grid : MonoBehaviour
    {
        public event Action<GridTile> TileSelected;
        public Dictionary<(int, int), GridTile> Map => _GridMap;

        [SerializeField]
        private List<GridTile> _GridTiles;
        [SerializeField]
        private GameObject _Grid;
        [SerializeField]
        private GameObject _GridCanvas;

        public List<GridUnit> Units;

        public GridTile GetOpenTile()
        {
            bool hasTile = false;
            GridTile tile = null;

            while (!hasTile)
            {
                tile = GetRandomTilePos();
                if (!tile.IsOccupied)
                {
                    hasTile = true; 
                }
            }

            return tile;
        }

        public void Unselect()
        {
            for (int i = 0; i < _GridTiles.Count; i++)
            {
                _GridTiles[i].Refresh();
            }
        }

        public List<GridTile> GetAdjacentTiles(GridTile tile)
        {
            var tiles = new List<GridTile>();

            for (int i = 0; i < _GridTiles.Count; i++)
            {
                if(_GridTiles[i].IsAdjacent(tile))
                {
                    tiles.Add(_GridTiles[i]);
                }
            }

            return tiles;
        }

        public List<GridTile> GetSelectableTiles(bool occupied, GridTile tile, int range)
        {
            var tiles = new List<GridTile>();

            for (int i = 0; i < _GridTiles.Count; i++)
            {
                var target = _GridTiles[i];
                if (tile.IsInRange(target, range) && tile.IsOccupied == occupied)
                {
                    tiles.Add(target);
                }
            }

            return tiles;
        }

        public GridTile GetRandomTilePos()
        {
            var roll = _RNG.GetRandomInt(_GridTiles.Count);
            var tile = _GridTiles[roll];
            return tile;
        }

        public void SetPC(GridUnit pc)
        {
            _PC = pc;
            _PC.gameObject.SetActive(true);
            _PC.transform.SetParent(_GridCanvas.transform);
        }

        public void SetOnGrid(GridUnit unit , GridTile location)
        {
            unit.gameObject.SetActive(true);
            unit.transform.SetParent(_GridCanvas.transform);
            unit.DoMove(location);
        }

        public void SetActive(bool isActive)
        {
            _GridCanvas.SetActive(isActive);
        }

        public void Init()
        {
            foreach (var tile in _GridTiles)
            {
                var pos = ((int)tile.GridPosition.x, (int)tile.GridPosition.y);

                if (!_GridMap.ContainsKey(pos))
                {
                    _GridMap.Add(pos, tile);
                    tile.OnClick += OnTileSelected;
                }
                else
                {
                    Debug.LogError("Check Grid Tiles :: tile already in map");
                }
            }
        }

        private void OnTileSelected(GridTile sender)
        {
            TileSelected?.Invoke(sender);
        }

        private Dictionary<(int, int), GridTile> _GridMap = new Dictionary<(int, int), GridTile>();
        private GridUnit _PC;
        private RNG _RNG = new RNG();
    }

}

public class RNG
{
    public int GetRandomInt(int max)
    {
        if (_Random.Count < _RandomMax)
        {
            var r = new System.Random(System.Environment.TickCount);
            _Random.Add(r);
            _RQ.Enqueue(r);
        }

        var rq = _RQ.Dequeue();
        _RQ.Enqueue(rq);
        return rq.Next(max);
    }

    private int _RandomMax = 9;
    private List<System.Random> _Random = new List<System.Random>();
    private Queue<System.Random> _RQ = new Queue<System.Random>();
}

