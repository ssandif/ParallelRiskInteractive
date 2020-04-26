using System;
using System.Collections.Generic;
using ParallelRisk;
using UnityEngine;
using static System.Math;

public class GameState : MonoBehaviour
{
    [SerializeField]
    private int maxDepth = 1;

    private BoardState _board;

    private TerritoryDisplay[] _territories;

    void Awake()
    {
        _board = Risk.StandardBoard();
        _territories = new TerritoryDisplay[42];
    }

    private void Start()
    {
        TerritoryDisplay[] territories = FindObjectsOfType<TerritoryDisplay>();
        foreach (TerritoryDisplay territory in territories)
        {
            if (Enum.TryParse(territory.name, out Risk.Id id))
            {
                _territories[(int)id] = territory;
            }
            else
            {
                Debug.Log("Missing territory");
            }
        }
        DrawTerritories();
    }

    public void Move()
    {
        if (_board.IsTerminal())
        {
            string winner = _board.Heuristic() > 0 ? "Max" : "Min";
            Debug.Log($"Player {winner} wins!");
            return;
        }
        Move move = Minimax.Serial<BoardState, Move>(_board, maxDepth);

        if (move.IsAttack)
        {
            Debug.Log($"Attack from {(Risk.Id)move.From.Id} to {(Risk.Id)move.To.Id}");
            _board = Attack(_board, move);
        }
        else
        {
            Debug.Log("PassTurn");
            _board = _board.PassTurn();
        }

        DrawTerritories();
    }

    private void DrawTerritories()
    {
        foreach (Territory territory in _board.Territories)
        {
            TerritoryDisplay display = _territories[territory.Id];
            display.UpdateTerritory(territory);
        }
    }

    private BoardState Attack(BoardState board, Move move)
    {
        Territory from = move.From;
        Territory to = move.To;

        // If attacking, must leave one troop behind
        int attackers = Min(move.From.TroopCount - 1, 3);
        int defenders = Min(move.To.TroopCount, 2);

        List<int> attack = RollDice(attackers);
        List<int> defense = RollDice(defenders);

        int comparisons = Min(attack.Count, defense.Count);

        for (int i = 0; i < comparisons; i++)
        {
            // Defender wins ties
            if (attack[i] > defense[i])
            {
                to = to.ModifyTroops(-1);
            }
            else
            {
                from = from.ModifyTroops(-1);
            }
        }

        if (to.TroopCount == 0)
        {
            to = to.ChangeControl(from.Player, from.TroopCount - 1);
            from.ModifyTroops(-from.TroopCount + 1);
        }

        return board.AttackUpdate(from, to);
    }

    private List<int> RollDice(int dice)
    {
        var list = new List<int>();
        for (int i = 0; i < dice; i++)
        {
            list.Add(UnityEngine.Random.Range(1, 7));
        }
        list.Sort();
        list.Reverse();
        return list;
    }
}
