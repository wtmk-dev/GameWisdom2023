using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : MonoBehaviour
{
    [SerializeField]
    private UnitActionBar _ActionBar;

    public void StartTransition(GridUnit player, BattleSystem battleSystem)
    {
        _Player = player;
        _Player.SetActionBar(_ActionBar);
        _BattleSystem = battleSystem;
        _ActionBar.Init(player.Abilities);
        _BattleSystem.Start();
    }

    private GridUnit _Player;
    private BattleSystem _BattleSystem;
}
