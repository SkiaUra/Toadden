using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;

public enum EntityStates
{
	WAITING,
	IDLE,
	ATTACK,
	DEAD
}

public enum EntityType
{
	ALLY,
	ENEMY
}

public class LivingEntity : MonoBehaviour
{
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
	public float targetRangeMin, targetRangeMax;
	[TabGroup("Battle", "Stats")]
	public int healthCurrent, healthMax;
	[TabGroup("Battle", "Stats")]
	public float cooldownAttack, attackSpeed, initiative;

	[TabGroup("Battle", "Character")]
	//public GameObject TMPstartPosition;
	[TabGroup("Battle", "Character")]
	public EntityType entityType;
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
	public Pos LinkedPos;

	private float distanceBetweenTarget;



	void Start()
	{
		initiative = Random.Range(0, 10);
		cooldownAttack = (1f / attackSpeed) + initiative / 10f;
		animator = this.GetComponentInChildren<Animator>();
		SetupHealthBar();
	}

	void Update()
	{
		cooldownAttack -= Time.deltaTime;

		healthBar.UpdatePosition(LinkedPos.startPos.transform.position);
		healthBar.RefreshVisuals(healthCurrent, healthMax);

		if (healthCurrent <= 0 && entityState != EntityStates.DEAD)
		{
			if (entityType == EntityType.ALLY) battleManager.AllyTeam.Remove(this);
			if (entityType == EntityType.ENEMY) battleManager.EnemyTeam.Remove(this);
			Autokill();
			entityState = EntityStates.DEAD;
		}

		switch (entityState)
		{
			case EntityStates.WAITING:
				MoveToPlacement();
				break;
			case EntityStates.IDLE:
				MoveToPlacement();
				if (cooldownAttack <= 0f)
				{
					canAttack = true;
					entityState = EntityStates.ATTACK;
				}

				break;
			case EntityStates.ATTACK:
				if (targetedEntity != null)
				{
					LaunchAttack();
				}
				else
				{
					targetedEntity = GetTarget();
					if (targetedEntity == null) entityState = EntityStates.IDLE;
				}
				break;
			case EntityStates.DEAD:
				// remove from battleManager

				break;
		}
	}

	public void MoveToPlacement()
	{
		// reposition entity
		distanceBetweenTarget = Vector3.Distance(transform.position, LinkedPos.startPos.transform.position);
		if (distanceBetweenTarget > 0f)
		{
			MoveAtRange(this, LinkedPos.startPos);
		}
		else
		{
			animator.SetBool("isIdle", true);
		}
	}

	public void MoveAtRange(LivingEntity _EntityToMove, GameObject _Target)
	{
		if (animator.GetBool("isIdle"))
		{
			animator.SetBool("isIdle", false);
		}
		Vector3 newPosition = Vector3.MoveTowards(_EntityToMove.transform.position, _Target.transform.position, moveSpeed * Time.deltaTime);
		_EntityToMove.transform.position = newPosition;
	}

	public void LaunchAttack()
	{
		if (!attackSkill) return;
		// Check Target
		// if (!targetedEntity || targetedEntity.healthCurrent <= 0) targetedEntity = GetTarget();

		// lire toutes les infos du skill range, type de focus, dégats, effet, etc...
		distanceBetweenTarget = Vector3.Distance(transform.position, targetedEntity.transform.position);
		if (attackSkill.moveAtAttackRange) MoveAtRange(this, targetedEntity.gameObject);

		if (distanceBetweenTarget <= attackSkill.attackRange)
		{
			Debug.LogWarning("Attack launched with " + this.name);
			targetedEntity.healthCurrent -= attackSkill.damage;
			cooldownAttack = 1f / attackSpeed;
			canAttack = false;
			entityState = EntityStates.IDLE;
		}
	}

	public LivingEntity GetTarget()
	{
		// check si la créa remplie la condition d'attaque
		switch (entityType)
		{
			case EntityType.ALLY:
				if (battleManager.EnemyTeam.Count != 0)
				{
					// get prefered target 
					var AvailableTargets = battleManager.EnemyTeam.Where(t => t.LinkedPos.formationPlace == FormationPlaces.FRONTLANE).ToArray();
					if (AvailableTargets.Length > 0)
					{
						return AvailableTargets[Random.Range(0, AvailableTargets.Length)];
					}
					else
					{
						// get one random yolo
						return battleManager.EnemyTeam[Random.Range(0, battleManager.EnemyTeam.Count)];
					}
				}
				else
				{
					return null;
				}

			case EntityType.ENEMY:
				if (battleManager.AllyTeam.Count != 0)
				{
					var AvailableTargets = battleManager.AllyTeam.Where(t => t.LinkedPos.formationPlace == FormationPlaces.FRONTLANE).ToArray();
					if (AvailableTargets.Length > 0)
					{
						return AvailableTargets[Random.Range(0, AvailableTargets.Length)];
					}
					else
					{
						return battleManager.AllyTeam[Random.Range(0, battleManager.AllyTeam.Count)];
					}
				}
				else
				{
					return null;
				}
		}
		Debug.LogWarning("Danger GetTarget()");
		return null;
	}

	private void SetupHealthBar()
	{
		healthBar = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity, battleManager.canvasManager.healthBarGroup).GetComponent<HealthBarController>();
		healthBar.RefreshVisuals(healthCurrent, healthMax);
		healthBar.connectedEntity = this;
		healthBar.canvasManager = battleManager.canvasManager;
		healthBar.UpdatePosition(LinkedPos.startPos.transform.position);
	}

	public async void Autokill(bool _InstantKill = false)
	{
		Tween destroyAnim = this.gameObject.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
		if (_InstantKill == false) await UniTask.WaitUntil(() => destroyAnim.IsPlaying() == false);
		Destroy(healthBar.gameObject);
		Destroy(this.gameObject);
	}

	private void OnDrawGizmos()
	{
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireDisc(this.transform.position, Vector3.forward, attackSkill.attackRange);
		if (targetedEntity != null) UnityEditor.Handles.DrawLine(this.transform.position, targetedEntity.transform.position);
	}
}
