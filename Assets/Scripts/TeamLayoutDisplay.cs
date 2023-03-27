using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLayoutDisplay : MonoBehaviour
{
    public bool ownedByOppo;
    public GameObject layoutArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hide() {
        gameObject.SetActive(false);
    }

    public void show() {
        if (gameObject.activeSelf) {
            hide();
        } else {
            gameObject.SetActive(true);
            FindAnyObjectByType<GameManager>().refreshTeamAreaLayout();
        }
    }
}
