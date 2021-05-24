using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/CreateNewSkill")]
public class Skill : ScriptableObject {
    public int damage;
    public FormationPlaces targetedLane;

    public GameObject CastFX;
    public Vector3 CastOffset;
    public GameObject HitFX;
    public Vector3 HitOffset;
    public GameObject TrailFX;
    public Vector3 TrailOffset;
    public bool moveAtAttackRange;
    public float attackRange;
    public float attackCD;

    public virtual async UniTask PlaySkill(LivingEntity _Caster, LivingEntity _Target) {
        Debug.LogWarning("Attack launched with " + _Caster.name + " on " + _Target);
        await UniTask.Delay(100);
        if (CastFX != null) PlayCastFX(CastFX, _Caster.transform);
        if (TrailFX != null) PlayTrailFX(TrailFX, _Caster.transform, _Target.transform);
        if (HitFX != null) PlayHitFX(HitFX, _Target.transform);
        _Target.healthCurrent -= damage;
        await UniTask.Delay(300);
        _Caster.cooldownAttack = 1f / _Caster.attackSpeed;
        _Caster.attackIsCasting = false;
        _Caster.entityState = EntityStates.IDLE;
    }

    public virtual void PlayCastFX(GameObject _AssetFX, Transform _PlayPosition) {
        Instantiate(_AssetFX, _PlayPosition.position + CastOffset, _PlayPosition.transform.rotation);
    }

    public virtual void PlayHitFX(GameObject _AssetFX, Transform _PlayPosition) {
        Instantiate(_AssetFX, _PlayPosition.position + HitOffset, Quaternion.identity);
    }

    public virtual async UniTask PlayTrailFX(GameObject _AssetFX, Transform _StartPosition, Transform _EndPosition) {
        GameObject trail = Instantiate(_AssetFX, _StartPosition.position + TrailOffset, Quaternion.identity);
        trail.transform.DOMove(_EndPosition.position + TrailOffset, 0.5f).SetEase(Ease.Linear);
    }
}
