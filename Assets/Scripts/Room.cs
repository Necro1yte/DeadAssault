using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Room : MonoBehaviour
{


    public static Room instance;

    List<Vector2> all_chest_room = new List<Vector2> ( );// List to reserve boss room
    public Vector2 boss_room;                            // Location of Boss room
    public Dictionary<Vector2, int> enemy_num = new Dictionary<Vector2, int> ( ); // Reservation of enemy's number


    public List<Vector2> room_id = new List<Vector2> ( );         // Reservation of room's id
    public List<Vector2> room_close = new List<Vector2> ( );      // Reservation of rooms neaby
    public List<Vector2> room_been = new List<Vector2> ( );       // Reservation of roons known
    public List<Vector2> room_never_been = new List<Vector2> ( ); // Reservation of rooms unknown
    public List<Vector2> room_allShow = new List<Vector2> ( );    // Reservation of rooms shown

    public int show_map_num = 1;

    public bool Ani_Is_Over;

    public Sprite chest_door_image;
    public Sprite boss_door_image;
    public Sprite boss_door_in_image;
    public Sprite boss_door_right;
    public Sprite boss_door_left;
    public Sprite room_never_been_image;
    public Sprite room_been_image;
    public Sprite room_present_image;

    public List<Sprite> blood_list = new List<Sprite> ( );

    public AudioClip fight;
    public AudioClip dead;
    public bool IsGoChest;
    public bool isPlayingDead = false;
    void Awake ( )
    {
        instance = this;
        room_allShow.Add (new Vector2 (2, 2));
        GameObject.Find("Manager").GetComponent<AudioSource>().clip = fight;
        GameObject.Find("Manager").GetComponent<AudioSource>().loop = true;
        GameObject.Find("Manager").GetComponent<AudioSource>().Play();
    }

    // Use this for initialization
    void Start ( )
    {
        room_been.Add (new Vector2 (2, 2));
        CopyRoom ( );
        Create_Room ( );
    }

    // Update is called once per frame
    void Update ( )
    {
        if (Player_Info.IsDeath && !isPlayingDead)
        {
            GameObject.Find("Manager").GetComponent<AudioSource>().clip = dead;
            GameObject.Find("Manager").GetComponent<AudioSource>().loop = false;
            GameObject.Find("Manager").GetComponent<AudioSource>().Play();
            isPlayingDead = true;
        }

        // Map shown with key Tab
        if ( Input.GetKeyDown (KeyCode.Tab) && ( show_map_num % 2 == 1 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").gameObject.SetActive (false);
            show_map_num++;
        }
        else if ( Input.GetKeyDown (KeyCode.Tab) && ( show_map_num % 2 == 0 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").gameObject.SetActive (true);
            show_map_num++;
        }

        
    }

    void Create_Room ( )
    {
        room_id.Add (new Vector2 (2, 2));

        while ( room_id.Count < 7 )
        {
            int x = Random.Range (0, 5);
            int y = Random.Range (0, 5);

            if ( !room_id.Contains (new Vector2 (x, y)) && ( x != 2 && ( y != 2 ) ) ) 
            {
                room_id.Add (new Vector2 (x, y));
            }
        }
        foreach ( var item in room_id )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + item.x.ToString ( ) + item.y.ToString ( )).gameObject.SetActive (true);
        }

        //Collecting each room
        for ( int i = 0; i < 7; i++ )
        {
            SeekNextRoom (room_id [ i ]);
        }

        //Install door for each room
        for ( int i = 0; i < room_id.Count; i++ )
        {
            install_door (room_id [ i ]);
        }
        
        boss_room = all_chest_room [ Random.Range (0, all_chest_room.Count - 1) ];      
        all_chest_room.Remove (boss_room);
        install_chest_door (boss_room, "boss_room");

        room_id.Remove (new Vector2 (2, 2));
        room_id.Remove (boss_room);

        enemy_num.Add (new Vector2 (2, 2), 0);
        
        enemy_num.Add (boss_room, 1);
        for ( int i = 0; i < room_id.Count; i++ )
        {
            enemy_num.Add (room_id [ i ], 1);
        }

        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_22").Find ("All_Kuang").Find ("Tip").gameObject.SetActive (true);

        room_id.Add (new Vector2 (2, 2));
        
        room_id.Add (boss_room);

        for ( int i = 0; i < room_id.Count; i++ )
        {
            if ( Room_Quan (room_id [ i ]) == 1 )
            {
                room_allShow.Add (room_id [ i ]);
            }
        }

        Map_Show ( );
    }

    // Calculate the room passed from current room to initial room
    // Judge whehter the room chosen is existing
    // Keep choosing the next room
    void SeekNextRoom ( Vector2 v )
    {
        List<Vector2> all_GoodRoom = new List<Vector2> ( );
        //Debug.Log ("origonal room is:" + v);
        for ( int j = 0; j < 5; j++ )
        {
            for ( int k = 0; k < 5; k++ )
            {
                if ( ( Room_Quan (new Vector2 (j, k)) == ( Room_Quan (v) - 1 ) ) && ( Room_Distance (v, new Vector2 (j, k)) == 1 ) )
                {
                    //Debug.Log ("seek room is:" + j + " , " + k);
                    all_GoodRoom.Add (new Vector2 (j, k));
                }
            }
        }


        if ( ( v == new Vector2 (2, 2) ) || all_GoodRoom [ 0 ] == new Vector2 (2, 2) )
        {
            return;
        }
        else if ( all_GoodRoom.Count == 1 )
        {
            if ( room_id.Contains (all_GoodRoom [ 0 ]) )
            {
            }
            else
            {

                room_id.Add (all_GoodRoom [ 0 ]);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + all_GoodRoom [ 0 ].x.ToString ( ) + all_GoodRoom [ 0 ].y.ToString ( )).gameObject.SetActive (true);
            }
        }
        //Randomly choose one room
        else if ( room_id.Contains (all_GoodRoom [ 0 ]) && room_id.Contains (all_GoodRoom [ 1 ]) )
        {
            int i = Random.Range (0, 2);


            if ( Room_Quan (v) != 1 )
            {
                SeekNextRoom (all_GoodRoom [ i ]);
            }
        }
        else if ( ( !room_id.Contains (all_GoodRoom [ 0 ]) ) && ( !room_id.Contains (all_GoodRoom [ 1 ]) ) )
        {
            int i = Random.Range (0, 2);

            //Debug.Log ("new room is:" + all_GoodRoom [ i ]);

            room_id.Add (all_GoodRoom [ i ]);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + all_GoodRoom [ i ].x.ToString ( ) + all_GoodRoom [ i ].y.ToString ( )).gameObject.SetActive (true);

            if ( Room_Quan (v) != 1 )
            {
                SeekNextRoom (all_GoodRoom [ i ]);
            }
        }
        else
        {
            if ( room_id.Contains (all_GoodRoom [ 0 ]) )
            {
                SeekNextRoom (all_GoodRoom [ 0 ]);
            }
            else
            {
                SeekNextRoom (all_GoodRoom [ 1 ]);
            }
        }
    }

    // Calculation of weight of each room
    float Room_Quan ( Vector2 v )
    {
        return Mathf.Abs(v.x - 2) + Mathf.Abs(v.y - 2);
    }
    
    // Caculation of two rooms
    float Room_Distance ( Vector2 v1, Vector2 v2 )
    {
        return Mathf.Abs (v1.x - v2.x) + Mathf.Abs (v1.y - v2.y);
    }

    void install_door ( Vector2 v )
    {
        int door_num = 0;

        if ( room_id.Contains (new Vector2 (v.x, v.y + 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").gameObject.SetActive (true);
            door_num++;
        }
        else
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up_content").gameObject.SetActive (false);
          
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_up").gameObject.SetActive (true);
        }

        if ( room_id.Contains (new Vector2 (v.x, v.y - 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").gameObject.SetActive (true);
            door_num++;
        }
        else
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down_content").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_down").gameObject.SetActive (true);
        }

        if ( room_id.Contains (new Vector2 (v.x - 1, v.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").gameObject.SetActive (true);
            door_num++;
        }
        else
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left_content").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_left").gameObject.SetActive (true);
        }

        if ( room_id.Contains (new Vector2 (v.x + 1, v.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").gameObject.SetActive (true);
            door_num++;
        }
        else
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right_content").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_right").gameObject.SetActive (true);
        }

        if ( door_num == 1 )
        {
            all_chest_room.Add (v);
        }
    }

    
    void CopyRoom ( )
    {
        for ( int j = 0; j < 5; j++ )
        {
            for ( int k = 0; k < 5; k++ )
            {
                if ( ( j != 2 ) || ( k != 2 ) )
                {
                    GameObject go = ( GameObject )Instantiate (GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_22").gameObject);
                    go.transform.parent = GameObject.Find ("Canvas").transform.Find ("All_Map");
                    go.transform.localScale = new Vector3 (1, 1, 1);
                    go.transform.GetComponent<RectTransform> ( ).anchoredPosition3D = new Vector3 (( j - 2 ) * 1350, ( k - 2 ) * 750, 0);
                    go.transform.name = "Room_" + j.ToString ( ) + k.ToString ( );
                    go.SetActive (false);
                }
            }
        }
    }

    // Door intalled
    void install_chest_door ( Vector2 v, string room_type )
    {
        if ( room_type == "chest_room" )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = chest_door_image;
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = chest_door_image;
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = chest_door_image;
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = chest_door_image;

            if ( room_id.Contains (v + new Vector2 (0, 1)) ) 
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = chest_door_image;
            }
            else if ( room_id.Contains (v - new Vector2 (0, 1)) )
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = chest_door_image;
            }
            else if ( room_id.Contains (v + new Vector2 (1, 0)) )
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = chest_door_image;
            }
            else if ( room_id.Contains (v - new Vector2 (1, 0)) )
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = chest_door_image;
            }
        }
        else if ( room_type == "boss_room" )
        {
            if ( room_id.Contains (v + new Vector2 (0, 1)) )
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = boss_door_image;
                
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = boss_door_image;
            
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
           
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down").Find ("light").gameObject.SetActive (true);
         
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
            else if ( room_id.Contains (v - new Vector2 (0, 1)) )
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up").Find ("light").gameObject.SetActive (true);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
            else if ( room_id.Contains (v + new Vector2 (1, 0)) )
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left").Find ("light").gameObject.SetActive (true);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
            else if ( room_id.Contains (v - new Vector2 (1, 0)) )
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right").Find ("light").gameObject.SetActive (true);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
        }

    }

    // Little map shown
    public void Map_Show ( )
    {
        room_close.Clear ( );
        room_never_been.Clear ( );
        for ( int i = 0; i < room_id.Count; i++ )
        {
            if ( Room_Quan (room_id [ i ]) == 1 )
            {
                room_close.Add (room_id [ i ]);
            }
        }
       
        for ( int i = 0; i < room_id.Count; i++ )
        {
            if ( !room_been.Contains (room_id [ i ]) )
            {
                room_never_been.Add (room_id [ i ]);
            }
        }

        for ( int i = 0; i < room_never_been.Count; i++ )
        {
            if ( Room_Quan (room_never_been [ i ]) == 1 )
            {
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (room_never_been [ i ].x.ToString ( ) + room_never_been [ i ].y.ToString ( )).GetComponent<Image> ( ).sprite = room_never_been_image;
            }
        }

        for ( int i = 0; i < room_been.Count; i++ )
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (room_been [ i ].x.ToString ( ) + room_been [ i ].y.ToString ( )).GetComponent<Image> ( ).sprite = room_been_image;
        }

        GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (Player_Info.instance.present_room.x.ToString ( ) + Player_Info.instance.present_room.y.ToString ( )).GetComponent<Image> ( ).sprite = room_present_image;

        for ( int i = 0; i < room_allShow.Count; i++ )
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (room_allShow [ i ].x.ToString ( ) + room_allShow [ i ].y.ToString ( )).gameObject.SetActive (true);
        }
    }

    public void Show_Room ( )
    {
        for ( int i = 0; i < room_id.Count; i++ )
        {
            if ( Room_Distance (Player_Info.instance.present_room, room_id [ i ]) == 1 )
            {
                if ( !room_allShow.Contains (room_id [ i ]) )
                {
                    room_allShow.Add (room_id [ i ]);
                }
            }
        }
    }

}
