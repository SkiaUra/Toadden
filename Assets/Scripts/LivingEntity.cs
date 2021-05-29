using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;

public enum EntityStates {
    WAITING,
    IDLE,
    ATTACK,
    DEAD
}

public enum Team {
    ALLY,
    ENEMY
}

public class LivingEntity : MonoBehaviour {
    [BoxGroup("Bindings")]
    public BattleManager battleManager;
    [BoxGroup("Bindings")]
    public Animator animator;
    [BoxGroup("Bindings")]
    public HealthBarController healthBar;
    [BoxGroup("Bindings")]
    public EntityStates entityState;

    [TabGroup("Battle", "Stats")]
    public float moveSpeed = 1f;

    [TabGroup("Battle", "Stats")]
    public int healthCurrent, healthMax;
    [TabGroup("Battle", "Stats")]
    public float cooldownAttack, attackSpeed, initiative;

    [TabGroup("Battle", "Character")]
    public Team team;
    [TabGroup("Battle", "Character")]
    public CreatureSpecies creatureSpacies;
    [TabGroup("Battle", "Character")]
    public bool canAttack = false;
    [TabGroup("Battle", "Character")]
    public LivingEntity targetedEntity;
    [TabGroup("Battle", "Character")]
    public GameObject healthBarPrefab;
    [Required]
    [TabGroup("Battle", "Character")]
    public Skill attackSkill;
    [TabGroup("Battle", "Character")]
    public GameObject startPos;
    [TabGroup("Battle", "Character")]
    public FormationPlaces formationPlace;
    [TabGroup("Battle", "Character")]
    public Pos linkedPos;

    public bool attackIsCasting;
    private float distanceBetweenTarget;

    void Start() {
        initiative = Random.Range(0, 10);
        cooldownAttack = (1f / attackSpeed) + initiative / 10f;
        animator = this.GetComponentInChildren<Animator>();
        SetupHealthBar();
    }

    async void Update() {
        cooldownAttack -= Time.deltaTime;

        healthBar.UpdatePosition(startPos.transform.position);
        healthBar.RefreshVisuals(healthCurrent, healthMax);

        if (healthCurrent <= 0 && entityState != EntityStates.DEAD) {
            if (team == Team.ALLY) battleManager.AllyTeam.Remove(this);
            if (team == Team.ENEMY) battleManager.EnemyTeam.Remove(this);
            Autokill();
            entityState = EntityStates.DEAD;
        }

        switch (entityState) {
            case EntityStates.WAITING:
                MoveToPlacement();
                break;
            case EntityStates.IDLE:
                MoveToPlacement();
                if (cooldownAttack <= 0f && canAttack) {
                    entityState = EntityStates.ATTACK;
                }
                break;
            case EntityStates.ATTACK:
                if (targetedEntity != null) {
                    // MOVE AT RANGE
                    distanceBetweenTarget = Vector3.Distance(transform.position, targetedEntity.transform.position);
                    if (attackSkill.moveAtAttackRange && distanceBetweenTarget > attackSkill.attackRange) MoveAtRange(this, targetedEntity.gameObject);
                    if (distanceBetweenTarget <= attackSkill.attackRange && !attackIsCasting) {
                        attackIsCasting = true;
                        await attackSkill.PlaySkill(this, targetedEntity);
                        attackIsCasting = false;
                    }
                } else {
                    targetedEntity = GetTarget();
                    if (targetedEntity == null) entityState = EntityStates.IDLE;
                }
                break;
            case EntityStates.DEAD:
                break;
        }
    }

    public void MoveToPlacement() {
        // reposition entity
        distanceBetweenTarget = Vector3.Distance(transform.position, startPos.transform.position);
        if (distanceBetweenTarget > 0f) {
            MoveAtRange(this, startPos);
        } else {
            animator.SetBool("isIdle", true);
        }
    }

    public void MoveAtRange(LivingEntity _EntityToMove, GameObject _Target) {
        if (animator.GetBool("isIdle")) {
            animator.SetBool("isIdle", false);
        }
        Vector3 newPosition = Vector3.MoveTowards(_EntityToMove.transform.position, _Target.transform.position, moveSpeed * Time.deltaTime);
        _EntityToMove.transform.position = newPosition;
    }

    public LivingEntity GetTarget() {
        // Setup les variables de controle
        List<LivingEntity> checkList = battleManager.AllyTeam;
        if (team == Team.ALLY) checkList = battleManager.EnemyTeam;
        if (team == Team.ENEMY) checkList = battleManager.AllyTeam;

        if (checkList.Count != 0) {
            // get prefered target
            var AvailableTargets = checkList.Where(t => t.formationPlace == attackSkill.targetedLane).ToArray();
            if (AvailableTargets.Length > 0) {
                Debug.Log("PreferedTarget selected between " + AvailableTargets.Length);
                return AvailableTargets[Random.Range(0, AvailableTargets.Length)];
            } else {
                // No PreferedTarget
                Debug.Log("No PreferedTarget Found");
                return checkList[Random.Range(0, checkList.Count)];
            }
        } else {
            Debug.Log("No Target Found");
            return null;
        }
    }

    private void SetupHealthBar() {
        healthBar = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity, battleManager.canvasManager.healthBarGroup).GetComponent<HealthBarController>();
        healthBar.RefreshVisuals(healthCurrent, healthMax);
        healthBar.connectedEntity = this;
        healthBar.canvasManager = battleManager.canvasManager;
        healthBar.UpdatePosition(startPos.transform.position);
    }

    public async void Autokill(bool _InstantKill = false) {
        Tween destroyAnim = this.gameObject.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        if (_InstantKill == false) await UniTask.WaitUntil(() => destroyAnim.IsPlaying() == false);
        if (healthBar != null) Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos() {
        UnityEditor.Handles.color = Color.green;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        UnityEditor.Handles.DrawWireDisc(pos, Vector3.forward, attackSkill.attackRange);
        UnityEditor.Handles.color = Color.red;
        if (targetedEntity != null) UnityEditor.Handles.DrawLine(pos, targetedEntity.transform.position);
    }
}