using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/MeleeSkill")]
public class MeleeSkill : Skill {

    public override async UniTask PlaySkill(LivingEntity _Caster, LivingEntity _Target) {
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
}
