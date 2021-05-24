using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/RangedSkill")]
public class RangedSkill : Skill {

    public Ease easingTrail;

    public override async UniTask PlaySkill(LivingEntity _Caster, LivingEntity _Target) {
        Debug.LogWarning("Attack launched with " + _Caster.name + " on " + _Target);
        if (CastFX != null) PlayCastFX(CastFX, _Caster.transform);
        if (TrailFX != null) PlayTrailFX(TrailFX, _Caster.transform, _Target.transform);
        await UniTask.Delay(200);
        _Target.healthCurrent -= damage;
        _Caster.cooldownAttack = 1f / _Caster.attackSpeed;
        _Caster.attackIsCasting = false;
        _Caster.entityState = EntityStates.IDLE;
    }

    public override async UniTask PlayTrailFX(GameObject _AssetFX, Transform _StartPosition, Transform _EndPosition) {
        GameObject trail = Instantiate(_AssetFX, _StartPosition.position + TrailOffset, Quaternion.identity);
        Tween _tween = trail.transform.DOMove(_EndPosition.position + TrailOffset, 0.5f).SetEase(Ease.Linear);
        await _tween.AsyncWaitForCompletion();
        PlayHitFX(HitFX, _EndPosition);
    }
}