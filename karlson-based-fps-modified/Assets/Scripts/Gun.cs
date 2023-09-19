using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform muzzle;

	[Header("Bullet Spread")]
	[SerializeField] private bool addBulletSpread = true;
	[SerializeField] private Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
	[SerializeField] private ParticleSystem shootingSystem;
	[SerializeField] private ParticleSystem impactSystem;
	[SerializeField] private TrailRenderer bulletTrail; 
    [SerializeField] private LayerMask mask;

    private Animator Animator;
	private float timeSinceLastShot;

	private void Awake()
	{
		Animator = GetComponent<Animator>();
	}

	private void Start()
    {
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += StartReload;
    }

    private void OnDisable() => gunData.reloading = false;

	public void StartReload()
    {
        if (!gunData.reloading && this.gameObject.activeSelf)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        gunData.reloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;

        gunData.reloading = false;
    }

    // cooldown (gunData.fireRate / 60f) = 1s/rps
    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    public void Shoot()
    {
        if (gunData.currentAmmo > 0) 
        {
            if (CanShoot())
            {
                Animator.SetBool("IsShooting", true);
                shootingSystem.Play();
                Vector3 direction = GetDirection();
				gunData.currentAmmo--;
				timeSinceLastShot = 0;
				OnGunShot();

				if (Physics.Raycast(cam.position, direction, out RaycastHit hitInfo, gunData.maxDistance, mask))
				{
					TrailRenderer trail = Instantiate(bulletTrail, cam.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hitInfo));
                    
                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
					damageable?.TakeDamage(gunData.damage);
				}
			}
        }
    }

	private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (addBulletSpread)
        {
            direction += new Vector3(
				Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x),
				Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y),
				Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z)
            );

            direction.Normalize();
		}

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hitInfo)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitInfo.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
		Animator.SetBool("IsShooting", true);
        trail.transform.position = hitInfo.point;
        Instantiate(impactSystem, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

        Destroy(trail.gameObject, trail.time);
	}

	private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(cam.position, cam.forward * gunData.maxDistance, Color.green);
    }

    private void OnGunShot() { }
}
