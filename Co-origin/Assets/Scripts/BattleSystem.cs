using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START,PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogText;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }
    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        playerHUD.setHUD(playerUnit);
        enemyHUD.setHUD(enemyUnit);

        dialogText.text = "A wild " + enemyUnit.unitName + " appeared";

        yield return new WaitForSeconds(2f);

        PlayerTurn();
    }
    void PlayerTurn()
    {
        dialogText.text = "choose an attack";
        state = BattleState.PLAYERTURN;
    }
    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.setHP(enemyUnit.currentHealth);
        dialogText.text = "attack successful";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    void EndBattle()
    {
        if (state == BattleState.WON)
            dialogText.text = "you won the battle";
        else if (state == BattleState.LOST)
            dialogText.text = "you lost the battle";
    }
    IEnumerator EnemyTurn()
    {
        dialogText.text = enemyUnit.name + "attacks!";

        yield return new WaitForSeconds(1f);
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.setHP(playerUnit.currentHealth);

        yield return new WaitForSeconds(1f);
        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }
    IEnumerator PlayerHeal()
    {
        dialogText.text = "player healed";
        playerUnit.Heal(5);
        playerHUD.setHP(playerUnit.currentHealth);

        yield return new WaitForSeconds(1f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
}
