using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeArtifact : MonoBehaviour
{

    [Tooltip("The place where the projectile will instantiate from")]
    public Transform ProjectileSpawnPoint;

    [Tooltip("The game object that will be shot from the artifact")]
    public GameObject FreezeProjectile;

    [Tooltip("Needs to be taken to spirit before being able to activate it")]
    public bool isSealed = true;

    [Tooltip("The cooldown before they can send another freeze projectile")]
    public float CooldownTime = 5.0f;

    private float currentCoolTimeRemaning;

    private bool onCooldown = false;

    public bool test;

    private void Start()
    {
        currentCoolTimeRemaning = CooldownTime;

        isSealed = true;
    }

    private void Update()
    {
        if (test)
        {
            FireProjectile();
            test = false;
        }

        if (onCooldown)
        {
            Cooldown();
        }
    }

    public void FireProjectile()
    {
        if (!isSealed)
        {
            if (!onCooldown)
            {
                // fire a projectile

                onCooldown = true;

                // spawn it 
                GameObject SpawnedFreezeProjectile = Instantiate(FreezeProjectile, ProjectileSpawnPoint);

                SpawnedFreezeProjectile.transform.localPosition = new Vector3(0, 0, 0);

                SpawnedFreezeProjectile.transform.parent = null;

                // SpawnedFreezeProjectile.transform.localRotation = new Quaternion(0, 0, 0, 1);

                //SpawnedFreezeProjectile.AddComponent<Rigidbody>();

                //SpawnedFreezeProjectile.AddComponent<FreezeProjectile>();

                //SpawnedFreezeProjectile.AddComponent<>

                // get the vector where the freeze artifact is pointing at

                // apply some force to it 
                // vector is -transform.right because 
                // we are pushing in the opposite direction of where the X 
                // direction is at for the artifact transform
                SpawnedFreezeProjectile.GetComponent<Rigidbody>().AddForce(-transform.right * 300);
            }
        }
    }

    private void Cooldown()
    {
        if (currentCoolTimeRemaning <= 0)
        {
            onCooldown = false;

            currentCoolTimeRemaning = CooldownTime;
        }
        else
        {
            currentCoolTimeRemaning -= Time.deltaTime;
        }
    }
}
