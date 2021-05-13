﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public enum BattleStates
{
	INIT,
	START,
	BATTLE,
	END
}

public class BattleManager : SerializedMonoBehaviour
{
	public BattleStates battleState;
	[Required]
	public CanvasManager canvasManager;
	[Required]
	public FormationManager formationManager;
	public GameObject AllyPrefab;
	public GameObject EnemyPrefab;

	public int startDelay;

	public List<LivingEntity> AllyTeam = new List<LivingEntity>();
	public List<LivingEntity> EnemyTeam = new List<LivingEntity>();

	void Start()
	{
		SetupBattle();
	}

	void Update()
	{
		switch (battleState)
		{
			case BattleStates.INIT:
				// 
				break;
			case BattleStates.START:
				// activate the start button
				break;
			case BattleStates.BATTLE:
				if (AllyTeam.Count == 0 || EnemyTeam.Count == 0)
				{
					canvasManager.buttonRestart.interactable = true;
					battleState = BattleStates.END;
				}
				break;
			case BattleStates.END:
				break;
		}
	}

	public void SetupBattle()
	{
		SetupEntities();
		canvasManager.buttonStartBattle.interactable = true;
		battleState = BattleStates.START;
	}

	public async void StartBattle()
	{
		await UniTask.Delay(startDelay * 1000);
		foreach (LivingEntity entity in AllyTeam)
		{
			entity.entityState = EntityStates.IDLE;
		}
		foreach (LivingEntity entity in EnemyTeam)
		{
			entity.entityState = EntityStates.IDLE;
		}
		battleState = BattleStates.BATTLE;
	}

	public void SetupEntities() // Spawn entities on each given positions of a formation
	{
		Debug.Log("Setup Entities");
		// Clear lost entities if necessary 
		if (AllyTeam.Count != 0)
		{
			foreach (LivingEntity entity in AllyTeam)
			{
				entity.Autokill(true);
			}
		}
		AllyTeam.Clear();

		if (EnemyTeam.Count != 0)
		{
			foreach (LivingEntity entity in EnemyTeam)
			{
				entity.Autokill(true);
			}
		}
		EnemyTeam.Clear();

		foreach (Pos _formation in formationManager.BattlePositions)
		{
			LivingEntity instantiatedEntity;
			switch (_formation.entityTeam)
			{
				case EntityType.ALLY:
					instantiatedEntity = Instantiate(AllyPrefab, _formation.startPos.transform.position, Quaternion.identity).GetComponent<LivingEntity>();
					AllyTeam.Add(instantiatedEntity);
					instantiatedEntity.entityType = EntityType.ALLY;
					instantiatedEntity.battleManager = this;
					instantiatedEntity.LinkedPos.startPos = _formation.startPos;
					instantiatedEntity.entityState = EntityStates.WAITING;
					instantiatedEntity.LinkedPos = _formation;
					_formation.entityonit = instantiatedEntity;
					break;
				case EntityType.ENEMY:
					instantiatedEntity = Instantiate(EnemyPrefab, _formation.startPos.transform.position, Quaternion.identity).GetComponent<LivingEntity>();
					EnemyTeam.Add(instantiatedEntity);
					instantiatedEntity.entityType = EntityType.ENEMY;
					instantiatedEntity.battleManager = this;
					instantiatedEntity.LinkedPos.startPos = _formation.startPos;
					instantiatedEntity.entityState = EntityStates.WAITING;
					instantiatedEntity.LinkedPos = _formation;
					_formation.entityonit = instantiatedEntity;
					break;
			}
		}
	}
}