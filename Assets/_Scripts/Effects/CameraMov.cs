using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Referencia https://www.youtube.com/watch?v=GTxiCzvYNOc
public class CameraMov : MonoBehaviour
{
    public Vector2 followOffset;
    public Vector2 speed;
    public SwitchManegerScript sws;
    public float vel = 7f;
    private Vector2 threshold;
    private Rigidbody2D rb;
    private bool outx;
    private bool outy;
    private PlayerControls controls;
    GameObject followObject;
    Vector3 newPosition;

    public Input_Joystick IJ;
    private void Start() {
        threshold = CalculateThreshold();
        followObject = GameObject.FindWithTag("Player");
        sws.playerSwitch += FindPlayer;
    }
    private void FixedUpdate() {
        speed = followObject.GetComponent<LandbasedEntity>().velocity;

        Vector2 follow = followObject.transform.position;
        float xDifference = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * follow.x);
        float yDifference = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * follow.y);
        if((!IJ.lookingDown)||(IJ.left || IJ.right || IJ.dash || IJ.jump)){
            newPosition = transform.position;
        }
        if (IJ.lookingDown && !IJ.left && !IJ.right && !IJ.dash && !IJ.jump){
            newPosition = new Vector3 (follow.x, follow.y-4f, -10);
            IJ.lookDown = false;   
        }
        else{
            if(Mathf.Abs(xDifference)>=threshold.x){
                outx = true;
            }
            if(Mathf.Abs(yDifference)>=threshold.y){
                outy = true;
            }
            if (outx){
                newPosition.x = follow.x;
            }
            if (outy){
                newPosition.y = follow.y;
            }
            if (Mathf.Abs(transform.position.x - follow.x)<=1f){
                outx = false;
            }
            if (Mathf.Abs(transform.position.y - follow.y)<=1f){
                outy = false;
            }
        }
        //float moveSpeed = speed.magnitude > vel ? speed.magnitude : vel;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, vel * Time.deltaTime);
    }

    private Vector3 CalculateThreshold() {
        Rect aspect = Camera.main.pixelRect;
        Vector2 t = new Vector2(Camera.main.orthographicSize * aspect.width / aspect.height, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Vector2 border = CalculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }

    public void FindPlayer()
    {
        followObject = sws.newPlayer;
    }
}
