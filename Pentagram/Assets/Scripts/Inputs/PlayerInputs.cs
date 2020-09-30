using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public class ViveControlls
    {
        public int bullets;
        public int grenades;
        public int rockets;

        public Stuff(int bul, int gre, int roc)
        {
            bullets = bul;
            grenades = gre;
            rockets = roc;
        }
    }


    public Stuff myStuff = new Stuff(10, 7, 25);
    public float speed;
    public float turnSpeed;
    public Rigidbody bulletPrefab;
    public Transform firePosition;
    public float bulletSpeed;


    void Update()
    {
        Movement();
        Shoot();
    }


    private void OnTriggerEnter(Collider other)
    {

        // if we are in range of an interactible object
        // highlight it

        var outline_script = other.GetComponent<Outline>();

        if (outline_script != null)
        {
            outline_script.OutlineMode = Outline.Mode.OutlineHidden;
            outline_script.OutlineMode = Outline.Mode.OutlineAll;
        }

        // now check to see what type of object 
        // it is 

        if (other.tag == "Handle")
        {
            // place your hand on the door handle
            // move your "hands"
            // to the door handle
            // this hand render the script is attached to

            hand_render.transform.position = other.GetComponent<Door>().handPosition;

            // if they press the trigger and push down play the door animation 

            if (pressedTrigger)
            {
                // get the door script
                // we are playing it's animation 

                var door_script = other.GetComponentInChildren<Door>();

                door_script.open();

                // set the 
            }




        }

        if (other.tag == "Flashlight")
        {

        }

        if (other.tag == "Key")
        {

        }


    }
}
