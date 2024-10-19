using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public GameObject paddle;
    public GameObject ball;
    public GameObject brickContainer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateGameplayElements()
    {
        paddle.SetActive(true);
        ball.SetActive(true);
        brickContainer.SetActive(true);
    }

    public void DeactivateGameplayElements()
    {
        paddle.SetActive(false);
        ball.SetActive(false);
        brickContainer.SetActive(false);
    }

    public void ResetBallPosition()
    {
        // Reset the ball to its starting position
        if (ball != null && paddle != null)
        {
            ball.transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y + 0.5f, 0);
            ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}