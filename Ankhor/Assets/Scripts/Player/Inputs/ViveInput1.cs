using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveInput1 : MonoBehaviour
{


    // check binding menu
    // this allows us to change it from the inspector 
    public SteamVR_Action_Single squeezeAction;


    public SteamVR_Action_Vector2 touchPadAction;

    private void Update()
    {
        // SteamVR_Input.action_set.inActions.the_action(Teleport)

        // SteamVR_Input_Sources.Any checks for any controller 

        if (SteamVR_Actions._default.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            print("Teleport Down");
        }

        if (SteamVR_Actions._default.Teleport.GetStateUp(SteamVR_Input_Sources.Any))
        {
            print("Grab Pinch Up");
        }

        /*

        // older steam vr version 
               
        if (SteamVR_Input.default.inActions.Teleport.getStateDown(SteamVR_Input_Sources.Any))
        {
            print("Teleport Down");
        }

        if (SteamVR_Input.default.inActions.Teleport.getStateUp(SteamVR_Input_Sources.Any))
        {
            print("Grab Pinch Up");
        }
        */

        // get the value from the squeeze action 
        // which is currenlty mapped to the trigger 

        float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);

        if(triggerValue > 0)
        {
            //print(triggerValue);

            Debug.Log("Trigger Value: " + triggerValue);
        }

        Vector2 touchPadValue = touchPadAction.GetAxis(SteamVR_Input_Sources.Any);

        if(touchPadValue != Vector2.zero)
        {
            //print(touchPadValue);

            Debug.Log("Touchpad Value: " + touchPadValue);
        }

    }


}
