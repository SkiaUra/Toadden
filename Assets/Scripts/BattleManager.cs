using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public enum BattleStates {
    INIT,
    START,
    BATTLE,
    END
}

public class BattleManager : SerializedMonoBehaviour {
    public BattleStates battleState;
    [Required]
    public GameProperties gameProperties;
    [Required]
    public InputManager inputManager;
    [Required]
    public CanvasManager canvasManager;
    [Required]
    public FormationManager formationManager;

    [Required]
    public GameObject entityFolder;

    public int startDelay;

    public List<LivingEntity> AllyTeam = new List<LivingEntity>();
    public List<LivingEntity> EnemyTeam = new List<LivingEntity>();

    void Start() {
        SetupBattle();
    }

    void Update() {
        switch (battleState) {
            case BattleStates.INIT:
                // 
                break;
            case BattleStates.START:
                // activate the start button
                break;
            case BattleStates.BATTLE:
                if (AllyTeam.Count == 0 || EnemyTeam.Count == 0) {
                    canvasManager.buttonRestart.interactable = true;
                    battleState = BattleStates.END;
                }
                break;
            case BattleStates.END:
                break;
        }
    }

    public void SetupBattle() {
        SetupEntities();
        canvasManager.buttonStartBattle.interactable = true;
        battleState = BattleStates.START;
    }

    public async void StartBattle() {
        await UniTask.Delay(startDelay * 1000);
        foreach (LivingEntity entity in AllyTeam) {
            entity.entityState = EntityStates.IDLE;
        }
        foreach (LivingEntity entity in EnemyTeam) {
            entity.entityState = EntityStates.IDLE;
        }
        inputManager.allowInput = false;
        battleState = BattleStates.BATTLE;
    }

    public void SetupEntities() // Spawn entities on each given positions of a formation
    {
        Debug.Log("Setup Entities");
        // Clear lost entities if necessary 
        if (AllyTeam.Count != 0) {
            foreach (LivingEntity entity in AllyTeam) {
                entity.Autokill(true);
            }
        }
        AllyTeam.Clear();

        if (EnemyTeam.Count != 0) {
            foreach (LivingEntity entity in EnemyTeam) {
                entity.Autokill(true);
            }
        }
        EnemyTeam.Clear();

        LivingEntity instantiatedEntity;
        GameObject entityToSpawn = null;
        int a = Mathf.RoundToInt(formationManager.BattlePositions.Count * 0.5f);

        // Setup ally team comp
        for (int i = 0; i < a; i++) {
            entityToSpawn = gameProperties.AllyFormation[i];
            var _Formation = formationManager.BattlePositions[i];

            instantiatedEntity = Instantiate(entityToSpawn, _Formation.startPos.transform.position, _Formation.startPos.transform.rotation).GetComponent<LivingEntity>();
            instantiatedEntity.gameObject.transform.SetParent(entityFolder.transform);
            AllyTeam.Add(instantiatedEntity);
            instantiatedEntity.team = Team.ALLY;
            instantiatedEntity.battleManager = this;
            instantiatedEntity.startPos = _Formation.startPos;
            instantiatedEntity.linkedPos = _Formation;
            instantiatedEntity.entityState = EntityStates.WAITING;
            _Formation.entityonit = instantiatedEntity;
        }

        // Setup Enemy wave
        for (int i = a; i < formationManager.BattlePositions.Count; i++) {
            var _Formation = formationManager.BattlePositions[i];
            if (_Formation.formationPlace == FormationPlaces.FRONTLANE) entityToSpawn = gameProperties.MeleeEntity;
            if (_Formation.formationPlace == FormationPlaces.MIDLANE) entityToSpawn = gameProperties.RangedEntity;
            if (_Formation.formationPlace == FormationPlaces.BACKLANE) entityToSpawn = gameProperties.RangedEntity;

            entityToSpawn = gameProperties.Enemyformation[i - a];

            instantiatedEntity = Instantiate(entityToSpawn, _Formation.startPos.transform.position, _Formation.startPos.transform.rotation).GetComponent<LivingEntity>();
            instantiatedEntity.gameObject.transform.SetParent(entityFolder.transform);
            EnemyTeam.Add(instantiatedEntity);
            instantiatedEntity.team = Team.ENEMY;
            instantiatedEntity.battleManager = this;
            instantiatedEntity.startPos = _Formation.startPos;
            instantiatedEntity.linkedPos = _Formation;
            instantiatedEntity.entityState = EntityStates.WAITING;
            _Formation.entityonit = instantiatedEntity;
        }
    }

    public LivingEntity SpawnNewCreatureOnBattlePos(CreatureSpecies _species, Pos _battlePos) {
        GameObject entityToSpawn = gameProperties.GetCreatureGO(_species);
        if (!entityToSpawn) return null;
        _battlePos.entityonit.Autokill(true);
        LivingEntity instantiatedEntity = Instantiate(entityToSpawn, _battlePos.startPos.transform.position, _battlePos.startPos.transform.rotation).GetComponent<LivingEntity>();
        instantiatedEntity.gameObject.transform.SetParent(entityFolder.transform);
        AllyTeam.Add(instantiatedEntity);
        instantiatedEntity.team = Team.ALLY;
        instantiatedEntity.battleManager = this;
        instantiatedEntity.startPos = _battlePos.startPos;
        instantiatedEntity.linkedPos = _battlePos;
        instantiatedEntity.entityState = EntityStates.WAITING;
        _battlePos.entityonit = instantiatedEntity;
        return instantiatedEntity;
    }
}