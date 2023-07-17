using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private CapsuleCollider col;
    private Animator anim;
    private Score score;
    private Vector3 dir;
    [SerializeField] private float speed;
    [SerializeField] private float JumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private int coins;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject ShieldPanel;
    [SerializeField] private GameObject StarPanel;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Score scoreScript;

    private bool isSliding;
    private bool Immortallity;

    private int lineToMove = 1;
    public float lineDistance = 4;
    private float maxSpeed = 40;
    
    void Start()
    {
        
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        col = GetComponent<CapsuleCollider>();
        score = scoreText.GetComponent<Score>();
        score.scoreMultiplier = 1;
        Time.timeScale = 1;
        coins = PlayerPrefs.GetInt("coins");
        coinsText.text = coins.ToString();
        StartCoroutine(SpeedIncrease());
        Immortallity = false;

    }


    private void Update()
    {
        if (SwipeController.swipeRight)
        {
            if (lineToMove < 2)
            {
                lineToMove++;
            } 
        }

        if (SwipeController.swipeLeft)
        {
            if (lineToMove > 0)
            {
                lineToMove--;
            }
        }

        if (SwipeController.swipeUp)
        {
            if (controller.isGrounded)
                Jump();
        }

        if (SwipeController.swipeDown)
        {
            StartCoroutine(Slide());
        }


        if (controller.isGrounded && !isSliding)
            anim.SetBool("isRunning", true);
        else
            anim.SetBool("isRunning", false);



        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (lineToMove == 0)
            targetPosition += Vector3.left * lineDistance;
        else if (lineToMove == 2)
            targetPosition += Vector3.right * lineDistance;

        if (transform.position == targetPosition)
            return;

        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 10 * Time.deltaTime;

        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);

    }

    private void Jump()
    {
        dir.y = JumpForce;
        anim.SetTrigger("isJumping");
    }

    void FixedUpdate()
    {
        dir.z = speed;
        dir.y += gravity * Time.fixedDeltaTime;
        controller.Move(dir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "obstacle")
        {
            if (Immortallity)
                Destroy(hit.gameObject);
            else
            {
                losePanel.SetActive(true);
                int lastRunScore = int.Parse(scoreScript.scoreText.text.ToString());
                PlayerPrefs.SetInt("lastRunScore", lastRunScore);
                Time.timeScale = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="coin")
        {
            coins += 1;
            PlayerPrefs.SetInt("coins", coins);
            coinsText.text = coins.ToString();
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag=="BonusStar")
        {
            
            StartCoroutine(Star());
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Shield")
        {
            
            StartCoroutine(Shield());
            Destroy(other.gameObject);
            
        }

    }

    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(2);
        if (speed < maxSpeed)
        {
            speed += 1;
            StartCoroutine(SpeedIncrease());
        }
    }

    private IEnumerator Slide()
    {
        col.center = new Vector3(0,-0.3f,0);
        controller.center = new Vector3(0, -0.3f, 0);
        col.height = 0.5f;
        controller.height = 0.5f;
        isSliding = true;
        anim.SetBool("isRunning", false);
        anim.SetTrigger("isSliding");

        yield return new WaitForSeconds(0.7f);

        col.center = new Vector3(0, 0, 0);
        controller.center = new Vector3(0, 0, 0);
        col.height = 2f;
        controller.height = 2f;
        isSliding = false;
    }

    private IEnumerator Star()
    {
        score.scoreMultiplier = 2;
        StarPanel.SetActive(true);

        yield return new WaitForSeconds(5);

        score.scoreMultiplier = 1;
        StarPanel.SetActive(false);
    }

    private IEnumerator Shield()
    {
        Immortallity = true;
        ShieldPanel.SetActive(true);

        yield return new WaitForSeconds(3);

        Immortallity = false;
        ShieldPanel.SetActive(false);
    }

    public void StarButton()
    {
        if (coins >= 20)
        {
            coins = coins - 20;
            PlayerPrefs.SetInt("coins", coins);
            coinsText.text = coins.ToString();
            StartCoroutine(Star());
        }

    }

    public void ShielButton()
    {
        if (coins >= 20)
        {
            coins = coins - 20;
            PlayerPrefs.SetInt("coins", coins);
            coinsText.text = coins.ToString();
            StartCoroutine(Shield());
        }
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
    }


}
