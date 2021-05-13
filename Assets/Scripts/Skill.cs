using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/CreateNewSkill")]
public class Skill : ScriptableObject
{
	public int damage;
	public FormationPlaces targetedLane;

	public GameObject HitFX;
	public bool moveAtAttackRange;
	public int attackRange;
	public float attackCD;

	public void PlayImpactFX()
	{

	}
}
