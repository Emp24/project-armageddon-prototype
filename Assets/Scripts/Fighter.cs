using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    /*
    1. Detect
    2. Rotate
    3. Move
    4. Shoot
    */
    //Nearest enemy position 
    public Transform nearestEnemyPosition;
    //How far the projectile can go 
    public float projectileMaxRange;
    public Transform gunPos;
    public GameObject bullet;
    public float nextFireTime;
    public float fireRate;

    public void Update()
    {
        //if distance between nearest enemy and player is less than max range, move towards nearest enemy
        ScanSurrounding();
        Rotation(nearestEnemyPosition, transform);
    }
    public void Movement(Vector2 nearestEnemyPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, nearestEnemyPosition, 1 * Time.deltaTime);
    }
    public void Rotation(Transform target, Transform objectTransform)
    {
        Vector2 targetVector = new Vector2(target.position.x, target.position.y);
        targetVector.x = targetVector.x - objectTransform.position.x;
        targetVector.y = targetVector.y - objectTransform.position.y;
        float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;
        objectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

    }

    public void FireGun(Transform gunPos, GameObject source)
    {
        GameObject newBullet = Instantiate(bullet, new Vector3(gunPos.position.x, gunPos.position.y, 0), Quaternion.identity);
        // GameObject newBullet = BulletProjectilePool.SharedInstance.GetPooledObject();
        if (newBullet != null)
        {
            // Projectile projectile = newBullet.GetComponent<Projectile>();
            //details can be injected from source
            // projectile.damage = source.GetComponent<IDamageable>().damage;
            // projectile.gameObject.layer = LayerMask.NameToLayer(source.GetComponent<IDamageable>().layer);
            // projectile.source = source;
            newBullet.transform.position = gunPos.position;
            newBullet.transform.rotation = gunPos.rotation;
            newBullet.SetActive(true);
        }
        double sourceRotationAngleRad = source.transform.rotation.z * Math.PI / 180;

        Vector3 bulletDirection = source.transform.up.normalized;
        bulletDirection.y = bulletDirection.y * (float)Math.Cos(sourceRotationAngleRad);
        // newBullet.GetComponent<MonoBehaviour>().StartCoroutine(MoveBullet(newBullet, bulletDirection));
        StartCoroutine(MoveBullet(newBullet, bulletDirection));
    }
    IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    {
        float startTime = Time.time;
        float duration = 2f;
        while (Time.time - startTime < duration) // Adjust the duration as needed.
        {
            bullet.transform.position += direction * 10f * Time.deltaTime;
            yield return null;
        }
        bullet.SetActive(false);
        // BulletProjectilePool.SharedInstance.ReturnToPool(bullet);
    }

    public void ScanSurrounding()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D collider in colliders)
        {
            //Get nearest enemy
            //Move towards nearest enemy 
            if (collider.gameObject != gameObject)
            {
                Debug.Log(collider.gameObject.name + " in range");
                nearestEnemyPosition = collider.transform;
                float distance = Vector2.Distance(nearestEnemyPosition.position, transform.position);
                // Debug.Log("distance: " + distance);
                if (distance >= 15f)
                {
                    Movement(nearestEnemyPosition.position);
                }
                else
                {
                    if (Time.time >= nextFireTime)
                    {
                        FireGun(gunPos, gameObject);
                        nextFireTime = Time.time + fireRate;
                    }
                }
            }
        }

    }
}


public class Projectile : MonoBehaviour
{
    public float damage;
    public GameObject source;
}
