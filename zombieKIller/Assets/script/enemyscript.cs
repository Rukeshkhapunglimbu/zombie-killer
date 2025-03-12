using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D enemyCollider;

    private GameManager gameManager;
    private bool isDead;

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        InitializeGameManager();
        ResetState();
    }

    private void InitializeComponents()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"Animator component missing on {gameObject.name}");
            }
        }

        if (enemyCollider == null)
        {
            enemyCollider = GetComponent<Collider2D>();
            if (enemyCollider == null)
            {
                Debug.LogError($"Collider2D component missing on {gameObject.name}");
            }
        }
    }

    private void InitializeGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }

    private void ResetState()
    {
        isDead = false;
        if (animator != null)
        {
            animator.SetBool("isDead", false);
        }
        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        if (other != null && other.CompareTag("bullet"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        if (gameManager != null)
        {
            gameManager.OnZombieKilled();
        }
    }
}