using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PData
    {
        public float speed;
    }

    [System.Serializable]
    public class Gdat
    {
        public PData player_data;
    }

    //Movement Variables
    private Vector2 input;
    private CharacterController charc;
    private Vector3 dir;

    //Gravity Variables
    private float grav = -9.81f;
    [SerializeField] private float GravityMult = 3.0f;

    //Roation and Velocity variables
    [SerializeField] private float stime = 0.05f;
    private float cvelocity;
    private float velocity;

    private float playerspeed;
    private float speedmult = 3.5f;

    void loadconfig()
    {
        TextAsset json = Resources.Load<TextAsset>("doofus_diary");

        if (json != null)
        {
            Gdat gameData = JsonUtility.FromJson<Gdat>(json.text);
            playerspeed = gameData.player_data.speed;
        }
        else
        {
            Debug.LogError("Config file not found.");
        }
    }

    private void Awake()
    {
        charc = GetComponent<CharacterController>();
    }

    private void MoveChar()
    {
        charc.Move(dir*playerspeed*speedmult*Time.deltaTime);
    }

    private void RotateChar()
    {
        if (input.sqrMagnitude == 0) return;
        var tangl = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, tangl, ref cvelocity, stime);
        transform.rotation=Quaternion.Euler(0, angle, 0);
    }
    private void ApplyGravity()
    {
        if(charc.isGrounded && velocity<0.0f)
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += grav * GravityMult * Time.deltaTime;
        }
        
        dir.y= velocity;
    }

    public void Start()
    {
        loadconfig();
    }

    public void Update()
    {
        ApplyGravity();
        RotateChar();
        MoveChar();
    }

    public void move(InputAction.CallbackContext context)
    {
        input=context.ReadValue<Vector2>();
        dir = new Vector3(input.x, 0.0f, input.y);

    }
}
