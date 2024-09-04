using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterWithPatterns : MonoBehaviour, IEnemy
{
    [Tooltip("Prefab of the bullet to be instantiated and fired.")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletMoveSpeed;
    [SerializeField] private int burstCount;
    [SerializeField] private float projectilesPerBurst;
    [SerializeField][Range(0, 359)] private float angleSpread;
    [Tooltip("Distance from the shooter where the bullets will spawn.")]
    [SerializeField] private float startingDistance = 0.1f;
    [Tooltip("Time in seconds between consecutive bursts.")]
    [SerializeField] private float timeBetweenBursts;
    [Tooltip("Time in seconds the shooter rests after completing all bursts.")]
    [SerializeField] private float restTime = 1f;
    [Tooltip("'Stagger' needs 'Oscillate' enabled in order to work. If enabled, projectiles are fired with a delay between each.")]
    [SerializeField] private bool stagger;
    [Tooltip("'Oscillate' needs 'Stagger' enabled in order to work. If enabled, alternates the direction of the spread between bursts.")]
    [SerializeField] private bool oscillate;

    private bool isShooting = false;

    int burstCountCopy;
    float projectilesPerBurstCopy;
    float angleSpreadCopy;
    float timeBetweenBurstsCopy;
    float restTimeCopy;

    private void OnValidate()
    {
        // Adjust values based on constraints
        if (!oscillate) { stagger = false; }
        if (projectilesPerBurst < 1) { projectilesPerBurst = 1; }
        if (burstCount < 1) { burstCount = 1; }
        if (timeBetweenBursts < 0.1f) { timeBetweenBursts = 0.1f; }
        if (restTime < 0.1f) { restTime = 0.1f; }
        if (startingDistance < 0.1f) { startingDistance = 0.1f; }
        if (angleSpread == 0) { projectilesPerBurst = 1; }
        if (bulletMoveSpeed <= 0) { bulletMoveSpeed = 0.1f; }
    }

    public void Attack()
    {
        if (!isShooting)
        {
            int randomPattern = Random.Range(1, 4); // Randomly select a pattern (1, 2, or 3)

            switch (randomPattern)
            {
                case 1:
                    StartCoroutine(AttackPatternOne());
                    break;
                case 2:
                    StartCoroutine(AttackPatternTwo());
                    break;
                case 3:
                    StartCoroutine(AttackPatternThree());
                    break;
            }
        }
    }

    // Pattern 1: Original Shooting Pattern
    private IEnumerator AttackPatternOne()
    {
        isShooting = true;
        SaveOriginalValues();

        // Modify parameters for pattern 1 if needed
        burstCount = 5;
        projectilesPerBurst = 36;
        angleSpread = 359;
        timeBetweenBursts = 0.5f;
        restTime = 2;

        yield return StartCoroutine(ShootRoutine());

        RestoreOriginalValues();
        isShooting = false;
    }

    // Pattern 2: Machine Gun - Rapid Fire Bullets that Follow the Player
    private IEnumerator AttackPatternTwo()
    {
        isShooting = true;
        SaveOriginalValues();

        // Machine gun settings
        burstCount = 1;
        projectilesPerBurst = 30; // Number of bullets in a rapid burst
        timeBetweenBursts = 0.1f; // Very short delay between bullets
        restTime = 1; // Short rest time

        for (int i = 0; i < burstCount; i++)
        {
            for (int j = 0; j < projectilesPerBurst; j++)
            {
                if (PlayerController.Instance != null)
                {
                    // Find direction towards the player
                    Vector2 playerDirection = (PlayerController.Instance.transform.position - transform.position).normalized;

                    // Calculate the spawn position
                    Vector2 pos = (Vector2)transform.position + playerDirection * startingDistance;

                    GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                    newBullet.transform.right = playerDirection; // Adjust bullet direction towards the player

                    if (newBullet.TryGetComponent(out Projectile projectile))
                    {
                        projectile.UpdateMoveSpeed(bulletMoveSpeed);
                    }
                }

                yield return new WaitForSeconds(timeBetweenBursts); // Fire bullets rapidly one at a time
            }

            yield return new WaitForSeconds(restTime); // Rest between bursts
        }

        RestoreOriginalValues();
        isShooting = false;
    }

    // Pattern 3: Spiral Burst - Shoots Bullets in a Spiral Pattern
    private IEnumerator AttackPatternThree()
    {
        isShooting = true;
        SaveOriginalValues();

        burstCount = 1;
        projectilesPerBurst = 30; // Total number of bullets in the spiral
        angleSpread = 360; // Full circle
        float spiralAngleStep = 12f; // Angle increment per bullet
        float currentAngle = 0f; // Starting angle

        for (int i = 0; i < projectilesPerBurst; i++)
        {
            Vector2 pos = FindBulletSpawnPos(currentAngle);
            GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
            newBullet.transform.right = newBullet.transform.position - transform.position;

            if (newBullet.TryGetComponent(out Projectile projectile))
            {
                projectile.UpdateMoveSpeed(bulletMoveSpeed);
            }

            currentAngle += spiralAngleStep; // Increment the angle to create a spiral effect

            yield return new WaitForSeconds(0.1f); // Delay between each bullet
        }

        yield return new WaitForSeconds(restTime); // Rest after the spiral attack

        RestoreOriginalValues();
        isShooting = false;
    }

    private IEnumerator ShootRoutine()
    {
        float startAngle, currentAngle, angleStep, endAngle;
        float timeBetweenProjectiles = 0f;

        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);

        if (stagger) { timeBetweenProjectiles = timeBetweenBursts / projectilesPerBurst; }

        for (int i = 0; i < burstCount; i++)
        {
            if (!oscillate)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }

            if (oscillate && i % 2 != 1)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }
            else if (oscillate)
            {
                currentAngle = endAngle;
                endAngle = startAngle;
                startAngle = currentAngle;
                angleStep *= -1;
            }

            for (int j = 0; j < projectilesPerBurst; j++)
            {
                Vector2 pos = FindBulletSpawnPos(currentAngle);

                GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                newBullet.transform.right = newBullet.transform.position - transform.position;

                if (newBullet.TryGetComponent(out Projectile projectile))
                {
                    projectile.UpdateMoveSpeed(bulletMoveSpeed);
                }

                currentAngle += angleStep;

                if (stagger) { yield return new WaitForSeconds(timeBetweenProjectiles); }
            }

            currentAngle = startAngle;

            if (!stagger) { yield return new WaitForSeconds(timeBetweenBursts); }
        }

        yield return new WaitForSeconds(restTime);
    }

    private void SaveOriginalValues()
    {
        burstCountCopy = burstCount;
        projectilesPerBurstCopy = projectilesPerBurst;
        angleSpreadCopy = angleSpread;
        timeBetweenBurstsCopy = timeBetweenBursts;
        restTimeCopy = restTime;
    }

    private void RestoreOriginalValues()
    {
        burstCount = burstCountCopy;
        projectilesPerBurst = projectilesPerBurstCopy;
        angleSpread = angleSpreadCopy;
        timeBetweenBursts = timeBetweenBurstsCopy;
        restTime = restTimeCopy;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        endAngle = targetAngle;
        currentAngle = targetAngle;
        float halfAngleSpread = 0f;
        angleStep = 0;
        if (angleSpread != 0)
        {
            angleStep = angleSpread / (projectilesPerBurst - 1);
            halfAngleSpread = angleSpread / 2f;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    private Vector2 FindBulletSpawnPos(float currentAngle)
    {
        float x = transform.position.x + startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = transform.position.y + startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        Vector2 pos = new Vector2(x, y);

        return pos;
    }
}
