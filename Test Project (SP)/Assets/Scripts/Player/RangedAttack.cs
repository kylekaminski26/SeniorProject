using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileForce = 10f;
    public Animator animator;
    public float fireRate = 0.375f;

    private float nextFire = 0.0f;
    private string directionOfShot = "Down";


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire2") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            animator.SetTrigger("Shooting");

            if (PlayerMovement.IsFacingUp)
                directionOfShot = "Up";
            else if (PlayerMovement.IsFacingDown)
                directionOfShot = "Down";
            else if (PlayerMovement.IsFacingLeft)
                directionOfShot = "Left";
            else if (PlayerMovement.IsFacingRight)
                directionOfShot = "Right";
            Invoke("Shoot", 0.375f);
        }
    }

    void Shoot()
    {
        Quaternion projectileRotation = Quaternion.Euler(0, 0, 180);

        switch(directionOfShot)
        {
            case "Up":
                projectileRotation = Quaternion.Euler(0, 0, 0);
                break;
            case "Down":
                projectileRotation = Quaternion.Euler(0, 0, 180);
                break;
            case "Left":
                projectileRotation = Quaternion.Euler(0, 0, 90);
                break;
            case "Right":
                projectileRotation = Quaternion.Euler(0, 0, -90);
                break;
            default:
                projectileRotation = Quaternion.Euler(0, 0, 180);
                break;
        }
        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectileRotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        switch (directionOfShot)
        {
            case "Up":
                rb.AddForce(firePoint.up * projectileForce, ForceMode2D.Impulse);
                break;
            case "Down":
                rb.AddForce(-firePoint.up * projectileForce, ForceMode2D.Impulse);
                break;
            case "Left":
                rb.AddForce(-firePoint.right * projectileForce, ForceMode2D.Impulse);
                break;
            case "Right":
                rb.AddForce(firePoint.right * projectileForce, ForceMode2D.Impulse);
                break;
            default:
                rb.AddForce(-firePoint.up * projectileForce, ForceMode2D.Impulse);
                break;
        }

        Destroy(projectile, 1f);
    }
}
