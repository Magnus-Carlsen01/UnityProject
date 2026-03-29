using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    public float chaseSpeed = 3f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;

    public int damage = 1;
    public int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    //Loot
    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;
    }
   
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 <<player.gameObject.layer);
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 1f, groundLayer);
            
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);

            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 1f, groundLayer);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true;
            }
        }
        
    }
     private void FixedUpdate()
        {
            if(isGrounded && shouldJump)
            {
                shouldJump = false;
                Vector2 direction = (player.position - transform.position).normalized;

                Vector2 jumpDirection = direction*jumpForce;

                rb.AddForce(new Vector2(jumpDirection.x, jumpForce),ForceMode2D.Impulse);
            }
        }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashWhite());
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }
    private void Die()
    {
        // Tạo list chứa các item thỏa điều kiện drop chance
        List<LootItem> potentialDrops = new List<LootItem>();
        
        foreach (LootItem loot in lootTable)
        {
            if (Random.Range(0f, 100f) < loot.dropChance)
            {
                potentialDrops.Add(loot);
            }
        }

        // Nếu có items thỏa điều kiện, chọn ngẫu nhiên 1 item để rơi ra
        if (potentialDrops.Count > 0)
        {
            LootItem selectedLoot = potentialDrops[Random.Range(0, potentialDrops.Count)];
            GameObject lootInstance = Instantiate(selectedLoot.itemPrefab, transform.position, Quaternion.identity);
            lootInstance.GetComponent<SpriteRenderer>().color = Color.red;
        }

        Destroy(gameObject);
    }
}

