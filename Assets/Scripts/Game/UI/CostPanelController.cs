using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CostPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text costText;
    
    public void SetCost(List<bool> costs)
    {
        int totalCosts = costs.Count(x => x);
        Debug.Log(totalCosts);
        costText.text = totalCosts.ToString();
        Debug.Log(costText.text);
    }
}
