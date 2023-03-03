using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Destroy Enchantment", menuName = "ScriptableObjects/Effects/Destroy enchantment")]
public class DestroyEnchantment : CardEffect
{
    public override void ExecuteEffect(GameObject target, Card card) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        gameManager.inGrave(target);
    }
}
