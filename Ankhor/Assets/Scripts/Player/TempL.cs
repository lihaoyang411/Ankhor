using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// for player hand on March 5th 2019

public class TempL : MonoBehaviour
{

    public GameObject PlayerPrefab;

    // the render of this hand to set active

    public GameObject HumanHandRender;

    // the spirit render of this hand

    public GameObject SpiritHandRender;

    public GameObject EmptyHandRender;

    // temporary copy of this hand

    public GameObject TempHandL;

    // other hand

    public GameObject HandR;

    // get a reference of the htc vive render 

    // this is also where you parent things when 
    // you pick them up
    // add Hand Hold Position on inspector

    public GameObject HoldItemPos;

    // to keep track what is in the hand

    public GameObject ItemOnHand;

    public bool holdingObject = false;

    public int SqueezeCooltime = 2;

    private float SqueezeCooltimeTimer;

    private bool isPerformingSqueezeAction = false;

    // to reference the player's inventory
    private List<GameObject> PlayerInventory;

    private bool isSpirit = false;


    private void Awake()
    {
        PlayerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PlayerInventory;
    }

    private void Start()
    {
        holdingObject = false;

        SqueezeCooltimeTimer = SqueezeCooltime;

        isPerformingSqueezeAction = false;
        ItemOnHand = PlayerInventory[0];

        // reference the player inventory cause either hand can do stuff
    }

    private void Update()
    {
        // always check the player script and 
        // update according to if the player is a spirit

        isSpirit = PlayerPrefab.GetComponent<Player>().isSpirit;

        if (ItemOnHand == PlayerInventory[0])
        {
            holdingObject = false;
        }

        if (isPerformingSqueezeAction)
        {
            SqueezeCooltimeTimer -= Time.deltaTime;
        }

        if (SqueezeCooltimeTimer <= 0)
        {
            SqueezeCooltimeTimer = SqueezeCooltime;

            isPerformingSqueezeAction = false;
        }

        if (ItemOnHand != null)
        {
            if (ItemOnHand.tag == "Flashlight")
            {
                Debug.Log("Flashlight in hand");
                if (PressedTriggerValis1())
                {
                    var flashlight_script = ItemOnHand.GetComponent<Flashlight>();

                    flashlight_script.ActivateFlashlight();

                    Debug.Log("Flashlight turned on/off");
                }
            }
        }
    }

    // check binding menu
    // this allows us to change it from the inspector 

    public SteamVR_Action_Single squeezeActionDef;
    public SteamVR_Action_Single squeezeActionLoco;

    private void OnTriggerEnter(Collider other)
    {

        // Debug.Log("On a trigger");

        // now check to see 
        // what type of object it is 

        if (other.tag == "Handle")
        {

            Debug.Log("Entered the Handle box trigger");

            // if you are on a "Handle" 
            // and you are not holding an object

            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnEnterHandleAction(other);
            }

            else
            {
                //Debug.Log("Hand needs to be free!");
            }
         }

        if (other.tag == "OpenableHandle")
        {
            Debug.Log("Entered the Handle box trigger");

            // if you are on a "Handle" 
            // and you are not holding an object

            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnEnterOpenableHandleAction(other);
            }

