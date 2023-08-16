using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;
using UnityEngine.UI;

namespace WTMK.Mechanics
{
    public class Grid : MonoBehaviour
    {
        public Dictionary<(int, int), GridTile> Map => _GridMap;

        [SerializeField]
        private List<GridTile> _GridTiles;
        [SerializeField]
        private GameObject _Grid;
        [SerializeField]
        private GameObject _GridCanvas;

        public void SetPC(GridUnit pc)
        {
            _PC = pc;
            _PC.gameObject.SetActive(true);
            _PC.transform.SetParent(_GridCanvas.transform);
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
            Debug.Log($"Clicked Tile ({sender.GridPosition.x} , {sender.GridPosition.y} ");
        }

        private Dictionary<(int, int), GridTile> _GridMap = new Dictionary<(int, int), GridTile>();
        private GridUnit _PC;
    }

}
