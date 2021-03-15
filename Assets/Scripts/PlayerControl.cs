using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    public bool IsUpMove = true;
    public bool IsDownMove = true;
    public bool IsRightMove = true;
    public bool IsLeftMove = true;
    Animator body_ani;
    float time = 1; 
    float h;
    float v;
    enum State
    {
        move_idle,
        move_up,
        move_down,
        move_right,
        move_left,

        attack_idle,
        attack_up,
        attack_down,
        attack_right,
        attack_left
    }
    State state = new State ( );

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ( )
    {
        body_ani = GameObject.Find ("hero").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update ( )
    {
        body_update();
    }

    void FixedUpdate()
    {
        Shoot ( );
        Move();
    }

    void body_update()
    {
        switch (state)
        {
            case State.move_idle:
                body_ani.SetBool("up", false);
                body_ani.SetBool("down", false);
                body_ani.SetBool("left", false);
                body_ani.SetBool("right", false);
                body_ani.SetBool("idle", true);
                break;
            case State.move_up:
                body_ani.SetBool("up", true);
                body_ani.SetBool("down", false);
                body_ani.SetBool("left", false);
                body_ani.SetBool("right", false);
                body_ani.SetBool("idle", false);
                break;
            case State.move_down:
                body_ani.SetBool("up", false);
                body_ani.SetBool("down", true);
                body_ani.SetBool("left", false);
                body_ani.SetBool("right", false);
                body_ani.SetBool("idle", false);
                break;
            case State.move_right:
                body_ani.SetBool("up", false);
                body_ani.SetBool("down", false);
                body_ani.SetBool("left", false);
                body_ani.SetBool("right", true);
                body_ani.SetBool("idle", false);
                break;
            case State.move_left:
                body_ani.SetBool("up", false);
                body_ani.SetBool("down", false);
                body_ani.SetBool("left", true);
                body_ani.SetBool("right", false);
                body_ani.SetBool("idle", false);
                break;
            default:
                break;
        }
    }

    void Move ( )
    {
        h = Input.GetAxis ("Horizontal");
        v = Input.GetAxis ("Vertical");

        if ( h > 0 )
        {
            state = State.move_right;
        }
        else if ( h < 0 )
        {
            state = State.move_left;
        }
        else if ( v > 0 )
        {
            state = State.move_up;
        }
        else if ( v < 0 )
        {
            state = State.move_down;
        }
        else
        {
            state = State.move_idle;
        }

        if ( v > 0 && IsUpMove )
        {
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position += new Vector3 (0, v * Time.deltaTime * Player_Info.Move_Speed, 0);
        }
        if ( v < 0 && IsDownMove )
        {
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position += new Vector3 (0, v * Time.deltaTime * Player_Info.Move_Speed, 0);
        }


        if ( h > 0 && IsRightMove )
        {
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position += new Vector3 (h * Time.deltaTime * Player_Info.Move_Speed, 0, 0);
        }
        if ( h < 0 && IsLeftMove )
        {
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position += new Vector3 (h * Time.deltaTime * Player_Info.Move_Speed, 0, 0);
        }
    }

    void Shoot ( )
    {
        if ( Input.GetKey (KeyCode.L) )
        {
            time += Time.deltaTime;

            if ( time>Player_Info.Shoot_Speed )
            {
                GameObject go = (GameObject)Instantiate (Resources.Load ("buttle/buttle"));
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (Vector2.right * 400);
                time = 0;
            }
        }
        else if ( Input.GetKey (KeyCode.J) )
        {
            time += Time.deltaTime;

            if ( time > Player_Info.Shoot_Speed )
            {
                GameObject go = ( GameObject )Instantiate (Resources.Load ("buttle/buttle"));
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (-Vector2.right * 400);
                time = 0;
            }
        }
        else if ( Input.GetKey (KeyCode.I) )
        {
            time += Time.deltaTime;

            if ( time > Player_Info.Shoot_Speed )
            {
                GameObject go = ( GameObject )Instantiate (Resources.Load ("buttle/buttle"));
                go.GetComponent<Buttle> ( ).IsVertical = true;
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (Vector2.up * 400);
                time = 0;
            }
        }
        else if ( Input.GetKey (KeyCode.K) )
        {
            time += Time.deltaTime;

            if ( time > Player_Info.Shoot_Speed )
            {
                GameObject go = ( GameObject )Instantiate (Resources.Load ("buttle/buttle"));
                go.GetComponent<Buttle> ( ).IsVertical = true;
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (-Vector2.up * 400);
                time = 0;
            }
        }
        else
        {
            time = 1;
        }

    }
}
