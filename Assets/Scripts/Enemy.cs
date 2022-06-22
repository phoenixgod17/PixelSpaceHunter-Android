﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] int health = 100;
    [SerializeField] float shotCounter;
    [SerializeField] float bulletSpeed = 8f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float explosionDestructionTime = 1f;
    LevelConfig levelConfig;
    [SerializeField] int scoreValue;
    [SerializeField] HealthBar healthBar;

    AudioSource myAudioSource;
    AudioClip laserSound;

    // Start is called before the first frame update
    void Start()
    {
        levelConfig = FindObjectOfType<LevelConfig>();
        shotCounter = Random.Range(levelConfig.GetMinTimeBetweenShots(), levelConfig.GetMaxTimeBetweenShots());
        healthBar.SetMaxHealth(health);
        myAudioSource = GetComponent<AudioSource>();        
        laserSound = myAudioSource.clip;
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {

        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(levelConfig.GetMinTimeBetweenShots(), levelConfig.GetMaxTimeBetweenShots());
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -bulletSpeed);
        myAudioSource.PlayOneShot(laserSound);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer)
        {
            return;
        }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<LevelSession>().AddToScore(scoreValue);        
        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, explosionDestructionTime);
        Destroy(gameObject);
    }
}
