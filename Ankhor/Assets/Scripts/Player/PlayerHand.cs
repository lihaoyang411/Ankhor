using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerHand : MonoBehaviour
{
    //Figure out which hand this script is for
    public bool isLeftHand;

    /// the render of the hands to set active

    public GameObject HumanHandRender;

    public GameObject EmptyHandRender;

    // this is where you parent things when 
    // you pick them up
    // add Hand Hold Position on inspector

    public GameObject HoldItemPos;

    // to keep track what is in the hand

    public GameObject ItemOnHand;

    public bool holdingObject = false;

    // used to keep track the force of the hand 
    private Vector3 lastFrame;
    public Vector3 handForce;

    // to reference the player's inventory
    private List<GameObject> PlayerInventory;

    bool bFlashlightSwitched = false;

    private bool isPickedupDelay;

    private float DelayForThrow = 1.0f;

    private float currDelayForThrow;

    // check binding menu
    // this allows us to change it from the inspector 

    public SteamVR_Action_Single squeezeActionDef;
    public SteamVR_Action_Single squeezeActionLoco;


    private void Awake()
    {
        PlayerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PlayerInventory;
    }

    private void Start()
    {
        holdingObject = false;
        ItemOnHand = PlayerInventory[0];
        // reference the player inventory cause either hand can do stuff

        lastFrame = transform.position;

        currDelayForThrow = DelayForThrow;
    }

    private void Update()
    {
        if (isPickedupDelay)
        {
            currDelayForThrow -= Time.deltaTime;
        }

        if (currDelayForThrow <= 0)
        {
            isPickedupDelay = false;

            currDelayForThrow = DelayForThrow;
        }

        // always check the player script and 
        // update according to if the player is a spirit

        if (ItemOnHand != null)
        {
            if (ItemOnHand.tag == "Flashlight")
            {
                Debug.Log("Flashlight in hand");
                if (PressedTriggerValis1() && !bFlashlightSwitched)
                {
                    var flashlight_script = ItemOnHand.GetComponent<Flashlight>();

                    flashlight_script.ActivateFlashlight();
                    bFlashlightSwitched = true;

                    Debug.Log("Flashlight turned on/off");
                }
            }

            if (ItemOnHand.tag == "ElectricityArtifact")
            {
                Debug.Log("Electricity Artifact in hand");
                if (PressedTriggerValis1() && !bFlashlightSwitched)
                {
                    var artifact_script = ItemOnHand.GetComponent<ElectricArtifact>();

                    artifact_script.Activate();
                    bFlashlightSwitched = true;

                    Debug.Log("Electricity Artifact Activated");
                }
            }

            if (ItemOnHand.tag == "DistractArtifact")
            {
                Debug.Log("Distract Artifact in hand");
                if (PressedTriggerValis1() && !bFlashlightSwitched && (currDelayForThrow >= DelayForThrow))
                {
                    var artifact_script = ItemOnHand.GetComponent<DistractArtifact>();

                    artifact_script.Activate(gameObject);
                    bFlashlightSwitched = true;

                    Debug.Log("Distract Artifact Activated");
                }
            }

            if (ItemOnHand.tag == "DistractItem")
            {
                Debug.Log("Distract Item in hand");
                if (PressedTriggerValis1() && !bFlashlightSwitched && (currDelayForThrow >= DelayForThrow))
                {
                    var artifact_script = ItemOnHand.GetComponent<DistractItem>();

                    artifact_script.Throw(gameObject);
                    bFlashlightSwitched = true;

                    Debug.Log("Distract Item was Thrown!");
                   
                }
            }

            if (ItemOnHand.tag == "FreezeArtifact")
            {
                Debug.Log("Freeze Artifact in hand");
                if (PressedTriggerValis1() && !bFlashlightSwitched)
                {
                    var artifact_script = ItemOnHand.GetComponent<FreezeArtifact>();

                    artifact_script.FireProjectile();
                    bFlashlightSwitched = true;

                    Debug.Log("Freeze Artifact fired a projectile!");
                }
            }
        }

        handForce *= 0.5f;
        // handForce *= 0.8f;
        handForce += transform.position - lastFrame;
        lastFrame = transform.position;
    }

    public void returnDistractArtifact()
    {
        if (isLeftHand)
        {
            GameObject LHoldItemPos = GameObject.Find("LeftHand").GetComponent<PlayerHand>().HoldItemPos;

            GameObject Artifact = GameObject.FindGameObjectWithTag("DistractArtifact");

            Artifact.GetComponent<Rigidbody>().useGravity = false;

            Artifact.GetComponent<Rigidbody>().isKinematic = true;

            Artifact.transform.position = LHoldItemPos.transform.position;

            Artifact.transform.rotation = LHoldItemPos.transform.rotation;

            // get the transform of the Object
            // set it as the child of ItemOnHand position
            Artifact.transform.SetParent(LHoldItemPos.transform);

            // move the hand a little so that it looks like
            // it is holding the object correctly

            ItemOnHand = Artifact;

            // got the object 
            holdingObject = true;
        }

        else
        {
            GameObject Artifact = GameObject.FindGameObjectWithTag("DistractArtifact");

            Artifact.GetComponent<Rigidbody>().useGravity = false;

            Artifact.GetComponent<Rigidbody>().isKinematic = true;

            Artifact.transform.position = HoldItemPos.transform.position;

            Artifact.transform.rotation = HoldItemPos.transform.rotation;

            // get the transform of the Object
            // set it as the child of ItemOnHand position
            Artifact.transform.SetParent(HoldItemPos.transform);

            // move the hand a little so that it looks like
            // it is holding the object correctly

            ItemOnHand = Artifact;

            // got the object 
            holdingObject = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // now check to see 
        // what type of object it is 

        if (other.tag == "Handle")
        {
            // if you are on a "Handle" 
            // and you are not holding an object

            if (!holdingObject)
            {
                // you can do the onHandleAction
                OnEnterHandleAction(other);
            }

        }

        if (other.tag == "OpenableHandle")
        {
            // if you are on a "Handle" 
            // and you are not holding an object

            if (!holdingObject)
            {
                // you can do the onHandleAction

                OnEnterOpenableHandleAction(other);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Handle")
        {
            // if you are on a "Handle" 
            // and you are not holding an object

            if (!holdingObject)
            {
                // you can do the onHandleAction
                OnStayHandleAction(other);
            }
        }

        if (other.tag == "OpenableHandle")
        {
            // if you are on a "Handle" 
            // and you are not holding an object
            if (!holdingObject)
            {
                // you can do the onHandleAction
                OnStayOpenableHandleAction(other);
            }
        }

        if (other.tag == "ControlPanel")
        {
            if (ItemOnHand.tag == "KeyCard")
            {
                if (PressedTriggerValis1() && !bFlashlightSwitched)
                {
                    var control_panel_script = other.GetComponent<ControlPanel>();

                    control_panel_script.Activate(gameObject);
                    bFlashlightSwitched = true;

                    Debug.Log("Control Panel Activated");
                }
            }
        }

        if (other.tag == "KeyItemHolder")
        {
            if (PressedTriggerValis1() && !bFlashlightSwitched)
            {
                var holder_script = other.GetComponent<KeyItemHolder>();

                holder_script.TakeObject();
                bFlashlightSwitched = true;

                Debug.Log("Took Key Item");
            }
        }

        if (
        other.tag == "Flashlight" || 
        other.tag == "physicalKey" || 
        other.tag == "netherKey" || 
        other.tag == "TutorialItem" ||
        other.tag == "Pickup" ||
        other.tag == "ElectricityArtifact" ||
        other.tag == "DistractArtifact" ||
        other.tag == "FreezeArtifact" ||
        other.tag == "DistractItem" ||
        other.tag == "KeyItemHolder" ||
        other.tag == "KeyCard")
        {
            if (!holdingObject)
            {
                OnStayPickupAction(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Handle")
        {
            var handle_script = other.GetComponent<Handle>();

            // if you just left the 
            // trigger for the handle
            // hide the hand on it

            // show the hand on the handle
            if (isLeftHand)
            {
                handle_script.HideHumanLeftHand();
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }
            else
            {
                handle_script.HideRightHand();
                GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }
        }

        if (other.tag == "OpenableHandle")
        {
            var handle_script = other.GetComponent<OpenableHandle>();

            // if you just left the 
            // trigger for the handle
            // hide the hand on it

            // show the hand on the handle

            if (isLeftHand)
            {
                handle_script.HideHumanLeftHand();
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }
            else
            {
                handle_script.HideHumanRightHand();
                GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }
        }


        if (other.tag == "Flashlight")
        {
            // if you left the 
            // trigger for the flashlight
            if (isLeftHand)
            {
                other.GetComponent<Flashlight>().HideLeftHand();
                GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }
            else
            {
                other.GetComponent<Flashlight>().HideRightHand();
                GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(HumanHandRender);
            }
        }
    }


    private bool PressedTrigger()
    {
        // get the value from the squeeze action
        // which is currenlty mapped to the trigger 
        float triggerValueDef = squeezeActionDef.GetAxis(SteamVR_Input_Sources.Any);
        float triggerValueLoco = squeezeActionLoco.GetAxis(SteamVR_Input_Sources.Any);

        if (triggerValueDef > 0 || triggerValueLoco > 0)
        {
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

        if (triggerValueDef == 1.0f || triggerValueLoco == 1.0f)
        {
            return true;
        }
        if(bFlashlightSwitched)
            bFlashlightSwitched = false;
        return false;
    }

    private void OnEnterHandleAction(Collider otherHandle)
    {
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherHandle.GetComponent<Handle>();

        if (handle_script == null)
        {
            return;
        }

        if (isLeftHand)
        {
            GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(EmptyHandRender);
            // show the hand on the handle
            handle_script.ShowHumanLeftHand();
        }
        else
        {
            GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(EmptyHandRender);
            // show the hand on the handle
            handle_script.ShowHumanRightHand();
        }


        // if they press the trigger and push down attempt to open the door

        if (PressedTriggerValis1())
        {
            handle_script.Open_Close_Door(false);
        }
    }

    private void OnEnterOpenableHandleAction(Collider otherOpenableHandle)
    {
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherOpenableHandle.GetComponent<OpenableHandle>();

        if (handle_script == null)
        {
            Debug.Log("Door handle does not have handle script");
            return;
        }

        if (isLeftHand)
        {
            GameObject.Find("LeftHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(EmptyHandRender);
            // show the hand on the handle
            handle_script.ShowHumanLeftHand();
        }
        else
        {
            GameObject.Find("RightHand").GetComponent<Valve.VR.InteractionSystem.Hand>().SetRenderModel(EmptyHandRender);
            // show the hand on the handle
            handle_script.ShowHumanRightHand();
        }

        // if they press the trigger and push down attempt to open the door

        if (PressedTriggerValis1())
        {
            handle_script.Open_Close_Door(false);
        }
    }

    private void OnStayHandleAction(Collider otherHandle)
    {
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherHandle.GetComponent<Handle>();

        if (handle_script == null)
        {
            return;
        }

        // don't "hide your hand" cause on trigger
        // enter already did that 

        // if they press the trigger and push down attempt to open the door

        if (PressedTriggerValis1())
        {
            handle_script.Open_Close_Door(false);
        }
    }

    private void OnStayOpenableHandleAction(Collider otherOpenableHandle)
    {
        // place your hand on the door handle
        // move your "hands"
        // to the door handle (like anchor it)

        var handle_script = otherOpenableHandle.GetComponent<OpenableHandle>();

        if (handle_script == null)
        {
            return;
        }

        // don't "hide your hand" cause on trigger
        // enter already did that 

        // if they press the trigger and push down attempt to open the door

        if (PressedTriggerValis1())
        {
            handle_script.Open_Close_Door(false);
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

        GameObject Pickup = other.gameObject;

        if (PressedTriggerValis1() && !PlayerInventory.Contains(Pickup))
        {// add it to your inventory
            PlayerInventory.Add(Pickup);

            // turn off gravity for the Pickup

            Pickup.GetComponent<Rigidbody>().useGravity = false;

            Pickup.GetComponent<Rigidbody>().isKinematic = true;

            Pickup.transform.position = HoldItemPos.transform.position;

            Pickup.transform.rotation = HoldItemPos.transform.rotation;

            // get the transform of the Object
            // set it as the child of ItemOnHand position
            Pickup.transform.SetParent(HoldItemPos.transform);

            // move the hand a little so that it looks like
            // it is holding the object correctly

            ItemOnHand = Pickup;

            // got the object 
            holdingObject = true;

            if (other.tag == "Flashlight")
                other.enabled = false;

            AkSoundEngine.PostEvent("ItemGrab", gameObject);

            isPickedupDelay = true;
        }
    }
}
