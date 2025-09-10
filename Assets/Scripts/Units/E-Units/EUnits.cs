using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EUnits : MonoBehaviour
{
    protected Vector2 frozen = new Vector2(0, 0);
    protected EffectManager effectManager;
    public GameObject damageTextCanvas;

    [Header("몬스터 스탯")]
    protected float maxHP;
    public float _hp;
    public float HP
    {
        get
        {
            return _hp;
        }
        set
        {
            if (value > 0) UpdateHP(value);
            else Death();
        }
    }
    public float speed;
    public float jumpPower;
    [Space(30)]

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    public RaycastHit2D Platforms;
    public bool alive, canAct, moving;
    public int move_dir;
    protected List<System.Action> Acts = new List<System.Action>();
    [SerializeField] GameObject healthBarPrefab;
    protected GameObject healthBar, healthBarCanvas;
    protected Image healthBarAmount;

    void Awake()
    {
        Init();
    }
    protected virtual void Init()
    {
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        damageTextCanvas = GameObject.FindWithTag("DamageTextUI");

        healthBarCanvas = GameObject.FindWithTag("HealthBarUI");
        healthBar = Instantiate(healthBarPrefab, healthBarCanvas.transform);
        healthBar.name = $"HealthBar_{gameObject.name}";
        healthBarAmount = healthBar.transform.GetChild(0).GetComponent<Image>();

        HP = maxHP;
        alive = true;
        canAct = true;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); 

        Acts.Add(Move);
        Acts.Add(Move);
        Acts.Add(Move);
        Acts.Add(Idle);
        Acts.Add(Jump);
        StartCoroutine(RandomActRepeat());

        move_dir = -1;
    }
    protected void UpdateHP(float v)
    {
        if (v >= maxHP) _hp = maxHP;
        else _hp = v;

        healthBarAmount.fillAmount = _hp / maxHP;
    }

    protected void Move()
    {
        moving = true;
        move_dir = Random.Range(0, 2) == 1 ? 1 : -1;
        anim.SetTrigger("walk");
        anim.ResetTrigger("jump");
    }

    protected void Idle()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        moving = false;
        anim.ResetTrigger("walk");
    }

    protected void Jump()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        moving = false;
        anim.ResetTrigger("walk");
        anim.SetTrigger("jump");
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    protected void Hurt()
    {
        rb.velocity = new Vector2(0, 0);

        moving = false;

        anim.ResetTrigger("walk");
        anim.ResetTrigger("jump");

        StartCoroutine(GettingHurt());
    }

    IEnumerator GettingHurt()
    {
        anim.SetTrigger("hurt");
        float time = 0f;
        while (time < 0.8f)
        {
            rb.velocity = frozen;
            time += Time.deltaTime;
            yield return null;    
        }
        anim.ResetTrigger("hurt");
    }

    protected IEnumerator RandomActRepeat()
    {
        while (alive)
        {
            yield return new WaitForSeconds(Random.Range(0.8f, 2f));
            if (canAct)
            {
                Acts[Random.Range(0, Acts.Count)].Invoke();
            }
        }
    }

    protected void Update()
    {
        rb.transform.localScale = new Vector3(move_dir * -1, 1, 1);
        healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, -1.2f));
    }

    protected void FixedUpdate()
    {
        Debug.DrawRay(rb.transform.position, Vector2.down * 1.02f, Color.green);
        Platforms = Physics2D.Raycast(rb.transform.position, Vector2.down, 1.02f, LayerMask.GetMask("Platforms"));
        if (Platforms.collider != null)
        {
            anim.ResetTrigger("jump");
        }

        if (moving)
        {
            rb.velocity = new Vector2(speed * move_dir, rb.velocity.y);
        }
    }

    public void GetDamaged(float Damage)
    {
        HP -= Damage;

        GameObject text = Instantiate(effectManager.LoadDamageText(1), damageTextCanvas.transform);
        text.GetComponent<UnitsInteractableUI>().followTarget = transform;
        text.GetComponent<TextMeshProUGUI>().text = Damage.ToString("0");

        Hurt();
    }

    protected void Death()
    {
        healthBarAmount.fillAmount = 0;
        rb.simulated = false;
        anim.SetTrigger("die");
        alive = false;
    }

    public void Remove()
    {
        Destroy(gameObject);
        Destroy(healthBar);
    }
}
