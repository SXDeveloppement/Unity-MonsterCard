using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Destroy Aura", menuName = "ScriptableObjects/Effects/Destroy aura")]
public class DestroyAura : CardEffect
{
    public override void ExecuteEffect(GameObject target, Card card) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //target.GetComponent<CardDisplay>().destroyAura();
        gameManager.inGrave(target);
    }
}
