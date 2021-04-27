using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Joystick : MonoBehaviour
{
    PlayerControls controls;
    public bool left, right, jump, dash, attack, changeRight, changeLeft, jumpInputUp, lookDown, lookingDown;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Left.performed += Left;
        controls.Gameplay.Right.performed += Right;

        controls.Gameplay.Left.canceled += LeftRelease;
        controls.Gameplay.Right.canceled += RightRelease;
        controls.Gameplay.Attack.canceled += AttackRelease;
        controls.Gameplay.Jump.canceled += JumpRelease;

        controls.Gameplay.Jump.performed += Jump;
        controls.Gameplay.Attack.performed += Attack;
        controls.Gameplay.Dash.performed += Dash;
        controls.Gameplay.Fly.performed += Fly;
        controls.Gameplay.ChangeCharacterLeft.performed += ChangeCharacterLeft;
        controls.Gameplay.ChangeCharacterRight.performed += ChangeCharacterRight;

        controls.Gameplay.Down.performed += LookDown;
        controls.Gameplay.Down.canceled += LookDownCancel;

        controls.Gameplay.Left.Enable();
        controls.Gameplay.Right.Enable();
        controls.Gameplay.Jump.Enable();
        controls.Gameplay.Attack.Enable();
        controls.Gameplay.Dash.Enable();
        controls.Gameplay.Fly.Enable();
        controls.Gameplay.ChangeCharacterLeft.Enable();
        controls.Gameplay.ChangeCharacterRight.Enable();
        controls.Gameplay.Down.Enable();
    }

    void Left(InputAction.CallbackContext context){
        // Debug.Log("LEFT");
        left = true;
    }
    void Right(InputAction.CallbackContext context){
        // Debug.Log("RIGHT");
        right = true;
    }

    void LeftRelease(InputAction.CallbackContext context){
        // Debug.Log("LEFTRELEASE");
        left = false;
    }
    void RightRelease(InputAction.CallbackContext context){
        // Debug.Log("RIGHTRELEASE");
        right = false;
    }

    void Jump(InputAction.CallbackContext context){
        // Debug.Log("JUMP");
        jump = true;
    }
    void JumpRelease(InputAction.CallbackContext context){
        // Debug.Log("JUMP");
        jumpInputUp = true;
    }
    void Dash(InputAction.CallbackContext context){
        Debug.Log("DASH");
        dash = true;
    }
    void Fly(InputAction.CallbackContext context){
        dash = true;
        // Debug.Log("FLY");
    }
    void Attack(InputAction.CallbackContext context){
        // Debug.Log("ATTACK");
        attack = true;
    }

    void AttackRelease(InputAction.CallbackContext context){
        // Debug.Log("ATTACK");
        attack = false;
    }
    void ChangeCharacterLeft(InputAction.CallbackContext context){
        // Debug.Log("CHANGELEFT");
        changeLeft = true;
    }
    void ChangeCharacterRight(InputAction.CallbackContext context){
        // Debug.Log("CHANGERIGHT");
        changeRight = true;
    }
    void LookDown(InputAction.CallbackContext context){
        lookDown = true;
        lookingDown = true;
    }
    void LookDownCancel(InputAction.CallbackContext context){
        lookingDown = false;
    }
}
