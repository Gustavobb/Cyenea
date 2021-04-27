using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedDoor : MonoBehaviour
{
    public int horizontalRayCount = 4;
    public float rayLenght;
    public float rayDist;
    public LayerMask collisionMask;
    Vector2 pos_clone;
    public Animator anim;

    public int id = 0;
    public bool opened = false;

    void Start()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        foreach (int idd in data.doorsClosed)
        {
            if (idd == id) 
            {
                opened = true;
                break;
            }
            // Debug.Log(idd);
        }

        if (opened) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (!opened) VerticalCollision();
    }

    void VerticalCollision()
    {
        pos_clone = (Vector2) transform.position;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos_clone, -Vector2.up, rayLenght, collisionMask);
            Debug.DrawRay(pos_clone, -Vector2.up * rayLenght, Color.red);
            pos_clone += new Vector2(rayDist, 0);

            if (hit && ! opened) 
            {
                List<int> doorIds = new List<int>();
                PlayerData data = SaveSystem.LoadPlayer();
                int size = 0;

                foreach (int idd in data.doorsClosed)
                {
                    size ++;
                    doorIds.Add(idd);
                }

                size ++;
                // Debug.Log(size);
                doorIds.Add(id);
                data.doorsClosed = new int[size];

                for (int j = 0; j < size; j++)
                    data.doorsClosed[j] = doorIds[j]; 

                SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, data.healthData, data.maxHealthData, data.manaData, data.obtainedPlayersNameData, data.doorsClosed);
                opened = true;
                anim.SetBool("Open", true);
            }
        }
    }
    
    public void DoneClose()
    {
        gameObject.SetActive(false);
    }
}
