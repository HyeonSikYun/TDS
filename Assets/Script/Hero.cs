using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("기본 발사 설정")]
    public float fireRate = 1f;        // 1초마다 발사
    public float shootingRange = 10f;  // 사거리
    public GameObject bulletPrefab;    // 총알 프리팹
    public Transform firePoint;        // 총알이 발사될 위치
    public float bulletSpeed = 20f;    // 총알 기본 속도
    public int bulletDamage = 10;      // 총알 데미지

    [Header("총 회전 설정")]
    public Transform gunTransform;     // 총 오브젝트 트랜스폼

    [Header("샷건 설정")]
    public int bulletsPerShot = 5;     // 한 번에 발사할 총알 수
    public float spreadAngle = 10f;    // 탄퍼짐 각도
    public float minSpeedMultiplier = 0.9f; // 최소 속도 배율
    public float maxSpeedMultiplier = 1.1f; // 최대 속도 배율

    [Header("궤적 라인 설정")]
    public Color lineColor = Color.white;
    public float lineWidth = 0.05f;
    public float lineDistance = 10f;   // 라인 길이
    public float lineFanAngle = 15f;   // 라인 부채꼴 각도

    private float nextFireTime = 0f;
    private Transform currentTarget;
    private LineRenderer leftLine;
    private LineRenderer rightLine;

    void Start()
    {
        leftLine = CreateLineRenderer("LeftLine");
        rightLine = CreateLineRenderer("RightLine");
    }

    private LineRenderer CreateLineRenderer(string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = transform;
        lineObj.transform.localPosition = Vector3.zero;

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.positionCount = 2;

        return line;
    }

    void Update()
    {
        FindClosestEnemy();

        if (currentTarget != null)
        {
            UpdateTrajectoryLines();
            UpdateGunRotation();
        }
        else
        {
            leftLine.enabled = false;
            rightLine.enabled = false;
        }

        if (Time.time >= nextFireTime)
        {
            if (currentTarget != null)
            {
                ShootShotgun();
            }
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void UpdateGunRotation()
    {
        if (currentTarget == null || gunTransform == null) return;

        Vector3 targetDirection = currentTarget.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FindClosestEnemy()
    {
        Zombie[] zombies = FindObjectsOfType<Zombie>();

        if (zombies.Length == 0)
        {
            currentTarget = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var zombie in zombies)
        {
            if (zombie == null) continue;

            float distance = Vector3.Distance(transform.position, zombie.transform.position);

            if (distance < closestDistance && distance <= shootingRange)
            {
                closestDistance = distance;
                closestEnemy = zombie.transform;
            }
        }

        currentTarget = closestEnemy;
    }

    void UpdateTrajectoryLines()
    {
        if (currentTarget == null) return;

        Vector3 baseDirection = (currentTarget.position - firePoint.position).normalized;
        float halfFanAngle = lineFanAngle / 2;

        Quaternion leftRotation = Quaternion.Euler(0, 0, halfFanAngle);
        Vector3 leftDirection = leftRotation * baseDirection;

        Quaternion rightRotation = Quaternion.Euler(0, 0, -halfFanAngle);
        Vector3 rightDirection = rightRotation * baseDirection;

        leftLine.SetPosition(0, firePoint.position);
        leftLine.SetPosition(1, firePoint.position + leftDirection * lineDistance);
        leftLine.enabled = true;

        rightLine.SetPosition(0, firePoint.position);
        rightLine.SetPosition(1, firePoint.position + rightDirection * lineDistance);
        rightLine.enabled = true;
    }

    void ShootShotgun()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        Vector3 baseDirection = (currentTarget.position - firePoint.position).normalized;
        float angleStep = spreadAngle / (bulletsPerShot - 1);
        float startAngle = -spreadAngle / 2;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angle = startAngle + (angleStep * i);
            angle += Random.Range(-angleStep / 4, angleStep / 4);

            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 direction = rotation * baseDirection;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            float speedMultiplier = Random.Range(minSpeedMultiplier, maxSpeedMultiplier);
            float currentBulletSpeed = bulletSpeed * speedMultiplier;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(direction);
                bulletScript.speed = currentBulletSpeed;
                bulletScript.damage = bulletDamage;
            }
        }
    }
}