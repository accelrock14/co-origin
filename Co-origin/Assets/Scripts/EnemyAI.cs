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

    // Start is called before the first frame update
    void Start()
    {
        battleSystemScript = battleSystem.GetComponent<BattleSystem>();
        playerBattleStation = battleSystemScript.playerBattleStation;
        enemyBattleStation = battleSystemScript.enemyBattleStation;
    }
    public void SelectEnemy()
    {
        battleSystemScript.SelectPlayer(battleSystemScript.selectedPlayer);
        int enemy = Random.Range(0, enemyBattleStation.childCount);
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
                targetGO = playerGO;
                targetUnit = playerUnit;
                targetHUD = playerGO.GetComponent<BattleHUD>();
                SelectBattleStation(enemyUnit.attacks[Random.Range(0, 1)]);
                return;
            }
        }
        foreach (Transform prefab in enemyBattleStation)
        {
            playerGO = prefab.gameObject;
            ChosenEnemy = playerGO.GetComponent<Unit>();
            if (ChosenEnemy.currentHealth <= 25)
            {
                targetGO = playerGO;
                targetUnit = ChosenEnemy;
                targetHUD = playerGO.GetComponent<BattleHUD>();
                SelectBattleStation(enemyUnit.attacks[2]);
                return;
            }
        }
        int target = Random.Range(0, playerBattleStation.childCount);
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
            battleSystemScript.selectedPlayer += 3;
            battleSystemScript.SelectPlayer(battleSystemScript.selectedPlayer);
        }
        if (isDead && playerBattleStation.childCount - 1 <= 0)
        {
            if (selectedBattleStation == playerBattleStation)
                battleSystemScript.state = BattleState.LOST;
            else
                battleSystemScript.state = BattleState.WON;
            battleSystemScript.EndBattle();
        }
        else
        {
            battleSystemScript.PlayerTurn();
        }
    }
}
