using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class Locomotion : MonoBehaviour
{
    public SteamVR_Action_Vector2 touchpadAction;
    public GameObject VivePlayer;
    public SteamVR_Action_Boolean runAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Run");
    public enum PlayerStatus {Running, Crouching, Walking};
    public PlayerStatus status;
    public float height = 1.75f;
    private float threshold = 0.0f;
    public float runSpeed = 5.0f;
    public float walkSpeed = 2.0f;
    public float crouchSpeed = 1.0f;
    public float maxStamina;
    public float stamina;
    private const int MAX_POS_REMEMBER = 20;

    private Queue<Vector3> qTrans = new Queue<Vector3>(MAX_POS_REMEMBER);
    private int qTransSize = 0;

    public bool playerDontMove;
    public Vector2 badDir;

    public enum PlayerMovementMode {Locomotion, Teleportation};
    public PlayerMovementMode moveMode;

    public float staminaPercent;


    void Awake()
    {
        threshold = height * 0.5f;
        stamina = maxStamina;
        staminaPercent = stamina / maxStamina;
        moveMode = PlayerMovementMode.Teleportation;
        //Debug.Log(threshold);
    }


    // Start is called before the first frame update
    void Start()
    {
        status = PlayerStatus.Walking;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveMode == PlayerMovementMode.Locomotion)
            LocomotionUpdate();
        else
            TeleportationUpdate();

        // updates stamina
        GameObject.FindWithTag("Watch").GetComponent<WatchManager>().UpdateStamina(staminaPercent);
    }

    private bool CheckHeight(float threshold)
    {
        Transform head = VivePlayer.transform.GetChild(0).GetChild(3);
        RaycastHit hit;
        Ray ray = new Ray(head.position, Vector3.down);
        Debug.DrawRay(head.position, Vector3.down, Color.yellow);

        if (Physics.Raycast(ray, out hit))
        {
            float distance = head.position.y - hit.transform.position.y;
            //Debug.Log("Distance is: " + distance);

            if (distance < threshold)
                return true;
        }
        return false;
    }


    // Deprecated
    /*
    private Vector2 GetMoveableDirection(Vector2 touchpadValue)
    {
        float retX = 0.0f;
        float retY = 0.0f;

        if(badDir.x > VivePlayer.transform.position.x)
        {
            if (touchpadValue.x < 0)
                retX = touchpadValue.x;
        }
        if (badDir.x < VivePlayer.transform.position.x)
        {
            if (touchpadValue.x > 0)
                retX = touchpadValue.x;
        }
        if (badDir.y > VivePlayer.transform.position.y)
        {
            if (touchpadValue.y < 0)
                retY = touchpadValue.y;
        }
        if (badDir.y < VivePlayer.transform.position.y)
        {
            if (touchpadValue.y > 0)
                retY = touchpadValue.y;
        }

        return new Vector2(retX, retY);
    }*/

    private void storePreviousLocation(Vector3 pos)
    {
        if(qTransSize == MAX_POS_REMEMBER)
        {
            qTrans.Dequeue();
            qTransSize--;
        }
        qTrans.Enqueue(pos);
        qTransSize++;
    }

    private Vector3 retrievePreviousLocation()
    {
        qTransSize--;
        return qTrans.Dequeue();
    }

    // Our update function in the locomotion movement mode
    private void LocomotionUpdate()
    {
        Vector2 touchpadValue = touchpadAction.GetAxis(SteamVR_Input_Sources.Any);
        //if(touchpadValue != Vector2.zero)
        //{
        //Debug.Log(touchpadValue);
        //}

        Vector3 fwd = GameObject.Find("FollowHead").transform.forward;
        Vector3 right = GameObject.Find("FollowHead").transform.right;
        float up = GameObject.Find("VRCamera").transform.position.y;

        if (runAction.GetStateDown(SteamVR_Input_Sources.Any) && status != PlayerStatus.Crouching)
        {
            //Debug.Log("Running");
            //running = true;
            status = PlayerStatus.Running;

        }
        if (runAction.GetStateUp(SteamVR_Input_Sources.Any) && status != PlayerStatus.Crouching)
        {
            //Debug.Log("Not Running");
            //running = false;
            status = PlayerStatus.Walking;
        }
        if (CheckHeight(threshold))
        {
            // print("Crouching");
            status = PlayerStatus.Crouching;
        }
        else
        {
            if (status == PlayerStatus.Crouching)
            {
                //print("Not crouching");
                status = PlayerStatus.Walking;
            }
        }


        if (status != PlayerStatus.Running)
            if (stamina < maxStamina)
                stamina += Time.deltaTime;


        if (playerDontMove)
        {
            //touchpadValue = GetMoveableDirection(touchpadValue);
            VivePlayer.transform.position = retrievePreviousLocation();
        }


        // Determine Locomotion Speeds
        if (status == PlayerStatus.Running)
        {
            stamina -= Time.deltaTime;
            if (stamina < 0.0f)
            {
                stamina = 0.0f;
                status = PlayerStatus.Walking;
            }
            else
            {
                VivePlayer.transform.position += (right * touchpadValue.x + fwd * touchpadValue.y) * Time.deltaTime * runSpeed;
                VivePlayer.transform.position = new Vector3(VivePlayer.transform.position.x, 0, VivePlayer.transform.position.z);

                // Need to update the sound range (run Speed)
                //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().InHearingRadiusCheck(runSpeed,
                    //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().playerHead, false);
            }
        }
        else if (status == PlayerStatus.Walking)
        {
            VivePlayer.transform.position += (right * touchpadValue.x + fwd * touchpadValue.y) * Time.deltaTime * walkSpeed;
            VivePlayer.transform.position = new Vector3(VivePlayer.transform.position.x, 0, VivePlayer.transform.position.z);

            // Need to update the sound range (run Speed)
            //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().InHearingRadiusCheck(walkSpeed,
                //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().playerHead, false);


            GameObject.Find("Teleporting").GetComponent<Teleport>().arcDistance = 6.0f;
        }
        else if (status == PlayerStatus.Crouching)
        {
            VivePlayer.transform.position += (right * touchpadValue.x + fwd * touchpadValue.y) * Time.deltaTime * crouchSpeed;
            VivePlayer.transform.position = new Vector3(VivePlayer.transform.position.x, 0, VivePlayer.transform.position.z);

            // Need to update the sound range (crouch Speed)
            //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().InHearingRadiusCheck(crouchSpeed,
                //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().playerHead, false);


            GameObject.Find("Teleporting").GetComponent<Teleport>().arcDistance = 3.0f;
        }
        //Debug.Log("Stamina: " + stamina);

        staminaPercent = stamina / maxStamina;

        storePreviousLocation(VivePlayer.transform.position);
    }

    // Our update function in the teleportation movement mode
    private void TeleportationUpdate()
    {
        if (stamina < maxStamina)
            stamina += Time.deltaTime;

        float teleportDist = GameObject.Find("Teleporting").GetComponent<Teleport>().getTeleportDistance();

        if (teleportDist > 0.0) {
            stamina -= teleportDist * 0.4f;

            // Test this for the sound radius =  teleportDist * 0.4f 
            //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().InHearingRadiusCheck(teleportDist * 0.4f, 
                //GameObject.FindWithTag("Cultist").GetComponent<CultistsAIManager>().playerHead, false);
        }

        staminaPercent = stamina / maxStamina;
        GameObject.Find("Teleporting").GetComponent<Teleport>().setTeleportStamina(staminaPercent);

        if (CheckHeight(threshold))
        {
            // print("Crouching");
            status = PlayerStatus.Crouching;
        }
        else
        {
            if (status == PlayerStatus.Crouching)
            {
                //print("Not crouching");
                status = PlayerStatus.Walking;
            }
        }

        // Determine Teleport Distances
        if (status == PlayerStatus.Running)
        {
            // The player should not be in running status while in teleportation
            status = PlayerStatus.Walking;
        }
        if (status == PlayerStatus.Walking)
        {
            GameObject.Find("Teleporting").GetComponent<Teleport>().arcDistance = 6.0f;
        }
        else if (status == PlayerStatus.Crouching)
        {

            GameObject.Find("Teleporting").GetComponent<Teleport>().arcDistance = 3.0f;
        }
        //Debug.Log("Stamina: " + stamina);

        //storePreviousLocation(VivePlayer.transform.position);
    }
}
