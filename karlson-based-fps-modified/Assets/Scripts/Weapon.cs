using UnityEngine.Events;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public UnityEvent OnGunShoot;
    public float FireCooldown;

    public bool Automatic;

    private float CurrentCooldown;

    void Start()
    {
        CurrentCooldown = FireCooldown;
    }


    void Update()
    {
        if (Automatic)
        {
            if (Input.GetMouseButton(0))
            {
                if (CurrentCooldown <= 0f)
                {
                    OnGunShoot?.Invoke();
                    CurrentCooldown = FireCooldown;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CurrentCooldown <= 0f)
                {
                    OnGunShoot?.Invoke();
                    CurrentCooldown = FireCooldown;
                }
            }
        }
        CurrentCooldown -= Time.deltaTime;
    }
}
