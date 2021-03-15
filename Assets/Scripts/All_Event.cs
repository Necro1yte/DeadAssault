using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class All_Event : MonoBehaviour {
    void Start()
    {

    }
    
    // Load a new scene for Game_Fight
    void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Game_Fight");
        }
	}
}
