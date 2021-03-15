using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class little_mummy : MonoBehaviour {

    bool IsCanMove;
    Vector3 aimPos;
    public float HP = 3;
    Animator ani;
    float time = 1;

    void Awake()
    {
        ani = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        aimPos = transform.position;
        float i = Random.Range(5, 6);
        float i2 = Random.Range(2, 3);
        transform.position = new Vector3((Random.insideUnitCircle * i).x, (Random.insideUnitCircle * i2).y, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsCanMove)
        {
            Move();
        }
    }


    void Move()
    {
        if (Vector2.Distance(transform.position, aimPos) > 0.3f)
        {
            transform.position = Vector3.Lerp(transform.position, aimPos, Time.deltaTime * 2f);
        }
        else
        {
            float i = Random.Range(2, 5);
            Vector2 point = Random.insideUnitCircle * i;
            aimPos = new Vector3(transform.position.x - point.x, transform.position.y - point.y, 0);
        }

    }

    // Select a new aimPos after collision with barriers
    void OnCollisionStay2D(Collision2D c)
    {
        float i = Random.Range(2, 5);
        Vector2 point = Random.insideUnitCircle * i;
        aimPos = new Vector3(transform.position.x - point.x, transform.position.y - point.y, 0);
    }

    // Collision between enemy and barriers
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.transform.tag == "Buttle")
        {
            HP -= Player_Info.Player_Attack_num;
            if (HP <= 0)
            {
                transform.GetComponent<CircleCollider2D>().enabled = false;
                IsCanMove = false;
                ani.SetBool("IsDeath", true);
            }
        }
    }

    // Invoked by animation event
    void IsCanMove_Math()
    {
        IsCanMove = true;
        //In order to avoid being collided by barriers
        transform.GetComponent<CircleCollider2D>().enabled = true;
    }

    // Invoked by animation event
    void Destroy_self()
    {
        Room.instance.enemy_num[Player_Info.instance.present_room]--;
        Destroy(transform.gameObject);
    }

}
