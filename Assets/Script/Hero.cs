using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("�⺻ �߻� ����")]
    public float fireRate = 1f;        // 1�ʸ��� �߻�
    public float shootingRange = 10f;  // ��Ÿ�
    public GameObject bulletPrefab;    // �Ѿ� ������
    public Transform firePoint;        // �Ѿ��� �߻�� ��ġ
    public float bulletSpeed = 20f;    // �Ѿ� �⺻ �ӵ�
    public int bulletDamage = 10;      // �Ѿ� ������

    [Header("�� ȸ�� ����")]
    public Transform gunTransform;     // �� ������Ʈ Ʈ������

    [Header("���� ����")]
    public int bulletsPerShot = 5;     // �� ���� �߻��� �Ѿ� ��
    public float spreadAngle = 10f;    // ź���� ����
    public float minSpeedMultiplier = 0.9f; // �ּ� �ӵ� ����
    public float maxSpeedMultiplier = 1.1f; // �ִ� �ӵ� ����

    [Header("���� ���� ����")]
    public Color lineColor = Color.white;
    public float lineWidth = 0.05f;
    public float lineDistance = 10f;   // ���� ����
    public float lineFanAngle = 15f;   // ���� ��ä�� ����

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