using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Qiang : MonoBehaviour {

	public static Qiang instance;
	Animator ani;
	public GameObject[] items;
	public GameObject[] items_now = new GameObject[10];
	public int item_num = 0;
	// Use this for initialization
	void Start () {
		
	}

	void Awake ()
    {
		instance = this;
		ani = transform.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter2D(Collider2D c)
    {
		if(c.transform.tag == "buttle")
        {
			ani.Play("buttle_boom_ani");

		}
    }

	void CreateItem(GameObject createCameObject, Vector3 createPosition, Quaternion createRotation)
	{
		GameObject itemGo = Instantiate(createCameObject, createPosition, createRotation);
		itemGo.transform.parent = GameObject.Find("Canvas").transform.Find("All_UI");
		items_now[item_num++] = itemGo;
	}

	void destory_self()
	{
		Destroy(transform.gameObject, 1f);
	}
}
