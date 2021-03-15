using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class little_skull : MonoBehaviour {

    bool IsCanMove;
    Vector3 aimPos;//当前目标点
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
        transform.position =new Vector3((Random.insideUnitCircle * i).x, (Random.insideUnitCircle * i2).y, 0);
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

    /// <summary>
    /// 怪物碰到物体后重新选取目标
    /// </summary>
    /// <param name="c"></param>
    void OnCollisionStay2D(Collision2D c)
    {
        float i = Random.Range(2, 5);
        Vector2 point = Random.insideUnitCircle * i;
        aimPos = new Vector3(transform.position.x - point.x, transform.position.y - point.y, 0);
    }

    /// <summary>
    /// 怪物碰撞到物体
    /// </summary>
    /// <param name="c"></param>
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

    /// <summary>
    /// 动画帧事件调用
    /// </summary>
    void IsCanMove_Math()
    {
        IsCanMove = true;
        //In order to avoid collision with barriers
        transform.GetComponent<CircleCollider2D>().enabled = true;
    }

    // Invoked by animation event
    void Destroy_self()
    {
        Room.instance.enemy_num[Player_Info.instance.present_room]--;
        Destroy(transform.gameObject);
    }

}
