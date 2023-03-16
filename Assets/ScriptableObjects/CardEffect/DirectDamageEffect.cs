using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Direct Damage Effect", menuName = "ScriptableObjects/Effects/Direct damage effect")]
public class DirectDamageEffect : CardEffect
{
    public int damageAmount;

    public override void ExecuteEffect(GameObject target, ElementalAffinity elementalAffinity) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int calculateDamage = GameManager.calculateDamage(target, elementalAffinity, damageAmount);
        if (target.GetComponent<MonsterDisplay>() != null)
            target.GetComponent<MonsterDisplay>().takeDamage(calculateDamage);
        else if (target.GetComponent<CardDisplay>() != null)
            target.GetComponent<SbireDisplay>().takeDamage(calculateDamage);
    }

    public override void DisableEffect() {
        throw new System.NotImplementedException();
    }
}
