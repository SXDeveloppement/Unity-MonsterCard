using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New", menuName = "ScriptableObjects/Effects/StatsBoost")]
public class StatsBoost : CardEffect {

    public int powerBoots;
    public int guardBoost;
    public int speedBoost;

    public override void ExecuteEffect(GameObject target, ElementalAffinity elementalAffinity, MonsterDisplay attacker) {

        applyBoost(attacker);
        if (attacker.ownedByOppo) {
            GameManager.OnSwapOppoBefore += removeBoost;
            GameManager.OnSwapOppoAfter += applyBoost;
        } else {
            GameManager.OnSwapBefore += removeBoost;
            GameManager.OnSwapAfter += applyBoost;
        }
    }

    public override void DisableEffect(MonsterDisplay monsterDisplay) {
        removeBoost(monsterDisplay);
        if (monsterDisplay.ownedByOppo) {
            GameManager.OnSwapOppoBefore -= removeBoost;
            GameManager.OnSwapOppoAfter -= applyBoost;
        } else {
            GameManager.OnSwapBefore -= removeBoost;
            GameManager.OnSwapAfter -= applyBoost;
        }
    }

    public void applyBoost(MonsterDisplay monsterDisplay) {
        monsterDisplay.buffPower += powerBoots;
        monsterDisplay.buffGuard += guardBoost;
        monsterDisplay.buffSpeed += speedBoost;
    }

    public void removeBoost(MonsterDisplay monsterDisplay) {
        monsterDisplay.buffPower -= powerBoots;
        monsterDisplay.buffGuard -= guardBoost;
        monsterDisplay.buffSpeed -= speedBoost;
    }
}
