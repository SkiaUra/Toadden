using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "GameProperties", menuName = "GameProperties")]
public class GameProperties : ScriptableObject {

    [Header("Entity To Spawn")]
    public GameObject MeleeEntity;
    public GameObject RangedEntity;
    public GameObject EnemyPrefab;

    [Required, AssetsOnly]
    public List<GameObject> AllyFormation;
    public List<GameObject> Enemyformation;
}
