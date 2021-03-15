using UnityEngine;
using System.Collections;

public class VS_Ani : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Invoked by animation event
    public void Ani_()
    {
        GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Ani_Player").Find("BG").gameObject.SetActive (false);
        Room.instance.Ani_Is_Over = true;

        GameObject.Find ("Canvas").transform.Find ("Player").gameObject.SetActive (true);
    }
}
