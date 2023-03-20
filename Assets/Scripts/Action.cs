using UnityEngine;
using UnityEditor;

public class Action {
    public GameObject actionPlayed; // L'action qui a été joué
    public GameObject target; // La cible de l'action

    public Action(GameObject actionPlayed, GameObject target) {
        this.actionPlayed = actionPlayed;
        this.target = target;
    }
}