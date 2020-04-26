using ParallelRisk;
using TMPro;
using UnityEngine;

public class TerritoryDisplay : MonoBehaviour
{
    private TextMeshProUGUI troopCount;

    void Start()
    {
        troopCount = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateTerritory(Territory territory)
    {
        troopCount.text = $"{territory.Player} ({territory.TroopCount})";
    }
}
