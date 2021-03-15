using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player_Info : MonoBehaviour
{
    public int gamenum = 0;
    public GameObject[] item;
    public GameObject[] item_now = new GameObject[100];
    public static int item_num = 0;
    public static Player_Info instance;
    public static List<Vector3> itemPositionList = new List<Vector3>();// Position with items possessed

    public static float Shoot_Speed;                                   // Player's shoot speed
    public static float Move_Speed = 4;                                // Player's move speed  
    public static float Player_AllHp = 6;                              // Player's life
    public static float Player_Hp = 6;
    public static float Player_Attack_num = 1;                         // Player's attack power
    public static bool IsCanMove = true;
    public static bool IsCanMove_Up = true;
    public static bool IsCanMove_Down = true;
    public static bool IsCanMove_Right = true;
    public static bool IsCanMove_Left = true;
    public static bool IsAllEnemyOver = false;
    public static bool IsDeath;

    public ArrayList al_blood = new ArrayList ( );

    public static bool IsWuDi;             //Player's unbeatable status
    public static bool IsJianSu;           //Player's slow status
    public static bool IsTingZhi;          //Player's move status
    public static bool IsGongJi;           //Player's attack status
    public float time_wudi = 0;           
    public float time_blood = 0;          
    public float time_jiansu = 0;         
    public float time_tingzhi = 0;        
    public float time_gongji = 0;         

    public Sprite full_HP;
    public Sprite half_HP;
    public Sprite empty_HP;

    // Door consists of two parts
    Animator up_door_right_ani;
    Animator up_door_left_ani;

    Animator down_door_right_ani;
    Animator down_door_left_ani;

    Animator right_door_right_ani;
    Animator right_door_left_ani;

    Animator left_door_left_ani;
    Animator left_door_right_ani;

    bool room_up;
    bool room_down;
    bool room_right;
    bool room_left;

    Vector2 room_initial;                            // Record for inital room
    public int room_leve = 1;                        // Room level with inital 1
    public Vector2 present_room = new Vector2 (2, 2);// Record for current room

    void Awake ( )
    {
        instance = this;
    }

    void Start()
    {
        Shoot_Speed = 0.99f;
    }

    void Update ( )
    {
        
        //Open door if there's no enemy
        if ( Room.instance.enemy_num [ present_room ] <= 0 )
        {
            Open_Door_Ani ( );
            if ( present_room == Room.instance.boss_room )
            {
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Ani_Player").Find ("Next_Floor").gameObject.SetActive (true);
            }
        }

        // Judge whether stop
        if (IsTingZhi)
        {
            time_tingzhi += Time.deltaTime;
            if (time_tingzhi < 2)
            {
                IsTingZhi = true;
                Move_Speed = 0;
            }
            else
            {
                time_tingzhi = 0;
                Move_Speed = 4;
                IsTingZhi = false;
            }
        }

        // Judge whether slow
        if (IsJianSu)
        {
            time_jiansu += Time.deltaTime;
            if(time_jiansu < 3)
            {
                IsJianSu = true;
                Move_Speed = 2;
            }
            else
            {
                time_jiansu = 0;
                Move_Speed = 4;
                IsJianSu = false;
            }
        }

        // Judge whether powerful
        if (IsGongJi)
        {
            time_gongji += Time.deltaTime;
            if (time_gongji < 3)
            {
                IsGongJi = true;
                Player_Attack_num = 3;
            }
            else
            {
                time_gongji = 0;
                Player_Attack_num = 1;
                IsGongJi = false;
            }
        }

        // Judge whether unbeatable
        if ( IsWuDi )
        {
            time_wudi += Time.deltaTime;
            if ( time_wudi<1 )
            {
                IsWuDi = true;

                time_blood += Time.deltaTime;
                if ( time_blood > 0.05f )
                {
                    GameObject blood_image = Instantiate (Resources.Load ("Player/Blood_Image") as GameObject);
                    al_blood.Add (blood_image);
                    int k = Random.Range (0, 10);
                    blood_image.GetComponent<Image> ( ).sprite = Room.instance.blood_list [ k ];
                    blood_image.transform.SetParent (GameObject.Find ("Canvas").transform.Find ("Blood"));
                    blood_image.GetComponent<Image> ( ).SetNativeSize ( );
                    blood_image.GetComponent<RectTransform> ( ).anchoredPosition = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<RectTransform> ( ).anchoredPosition;
                    
                    // Leave blood during unbeatable time

                    blood_image.transform.localScale = new Vector3 (1, 1, 1);
                    time_blood = 0;
                }
               
            }
            else
            {
                IsWuDi = false;
                time_wudi = 0;
            }
        }

        // A new start with key Space after player's death
        if ( IsDeath )
        {
            if ( Input.GetKeyDown(KeyCode.Space) )
            {
                Player_Hp = 6;
                IsDeath = false;
                SceneManager.LoadScene ("Game_Fight");
               
            }
        }
    }

    void FixedUpdate ( )
    {
        if ( room_up )
        {
            Map_Up ( );
            foreach ( var item in al_blood )
            {
                Destroy ((GameObject)item);
            }
        }
        else if ( room_down )
        {
            Map_Down ( );
            foreach ( var item in al_blood )
            {
                Destroy (( GameObject )item);
            }
        }
        else if ( room_right )
        {
            Map_Right ( );
            if ( GameObject.Find ("Canvas").transform.Find ("Blood").childCount > 0 )
            {
                Destroy (GameObject.Find ("Canvas").transform.Find ("Blood").GetChild (0).gameObject);
            }
        }
        else if ( room_left )
        {
            Map_Left ( );
            foreach ( var item in al_blood )
            {
                Destroy (( GameObject )item);
            }
        }
    }

    void Map_Up ( )
    {
        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.y > ( room_initial.y - 750 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (-Vector2.up * Time.deltaTime * 80);
        }
        else
        {
            // Generate enemys
            room_up = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x, room_initial.y - 750);//房间归为
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<CircleCollider2D> ( ).enabled = true;//碰撞体显示
            GameObject.Find("hero").gameObject.SetActive(true);
            if ( Room.instance.enemy_num [ present_room ] > 0 )
                // Close door if contains no enemy（open the collider of door）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Map_Down ( )
    {

        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.y < ( room_initial.y + 750 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (Vector2.up * Time.deltaTime * 80);
        }
        else
        {
            room_down = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x, room_initial.y + 750);
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<CircleCollider2D> ( ).enabled = true;
            GameObject.Find("hero").gameObject.SetActive(true);
            if ( Room.instance.enemy_num [ present_room ] > 0 )
            // Close door if contains no enemy（open the collider of door）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Map_Right ( )
    {

        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.x > ( room_initial.x - 1350 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (-Vector2.right * Time.deltaTime * 80);
        }
        else
        {
            room_right = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x - 1350, room_initial.y);
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<CircleCollider2D> ( ).enabled = true;
            GameObject.Find("hero").gameObject.SetActive(true);
            if ( Room.instance.enemy_num [ present_room ] > 0 )
            // Close door if contains no enemy（open the collider of door）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Map_Left ( )
    {

        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.x < ( room_initial.x + 1350 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (Vector2.right * Time.deltaTime * 80);
        }
        else
        {
            room_left = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x + 1350, room_initial.y);
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<CircleCollider2D> ( ).enabled = true;
            GameObject.Find("hero").gameObject.SetActive(true);
            if ( Room.instance.enemy_num [ present_room ] > 0 )
                // Close door if contains no enemy（open the collider of door）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Open_Door_Ani ( )
    {

        // Open collider if there are rooms above
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x, present_room.y + 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_up").gameObject.SetActive (false);
        }
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x, present_room.y - 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_down").gameObject.SetActive (false);
        }
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x + 1, present_room.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_right").gameObject.SetActive (false);
        }
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x - 1, present_room.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_left").gameObject.SetActive (false);
        }

        // Animation of up door
        up_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_right").GetComponent<Animator> ( );
        up_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_left").GetComponent<Animator> ( );
        up_door_right_ani.Play ("door_up_right_open");
        up_door_left_ani.Play ("door_up_left_open");

        // Animation of down door
        down_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_right").GetComponent<Animator> ( );
        down_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_left").GetComponent<Animator> ( );
        down_door_right_ani.Play ("door_up_right_open");
        down_door_left_ani.Play ("door_up_left_open");

        // Animation of right door
        right_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_right").GetComponent<Animator> ( );
        right_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_left").GetComponent<Animator> ( );
        right_door_right_ani.Play ("door_up_right_open");
        right_door_left_ani.Play ("door_up_left_open");

        // Animation of left door
        left_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_right").GetComponent<Animator> ( );
        left_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_left").GetComponent<Animator> ( );
        left_door_right_ani.Play ("door_up_right_open");
        left_door_left_ani.Play ("door_up_left_open");
    }

    void Close_Door_Ani ( )
    {
        // All colliders of all walls
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_up").gameObject.SetActive (true);
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_down").gameObject.SetActive (true);
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_right").gameObject.SetActive (true);
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_left").gameObject.SetActive (true);

        // Animation of up door
        up_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_right").GetComponent<Animator> ( );
        up_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_left").GetComponent<Animator> ( );
        up_door_right_ani.Play ("door_up_right_close");
        up_door_left_ani.Play ("door_up_left_close");

        // Animation of down door
        down_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_right").GetComponent<Animator> ( );
        down_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_left").GetComponent<Animator> ( );
        down_door_right_ani.Play ("door_up_right_close");
        down_door_left_ani.Play ("door_up_left_close");

        // Animation of right door
        right_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_right").GetComponent<Animator> ( );
        right_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_left").GetComponent<Animator> ( );
        right_door_right_ani.Play ("door_up_right_close");
        right_door_left_ani.Play ("door_up_left_close");

        // Animation of left door
        left_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_right").GetComponent<Animator> ( );
        left_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_left").GetComponent<Animator> ( );
        left_door_right_ani.Play ("door_up_right_close");
        left_door_left_ani.Play ("door_up_left_close");

        Create_Enemy ( );
        if (present_room != Room.instance.boss_room)
        {
            Create_Barriers();
        }
        
    }

    void Create_Enemy()
    {
        if (present_room == Room.instance.boss_room)
        {

            int radom = Random.Range(7, 8);
            Room.instance.enemy_num[present_room] = 0;
            for (int i = 0; i < radom; i++)
            {
                float x = Random.Range(-400, 400);
                float y = Random.Range(-200, 200);
                GameObject go = (GameObject)Instantiate(Resources.Load("enemy/little_pharaoh"));
                ++Room.instance.enemy_num[present_room];
                
            }
        }
        else
        {
            int radom = Random.Range(5,8);
            Room.instance.enemy_num[present_room] = 0;
            for (int i = 0; i < radom; i++)
            {
                float x = Random.Range(-400, 400);
                float y = Random.Range(-200, 200);

                {
                    // Randomly decide the type of enemy
                    int enemy_type = Random.Range(0, 3);
                    if (enemy_type == 0)
                    {
                        GameObject go = (GameObject)Instantiate(Resources.Load("enemy/little_skull"));
                        ++Room.instance.enemy_num[present_room];
                    }
                    else if (enemy_type == 1)
                    {
                        GameObject go = (GameObject)Instantiate(Resources.Load("enemy/little_mummy"));
                        ++Room.instance.enemy_num[present_room];
                    }
                    else if (enemy_type == 2)
                    {
                        GameObject go = (GameObject)Instantiate(Resources.Load("enemy/little_pharaoh"));
                        ++Room.instance.enemy_num[present_room];
                    }
                }

            }
        }
        
    }

    void Create_Barriers()
    {
        // A new bucket
        int tempRand1 = 0;
        int tempRand2 = 0;
        tempRand1 = Random.Range(-1, 2);
        tempRand2 = Random.Range(-1, 2);
        Vector3 deltaVec = new Vector3();
        Vector3 tempVec = new Vector3(tempRand1 * 5, tempRand2 * 3, 0);
        Vector3 tempVec_re = -tempVec;
        
        if (tempRand1 < 0)
            deltaVec = new Vector3(1, 0, 0);
        else
            deltaVec = new Vector3(-1, 0, 0);

        int flag = 3;

        if(gamenum <= 3)
        {
            for (int i = 0; i < flag; i++)
            {
                CreateItem(item[0], tempVec, Quaternion.identity);
                float flag1 = Random.Range(0, 10);
                if (flag1 > 3 && flag1 <= 7)
                    CreateItem(item[4], tempVec, Quaternion.identity);
                if (flag1 > 7)
                    CreateItem(item[5], tempVec, Quaternion.identity);
                tempVec = tempVec + deltaVec;
            }
            for (int i = 0; i < flag; i++)
            {
                CreateItem(item[0], tempVec_re, Quaternion.identity);
                float flag1 = Random.Range(0, 10);
                if (flag1 > 3 && flag1 <= 7)
                    CreateItem(item[4], tempVec_re, Quaternion.identity);
                if (flag1 > 7)
                    CreateItem(item[5], tempVec_re, Quaternion.identity);
                tempVec_re = tempVec_re + deltaVec;
            }
        }
        if(gamenum > 3&& gamenum <= 6)
        {
            for (int i = 0; i < flag; i++)
            {
                CreateItem(item[0], tempVec, Quaternion.identity);
                float flag1 = Random.Range(0, 10);
                if (flag1 > 2 && flag1 <= 5)
                    CreateItem(item[4], tempVec, Quaternion.identity);
                if (flag1 > 5 && flag1 <= 7)
                    CreateItem(item[5], tempVec, Quaternion.identity);
                if(flag1 > 7)
                    CreateItem(item[8], tempVec, Quaternion.identity);
                tempVec = tempVec + deltaVec;
            }
            for (int i = 0; i < flag; i++)
            {
                CreateItem(item[0], tempVec_re, Quaternion.identity);
                float flag1 = Random.Range(0, 10);
                if (flag1 > 2 && flag1 <= 5)
                    CreateItem(item[4], tempVec_re, Quaternion.identity);
                if (flag1 > 5 && flag1 <= 7)
                    CreateItem(item[5], tempVec_re, Quaternion.identity);
                if (flag1 > 7)
                    CreateItem(item[8], tempVec_re, Quaternion.identity);
                tempVec_re = tempVec_re + deltaVec;
            }
        }
        if (gamenum > 6)
        {
            for (int i = 0; i < flag; i++)
            {
                CreateItem(item[0], tempVec, Quaternion.identity);
                float flag1 = Random.Range(0, 10);
                if (flag1 > 2 && flag1 <= 5)
                    CreateItem(item[4], tempVec, Quaternion.identity);
                if (flag1 > 5 && flag1 <= 7)
                    CreateItem(item[5], tempVec, Quaternion.identity);
                if (flag1 > 7 && flag1 <= 8)
                    CreateItem(item[8], tempVec, Quaternion.identity);
                if(flag1 > 8)
                    CreateItem(item[9], tempVec, Quaternion.identity);
                tempVec = tempVec + deltaVec;
            }
            for (int i = 0; i < flag; i++)
            {
                CreateItem(item[0], tempVec_re, Quaternion.identity);
                float flag1 = Random.Range(0, 10);
                if (flag1 > 2 && flag1 <= 5)
                    CreateItem(item[4], tempVec_re, Quaternion.identity);
                if (flag1 > 5 && flag1 <= 7)
                    CreateItem(item[5], tempVec_re, Quaternion.identity);
                if (flag1 > 7 && flag1 <= 8)
                    CreateItem(item[8], tempVec_re, Quaternion.identity);
                if (flag1 > 8)
                    CreateItem(item[9], tempVec_re, Quaternion.identity);
                tempVec_re = tempVec_re + deltaVec;
            }
        }
       
        // A new trap
        if (gamenum > 2)
            CreateItem(item[6], CreateRandomPosition(7, 3), Quaternion.identity);
        // A new cord
        if (gamenum > 5)
            CreateItem(item[7], CreateRandomPosition(7, 3), Quaternion.identity);
        // A new bone
        for (int i = 0; i < 1; i++)
        {
            CreateItem(item[1], CreateRandomPosition(5,3), Quaternion.identity);
        }
        // A new bone
        for (int i = 0; i < 1; i++)
        {
            CreateItem(item[2], CreateRandomPosition(2, 2), Quaternion.identity);
        }
        // A new stick
        for (int i = 0; i < 4; i++)
        {
            CreateItem(item[3], CreateRandomPosition(7, 3), Quaternion.identity);
        }
        gamenum += 1;print(gamenum);
    }
    void CreateItem(GameObject createCameObject, Vector3 createPosition, Quaternion createRotation)
    {
        GameObject itemGo = Instantiate(createCameObject, createPosition, createRotation);
        itemGo.transform.parent = GameObject.Find("Canvas").transform.Find("All_UI");
        item_now[item_num++] = itemGo;
        itemPositionList.Add(createPosition);
    }
    // Method for random position
    Vector3 CreateRandomPosition(int x, int y)
    {
        while (true)
        {
            Vector3 createPosition = new Vector3(Random.Range(-x, x), Random.Range(-y, y), 0);
            if (!HasThePosition(createPosition))
            {
                return createPosition;
            }
        }
    }
    // Method to judge whether there is item on this position
    bool HasThePosition(Vector3 createPos)
    {
        for (int i = 0; i < itemPositionList.Count; i++)
        {
            if (createPos == itemPositionList[i])
            {
                return true;
            }
        }
        return false;
    }

    public void OnCollisionStay2D ( Collision2D c )
    {
        if ( c.transform.name == "wall_u_l" || c.transform.name == "wall_u_r" || c.transform.name == "door_up" )
        {
            PlayerControl.instance.IsUpMove = false;
        }

        if ( c.transform.name == "wall_d_l" || c.transform.name == "wall_d_r" || c.transform.name == "door_down" )
        {
            PlayerControl.instance.IsDownMove = false;
        }

        if ( c.transform.name == "wall_r_u" || c.transform.name == "wall_r_d" || c.transform.name == "door_right" )
        {
            PlayerControl.instance.IsRightMove = false;
        }

        if ( c.transform.name == "wall_l_u" || c.transform.name == "wall_l_d" || c.transform.name == "door_left" )
        {
            PlayerControl.instance.IsLeftMove = false;
        }
        if (c.transform.tag == "enemybullet")
        {
            if (Player_Hp % 2 == 0)
                GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + (Player_Hp / 2).ToString()).GetComponent<Image>().sprite = half_HP;
            else if (Player_Hp % 2 != 0)
                GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + ((Player_Hp / 2) + 0.5).ToString()).GetComponent<Image>().sprite = empty_HP;
            time_wudi = 0;
            IsWuDi = true;
            Player_Hp--;
        }
        if (c.transform.tag == "Duyao")
        {
            Destroy(c.gameObject);
            if (Player_Hp > 0)
            {
                if (Player_Hp % 2 == 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + (Player_Hp / 2).ToString()).GetComponent<Image>().sprite = half_HP;
                else if (Player_Hp % 2 != 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + ((Player_Hp / 2) + 0.5).ToString()).GetComponent<Image>().sprite = empty_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }
        }
        if (c.transform.tag == "Baoxiang")
        {
            Destroy(c.gameObject);
            if (Player_Hp < 6)
            {
                if (Player_Hp % 2 == 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + (Player_Hp / 2+1).ToString()).GetComponent<Image>().sprite = half_HP;
                else if (Player_Hp % 2 != 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + ((Player_Hp / 2) + 0.5).ToString()).GetComponent<Image>().sprite = full_HP;
                Player_Hp += 1;
            }
        }
        if (c.transform.tag == "Gongsu")
        {
            Destroy(c.gameObject);
            if(Shoot_Speed > 0.2f)
                Shoot_Speed = Shoot_Speed - 0.05f;
        }
        if (c.transform.tag == "Gongji")
        {
            Destroy(c.gameObject);
            time_gongji = 0;
            IsGongJi = true;
        }
        if (c.transform.tag == "Xianjing")
        {
            Destroy(c.gameObject);
            time_jiansu = 0;
            IsJianSu = true;
        }
        if (c.transform.tag == "Shengzi")
        {
            Destroy(c.gameObject);
            time_tingzhi = 0;
            IsTingZhi = true;
        }
      
        
        if ( c.transform.tag == "Enemy"  && ( !IsWuDi )  )
        {
            if ( (Player_Hp != 0)&& (Player_Hp%2==0) )
            {

                //Debug.Log ("HP" + ( Player_Hp / 2 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + (Player_Hp/2).ToString()).GetComponent<Image> ( ).sprite = half_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }
            else if ( ( Player_Hp != 0 ) && ( Player_Hp % 2 != 0 ) )
            {

                //Debug.Log ("HP" + ( Player_Hp / 2 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( (Player_Hp / 2)+0.5 ).ToString ( )).GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }

            if( Player_Hp == 0 && IsDeath == false)
            {
                DestroyItems(item_now);
                IsDeath = true;
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP1").GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;

                // Death with a black scene
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Game_Over_BG").gameObject.SetActive (true);

                // Death with Player's disapperance
                GameObject.Find("hero").gameObject.SetActive(false);

                // Death with new image
                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").Find ("Hero_Death").gameObject.SetActive (true);
            }
        }

    }

    public void OnTriggerStay2D(Collider2D c)
    {
        
        if (c.transform.tag == "Duyao")
        {
            Destroy(c.gameObject);
            if (Player_Hp > 0)
            {
                if (Player_Hp % 2 == 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + (Player_Hp / 2).ToString()).GetComponent<Image>().sprite = half_HP;
                else if (Player_Hp % 2 != 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + ((Player_Hp / 2) + 0.5).ToString()).GetComponent<Image>().sprite = empty_HP;
                Player_Hp -= 1;
            }
        }
        if (c.transform.tag == "Baoxiang")
        {
            Destroy(c.gameObject);
            if (Player_Hp < 6)
            {
                if (Player_Hp % 2 == 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + (Player_Hp / 2 + 1).ToString()).GetComponent<Image>().sprite = half_HP;
                else if (Player_Hp % 2 != 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + ((Player_Hp / 2) + 0.5).ToString()).GetComponent<Image>().sprite = full_HP;
                Player_Hp += 1;
            }
        }
        if (c.transform.tag == "Gongsu")
        {
            Destroy(c.gameObject);
            if (Shoot_Speed > 0.2f)
                Shoot_Speed = Shoot_Speed - 0.05f;
        }
        if (c.transform.tag == "Gongji")
        {
            Destroy(c.gameObject);
            time_gongji = 0;
            IsGongJi = true;
        }
        if (c.transform.tag == "Xianjing")
        {
            Destroy(c.gameObject);
            time_jiansu = 0;
            IsJianSu = true;
        }
        if (c.transform.tag == "Shengzi")
        {
            Destroy(c.gameObject);
            time_tingzhi = 0;
            IsTingZhi = true;
        }

        if ( c.transform.tag == "Enemy" && ( !IsWuDi ) )
        {
            if ( ( Player_Hp != 0 ) && ( Player_Hp % 2 == 0 ) )
            {
                //Debug.Log ("HP" + ( Player_Hp / 2 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( Player_Hp / 2 ).ToString ( )).GetComponent<Image> ( ).sprite = half_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }
            else if ( ( Player_Hp != 0 ) && ( Player_Hp % 2 != 0 ) )
            {
                //Debug.Log ("HP" + ( (Player_Hp / 2) + 0.5 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( ( Player_Hp / 2 ) + 0.5 ).ToString ( )).GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }

            if ( Player_Hp == 0 )
            {
                IsDeath = true;
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP1").GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;

                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Game_Over_BG").gameObject.SetActive (true);

                GameObject.Find ("Canvas").transform.Find ("All_Enemy").gameObject.SetActive (false);

                GameObject.Find("hero").gameObject.SetActive(false);

                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").Find ("Hero_Death").gameObject.SetActive (true);
            }
        }
    }

    // Collision between player and walls
    void OnCollisionExit2D ( Collision2D c )
    {
        if ( c.transform.name == "wall_u_l" || c.transform.name == "wall_u_r" || c.transform.name == "door_up" )
        {
            PlayerControl.instance.IsUpMove = true;
        }

        if ( c.transform.name == "wall_d_l" || c.transform.name == "wall_d_r" || c.transform.name == "door_down" )
        {
            PlayerControl.instance.IsDownMove = true;
        }

        if ( c.transform.name == "wall_r_u" || c.transform.name == "wall_r_d" || c.transform.name == "door_right" )
        {
            PlayerControl.instance.IsRightMove = true;
        }

        if ( c.transform.name == "wall_l_u" || c.transform.name == "wall_l_d" || c.transform.name == "door_left" )
        {
            PlayerControl.instance.IsLeftMove = true;
        }

    }

    void DestroyItems(GameObject[] items)
    {
        foreach(GameObject item in items)
        {
            if (item)
                Destroy(item);
        }
    }
    
    // Room update under the trigger between player and door
    void OnTriggerEnter2D ( Collider2D c )
    {
        if (c.transform.tag == "enemybullet")
        {
            if (Player_Hp > 0)
            {
                if (Player_Hp % 2 == 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + (Player_Hp / 2).ToString()).GetComponent<Image>().sprite = half_HP;
                else if (Player_Hp % 2 != 0)
                    GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP" + ((Player_Hp / 2) + 0.5).ToString()).GetComponent<Image>().sprite = empty_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }
            if (Player_Hp == 0 && IsDeath == false)
            {
                IsDeath = true;
                GameObject.Find("Canvas").transform.Find("All_UI").Find("Player_Hp").Find("HP1").GetComponent<Image>().sprite = empty_HP;
                time_wudi = 0;

                GameObject.Find("Canvas").transform.Find("All_UI").Find("Game_Over_BG").gameObject.SetActive(true);

                GameObject.Find("Canvas").transform.Find("All_Enemy").gameObject.SetActive(false);

                GameObject.Find("hero").gameObject.SetActive(false);

                GameObject.Find("Canvas").transform.Find("Player").Find("Hero").Find("Hero_Death").gameObject.SetActive(true);
            }
        }
        
        
        if ( c.tag == "Up_Door" || c.tag == "Down_Door" || c.tag == "Right_Door" || c.tag == "Left_Door" )
        {
            room_initial = GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition;//记录房间的初始位置
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<CircleCollider2D> ( ).enabled = false;//房间移动的时候将以撒的collider暂时关闭
            DestroyItems(item_now);
            item_num = 0;
        }

        if ( c.transform.tag == "Up_Door" )
        {
            present_room += new Vector2 (0, 1);
            room_up = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (10, -200);
        }
        else if ( c.transform.tag == "Down_Door" )
        {
            present_room -= new Vector2 (0, 1);
            room_down = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (10, 240);
        }
        else if ( c.transform.tag == "Right_Door" )
        {
            present_room += new Vector2 (1, 0);
            room_right = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (-500, 20);
        }
        else if ( c.transform.tag == "Left_Door" )
        {
            present_room -= new Vector2 (1, 0);
            room_left = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Hero").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (500, 20);
        }

        
        if ( !Room.instance.room_been.Contains (present_room) )
        {
            Room.instance.room_been.Add (present_room);
        }

        if ( c.transform.name == "Next_Floor" )
        {
            
            GameObject.Find ("Canvas").transform.Find ("Player").gameObject.SetActive (false);
            //GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Ani_Player").Find ("Hero_Drop").gameObject.SetActive (true);
            ++room_leve;
            SceneManager.LoadScene("Game_Fight");
        }
    }

   

}
