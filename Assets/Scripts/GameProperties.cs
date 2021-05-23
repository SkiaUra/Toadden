using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameProperties", menuName = "GameProperties")]
public class GameProperties : ScriptableObject {

    [Header("Entity To Spawn")]
    public GameObject MeleeEntity;
    public GameObject RangedEntity;
    public GameObject EnemyPrefab;
}
