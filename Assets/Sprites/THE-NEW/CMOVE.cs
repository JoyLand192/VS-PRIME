using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMOVE : MonoBehaviour
{
    public KeyCode left = KeyCode.LeftArrow;
    public KeyCode right = KeyCode.RightArrow;
    public KeyCode up = KeyCode.UpArrow;

    
    public float defspeed, speed, jump, dbjump; //defspeed = 기본 속도
    float xmove, ymove;


    public Animator anm, dbjanm;

    public bool jumping, doublejumping, land;

    public RaycastHit2D landray; //플랫폼 충돌 감지
    
    public BoxCollider2D box; //히트박스 세부설정

    public Transform dbj;
    Rigidbody2D body;
    bool dbjp, dbjfollowing;
    Vector2 visible = new Vector2(1, 1);
    Vector2 invisible = new Vector2(0, 0);

    AnimatorStateInfo dbjstate;

    void Start() 
    {
        body = this.GetComponent<Rigidbody2D>();
        anm = this.GetComponentInChildren<Animator>();
        body.freezeRotation = true;
        box = this.GetComponent<BoxCollider2D>();
        dbjanm = dbj.GetComponent<Animator>();
        
        defspeed = 12;
    }
    
    void Update()
    {   
        if (Input.GetKey(left)){
            this.transform.localScale = new Vector2(1,1);
            box.offset = new Vector2(0.07f, box.offset.y);
            box.size = new Vector2(1.3f, box.size.y);
            speed = defspeed * -1;
        } else if (Input.GetKey(right)){
            this.transform.localScale = new Vector2(-1,1);
            box.offset = new Vector2(-0.67f, box.offset.y);
            box.size = new Vector2(1.3f, box.size.y);
            speed = defspeed;
        } else {    
            speed = 0;
            box.size = new Vector2(0.7f, 1.9f);
            if (this.transform.localScale.x > 0) { 
                box.offset = new Vector2(0.37f, box.offset.y);
            } else {
                box.offset = new Vector2(-0.33f, box.offset.y);
            }
        }

        if (Input.GetKeyDown(up))
        {
            if (!anm.GetBool("FLOATING")){
                jumping = true;
            } else if (!anm.GetBool("FALLING") || !anm.GetBool("DOUBLEJUMP")) {
                doublejumping = true;
                dbjanm.SetTrigger("VISIBLE");
            }
        }

        if (land){
            anm.ResetTrigger("FLOATING");
            anm.ResetTrigger("FALLING");
            anm.ResetTrigger("DOUBLEJUMP");
        }

        if (box != null) {
            Debug.Log("박스 확인됨");
        }

        dbjstate = dbjanm.GetCurrentAnimatorStateInfo(0);
    }
    void DBJP(){
        dbjanm.ResetTrigger("VISIBLE");
        dbjfollowing = true;
    }
    void FixedUpdate()
    {
        
        if (jumping) 
        {
            body.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            jumping = false;
            anm.SetTrigger("FLOATING");
        }

        if (doublejumping) 
        {
            body.AddForce(Vector2.up * dbjump, ForceMode2D.Impulse);
            doublejumping = false;
            anm.SetTrigger("DOUBLEJUMP");
            dbjanm.ResetTrigger("VISIBLE");
            DBJP();
        }

        if (dbjfollowing){
            dbj.transform.position = new Vector2(this.transform.position.x, this.transform.position.y-0.5f);
            dbj.transform.localScale = visible;
            dbjfollowing = false;  
        }

        if (dbjstate.IsName("not jumping")){
            dbj.localScale = invisible;
        }


        landray = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, LayerMask.GetMask("land"));

        if (landray.collider != null)
        {
            Debug.Log(landray.collider.name);
            land = true;
            anm.ResetTrigger("FLOATING");
            anm.ResetTrigger("FALLING");
            anm.ResetTrigger("DOUBLEJUMP");
        } else {
            land = false;
            Debug.Log("!! 콜라이더 인식 불가");
            if (!jumping && !doublejumping) {
                anm.SetTrigger("FALLING");
            }
        }

        xmove = speed;

        this.body.velocity = (new Vector2(xmove, body.velocity.y));

        Debug.DrawRay(body.position, Vector2.down, new Color(0, 1, 0));
    }
    
}
