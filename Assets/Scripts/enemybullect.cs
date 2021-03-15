using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class enemybullect : MonoBehaviour
{
    public Sprite full_HP;
    public Sprite half_HP;
    public Sprite empty_HP;

    public float moveSpeed = 5;
    public int temp;

    // Use this for initialization
    void Start()
    {
       temp = Random.Range(0, 4);
    }

    // Update is called once per frame
    void Update()
    {
        if(temp == 0)
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime, Space.World);
        if (temp == 1)
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime, Space.World); 
        if (temp == 2)
            transform.Translate(-Vector2.up * moveSpeed * Time.deltaTime, Space.World); 
        if (temp == 3)
            transform.Translate(-Vector2.right * moveSpeed * Time.deltaTime, Space.World);
    }


    private void OnTriggerEnter2D(Collider2D c)
    {

        if (c.transform.tag == "Player")
        {
            if (Player_Info.Player_Hp > 0)
            {
                transform.GetComponent<Rigidbody2D>().drag = 999;           // Collider happens and stop move
                transform.GetComponent<CircleCollider2D>().enabled = false; // Disappear collider
                if (Player_Info.Player_Hp % 2 == 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + (Player_Info.Player_Hp / 2).ToString()).GetComponent<Image>().sprite = half_HP;
                else if (Player_Info.Player_Hp % 2 != 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + ((Player_Info.Player_Hp / 2) + 0.5).ToString()).GetComponent<Image>().sprite = empty_HP;
                Destroy(gameObject);
            }
        }
        else if (c.transform.tag == "Barrier")
        {
            transform.GetComponent<Rigidbody2D>().drag = 999;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(gameObject);
        }
        else if (c.transform.tag != "Enemy" && c.transform.tag != "Buttle" && c.transform.tag != "Baoxiang" && c.transform.tag != "Xianjing" && c.transform.tag != "Shengzi" && c.transform.tag != "Duyao" && c.transform.tag != "Gongsu" && c.transform.tag != "Gongji"  && c.transform.tag != "enemybullet")
        {
            transform.GetComponent<Rigidbody2D>().drag = 999;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(gameObject);
        }

    }

}