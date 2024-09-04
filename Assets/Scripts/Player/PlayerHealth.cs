using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool IsDead {  get; private set; }

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;
    [SerializeField] private AudioClip HitSoundClip;
    [SerializeField] private AudioClip DeathSoundClip;
    [SerializeField] private float HitSoundVolume = 0.25f;
    [SerializeField] private float DeathSoundVolume = 0.25f;

    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private bool canTakeDamage = true;

    private Knockback knockback;
    private Flash flash;
    private Slider healthSlider;

    const string HEALTH_SLIDER_TEXT = "Health Slider";
    const string TOWN_TEXT = "Town";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
        flash = GetComponent<Flash>();
    }

    private void Start()
    {
        IsDead = false;
        currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if(enemy)
        {
            TakeDamage(1, other.transform);
        }
    }

    public void HealPlayer()
    {
        if(currentHealth < maxHealth)
        {
            currentHealth += 1;
            UpdateHealthSlider();
        }
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) { return; }


        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());
        SFXManager.instance.PlaySFXClip(HitSoundClip, transform, HitSoundVolume);

        canTakeDamage = false;
        currentHealth -= damageAmount;
        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfPlayerDeath();
    }

    private void CheckIfPlayerDeath()
    {
        if(currentHealth <= 0 && !IsDead)
        {
            IsDead = true;
            Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }

    private IEnumerator DeathLoadSceneRoutine()
    {
        spriteRenderer.color = Color.red;
        SFXManager.instance.PlaySFXClip(DeathSoundClip, transform, DeathSoundVolume);
        yield return new WaitForSeconds(3f);

        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the scene loaded event

        Destroy(gameObject);
        Stamina.Instance.ReplenishStaminaOnDeath();
        EconomyManager.Instance.ResetCurrentGold();
        //spriteRenderer.color = Color.white;
        SceneManager.LoadScene(TOWN_TEXT);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the new player object and ensure the camera follows it
        CameraController.Instance.SetPlayerCameraFollow();
        // Unsubscribe from the event so it doesn't trigger again unnecessarily
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void UpdateHealthSlider()
    {
        if(healthSlider == null)
        {
            healthSlider = GameObject.Find(HEALTH_SLIDER_TEXT).GetComponent<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}