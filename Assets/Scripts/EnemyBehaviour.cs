using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [SerializeField] GameObject _player;

    float _enemyHealth = 100f;
    float _enemyMoveSpeed = 2f;
    Quaternion _targetRotation;
    bool _disableEnemy = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        _player = GameObject.FindGameObjectWithTag("Player");

        // Null check for _gameManager and _player
        if (_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }

        if (_player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager != null && !_gameManager._gameOver && !_disableEnemy && _player != null)
        {
            MoveEnemy();
            RotateEnemy();
        }
    }

    void MoveEnemy()
    {
        if (_player != null) // Null check for _player
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _enemyMoveSpeed * Time.deltaTime);
        }
    }

    void RotateEnemy()
    {
        if (_player != null) // Null check for _player
        {
            Vector2 _moveDirection = _player.transform.position - transform.position;
            _moveDirection.Normalize();

            _targetRotation = Quaternion.LookRotation(Vector3.forward, _moveDirection);

            if (transform.rotation != _targetRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, 200 * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(Damaged());
            _enemyHealth -= 40f;
            Destroy(collision.gameObject); // Moved inside the correct scope

            if (_enemyHealth <= 0f)
            {
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            _gameManager._gameOver = true;
            collision.gameObject.SetActive(false);
        }
    }

    IEnumerator Damaged()
    {
        _disableEnemy = true;
        yield return new WaitForSeconds(0.5f);
        _disableEnemy = false;
    }
}
