using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private WTMK.Mechanics.Grid _Grid;

    [SerializeField]
    private GameObject _Unit_Prefab;

    void Awake()
    {
        _Grid = GetComponent<WTMK.Mechanics.Grid>();
        _Grid.Init();
    }

    void Start()
    {
        var clone = Instantiate<GameObject>(_Unit_Prefab);
        _PC = clone.GetComponent<GridUnit>();
        _PC.OnSelected += OnPlayerSelected;
        _Grid.SetPC(_PC);

        var key = (4, 1);
        _PC.DoMove(_Grid.Map[key]);
    }

    void Update()
    {

    }

    private void OnPlayerSelected(GridUnit unit)
    {
        Debug.Log("unit");
    }

    private GridUnit _PC;
}
