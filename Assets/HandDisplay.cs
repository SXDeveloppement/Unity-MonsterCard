using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandDisplay : MonoBehaviour
{
    public float zStep;
    public bool childHaveChanged;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (childHaveChanged) {
            childHaveChanged = false;
            GetComponent<HorizontalLayoutGroup>().enabled = false;
            GetComponent<HorizontalLayoutGroup>().enabled = true;
            for (int i = 0; i < transform.childCount; i++) {
                // On réduit la position Z de l'enfant de zStep * le numéro de l'enfant
                transform.GetChild(i).transform.localPosition 
                    = new Vector3(
                        transform.GetChild(i).transform.localPosition.x, 
                        transform.GetChild(i).transform.localPosition.y, 
                        -zStep * i
                        );
            }

            Debug.Log("Refresh Hand");
        }
    }
}
