using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
	private PlayerCollisionDetection coll;
	[HideInInspector]
	public Rigidbody2D rb;
	

	[Space]
	[Header("Stats")]
	[SerializeField]
	private float speed = 10;
	[SerializeField]
	private float jumpForce = 50;
	[SerializeField]
	private float slideSpeed = 5;

	private bool wallSlide;


	private bool groundTouch;
	Animator anim;
	SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
	{
		coll = GetComponent<PlayerCollisionDetection>();
		rb = GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
		spriteRenderer = this.GetComponent<SpriteRenderer>();	

    }

    // Update is called once per frame
    void Update()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		Vector2 dir = new Vector2(x, y);

		Walk(dir);
		

		if (coll.GetPlayerOnGround() )
		{
			PlayerBetterJumping _bestjump = GetComponent<PlayerBetterJumping>();
			if(_bestjump != null)
				_bestjump.enabled = true;
		}

		
		

		if (coll.GetPlayerOnWall() && !coll.GetPlayerOnGround())
		{
			if (x != 0 && this.rb.velocity.y <= 0)
			{
				wallSlide = true;
				WallSlide();
			}
		}

		if (!coll.GetPlayerOnWall() || coll.GetPlayerOnGround())
			wallSlide = false;

		if (Input.GetButtonDown("Jump"))
		{
			if (coll.GetPlayerOnGround())
				Jump(Vector2.up, false);
		}

		if (coll.GetPlayerOnGround() && !groundTouch)
		{
			groundTouch = true;
		}

		if (!coll.GetPlayerOnGround() && groundTouch)
		{
			groundTouch = false;
		}

		if (anim != null && anim.enabled)
		{
			if (rb.velocity.y > 0.2f && !anim.GetBool("Jump") && !groundTouch)
			{
				anim.SetBool("Jump", true);
				anim.SetBool("Fall", false);
				anim.SetBool("Run", false);
				anim.SetBool("Idle", false);
			}
			else if (rb.velocity.y < -0.2f && !anim.GetBool("Fall") && !groundTouch)
			{
				anim.SetBool("Jump", false);
				anim.SetBool("Fall", true);
				anim.SetBool("Run", false);
				anim.SetBool("Idle", false);
			}
			else if (rb.velocity.x != 0 && !anim.GetBool("Run") && groundTouch)
			{

				anim.SetBool("Run", true);
				anim.SetBool("Jump", false);
				anim.SetBool("Fall", false);
				anim.SetBool("Idle", false);
			}
			else if (!anim.GetBool("Idle") && groundTouch)
			{
				anim.SetBool("Jump", false);
				anim.SetBool("Fall", false);
				anim.SetBool("Run", false);
				anim.SetBool("Idle", true);
			}

			if (spriteRenderer != null)
			{
                if (dir.x > 0)
				{
					spriteRenderer.flipX = false;
				}
				else if(dir.x < 0)
				{
                    spriteRenderer.flipX = true;
                }

            }
		}

		if (wallSlide)
		return;
	}

	private void WallSlide()
	{
		bool pushingWall = false;
		if ((rb.velocity.x > 0 && coll.GetPlayerOnRightWall()) || (rb.velocity.x < 0 && coll.GetPlayerOnLeftWall()))
		{
			pushingWall = true;
		}
		float push = pushingWall ? 0 : rb.velocity.x;

		rb.velocity = new Vector2(push, -slideSpeed);
	}

	private void Walk(Vector2 dir)
	{
		rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
       
    }

	private void Jump(Vector2 dir, bool wall)
	{
		rb.velocity = new Vector2(rb.velocity.x, 0);
		rb.velocity += dir * jumpForce;
       
    }
}
