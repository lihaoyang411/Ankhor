using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ActionSetter : MonoBehaviour
{
    public SteamVR_ActionSet locomotionSet;
    public SteamVR_ActionSet teleportSet;
    bool bStarting = true;
    int counter = 0;
    public SteamVR_Action_Boolean menuActionDef = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ChangeMovementMode");
    public SteamVR_Action_Boolean menuActionLoco = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ChangeMovementMode");
    bool bSwitched, bPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        if(locomotionSet != null && teleportSet != null)
        {
            locomotionSet.Activate();
            Debug.Log("Teleport Active after start? " + teleportSet.IsActive());
            Debug.Log("Locomotion Active after start? " + locomotionSet.IsActive());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (bStarting)
        {
            //Debug.Log(bStarting);
            locomotionSet.Deactivate();
            GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = true;
            GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = true;
            GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = false;
            GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = false;

            bStarting = false;
        }
        // If you want to start with teleportation enabled rather than locomotion, Comment out the code from here...
        //if (counter < 1)
        //{
        //    counter++;
        //}
        //else if(counter == 1)
        //{
        //    teleportSet.Deactivate();
        //    locomotionSet.Activate();
        //    GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = false;
        //    GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = false;
        //    GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = true;
        //    GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = true;
        //    counter++;
        //}
        // ... to here. Thank steamVR for their EXCELLENT documentation that lead to this perfectly understandable code. Lol.

        if (menuActionDef.GetStateDown(SteamVR_Input_Sources.Any) || menuActionLoco.GetStateDown(SteamVR_Input_Sources.Any))
        {
            bPressed = true;
        }
        if (menuActionDef.GetStateUp(SteamVR_Input_Sources.Any) || menuActionLoco.GetStateUp(SteamVR_Input_Sources.Any))
        {
            bPressed = false;
            bSwitched = false;
        }


        if (bPressed && !bSwitched && locomotionSet != null && teleportSet != null)
        {
            bool active = teleportSet.IsActive();
            //active = !active;
            //Debug.Log("Teleport Active Before? " + teleportSet.IsActive());
            //Debug.Log("Locomotion Active Before? " + locomotionSet.IsActive());

            if (active)
            {
                setMovementLocomotion();
            }
            else
            {
                setMovementTeleport();
            }
            bSwitched = true;
            //Debug.Log("Teleport Active After? " + teleportSet.IsActive());
            //Debug.Log("Locomotion Active After? " + locomotionSet.IsActive());
        }
        
    }

    public void setMovementTeleport()
    {
        locomotionSet.Deactivate();
        teleportSet.Activate();
        GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = true;
        GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = true;
        GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = false;
        GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = false;
        this.GetComponent<Locomotion>().moveMode = Locomotion.PlayerMovementMode.Teleportation;
    }
    public void setMovementLocomotion()
    {
        teleportSet.Deactivate();
        locomotionSet.Activate();
        GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = false;
        GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[0].enabled = false;
        GameObject.Find("LeftHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = true;
        GameObject.Find("RightHand").GetComponents<SteamVR_Behaviour_Pose>()[1].enabled = true;
        this.GetComponent<Locomotion>().moveMode = Locomotion.PlayerMovementMode.Locomotion;
    }
}
