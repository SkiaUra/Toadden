using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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




	void Start()
	{
		battleManager = GetComponent<BattleManager>();
	}


}

