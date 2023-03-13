using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Raw Damage On Draw", menuName = "ScriptableObjects/Effects/Raw Damage on draw")]
public class DamageOnDraw : CardEffect
{
    public int damageAmount;

    public override void ExecuteEffect(GameObject target, Card card) {
        GameManager.OnDraw += dealRawDamageOnDraw;
    }

    public override void DisableEffect() {
        GameManager.OnDraw -= dealRawDamageOnDraw;
    }

    public void dealRawDamageOnDraw() {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.GO_MonsterInvokedOppo.GetComponent<MonsterDisplay>().takeDamage(damageAmount);
    }
}
