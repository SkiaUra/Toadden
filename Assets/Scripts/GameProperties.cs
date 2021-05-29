using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "GameProperties", menuName = "GameProperties")]
public class GameProperties : SerializedScriptableObject {

    [Title("Entity To Spawn")]
    public CreatureAsset[] creatureAssets;

    [Space]
    [BoxGroup("Battle Tester")]
    [Title("Entity To Spawn")]
    public GameObject MeleeEntity;
    [BoxGroup("Battle Tester")]
    public GameObject RangedEntity;
    [BoxGroup("Battle Tester")]
    public GameObject EnemyPrefab;


    [BoxGroup("Battle Tester")]
    [ButtonGroup("Battle Tester/Add Entity")]
    private void Rusher() {
        AllyFormation.Add(MeleeEntity);
    }

    [BoxGroup("Battle Tester")]
    [ButtonGroup("Battle Tester/Add Entity")]
    private void Mortar() {
        AllyFormation.Add(RangedEntity);
    }

    [BoxGroup("Battle Tester")]
    [Required, AssetsOnly]
    [PropertyOrder(0)]
    public List<GameObject> AllyFormation;
    [Space]
    public List<GameObject> Enemyformation;

    public GameObject GetCreatureGO(CreatureSpecies _species) {
        foreach (CreatureAsset creature in creatureAssets) {
            if (creature.species == _species) return creature.gameObject;
        }
        return creatureAssets[0].gameObject;
    }

    public Sprite GetCreatureSprite(CreatureSpecies _species) {
        foreach (CreatureAsset creature in creatureAssets) {
            if (creature.species == _species) return creature.sprite;
        }
        return creatureAssets[0].sprite;
    }

    [Serializable]
    public struct CreatureAsset {
        public CreatureSpecies species;
        public GameObject gameObject;
        public Sprite sprite;
    }
}
