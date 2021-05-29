using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class PackElement {
    public GameObject EntityGO;
    public CreatureSpecies CreatureClass;

    public PackElement(GameObject _gameObject, CreatureSpecies _creaClass) {
        EntityGO = _gameObject;
        CreatureClass = _creaClass;
    }
}

public class PackManager : MonoBehaviour {
    public GameProperties gameProperties;
    public BattleManager battleManager;
    public List<PackElement> PackComposition = new List<PackElement>();


    [Button(ButtonSizes.Medium)]
    public void AddCreature(CreatureSpecies species) {
        GameObject go = gameProperties.GetCreatureGO(species);
        PackElement a = new PackElement(go, species);
        PackComposition.Add(a);
    }
}
