using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    Rigidbody2D _rb;
    Camera _mainCamera;

    float _moveVertical;
    float _moveHorizontal;
    float _moveSpeed = 5f;
    float _speedLimiter = 0.7f;
    Vector2 _moveVelocity;

    Vector2 _mousePos;
    Vector2 _offset;

    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject _bulletSpawn;
    bool _isShooting = false;
    float bulletSpeed = 15f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;

        // Rotate the player a bit to the left (e.g., -45 degrees)
        transform.rotation = Quaternion.Euler(0f, 0f, -45f);  // Fixed Z-axis rotation to the left
    }

    // Update is called once per frame
    void Update()
    {
        // Handle Player Movement
        _moveHorizontal = Input.GetAxisRaw("Horizontal");
        _moveVertical = Input.GetAxisRaw("Vertical");
        _moveVelocity = new Vector2(_moveHorizontal, _moveVertical) * _moveSpeed;

        // Handle Shooting
        if (Input.GetMouseButtonDown(0))
        {
            _isShooting = true;
        }
    }

    void FixedUpdate()
    {
        // Handle Player Movement and Rotation
        MovePlayer();
        RotatePlayer();

        // Shoot if the player is shooting
        if (_isShooting)
        {
            StartCoroutine(Fire());
        }
    }

    void MovePlayer()
    {
        if (_moveHorizontal != 0 || _moveVertical != 0)
        {
            _rb.linearVelocity = _moveVelocity; // Using linearVelocity instead of velocity
        }
        else
        {
            _moveVelocity *= _speedLimiter;
            _rb.linearVelocity = _moveVelocity; // Using linearVelocity instead of velocity
        }
    }

    void RotatePlayer()
    {
        _mousePos = Input.mousePosition;
        Vector3 screenPoint = _mainCamera.WorldToScreenPoint(transform.localPosition);
        _offset = new Vector2(_mousePos.x - screenPoint.x, _mousePos.y - screenPoint.y).normalized;

        float angle = Mathf.Atan2(_offset.y, _offset.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    // Fire function to shoot the bullet
    IEnumerator Fire()
    {
        _isShooting = false; // Reset shooting flag

        // Create a bullet and shoot it in the direction of the mouse
        GameObject bullet = Instantiate(_bullet, _bulletSpawn.transform.position, Quaternion.identity);

        // Set the bullet's linear velocity based on the direction (offset) and speed
        bullet.GetComponent<Rigidbody2D>().linearVelocity = _offset * bulletSpeed;  // Use linearVelocity instead of velocity

        // Add a small delay for subsequent shooting
        yield return new WaitForSeconds(0.2f); // Adjust the delay between shots as needed
    }
}
