using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // Điểm A
    public Transform pointB; // Điểm B
    public float moveSpeed = 2f; // Tốc độ di chuyển

    private Vector3 targetPosition; // Vị trí mục tiêu
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPosition = pointB.position; // Bắt đầu di chuyển đến điểm B
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (transform.position == targetPosition)
        {
            // Đổi hướng di chuyển khi đến điểm A hoặc B
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Gắn kết đối tượng với nền tảng khi va chạm
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Thả đối tượng khỏi nền tảng khi không còn va chạm
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
