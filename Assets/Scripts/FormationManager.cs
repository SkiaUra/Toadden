using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class Pos
{
	public GameObject startPos;
	public LivingEntity entityonit;
	public FormationPlaces formationPlace;
	public EntityType entityTeam;
}

public enum FormationPlaces
{
	FRONTLANE,
	MIDLANE,
	BACKLANE
}

public class FormationManager : MonoBehaviour
{
	BattleManager battleManager;

	[Required]
	public List<GameObject> AllPositions = new List<GameObject>();
	public List<Pos> BattlePositions = new List<Pos>();

	[Button("Setup Position List")]
	public void SetupList()
	{
		BattlePositions.Clear();
		for (int i = 0; i < AllPositions.Count; i++)
		{
			Pos p = new Pos();
			BattlePositions.Add(p);
			p.startPos = AllPositions[i];
			if (AllPositions[i].name.Contains("Ally")) p.entityTeam = EntityType.ALLY;
			if (AllPositions[i].name.Contains("Enemy")) p.entityTeam = EntityType.ENEMY;
			if (AllPositions[i].name.Contains("Front")) p.formationPlace = FormationPlaces.FRONTLANE;
			if (AllPositions[i].name.Contains("Mid")) p.formationPlace = FormationPlaces.MIDLANE;
			if (AllPositions[i].name.Contains("Back")) p.formationPlace = FormationPlaces.BACKLANE;
		}
	}

	[Button("Switch Formations")]
	public void SwitchFormations(FormationPlaces _SrcLane, FormationPlaces _DstLane)
	{
		if (_SrcLane == _DstLane)
		{
			Debug.LogError("Cannot swap the same pos");
			return;
		}

		Debug.Log("Starting to switch " + _SrcLane + " with " + _DstLane);
		List<Pos> SrcList = new List<Pos>();
		List<Pos> DstList = new List<Pos>();

		// récuperer toutes les pos qui ont _SrcLane et ally && _DstLane et ally
		foreach (Pos CurrentPos in BattlePositions)
		{
			if (CurrentPos.entityTeam == EntityType.ALLY && CurrentPos.formationPlace == _SrcLane) SrcList.Add(CurrentPos);
			if (CurrentPos.entityTeam == EntityType.ALLY && CurrentPos.formationPlace == _DstLane) DstList.Add(CurrentPos);
		}

		if (SrcList.Count == 0 || DstList.Count == 0) Debug.Log("empty list");

		// pour chaque pos de la DstList, je remplace entityOnIt par celle de la SrcList
		LivingEntity memoryEntity;
		GameObject memoryPos;

		for (int i = 0; i < DstList.Count; i++)
		{
			memoryEntity = DstList[i].entityonit;
			memoryPos = DstList[i].startPos;

			// Swap entities
			DstList[i].entityonit = SrcList[i].entityonit;
			SrcList[i].entityonit = memoryEntity;

			// Setup positions
			DstList[i].entityonit.startPos = DstList[i].startPos;
			SrcList[i].entityonit.startPos = SrcList[i].startPos;

			// Setup Place
			DstList[i].entityonit.formationPlace = DstList[i].formationPlace;
			SrcList[i].entityonit.formationPlace = SrcList[i].formationPlace;
		}
	}

	void Start()
	{
		battleManager = GetComponent<BattleManager>();
	}
}

