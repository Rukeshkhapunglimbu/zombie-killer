// using UnityEngine;
// public class BulletScript : MonoBehaviour
// {
//     [Header("Bounce Settings")]
//     [SerializeField] private int maxBounces = 9;
//     [SerializeField] private float normalBulletSpeed = 15f;

//     [Header("Audio")]
//     [SerializeField] private AudioClip bounceSound;

//     private int bounceCount = 0;

//     [SerializeField] int lastbounceCount = 0;
//     private GameManager manager;
//     private AudioSource audioSource;
//     private Rigidbody2D rb;

//     [SerializeField] bool lastbulletUsed = false;

//     private void Awake()
//     {
//         InitializeComponents();
//     }

//     private void Start()
//     {
//         InitializeGameManager();
//         SetStraightVelocity();
//     }
//     private void InitializeComponents()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         if (rb == null)
//         {
//             rb = gameObject.AddComponent<Rigidbody2D>();
//         }
//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null)
//         {
//             audioSource = gameObject.AddComponent<AudioSource>();
//         }
//     }

//     private void InitializeGameManager()
//     {
//         manager = FindObjectOfType<GameManager>();
//         if (manager == null)
//         {
//             Debug.LogError("GameManager not found in the scene!");
//         }
//     }

//     public void SetStraightVelocity()
//     {
//         if (rb != null)
//         {
//             rb.velocity = transform.right * normalBulletSpeed;
//         }
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision == null) return;

//         PlayBounceSound();
//         bounceCount++;
//         if (gameObject.tag ==  "lastBullet")
//         {
//             lastbounceCount++;
//         }


//         if (collision.gameObject.CompareTag("Enemy"))
//         {
//             HandleEnemyCollision(collision.gameObject);
//         }

//         CheckBounceLimit();
//     }

//     private void HandleEnemyCollision(GameObject enemy)
//     {
//         // Get enemy component and handle death
//         var enemyComponent = enemy.GetComponent<EnemyScript>();
//         if (enemyComponent != null)
//         {
//             enemyComponent.Die();
//         }
//         Destroy(gameObject);
//     }

//     private void CheckBounceLimit()
//     {
//         if (bounceCount >= maxBounces && !lastbulletUsed)
//         {
//             Destroy(gameObject);
//         }
//         if (manager != null && manager.bulletsUsed >= manager.maxBullets)
//         {
//             gameObject.tag = "lastBullet";

//             if (lastbounceCount >= maxBounces)
//             {
//                 Debug.Log("defeat");
//                 manager.ShowDefeatPanel();
//             }
//         }
//     }
//     private void PlayBounceSound()
//     {
//         if (audioSource != null && bounceSound != null)
//         {
//             audioSource.PlayOneShot(bounceSound);
//         }
//     }
// }
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private int maxBounces = 9;
    [SerializeField] private float normalBulletSpeed = 15f;

    [Header("Audio")]
    [SerializeField] private AudioClip bounceSound;

    private int bounceCount = 0; // Tracks individual bullet bounces
    private GameManager manager;
    private AudioSource audioSource;
    private Rigidbody2D rb;

    private bool isLastBullet = false; // Tracks if this is the last bullet

    private void Awake()
    {
        InitializeComponents();
    }
    private void Start()
    {
        InitializeGameManager();
        SetStraightVelocity();
        CheckIfLastBullet(); // Check if this is the last bullet at the start
    }
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
//      private void InitializeComponents()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         if (rb == null)
//         {
//             rb = gameObject.AddComponent<Rigidbody2D>();
//         }
//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null)
//         {
//             audioSource = gameObject.AddComponent<AudioSource>();
//         }
//     }
    private void InitializeGameManager()
    {
        manager = FindObjectOfType<GameManager>();
        if (manager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }
    public void SetStraightVelocity()
    {
        if (rb != null)
        {
            rb.velocity = transform.right * normalBulletSpeed;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        Debug.Log("Collision detected with: " + collision.gameObject.name);
        PlayBounceSound();
        bounceCount++;

        // Destroy bullet after 9 bounces
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);

            // If this is the last bullet, show defeat panel
            if (isLastBullet)
            {
                Debug.Log("Defeat: Last bullet bounced 9 times");
                manager.ShowDefeatPanel();
            }
        }
    }
    private void CheckIfLastBullet()
    {
        if (manager != null && manager.bulletsUsed >= manager.maxBullets)
        {
            isLastBullet = true;
        }
    }
  private void PlayBounceSound()
    {
        if (audioSource != null && bounceSound != null)
        {
            Debug.Log("Playing bounce sound");
            audioSource.PlayOneShot(bounceSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or bounceSound is null");
        }
    }
}