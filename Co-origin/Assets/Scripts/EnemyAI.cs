using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject battleSystem;
    BattleSystem battleSystemScript;

    Transform playerBattleStation;
    Transform enemyBattleStation;
    Transform selectedBattleStation;

    GameObject playerGO;
    GameObject enemyGO;
    GameObject targetGO;

    Unit playerUnit;
    Unit enemyUnit;
    Unit targetUnit;
    Unit ChosenEnemy;

    BattleHUD targetHUD;

    bool isLowHealth = false;
    bool isWeek;

    // Start is called before the first frame update
    void Start()
    {
        battleSystemScript = battleSystem.GetComponent<BattleSystem>();
        playerBattleStation = battleSystemScript.playerBattleStation;
        enemyBattleStation = battleSystemScript.enemyBattleStation;
    }
    public void SelectEnemy()
    {
        int enemy = Random.Range(0, enemyBattleStation.childCount - 1);
        int i = 0;
        foreach (Transform prefab in enemyBattleStation)
        {
            enemyGO = prefab.gameObject;
            ChosenEnemy = enemyGO.GetComponent<Unit>();
            if (i == enemy)
            {
                enemyUnit = ChosenEnemy;
            }
            i++;
        }
        ChooseTarget();
    }
    void ChooseTarget()
    {
        foreach(Transform prefab in playerBattleStation)
        {
            playerGO = prefab.gameObject;
            playerUnit = playerGO.GetComponent<Unit>();
            if (playerUnit.currentHealth <= 25)
            {
                isLowHealth = true;
                targetGO = playerGO;
                targetUnit = playerUnit;
                targetHUD = playerGO.GetComponent<BattleHUD>();
            }
        }
        if (isLowHealth)
            SelectBattleStation(enemyUnit.attacks[Random.Range(0,1)]);
        else
        {
            foreach(Transform prefab in enemyBattleStation)
            {
                enemyGO = prefab.gameObject;
                ChosenEnemy = enemyGO.GetComponent<Unit>();
                if (ChosenEnemy.currentHealth <= 25)
                {
                    isWeek = true;
                    targetGO = enemyGO;
                    targetUnit = ChosenEnemy;
                    targetHUD = enemyGO.GetComponent<BattleHUD>();
                }
            }
            if (isWeek)
                SelectBattleStation(enemyUnit.attacks[2]);
            else
            {
                int target = Random.Range(0, playerBattleStation.childCount - 1);
                int i = 0;
                foreach (Transform prefab in playerBattleStation)
                {
                    playerGO = prefab.gameObject;
                    playerUnit = playerGO.GetComponent<Unit>();
                    if (i == target)
                    {
                        targetGO = playerGO;
                        targetUnit = playerUnit;
                        targetHUD = playerGO.GetComponent<BattleHUD>();
                    }
                    i++;
                }
                SelectBattleStation(enemyUnit.attacks[Random.Range(0, 1)]);
            }
        }
    }
    void SelectBattleStation(int damage)
    {
        if (damage <= 0)
            selectedBattleStation = enemyBattleStation;
        else
            selectedBattleStation = playerBattleStation;
        bool isDead = targetUnit.TakeDamage(damage);
        targetHUD.setHP(targetUnit.currentHealth);

        if (isDead)
        {
            Destroy(targetGO);
            battleSystemScript.SelectPlayer(battleSystemScript.selectedPlayer);
        }
        if (playerBattleStation.childCount - 1 <= 0)
        {
            battleSystemScript.state = BattleState.LOST;
            battleSystemScript.EndBattle();
        }
        else
        {
            battleSystemScript.PlayerTurn();
        }
    }
}
