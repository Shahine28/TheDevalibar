using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class TablesManager : MonoBehaviour
{
    [SerializeField] private List<Table> Tables;

    void Awake()
    {
        ServiceLocator.Register(this);
    }
   
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideUpgradeButtonTables()
    {
        foreach (Table table in Tables)
        {
           table.HideUpgradeButton(); 
        }
    }

    public void ShowUpgradeButtonTables()
    {
        foreach (Table table in Tables)
        {
            table.ShowUpgradeButton();
        }
    }
    public List<Table> GetAccessibleTables(string CharacterDisability = "")
    {
        if (Tables[0].TableNumber == -1)
        {
            foreach (Table table in Tables)
            {
                table.SetTableNode();
            }
        }
        NodeManager nodeManager = ServiceLocator.Get<NodeManager>();
        // 0. Aucun handicap ➜ règle classique
        if (CharacterDisability == "")
        {
            return Tables
                .Where(t => !t.IsUsedByCustomer)
                .OrderBy(t =>
                {
                    int blockingCount = 0;
                    int nonBlockingCount = 0;

                    foreach (var key in t.constraintDict.keys)
                    {
                        var constraint = nodeManager.constraints.FirstOrDefault(c => c.name == key);
                        if (constraint != null)
                        {
                            if (constraint.IsBlockingConstraint)
                                blockingCount++;
                            else
                                nonBlockingCount++;
                        }
                    }

                    return (
                        blockingCount > 0 ? 2 : nonBlockingCount > 0 ? 1 : 0, // 0: aucune contrainte, 1: non bloquantes, 2: bloquantes
                        blockingCount,
                        nonBlockingCount
                    );
                })
                .ToList();
        }


        
        if (!nodeManager) return null;
        Constraint constraint = nodeManager.constraints
            .FirstOrDefault(c => c.name == CharacterDisability);

        bool isBlocking = constraint != null && constraint.IsBlockingConstraint;

        // 2. Sélectionne les tables libres + si elles supportent la contrainte
        var matchingTables = Tables
            .Where(t => !t.IsUsedByCustomer)
            .Select(table =>
            {
                var keys = table.constraintDict.keys;
                var values = table.constraintDict.values;

                bool supportsDisability = false;

                for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
                {
                    if (keys[i] == CharacterDisability && values[i])
                    {
                        supportsDisability = true;
                        break;
                    }
                }

                return new
                {
                    Table = table,
                    IsCompatible = supportsDisability
                };
            })
            .ToList();

        if (isBlocking)
        {
            // 3. Si handicap bloquant ➜ uniquement tables compatibles
            return matchingTables
                .Where(x => x.IsCompatible)
                .Select(x => x.Table)
                .ToList();
        }
        
        // 4. Si non bloquant ➜ tables compatibles d'abord, puis autres
        return matchingTables
            .OrderByDescending(x => x.IsCompatible) // compatibles d'abord
            .Select(x => x.Table)
            .ToList();
        
    }
}
