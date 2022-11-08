using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketScript : MonoBehaviour
{
    [Header("Rocket")]
    Rigidbody2D rb;
    public float rocketForce = 2.0f;
    public float rocketTorque = 0.5f;

    public float outOfBoundValueX = 45;
    public float outOfBoundValueYBottom = -19;
    public float outOfBoundValueYTop = 27;
    public float landingAngle = 10;
    public float landingSpeed = 10;

    bool isOutOfMap;

    [Header("Thruster")]
    public SpriteRenderer thruster;
    bool thrusterActive;
    public float thrusterGrowth = 0.05f;
    public float thrusterMin = 0;
    public float thrusterMax = 0.6f;

    [Header("SoundEffects")]
    public AudioSource accelerationSound;
    public AudioSource explosionSound;

    public Animator explosionAnimation;
    private bool isExploded = false;

    private void Awake()
    {
        explosionAnimation.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        thrusterActive = false;
        isOutOfMap = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateThrusterScale();
        RespawnShipWhenOutOfMap();
    }

    private void FixedUpdate()
    {
        if(isExploded == false)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
            {
                //Full force reactor
                rb.AddRelativeForce(new Vector2(0, rocketForce), ForceMode2D.Force);
                thrusterActive = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                //Half force reactor for landing
                rb.AddRelativeForce(new Vector2(0, rocketForce / 2), ForceMode2D.Force);
                thrusterActive = true;
            }
            else
            {
                thrusterActive = false;
            }

            if (Input.GetKey(KeyCode.A))
            {
                rb.AddTorque(rocketTorque);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddTorque(-rocketTorque);
            }
        }
        else if(isExploded)
        {
            ResetGame();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("LandingZone") && transform.eulerAngles.z < landingAngle && transform.eulerAngles.z > -landingAngle && collision.relativeVelocity.y < landingSpeed) { 
       
            Debug.Log(collision.relativeVelocity);
            Debug.Log("Landing succesfull");

        } 
        else
        {
            Debug.Log(collision.relativeVelocity);
            Debug.Log("Failure");
            if(isExploded == false)
                RocketExplode();
        }
    }

    private void UpdateThrusterScale()
    {
        if (thrusterActive)
        {
            //Grow thruster
            thruster.transform.localScale += new Vector3(0, thrusterGrowth * Time.deltaTime, 0);

            //Decrease thruster
            if (thruster.transform.localScale.y > thrusterMax)
            {
                thruster.transform.localScale = new Vector3(thruster.transform.localScale.x, thrusterMax, thruster.transform.localScale.z);
            }
        }
        else
        {
            //Decrease thruster if not on
            thruster.transform.localScale += new Vector3(0, -thrusterGrowth * Time.deltaTime, 0);

            //Keeps from going backwards
            if (thruster.transform.localScale.y < thrusterMin)
            {
                thruster.transform.localScale = new Vector3(thruster.transform.localScale.x, thrusterMin, thruster.transform.localScale.z);
            }
        }

        
        AccelerationSoundManager(thrusterActive);
    }

    private void RespawnShipWhenOutOfMap()
    {
        if(isOutOfMap == true)
        {
            if (gameObject.transform.position.x < -outOfBoundValueX)
            {
                if (gameObject.transform.position.y > outOfBoundValueYBottom)
                {
                    gameObject.transform.position = new Vector3(outOfBoundValueX, gameObject.transform.position.y, gameObject.transform.position.z);
                    if(gameObject.transform.position.y < outOfBoundValueYTop)
                    {
                        isOutOfMap = false;
                    }
                }
                else
                {
                    gameObject.transform.position = new Vector3(outOfBoundValueX, +outOfBoundValueYBottom, gameObject.transform.position.z);
                }

            }
            else if (gameObject.transform.position.x > outOfBoundValueX)
            {
                if (gameObject.transform.position.y > outOfBoundValueYBottom)
                {
                    gameObject.transform.position = new Vector3(-outOfBoundValueX, gameObject.transform.position.y, gameObject.transform.position.z);
                    if (gameObject.transform.position.y < outOfBoundValueYTop)
                    {
                        isOutOfMap = false;
                    }
                }
                else
                {
                    gameObject.transform.position = new Vector3(-outOfBoundValueX, +outOfBoundValueYBottom, gameObject.transform.position.z);
                }

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isOutOfMap = true;
    }

    private void AccelerationSoundManager(bool isOn)
    {
        if (isOn && accelerationSound.isPlaying == false)
        {
            //Debug.Log("Play");
            accelerationSound.Play();
        }
        else
        {
            //Debug.Log("Stop");
            accelerationSound.Pause();
        }
    }

    private void RocketExplode()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        explosionAnimation.enabled = true;
        thruster.enabled = false;
        rb.velocity = Vector3.zero;
        explosionSound.Play();

        isExploded = true;
    }

    private void ResetGame()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
