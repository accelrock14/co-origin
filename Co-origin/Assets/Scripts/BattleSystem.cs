using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, CHOOSETARGET, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    int selectedEnemy = 0;
    public int selectedPlayer = 0;

    int attackNumber = 0;

    public GameObject[] playerPrefabs;
    [SerializeField]private GameObject[] enemyPrefabs;
    public GameObject selector;
    public GameObject playerSelector;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;
    public Transform enemyAIPrefab;
    Transform selectedBattleStation;

    BattleHUD playerHUD;
    BattleHUD enemyHUD;
    EnemyAI enemyAI;

    Unit playerUnit;
    Unit enemyUnit;

    GameObject enemyGO;
    GameObject playerGO;

    public Text dialogText;
    public Text attack1;
    public Text attack2;
    public Text attack3;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        selectedBattleStation = enemyBattleStation;
        enemyAI = enemyAIPrefab.GetComponent<EnemyAI>();
        StartCoroutine(SetupBattle());
    }
    private void Update()
    {
        if (state == BattleState.PLAYERTURN)
        {
            int previousSeletedPlayer = selectedPlayer;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedPlayer >= playerBattleStation.childCount - 1)
                    selectedPlayer = 0;
                else
                    selectedPlayer++;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedPlayer <= 0)
                    selectedPlayer = playerBattleStation.childCount - 1;
                else
                    selectedPlayer--;
            }
            if (previousSeletedPlayer != selectedPlayer)
                SelectPlayer(selectedPlayer);
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectEnemy(selectedEnemy, selectedBattleStation);
                selector.SetActive(true);
                playerSelector.SetActive(false);
                attackNumber = 0;
                state = BattleState.CHOOSETARGET;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectEnemy(selectedEnemy, selectedBattleStation);
                selector.SetActive(true);
                playerSelector.SetActive(false);
                attackNumber = 1;
                state = BattleState.CHOOSETARGET;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectEnemy(selectedEnemy, selectedBattleStation);
                selector.SetActive(true);
                playerSelector.SetActive(false);
                attackNumber = 2;
                state = BattleState.CHOOSETARGET;
            }
        }
        if (state == BattleState.CHOOSETARGET)
        {
            int previousSelectedEnemy = selectedEnemy;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedBattleStation = playerBattleStation;
                SelectEnemy(selectedEnemy, selectedBattleStation);
            }              
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedBattleStation = enemyBattleStation;
                SelectEnemy(selectedEnemy, selectedBattleStation);
            }   
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedEnemy >= selectedBattleStation.childCount - 1)
                    selectedEnemy = 0;
                else
                    selectedEnemy++;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedEnemy <= 0)
                    selectedEnemy = selectedBattleStation.childCount - 1;
                else
                    selectedEnemy--;
            }
            if (previousSelectedEnemy != selectedEnemy)
                SelectEnemy(selectedEnemy,selectedBattleStation);
            if(Input.GetKeyDown(KeyCode.Return))
            {
                selector.SetActive(false);
                StartCoroutine(PlayerAttack());
            }
        }
    }

    IEnumerator SetupBattle()
    { 
        SpawnPrefabs();

        SetupCharecters();

        dialogText.text = "A wild " + enemyUnit.unitName + " appeared";

        yield return new WaitForSeconds(2f);

        PlayerTurn();
    }
    public void PlayerTurn()
    {
        dialogText.text = "choose an attack";
        state = BattleState.PLAYERTURN;
        SelectPlayer(selectedPlayer);
        playerSelector.SetActive(true);
    }
    IEnumerator PlayerAttack()
    {
        state = BattleState.ENEMYTURN;
        SelectEnemy(selectedEnemy,selectedBattleStation);
        bool isDead = enemyUnit.TakeDamage(playerUnit.attacks[attackNumber]);
        enemyHUD.setHP(enemyUnit.currentHealth);
        dialogText.text = "attack successful";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            Destroy(enemyGO);            
            if (selectedEnemy >= selectedBattleStation.childCount - 1)
                selectedEnemy = 0;
            if (selectedEnemy <= 0)
                selectedEnemy = selectedBattleStation.childCount - 1;
            SelectEnemy(selectedEnemy, selectedBattleStation);
        }

        if (isDead && enemyBattleStation.childCount - 1 <= 0)
        {
            if (selectedBattleStation == playerBattleStation)
                state = BattleState.LOST;
            else
                state = BattleState.WON;
            EndBattle();
        }
        else
        {
            enemyAI.SelectEnemy();
        }
    }
    public void EndBattle()
    {
        if (state == BattleState.WON)
            dialogText.text = "you won the battle";
        else if (state == BattleState.LOST)
            dialogText.text = "you lost the battle";
    }
    void SpawnPrefabs()
    {
        for(int i=0;i < enemyPrefabs.Length;i++)
        {
            Instantiate(enemyPrefabs[i],enemyBattleStation);
        }
        for(int j=0; j< playerPrefabs.Length; j++)
        {
            Instantiate(playerPrefabs[j], playerBattleStation);
        }
    }
    void SelectEnemy(int target, Transform battlestation)
    {
        int i = 0;
        
        foreach (Transform enemy in battlestation)
        {
            if (target >= selectedBattleStation.childCount - 1)
                target = selectedBattleStation.childCount - 1;
            if (i == target)
            {
                enemyGO = enemy.gameObject;
                enemyUnit = enemyGO.GetComponent<Unit>();
                enemyHUD = enemyGO.GetComponent<BattleHUD>();
                enemyHUD.setHUD(enemyUnit);

                selector.transform.position = enemy.position;
            }
            i++;
        }
    }
    public void SelectPlayer(int target)
    {
        int i = 0;
        
        foreach (Transform player in playerBattleStation)
        {
            if (target >= playerBattleStation.childCount - 1)
                target = playerBattleStation.childCount - 1;
            if (i == target)
            {
                playerGO = player.gameObject;
                playerUnit = playerGO.GetComponent<Unit>();
                playerHUD = playerGO.GetComponent<BattleHUD>();
                playerHUD.setHUD(playerUnit);
                playerSelector.transform.position = player.position;
                if (state == BattleState.PLAYERTURN)
                {
                    attack1.text = "1. " + playerUnit.attack1;
                    attack2.text = "2. " + playerUnit.attack2;
                    attack3.text = "3. " + playerUnit.attack3;
                }
            }
            i++;
        }
    }
    void SetupCharecters()
    {
        int i=0;
        foreach (Transform player in playerBattleStation)
        {
                playerGO = player.gameObject;
                playerUnit = playerGO.GetComponent<Unit>();
                playerHUD = playerGO.GetComponent<BattleHUD>();
                playerHUD.setHUD(playerUnit);
            if(i == selectedPlayer)
            {
                attack1.text = "1. " + playerUnit.attack1;
                attack2.text = "2. " + playerUnit.attack2;
                attack3.text = "3. " + playerUnit.attack3;
            }
            i++;
        }
        foreach (Transform enemy in enemyBattleStation)
        {
                enemyGO = enemy.gameObject;
                enemyUnit = enemyGO.GetComponent<Unit>();
                enemyHUD = enemyGO.GetComponent<BattleHUD>();
                enemyHUD.setHUD(enemyUnit);
        }
    }
}
