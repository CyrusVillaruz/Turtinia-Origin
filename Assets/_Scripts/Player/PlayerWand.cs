using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWand : MonoBehaviour
{
    private PlayerController player;
    private SpriteRenderer sr;

    private float timeBtwShots;
    private float projectileManaCost;

    public GameObject projectile;
    public Transform shotPoint;
    //public PlayerInput playerInput;

    public float offset;
    public float startTimeBtwShots;

    public bool isFiring = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<PlayerController>();
        //playerInput = GetComponent<PlayerInput>();
        projectileManaCost = projectile.GetComponent<PlayerMagicMissile>().manaCost;
    }

    public void OnFire()
    {
        isFiring = true;
    }

    private void Update()
    {
        //if (player.isInvincible)
        //{
        //    playerInput.enabled = false;
        //    sr.enabled = false;
        //}
        //else
        //{
        //    playerInput.enabled = true;
        //    sr.enabled = true;
        //}

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + offset);

        timeBtwShots -= Time.deltaTime;

        if (isFiring && timeBtwShots <= 0 && player.currentMana >= projectileManaCost)
        {
            Instantiate(projectile, shotPoint.position, shotPoint.rotation);
            timeBtwShots = startTimeBtwShots;
            player.ConsumeMana(projectileManaCost);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isFiring = false;
        }
    }
}
