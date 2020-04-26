using UnityEngine;
using ParallelRisk;

public class TestParallelRisk : MonoBehaviour
{
    [SerializeField]
    private int maxDepth = 1;

    void Start()
    {
        BoardState board = Risk.StandardBoard();
        Move move = Minimax.Serial<BoardState, Move>(board, maxDepth);

        if (move.IsAttack)
            Debug.Log($"Attack from {(Risk.Id)move.From.Id} to {(Risk.Id)move.To.Id}");
        else
            Debug.Log("PassTurn");
    }
}