            else
            {
                //Debug.Log("Hand needs to be free!");
            }
        }
          if (other.tag == "Flashlight")
        {

            Debug.Log("Entered the Flashlight box trigger"); 
            if (!holdingObject)
            {
                // allow the player to pick it up

                // PickUpFlashlight(other);


            }

            else if (holdingObject && (ItemOnHand != null))
            {
                if (ItemOnHand.tag == "Flashlight")
                {
                    // you are holding the flashlight you are colliding with 

                    // if they hit the trigger 
                    // activate it

                    if (PressedTriggerValis1())
                    {
                        var flashlight_script = other.GetComponent<Flashlight>();

                        flashlight_script.ActivateFlashlight();

                        Debug.Log("Flashlight turned on/off");
                    }
                }
            }

            else
            {
                //Debug.Log("Hand needs to be free!");
            }
        }       }

    private void OnTriggerStay(Collider other)
    {
        // Debug.Log("On trigger stay");

        // now check to see 
        // what type of object it is 

        if (other.tag == "Handle")
        {
            Debug.Log("On the Handle box trigger");

            // if you are on a "Handle" 
            // and you are not holding an object

            // 

            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnStayHandleAction(other);
            }

            else
            {
                Debug.Log("Hand needs to be free!");
            }

        }

        if (other.tag == "OpenableHandle")
        {
            Debug.Log("On the Handle box trigger");

            // if you are on a "Handle" 
            // and you are not holding an object

            // 

            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnStayOpenableHandleAction(other);
            }

            else
            {
                Debug.Log("Hand needs to be free!");
            }

        }

        if (other.tag == "Flashlight")
        {

            //Debug.Log("On the Flashlight box trigger");

            if (!holdingObject)
            {
                OnStayPickupAction(other);
            }


            else
            {
                // Debug.Log("Hand needs to be free!");
            }
        }




        if (other.tag == "physicalKey")
        {

            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnStayPickupAction(other);
            }

            else
            {
                //Debug.Log("Hand needs to be free!");
            }

        }

        if (other.tag == "netherKey")
        {
            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnStayPickupAction(other);
            }

            else
            {
                //Debug.Log("Hand needs to be free!");
            }
        }

        if (other.tag == "Pickup")
        {
            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnStayPickupAction(other);
            }

            else
            {
                //Debug.Log("Hand needs to be free!");
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("On trigger exit");

        // now check to see 
        // what type of object it is 

        if (other.tag == "Handle")
        {
            Debug.Log("Left the Handle box trigger");

            var handle_script = other.GetComponent<Handle>();

            // if you just left the 
            // trigger for the handle
            // hide the hand on it

            // show the hand on the handle
            if (isSpirit)
            {


                handle_script.HideSpiritLeftHand();

                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(SpiritHandRender);
            }
            else
            {

                handle_script.HideHumanLeftHand();

                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }

            // show your hand again

            // HandRender.SetActive(true);


        }

        if (other.tag == "OpenableHandle")
        {
            Debug.Log("Left the Handle box trigger");

            var handle_script = other.GetComponent<OpenableHandle>();

            // if you just left the 
            // trigger for the handle
            // hide the hand on it

            // show the hand on the handle
            if (isSpirit)
            {
                handle_script.HideSpiritLeftHand();

                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(SpiritHandRender);
            }
            else
            {
                Debug.LogWarning("is trying to hide on trigger exit");

                handle_script.HideHumanLeftHand();

                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);

            }

            // show your hand again

            // HandRender.SetActive(true);


        }


        if (other.tag == "Flashlight")
        {

            Debug.Log("Left the Flashlight box trigger");

            // if you left the 
            // trigger for the flashlight

            other.GetComponent<Flashlight>().HideLeftHand();

            // HandRender.SetActive(true);

            // show your hand now
            if (isSpirit)
            {
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(SpiritHandRender);

                // also 
                ItemOnHand = null;
            }
            else
            {
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);

                // also 
                ItemOnHand = null;
            }

        }

        /*
        if (other.tag == "Key") {

        }
        */

    }


    private bool PressedTrigger()
    {
        // get the value from the squeeze action
        // which is currenlty mapped to the trigger 
        float triggerValueDef = squeezeActionDef.GetAxis(SteamVR_Input_Sources.Any);
        float triggerValueLoco = squeezeActionLoco.GetAxis(SteamVR_Input_Sources.Any);

        if (triggerValueDef > 0 || triggerValueLoco > 0)
        {

            Debug.Log("Player pulled the trigger");

            //Debug.Log("Trigger Value: " + triggerValue);

            return true;
        }

        return false;

    }


    private bool PressedTriggerValis1()
    {
        // get the value from the squeeze action
        // which is currenlty mapped to the trigger 
        float triggerValueDef = squeezeActionDef.GetAxis(SteamVR_Input_Sources.Any);
        float triggerValueLoco = squeezeActionLoco.GetAxis(SteamVR_Input_Sources.Any);

        if (!isPerformingSqueezeAction)
        {
            if (triggerValueDef == 1 || triggerValueLoco == 1)
            {

                Debug.Log("Player pulled the trigger");

                //Debug.Log("Trigger Value: " + triggerValue);

                isPerformingSqueezeAction = true;

                return true;
            }
        }
        return false;
    }


    /// 
    /// 
    /// 
    /// Below are all actions the player
    /// can do when they are on the
    /// object's box collider
    /// 
    /// 
    /// 



    // for when player is on handle box collider

    private void OnEnterHandleAction(Collider otherHandle)
    {
        Debug.Log("Player is in handle collider");
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherHandle.GetComponent<Handle>();

        if (handle_script == null)
        {
            Debug.Log("Door handle does not have handle script");
            return;
        }

        // "hide your hand"

        // HandRender.SetActive(false);\

        GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(EmptyHandRender);

        // show the hand on the handle
        if (isSpirit)
        {
            handle_script.ShowSpiritLeftHand();
        }
        else
        {
            handle_script.ShowHumanLeftHand();
        }


        // if they press the trigger and push down attempt to open the door

        if (PressedTrigger())
        {
            Debug.Log("User pressed trigger");

            handle_script.Open_Close_Door(isSpirit);

            /*

            // get the door script

            var door_script = handle_script.Door_w_Script.GetComponent<Door>();

            // if the door was not open

            if (!door_script.isOpen)
            {
                // try to open it

                bool openSuccessful = door_script.AttemptToOpen(isSpirit);

                if (openSuccessful)
                {
                    door_script.Open();

                }
                else
                {
                    // play a little animation to show that 
                    // the door cannot open

                    door_script.OpenFailed();
                }
            }

            else
            {
                // door was open so close it

                bool closeSuccessful = door_script.AttemptToClose(isSpirit);

                if (closeSuccessful)
                {
                    door_script.Close();
                }
                else
                {
                    Debug.Log("Cannot close door!");
                }
            }
            */
        }
    }

    private void OnEnterOpenableHandleAction(Collider otherOpenableHandle)
    {
        Debug.Log("Player is in handle collider");
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherOpenableHandle.GetComponent<OpenableHandle>();

        if (handle_script == null)
        {
            Debug.Log("Door handle does not have handle script");
            return;
        }

        // "hide your hand"

        // HandRender.SetActive(false);

        GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(EmptyHandRender);

        // show the hand on the handle
        if (isSpirit)
        {
            handle_script.ShowSpiritLeftHand();
        }
        else
        {
            handle_script.ShowHumanLeftHand();
        }

        // if they press the trigger and push down attempt to open the door

        if (PressedTrigger())
        {
            Debug.Log("User pressed trigger");

            handle_script.Open_Close_Door(isSpirit);

        }
    }


    private void OnStayHandleAction(Collider otherHandle)
    {
        Debug.Log("Player is stay trigger enter for Handle");
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherHandle.GetComponent<Handle>();

        if (handle_script == null)
        {
            Debug.Log("Door handle does not have handle script");
            return;
        }

        // don't "hide your hand" cause on trigger
        // enter already did that 

        // if they press the trigger and push down attempt to open the door

        if (PressedTrigger())
        {
            Debug.Log("User pressed trigger");

            handle_script.Open_Close_Door(isSpirit);

            /*
            // get the door script

            var door_script = handle_script.Door_w_Script.GetComponent<Door>();

            // if the door was not open

            if (!door_script.isOpen)
            {
                // try to open it

                bool openSuccessful = door_script.AttemptToOpen(isSpirit);

                if (openSuccessful)
                {
                    door_script.Open();

                    // let the player see the hand open the door for like 1 sec

                }
                else
                {
                    // play a little animation to show that 
                    // the door cannot open

                    door_script.OpenFailed();
                }
            }

            else
            {
                // door was open so close it

                bool closeSuccessful = door_script.AttemptToClose(isSpirit);

                if (closeSuccessful)
                {
                    door_script.Close();
                }
                else
                {
                    Debug.Log("Cannot close door!");
                }
            }
            */
        }
    }

    private void OnStayOpenableHandleAction(Collider otherOpenableHandle)
    {
        Debug.Log("Player is stay trigger enter for Handle");
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherOpenableHandle.GetComponent<OpenableHandle>();

        if (handle_script == null)
        {
            Debug.Log("Door handle does not have handle script");
            return;
        }

        // don't "hide your hand" cause on trigger
        // enter already did that 

        // if they press the trigger and push down attempt to open the door

        if (PressedTrigger())
        {
            Debug.Log("User pressed trigger");

            handle_script.Open_Close_Door(isSpirit);
        }
    }


    // general collectibles pick up
    // such as keys and stuff

    private void OnStayPickupAction(Collider other)
    {
        // if they pressed the trigger 
        // add it to your inventory 

        // need to make sure they are not already holding something
        // in their hands 

        if (PressedTriggerValis1())
        {
            Debug.Log("User pressed trigger");

            GameObject Pickup = other.gameObject;

            // add it to your inventory 
            PlayerInventory.Add(Pickup);

            // take the object 
            // and attach it to the hand

            // hide the stationary hand for pick up 

            // other.gameObject.HideLeftHand();

            // now show your hand again

            // HandRender.SetActive(true);

            // turn off gravity for the Pickup

            Pickup.GetComponent<Rigidbody>().useGravity = false;

            Pickup.GetComponent<Rigidbody>().isKinematic = true;

            Pickup.transform.position = HoldItemPos.transform.position;

            Pickup.transform.rotation = HoldItemPos.transform.rotation;

            //Vector3 lookVector = HoldItemPos.transform.position - otherflashlight.transform.position;

            //lookVector.y = transform.position.y;

            //Quaternion rot = Quaternion.LookRotation(-lookVector);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

            // get the transform of the Object
            // set it as the child of ItemOnHand position
            Pickup.transform.SetParent(HoldItemPos.transform);

            Debug.Log("Pickup is now the child of the ItemHolded gameobject");

            // move the hand a little so that it looks like
            // it is holding the object correctly

            ItemOnHand = Pickup;

            // got the object 
            holdingObject = true;

            other.enabled = false;

        }



    }


    // Flashlight Pick Up

    private void PickUpFlashlight(Collider otherflashlight)
    {
        Debug.Log("User is on the trigger for the flashlight");
        // latch the hand to the flashlight

        var flashlight_script = otherflashlight.GetComponent<Flashlight>();

        if (flashlight_script == null)
        {
            Debug.Log("Flashlight does not have handle script");
            return;
        }

        PlayerInventory.Add(otherflashlight.gameObject);

        // "hide your hand"

        // HandRender.SetActive(false);

        GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(SpiritHandRender);

        // show the hand on the flashlight

        flashlight_script.ShowLeftHand(holdingObject);

        // if they press the trigger 
        // pick it up

        if (PressedTrigger())
        {
            Debug.Log("User pressed trigger");
            // take the flashlight 
            // and attach it to the hand

            // hide the stationary hand for pick up 

            flashlight_script.HideLeftHand();

            // now show your hand again

            // HandRender.SetActive(true);

            if (isSpirit)
            {
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(SpiritHandRender);
            }
            else
            {
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }

            // turn off gravity for the flashlight
            otherflashlight.GetComponent<Rigidbody>().useGravity = false;

            /*
             
            Debug.Log("Position of ItemOnHand " + otherflashlight.transform.position);

            Debug.Log("Position of Flashlight " + otherflashlight.transform.position);

            // move it to where the hand holds it
            ItemOnHand.transform.position = otherflashlight.gameObject.transform.position;

            Debug.Log("Position of Flashlight Before Parenting" + otherflashlight.transform.position);
            
            ItemOnHand.transform.parent = otherflashlight.gameObject.transform;

            Debug.Log("Position of Flashlight After Parenting" + otherflashlight.transform.position);

            Debug.Log("Flashlight is now the child of the item holded gameobject");

            */

            otherflashlight.GetComponent<Rigidbody>().isKinematic = true;

            otherflashlight.transform.position = HoldItemPos.transform.position;

            otherflashlight.transform.rotation = HoldItemPos.transform.rotation;

            //Vector3 lookVector = HoldItemPos.transform.position - otherflashlight.transform.position;

            //lookVector.y = transform.position.y;

            //Quaternion rot = Quaternion.LookRotation(-lookVector);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);

            // get the transform of the flashlight
            // set it as the child of ItemOnHand position
            otherflashlight.transform.SetParent(HoldItemPos.transform);

            Debug.Log("Flashlight is now the child of the ItemHolded gameobject");

            // move the hand a little so that it looks like
            // it is holding the flashlight correctly

            ItemOnHand = otherflashlight.gameObject;

            // got the flashlight 
            holdingObject = true;

        }
    }
 
}
