using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    bool hasMine;
    bool hasSword;
    bool hasTornado;
    bool hasFireball;

    bool doLerp;
    bool canDash;
    public bool isDashing;
    bool walkPlaying;

    bool canMine;
    bool canMove;
    bool canSword;
    bool canTornado;
    bool canFireball;
    
    float endLag;
    public float lerpSpeed;
    float dashingTime;
    float rotationSpeed;
    float swordHitFrame;
    float tornadoHitFrame;

    float dashCD;
    float mineCD;
    float swordCD;
    float tornadoCD;
    float fireballCD;

    Vector2 move;
    Vector2 rotation;

    PlayerInput playerInput;
    InputAction _move;
    InputAction _dash;
    InputAction _mine;
    InputAction _sword;
    InputAction _tornado;
    InputAction _fireball;

    Animator animator;
    Rigidbody2D player;
    HealthBar healthBar;

    public float moveSpeed;
    public float fireballSpeed;

    [Header("Dash")]
    [SerializeField] float dashTime;
    [SerializeField] float dashDistance;
    [SerializeField] float dashCoolDown;
    [SerializeField] float coefDecelDash;

    [Header("Cooldowns")]
    public float mineCooldown;
    public float swordCooldown;
    public float tornadoCooldown;
    public float fireballCooldown;

    [Header("Game Objects")]
    public GameObject mine;
    public GameObject sword;
    public GameObject center;
    public GameObject tornado;
    public GameObject fireball;

    [Header("Icons")]
    [SerializeField] Image swordIcon;
    [SerializeField] Image fireballIcon;
    [SerializeField] Image shockwaveIcon;
    [SerializeField] Image mineIcon;

    [Header("Sounds")]
    public AudioSource hit;
    public AudioSource dash;
    public AudioSource walk;
    public AudioSource mineFX;
    public AudioSource slime;
    public AudioSource death;
    public AudioSource sword0;
    public AudioSource sword1;
    public AudioSource sword2;
    public AudioSource tornadoFX;
    public AudioSource fireballFX;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Rigidbody2D>();
        healthBar = GetComponent<HealthBar>();
        playerInput = GetComponent<PlayerInput>();

        hasMine = false;
        hasSword = false;
        hasTornado = false;
        hasFireball = false;

        moveSpeed = 5f;
        rotationSpeed = 9000f;
        swordHitFrame = 0f;
        tornadoHitFrame = 0f;

        swordCD = 0f;
        tornadoCD = 0f;
        fireballCD = 0f;

        #region Input Actions
        _move = playerInput.actions.FindAction("Move");
        _move.performed += ctx => move = ctx.ReadValue<Vector2>();
        _move.canceled += _ => move = Vector2.zero;

        _dash = playerInput.actions.FindAction("Dash");
        _dash.performed += _ => Dash();

        _mine = playerInput.actions.FindAction("Mine");
        _mine.performed += _ => Mine();

        _sword = playerInput.actions.FindAction("Sword");
        _sword.performed += _ => Sword();

        _tornado = playerInput.actions.FindAction("Tornado");
        _tornado.performed += _ => Tornado();

        _fireball = playerInput.actions.FindAction("Fireball");
        _fireball.performed += _ => Fireball();
        #endregion
    }

    void Update()
    {
        endLag -= Time.deltaTime;
        dashCD -= Time.deltaTime;
        mineCD -= Time.deltaTime;
        swordCD -= Time.deltaTime;
        tornadoCD -= Time.deltaTime;
        fireballCD -= Time.deltaTime;
        dashingTime -= Time.deltaTime;
        swordHitFrame -= Time.deltaTime;
        tornadoHitFrame -= Time.deltaTime;

        SetUIIconCooldown();

        if (!isDashing && lerpSpeed < 0.1f)
            rotation = move;

        if (dashCD <= 0) canDash = true;
        if (mineCD <= 0) canMine = true;
        if (swordCD <= 0) canSword = true;
        if (tornadoCD <= 0) canTornado = true;
        if (fireballCD <= 0) canFireball = true;
        if (!canMove && endLag <= 0) canMove = true;
        if (lerpSpeed > 0f) lerpSpeed -= Time.deltaTime * 5f;
        if (sword.activeSelf && swordHitFrame <= 0) sword.SetActive(false);
        if (tornado.activeSelf && tornadoHitFrame <= 0) tornado.SetActive(false);

        SetPlayerSprite();

        if (dashingTime <= 0)
        {
            isDashing = false;
            if (doLerp)
            {
                doLerp = false;
                lerpSpeed = 1f;
            }
        }

        if (move.magnitude > 0)
        {
            if (!walkPlaying)
            {
                walk.Play();
                walkPlaying = true;
            }
        }
        else
        {
            walk.Stop();
            walkPlaying = false;
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {   
            if (!isDashing)
            {
                SetVelocity(moveSpeed);
                if(player.velocity.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(transform.forward, move);
                    Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    center.transform.rotation = rotation;
                }
            }
        }
        else
            player.velocity = Vector2.zero;

        if (isDashing)
            SetVelocity(dashDistance / dashTime);
        else if (lerpSpeed > 0f)
            SetVelocity(Mathf.Lerp(moveSpeed, (dashDistance / dashTime), lerpSpeed));
        else if (player.velocity.magnitude > moveSpeed)
            SetVelocity(moveSpeed);
    }

    private void SetVelocity(float speed)
    {
        player.velocity = new Vector2(rotation.x * speed, rotation.y * speed);
    }

    private void SetUIIconCooldown()
    {
        if (hasFireball) fireballIcon.fillAmount = fireballCD / fireballCooldown;
        else fireballIcon.fillAmount = 1;

        if (hasSword) swordIcon.fillAmount = swordCD / swordCooldown;
        else swordIcon.fillAmount = 1;

        if (hasMine) mineIcon.fillAmount = mineCD / mineCooldown;
        else mineIcon.fillAmount = 1;

        if (hasTornado) shockwaveIcon.fillAmount = tornadoCD / tornadoCooldown;
        else shockwaveIcon.fillAmount = 1;
    }

    void SetPlayerSprite()
    {
        if (move.magnitude > 0)
            animator.SetBool("IsMoving", true);
        else
            animator.SetBool("IsMoving", false);

        if (move.y < -0.1f)
            animator.SetBool("Front", true);
        else if (move.y > 0.1f)
            animator.SetBool("Front", false);

        if (move.x < -0.1f)
            animator.SetTrigger("Left");
        else if (move.x > 0.1f)
            animator.SetTrigger("Right");
    }

    public void Dash()
    {
        if (canDash && move != Vector2.zero)
        {
            doLerp = true;
            canDash = false;
            isDashing = true;
            dashCD = dashCoolDown;
            dashingTime = dashTime;

            SetVelocity(dashDistance / dashTime);

            dash.Play();
        }
    }

    public void Mine()
    {
        if (hasMine && canMine)
        {
            mineCD = mineCooldown;
            canMove = canMine = false;
            Instantiate(mine, gameObject.transform.position, Quaternion.identity);

            mineFX.Play();
        }
    }

    public void Sword()
    {
        if (hasSword && canSword)
        {
            endLag = 0.2f;
            swordHitFrame = 0.15f;
            swordCD = swordCooldown;
            canMove = canSword = false;

            sword.SetActive(true);

            int rnd = Random.Range(0, 3);
            if (rnd == 0)
                sword0.Play();
            else if (rnd == 1)
                sword1.Play();
            else 
                sword2.Play();
        }
    }

    public void Tornado()
    {
        if (hasTornado && canTornado)
        {
            endLag = 0.5f;
            tornadoHitFrame = 0.3f;
            tornadoCD = tornadoCooldown;
            canMove = canTornado = false;

            tornado.SetActive(true);
            tornadoFX.Play();
        }
    }

    public void Fireball()
    {
        if (hasFireball && canFireball)
        {
            endLag = 0.2f;
            fireballCD = fireballCooldown;
            canMove = canFireball = false;

            GameObject newFireball = Instantiate(fireball, gameObject.transform.position, Quaternion.identity);
            newFireball.GetComponent<Rigidbody2D>().velocity = center.transform.up * fireballSpeed;
            newFireball.transform.rotation = Quaternion.Euler(center.transform.rotation.eulerAngles - Vector3.forward * 180f);

            fireballFX.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Slime"))
        {
            int oldHP = healthBar.maxHealth;
            healthBar.maxHealth = (int)((float)healthBar.maxHealth * 1.1f);
            healthBar.currentHealth += healthBar.maxHealth - oldHP;
            slime.Play();
        }
        else if (collider.gameObject.CompareTag("Mine"))
        {
            if (hasMine)
            {
                mine.GetComponent<SwordHitBox>().damage = (int)((float)mine.GetComponent<SwordHitBox>().damage * 1.25f);
                mineCooldown *= 0.75f;
            }
            else
                mine.GetComponent<SwordHitBox>().damage = 30;

            int oldHP = healthBar.maxHealth;
            healthBar.maxHealth = (int)((float)healthBar.maxHealth * 1.1f);
            healthBar.currentHealth += healthBar.maxHealth - oldHP;

            slime.Play();
            hasMine = true;
            Destroy(collider.gameObject);
        }
        else if (collider.gameObject.CompareTag("Sword"))
        {
            if (hasSword)
            {
                sword.GetComponent<SwordHitBox>().damage = (int)((float)sword.GetComponent<SwordHitBox>().damage * 1.25f);
                swordCooldown *= 0.75f;
            }
            else
                sword.GetComponent<SwordHitBox>().damage = 10;

            int oldHP = healthBar.maxHealth;
            healthBar.maxHealth = (int)((float)healthBar.maxHealth * 1.1f);
            healthBar.currentHealth += healthBar.maxHealth - oldHP;

            slime.Play();
            hasSword = true;
            Destroy(collider.gameObject);
        }
        else if (collider.gameObject.CompareTag("Tornado"))
        {
            if (hasTornado)
            {
                tornado.GetComponent<SwordHitBox>().damage = (int)((float)tornado.GetComponent<SwordHitBox>().damage * 1.25f);
                tornadoCooldown *= 0.75f;
            }
            else
                tornado.GetComponent<SwordHitBox>().damage = 70;

            int oldHP = healthBar.maxHealth;
            healthBar.maxHealth = (int)((float)healthBar.maxHealth * 1.1f);
            healthBar.currentHealth += healthBar.maxHealth - oldHP;

            slime.Play();
            hasTornado = true;
            Destroy(collider.gameObject);
        }
        else if (collider.gameObject.CompareTag("Fireball"))
        {
            if (hasFireball)
            {
                fireball.GetComponent<SwordHitBox>().damage = (int)((float)fireball.GetComponent<SwordHitBox>().damage * 1.25f);
                fireballCooldown *= 0.75f;
            }
            else
                fireball.GetComponent<SwordHitBox>().damage = 25;

            int oldHP = healthBar.maxHealth;
            healthBar.maxHealth = (int)((float)healthBar.maxHealth * 1.1f);
            healthBar.currentHealth += healthBar.maxHealth - oldHP;

            slime.Play();
            hasFireball = true;
            Destroy(collider.gameObject);
        }
    }
}