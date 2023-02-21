using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "ScriptableObjects/Card")]
public class Card : ScriptableObject {
    public new string name;
    public string description;
    public Sprite artwork;
    public int manaCost;
    public int priority;
    public ElementalAffinity elementalAffinity;
    public Type type;
    public From from;
    public CardEffect[] effects;

    public void activeEffect(GameObject target) {
        foreach(var effect in effects) {
            effect.ExecuteEffect(target, this);
        }
    }
}