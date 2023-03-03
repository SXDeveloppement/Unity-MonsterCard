using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Draw Effect", menuName = "ScriptableObjects/Effects/Draw effect")]
public class DrawEffect : CardEffect
{
    public int drawAmount;

    public override void ExecuteEffect(GameObject target, Card card) {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        gameManager.draw(drawAmount);
    }
}
