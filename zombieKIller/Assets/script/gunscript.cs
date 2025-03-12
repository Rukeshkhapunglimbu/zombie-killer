using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class GunScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameManager gameManager;

    [Header("Settings")]
    [SerializeField] private float shootCooldown = 0.2f;
    [SerializeField] private float touchEndDelay = 0.1f; // Delay to check UI interaction

    private Vector2 worldPosition;
    private Vector2 direction;
    private float lastShootTime;
    private bool isTouchValid;
    private Vector2 touchStartPosition;

    private void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found in the scene!");
            }
        }

        if (gun == null) gun = gameObject;
        if (bulletSpawnPoint == null) bulletSpawnPoint = transform;
    }

    private void Update()
    {
        if (gameManager != null && gameManager.isGameOver) return;

        HandleGunRotation();
        HandleGunShooting();
    }

    private void HandleGunRotation()
    {
        Vector2 inputPosition = GetInputPosition();
        if (inputPosition != Vector2.zero)
        {
            direction = (inputPosition - (Vector2)gun.transform.position).normalized;
            gun.transform.right = direction;
        }
    }

    private Vector2 GetInputPosition()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // Store touch start position
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                touchStartPosition = touch.position;
                isTouchValid = !IsTouchingUI();
            }
            
            // Only return position if the touch started on a valid area
            if (isTouchValid)
            {
                return Camera.main.ScreenToWorldPoint(touch.position);
            }
        }
        else if (Mouse.current != null && !EventSystem.current.IsPointerOverGameObject())
        {
            return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
        return Vector2.zero;
    }

    private void HandleGunShooting()
    {
        if (ShouldPreventShooting()) return;

        bool shouldShoot = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == UnityEngine.TouchPhase.Ended && isTouchValid)
            {
                // Check if the touch ended near where it started
                float touchDistance = Vector2.Distance(touchStartPosition, touch.position);
                if (touchDistance < Screen.width * 0.05f) // 5% of screen width tolerance
                {
                    StartCoroutine(ValidateAndShoot());
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            shouldShoot = true;
        }

        if (shouldShoot && Time.time >= lastShootTime + shootCooldown)
        {
            ShootBullet();
            lastShootTime = Time.time;
        }
    }

    private IEnumerator ValidateAndShoot()
    {
        // Wait a short moment to ensure UI interaction is complete
        yield return new WaitForSeconds(touchEndDelay);
        
        if (!IsTouchingUI() && Time.time >= lastShootTime + shootCooldown)
        {
            ShootBullet();
            lastShootTime = Time.time;
        }
    }

    private bool ShouldPreventShooting()
    {
        return gameManager.isGamePaused || 
               gameManager.isGameOver || 
               IsTouchingUI() ||
               gameManager.bulletsUsed >= gameManager.maxBullets;
    }

    private void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, gun.transform.rotation);
        gameManager?.OnBulletFired();
    }

    private bool IsTouchingUI()
    {
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return EventSystem.current.IsPointerOverGameObject();
    }
}