﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMapScript : MonoBehaviour
{
    public int id = 0;
    public int horizontalRayCount = 4;
    public float rayLenght;
    public float rayDist;
    public LayerMask collisionMask;
    Vector2 pos_clone;

    void FixedUpdate()
    {
        VerticalCollision();
    }

    void VerticalCollision()
    {
        pos_clone = (Vector2) transform.position;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos_clone, -Vector2.up, rayLenght, collisionMask);
            Debug.DrawRay(pos_clone, -Vector2.up * rayLenght, Color.red);
            pos_clone += new Vector2(rayDist, 0);

            if (hit) 
            {
                // Debug.Log(pos_clone);
            }
        }
    }
}
