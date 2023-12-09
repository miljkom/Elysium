using UnityEngine;

public class CollisionWithWall : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            player.SetInCollisionWithWall();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            player.NotInCollisionWithWall();
        }
    }
}
