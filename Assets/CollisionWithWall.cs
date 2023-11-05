using UnityEngine;

public class CollisionWithWall : MonoBehaviour
{
    [SerializeField] private PhoneMovement phoneMovement;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            phoneMovement.SetInCollisionWithWall(true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            phoneMovement.SetInCollisionWithWall(true);
        }
    }
}
