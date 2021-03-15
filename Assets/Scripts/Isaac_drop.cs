using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Isaac_drop : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Invoked after player drops to another layer
    public void Drop_Ani()
    {
        SceneManager.LoadScene ("Game_Fight");
    }
}
