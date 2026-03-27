using UnityEngine;
using UnityEngine.UI;   
using TMPro;            
using System.Collections; 

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float mass = 1200f;
    public float acceleration = 10f;
    public float turnPower = 2f;
    public float dragCoefficient = 0.3f;

    [Header("UI")]
    public GameObject gameOverText;
    public GameObject creditPanel; 
    public float creditScrollSpeed = 20f;
    public float winTextDuration = 2f; 

    private Rigidbody rb;
    private bool gameStarted = false;
    private bool gameEnded = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (gameOverText != null)
            gameOverText.SetActive(false);

        if (creditPanel != null)
            creditPanel.SetActive(false);

        Time.timeScale = 1f;
        Invoke("StartGame", 0.5f);
    }

    void StartGame()
    {
        gameStarted = true;
    }

    void FixedUpdate()
    {
        if (gameEnded) return;

        Move();
        Turn();
        ApplyDrag();
        Drift();
    }

    void Move()
    {
        float input = Input.GetAxis("Vertical");
        rb.AddForce(transform.forward * mass * acceleration * input);
    }

    void Turn()
    {
        float turn = Input.GetAxis("Horizontal");
        float speed = rb.velocity.magnitude;

        if (speed > 0.5f)
        {
            float turnAmount = turn * turnPower * speed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }
    }

    void ApplyDrag()
    {
        rb.AddForce(-dragCoefficient * rb.velocity);
    }

    void Drift()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.angularDamping = 0.5f;
            rb.drag = 0.5f;
        }
        else
        {
            rb.angularDamping = 3f;
            rb.drag = 1f;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (!gameStarted || gameEnded) return;

        if (col.gameObject.CompareTag("Wall"))
        {
            GameOver();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!gameStarted || gameEnded) return;

        if (other.CompareTag("Finish"))
        {
            WinGame();
        }
    }

    void GameOver()
    {
        gameEnded = true;

        if (gameOverText != null)
            gameOverText.SetActive(true);

        Time.timeScale = 0f;
    }

    void WinGame()
    {
        gameEnded = true;

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
            gameOverText.GetComponent<TextMeshProUGUI>().text = "YOU WIN!";
        }

        if (creditPanel != null)
            creditPanel.SetActive(true);

        Time.timeScale = 0f;

        
        StartCoroutine(HideWinText(winTextDuration));
    }

    IEnumerator HideWinText(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (gameOverText != null)
            gameOverText.SetActive(false);
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }

        
        if (creditPanel != null && creditPanel.activeSelf)
        {
            creditPanel.transform.Translate(Vector3.up * creditScrollSpeed * Time.unscaledDeltaTime);
        }
    }

    void ResetGame()
    {
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
            gameOverText.GetComponent<TextMeshProUGUI>().text = "GAME OVER";
        }

        if (creditPanel != null)
            creditPanel.SetActive(false);

        Time.timeScale = 1f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = startRotation;

        gameStarted = true;
        gameEnded = false;

        if (creditPanel != null)
            creditPanel.transform.localPosition = Vector3.zero;
    }
}
