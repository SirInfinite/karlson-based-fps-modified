using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GunData gunData;
    [SerializeField] Transform muzzle;

    float timeSinceLastShot;
    public LayerMask interactableLayers;

    private void Start()
    {
        PlayerShoot.shootInput += Shoot;
    }

    // checks if can shoot if we arent reloading, and if its been more than the cooldown (1s/rps)
    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    public void Shoot()
    {
        if (gunData.currentAmmo > 0) 
        {
            if (CanShoot())
            {
                if (Physics.Raycast(muzzle.position, transform.forward, out RaycastHit hitInfo, gunData.maxDistance, interactableLayers))
                {
                    Debug.Log(hitInfo.transform.name);
                    Debug.DrawLine(muzzle.position, hitInfo.point, Color.red, 0.5f);

                    gunData.currentAmmo--;
                    timeSinceLastShot = 0;
                    OnGunShot();
                }
            }
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(muzzle.position, transform.forward, Color.green);
        Debug.DrawLine(muzzle.position, muzzle.position + muzzle.forward * 2f, Color.blue);
    }

    private void OnGunShot()
    {
        
    }
}
