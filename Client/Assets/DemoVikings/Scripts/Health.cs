using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* This is for Health of Player */
public class Health : Photon.MonoBehaviour/*, IPunObservable*/
{
    [SerializeField]
    private TextMesh m_text;
    public int m_health, m_maxhealth;

    public int PlayerHealth
    {
        get
        {
            return m_health;
        }
        set
        {
            m_health = value;
        }
    }
    public int PlayerMaxHealth
    {
        get
        {
            return m_maxhealth;
        }
        set
        {
            m_maxhealth = value;
        }
    }

    private GameObject m_player;
    public static bool isDied, isSent;
    private bool isShielded, isSpawned;
    private double m_timer, m_shieldtime;

    private const int MINUS_SCORE_DIED = 5;

    public string shield = "Shield";
    public GameObject m_shield;

    // Use this for initialization
    void Start ()
    {
        m_maxhealth = 10;
        m_timer = 0.0;
        m_shieldtime = 0.0;
        isDied = false;
        isShielded = false;
        isSpawned = false;
        isSent = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        PrintHealth();
        // Debug.Log("In health : " + m_health);
        if (GetComponentInParent<PhotonView>().isMine)
        {
            if (m_health <= 0 && !isShielded)
            {
                m_health = 0;
                isDied = true;
            }
        }
        
        // When died, respawn after buffer
        if (isDied)
        {
            /* Sent to GameSpark score of Player for Leaderboard */
            if (!isSent) // only send once
            {
                new GameSparks.Api.Requests.LogEventRequest()
                    .SetEventKey("SUBMIT_SCORE")
                    .SetEventAttribute("SCORE", transform.parent.GetComponent<Highscore>().m_score)
                    .Send((response) =>
                    {
                        if (!response.HasErrors)
                        {
                            Debug.Log("Score Posted Successfully..." + transform.parent.GetComponent<Highscore>().m_score);
                        }
                        else
                        {
                            Debug.Log("Error Posting Score..." + response.Errors.JSON.ToString());
                        }
                    });
                isSent = true;
            }

            m_timer += Time.deltaTime;
            //Debug.Log("died Time : " + m_timer);
            if (m_timer > 3.0)
            {
                //transform.parent.GetComponent<Highscore>().m_score -= MINUS_SCORE_DIED;
                transform.parent.GetComponent<Highscore>().m_score = 0; // reset score to zero

                //isShielded = true;
                //isSpawned = true;
                m_health = m_maxhealth;
                isDied = false;
                isSent = false;
                m_timer = 0.0;
            }
        }

        // Activate shield 
        if (isShielded && !GetComponentInParent<PhotonView>().isMine)
        {
            //create shield
            m_shield = PhotonNetwork.Instantiate(shield, transform.parent.position, Quaternion.identity, 0);
            m_shield.transform.SetParent(transform.parent);

            isShielded = false;
        }

        // Activate countdown for shield
        if (isSpawned)
        {
            m_shieldtime += Time.deltaTime;
            //Debug.Log("Shield Time : " + m_shieldtime);
            if (m_shieldtime > 5.0 && m_shield)
            {
                // destroy shield 
                PhotonNetwork.Destroy(m_shield);

                isSpawned = false;
                m_shieldtime = 0.0;
            }
        }
    }
    
    // Print health
    public string PrintHealth()
    {
        return m_text.text = m_health + " / " + m_maxhealth;
    }
}
