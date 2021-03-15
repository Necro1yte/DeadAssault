using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class little_pharaoh : MonoBehaviour {


    public float HP = 8;
    Animator ani;
    float time = 1;
    public GameObject ExplosionPrefab;
    private float timeVal;
    public GameObject bullectPrefab;
    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        float i = Random.Range(5, 6);
        float i2 = Random.Range(2, 3);
        transform.position = new Vector3((Random.insideUnitCircle * i).x, (Random.insideUnitCircle * i2).y, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeVal >= 0.2)
        {
            Attack();
        }
        else
        {
            timeVal += Time.deltaTime;
        }
    }

    private void Attack()
    {
        Instantiate(bullectPrefab, transform.position, Quaternion.Euler(transform.eulerAngles));   
        timeVal = 0;  
    }
    
    // Collsion between enemy and player
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.transform.tag == "Buttle")
        {
            HP -= Player_Info.Player_Attack_num;
            if (HP <= 0)
            {
                Room.instance.enemy_num[Player_Info.instance.present_room]--;
                Destroy(transform.gameObject);
                Instantiate(ExplosionPrefab, c.transform.position, c.transform.rotation);
            }
        }
    }

}
