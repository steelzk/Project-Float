using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;

public class SPlayerController : NetworkBehaviour
{
    public string PlayerName;
    public SteamId PlayerSteamId;

    public float gravityScale = 20f;
    public float groundFriction = 6f;

    public float moveSpeed = 7.0f;
    public float shiftMultiplier = 0.5f;
    public float runAcceleration = 14f;
    public float runDeacceleration = 10f;
    public float airAcceleration = 2f;
    public float airDecceleration = 2f;
    public float airControl = 0.3f;
    public float sideStrafeAcceleration = 50f;
    public float sideStrafeSpeed = 1f;
    public float jumpSpeed = 8f;
    public bool autoJump = true;

    public float mouseSensitivity;
    [SerializeField] private Camera playerView;

    private float xRot, yRot;

    private CharacterController cc;
    [SyncVar] private Vector3 playerVelocity;
    bool wishJump = false;
    float playerFriction = 0f;

    private void OnEnable()
    {
        cc = GetComponent<CharacterController>();
    }

    public override void OnStartAuthority()
    {
        base.OnStartLocalPlayer();

        transform.GetComponentInChildren<Camera>().enabled = true;
        LockCursor();
    }

    private void Update()
    {
        if (!hasAuthority || !NetworkClient.active) return;

        LookMovement();
        // get user input
        Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        wishDir = transform.TransformDirection(wishDir);
        QueueJump(); // jump input

        // calculate velocity
        if (cc.isGrounded)
            GroundMovement(wishDir);
        else
            AirMovement(wishDir);

        cc.Move(playerVelocity * Time.deltaTime);
    }

    public void LockCursor(bool doLock = true)
    {
        Cursor.lockState = doLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !doLock;
    }

    private void LookMovement()
    {
        // camera rotation
        xRot -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        yRot += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;

        // clamp rotation
        if (xRot < -90)
            xRot = -90;
        else if (xRot > 90)
            xRot = 90;

        // rotate clients camera
        playerView.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    private void GroundMovement(Vector3 wishDir)
    {
        if (!cc.isGrounded) return;

        if (!wishJump)
            ApplyFriction(1f);
        else
            ApplyFriction(0);

        float wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            wishSpeed *= shiftMultiplier;

        Accelerate(wishDir, wishSpeed, runAcceleration);

        // reset gravity
        playerVelocity.y = -gravityScale * Time.deltaTime;

        if (wishJump)
        {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    private void AirMovement(Vector3 wishDir)
    {
        if (cc.isGrounded) return;

        float wishVel = airAcceleration;
        float accel;

        float wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        // CPM: AirControl
        float wishSpeed2 = wishSpeed;
        if (Vector3.Dot(playerVelocity, wishDir) < 0)
        {
            accel = airDecceleration;
        }
        else
        {
            accel = airAcceleration;
        }

        // if strafing left or right
        if (wishDir.y == 0 && wishDir.x != 0)
        {
            if (wishSpeed > sideStrafeSpeed)
                wishSpeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }

        // accelerate
        Accelerate(wishDir, wishSpeed, accel);

        if (airControl > 0)
        {
            AirControl(wishDir, wishSpeed2);
        }

        // apply gravity
        playerVelocity.y -= gravityScale * Time.deltaTime;
    }

    private void AirControl(Vector3 wishDir, float wishspeed)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if (Mathf.Abs(wishDir.y) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;
        zspeed = playerVelocity.y;
        playerVelocity.y = 0;

        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishDir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishDir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishDir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishDir.z * k;

            playerVelocity.Normalize();
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed;
        playerVelocity.z *= speed;
    }

    private void ApplyFriction(float t)
    {
        Vector3 vec = playerVelocity;
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        // Only if the player is on the ground then apply friction 
        if (cc.isGrounded)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * groundFriction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        playerFriction = newspeed;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(playerVelocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;
    }

    private void QueueJump()
    {
        // jump input
        if (autoJump)
        {
            wishJump = Input.GetButton("Jump");
            return;
        }

        if (Input.GetButtonDown("Jump") && !wishJump)
            wishJump = true;
        else if (Input.GetButtonUp("Jump"))
            wishJump = false;
    }
}
