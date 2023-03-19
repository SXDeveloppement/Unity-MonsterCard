using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Raw Damage On Draw", menuName = "ScriptableObjects/Effects/Raw Damage on draw")]
public class DamageOnDraw : CardEffect
{
    public int damageAmount;

    public override void ExecuteEffect(GameObject target, ElementalAffinity elementalAffinity, MonsterDisplay attacker) {
        if (!attacker.ownedByOppo)
            GameManager.OnDraw += dealRawDamageOnDraw;
        else
            GameManager.OnDrawOppo += dealRawDamageOnDraw;
    }

    public override void DisableEffect(MonsterDisplay monsterDisplay) {
        if (!monsterDisplay.ownedByOppo)
            GameManager.OnDraw -= dealRawDamageOnDraw;
        else
            GameManager.OnDrawOppo -= dealRawDamageOnDraw;
    }

    public void dealRawDamageOnDraw(MonsterDisplay monsterDisplay) {
        if (!monsterDisplay.ownedByOppo)
            GameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().takeDamage(damageAmount);
        else
            GameManager.GO_MonsterInvoked.GetComponent<MonsterDisplay>().takeDamage(damageAmount);
    }
}
