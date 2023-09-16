using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] public new string name;

    [Header("Shooting")]
    [SerializeField] public float damage;
    [SerializeField] public float maxDistance;

    [Header("Reloading")]
    [SerializeField] public int currentAmmo;
    [SerializeField] public int magSize;
    [SerializeField] public float fireRate;
    [SerializeField] public float reloadTime;
    [HideInInspector] public bool reloading;
}
