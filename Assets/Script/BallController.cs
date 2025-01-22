using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody2D _rb;

    [SerializeField] Vector2[] _startPos;

    [SerializeField] GameObject _trail;

    public void ResetBall()
    {
        transform.position = Vector3.zero;

        if(_rb == null) _rb = GetComponent<Rigidbody2D>();

        _rb.velocity = _startPos[Random.Range(0, _startPos.Length)];

        _trail.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WALL"))
        {
            Vector2 _newVelocity = new Vector2(_rb.velocity.x, _rb.velocity.y);

            _newVelocity.y = -_newVelocity.y;

            _rb.velocity = _newVelocity;
        }

        if (collision.gameObject.CompareTag("PLAYER"))
        {
            PlayerController p = collision.gameObject.GetComponent<PlayerController>();

            Vector2 _newVelocity = new Vector2(_rb.velocity.x, _rb.velocity.y);

            _newVelocity.x = -_newVelocity.x;
            
            if(p != null && p.GetDirection() != 0)
            {
                _newVelocity.y = _newVelocity.y * p.GetDirection();
            }

            _rb.velocity = _newVelocity;

            _rb.velocity *= 1.05f;
        }

        if (collision.gameObject.CompareTag("WALL_PLAYER_ONE"))
        {
            _trail.SetActive(false);

            GameManager.OnUpdatePoints?.Invoke(0, 1);
        }

        if (collision.gameObject.CompareTag("WALL_PLAYER_TWO"))
        {
            _trail.SetActive(false);

            GameManager.OnUpdatePoints?.Invoke(1, 0);
        }
    }
}
