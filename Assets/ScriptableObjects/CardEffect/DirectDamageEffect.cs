using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Direct Damage Effect", menuName = "ScriptableObjects/Effects/Direct damage effect")]
public class DirectDamageEffect : CardEffect
{
    public int damageAmount;

    public override void ExecuteEffect(GameObject target, Card card) {
        target.GetComponent<MonsterDisplay>().takeDamage(damageAmount, card.elementalAffinity);
    }
}
