using UnityEngine;
using System.Collections;

public class Buttle : MonoBehaviour {

    // Use this for initialization
    public GameObject[] item;
    public static Buttle instance;
    Animator ani;
    public bool IsVertical;
    public GameObject ExplosionPrefab;

    void Awake()
    {
        instance = this;
        ani = transform.GetComponent<Animator>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.transform.tag == "qiang")
        {
            // Collision happens and stop
            transform.GetComponent<Rigidbody2D>().drag = 999;
            // To disappear the collider
            transform.GetComponent<CircleCollider2D>().enabled = false;     
            Instantiate(ExplosionPrefab, c.transform.position, c.transform.rotation);
            ani.Play("buttle_boom_ani");
            Destroy(c.gameObject);
        }
        else if (c.transform.tag == "Barrier")
        {
            transform.GetComponent<Rigidbody2D>().drag = 999;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            ani.Play("buttle_boom_ani");
        }
        else if (c.transform.tag == "enemybullet")
        {
            transform.GetComponent<Rigidbody2D>().drag = 999;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            ani.Play("buttle_boom_ani");
            Destroy(c.gameObject);
        }
        else if (c.transform.tag != "Player" && c.transform.tag != "Buttle" && c.transform.tag != "Baoxiang" && c.transform.tag != "Xianjing" && c.transform.tag != "Shengzi" && c.transform.tag != "Duyao" && c.transform.tag != "Gongsu" && c.transform.tag != "Gongji")
        {
            transform.GetComponent<Rigidbody2D>().drag = 999;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            ani.Play("buttle_boom_ani");
        }
        

    }

    //Method invoked after explosion animation
    void destory_self()
    {
        Destroy(transform.gameObject, 1f);
    }

}
