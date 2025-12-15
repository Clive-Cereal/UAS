using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCam;
        
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;          // hrizontal 
    [SerializeField] private float verticalSpeed = 2f;      // vertical
    [SerializeField] private float inertia = 7f;            // inertia 

    [Header("Camera (3rd Person)")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 2f, -4f); // camera offset from player
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Interaction")]
    [SerializeField] private float maxDistance = 10f;

    [Header("Crosshair")]
    [SerializeField] private float crosshairSize = 16f;   // crosshair line length
    [SerializeField] private float crosshairThickness = 2f;
    [SerializeField] private Color crosshairColor = Color.white;

    [Header("Model")]
    [SerializeField] private Transform model;  
    [SerializeField] private Vector3 modelRotationOffset; 

    [Header("Visual Tilt")]
    [SerializeField] private float maxVerticalTilt = 15f;   
    [SerializeField] private float tiltSmooth = 4f; 
   
    private static Texture2D _lineTex;

    private Ray ray;

    private Vector3 currentVelocity = Vector3.zero;
    private float yaw;
    private float pitch;
    private Rigidbody rb;
    void Start()
    {
        if (mainCam == null)
            mainCam = Camera.main;
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false; 
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        // default camera 
        if (mainCam != null)
        {
            Vector3 dir = (mainCam.transform.position - transform.position).normalized;
            // calculate pitch yaw from direction vector
            pitch = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
            yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraOrbit(); // camera orbit around player
        HandleMovement();    // movement
        
        ray = mainCam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable)) //Hover
            {
                if (Input.GetMouseButtonDown(0)) //Click
                {
                    interactable.TryInteract();
                }
            }
        }
    }

    // third person camera orbit around player
    private void HandleCameraOrbit()
    {
        if (mainCam == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        // camera position 
        Vector3 desiredPos = transform.position + rot * cameraOffset;
        mainCam.transform.position = desiredPos;

        // camera look at player
        mainCam.transform.rotation = rot;
    }
    
    // underwater style movement
    private void HandleMovement()
    {
        if (mainCam == null) return;

        float h = Input.GetAxisRaw("Horizontal"); // A / D
        float v = Input.GetAxisRaw("Vertical");   // W / S

        float ascend = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        float descend = Input.GetKey(KeyCode.LeftControl) ? 1f : 0f;

        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = mainCam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // horizontal movement direction
        Vector3 verticalDir = Vector3.up * (ascend - descend);

        Vector3 targetVelocity = moveDir * moveSpeed + verticalDir * verticalSpeed;

        // inertia smoothing
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            Time.deltaTime * inertia
        );

        // apply movement
        // apply movement via Rigidbody so collisions work
        rb.MovePosition(rb.position + currentVelocity * Time.deltaTime);


        // rotate player model to movement direction
        Vector3 flatVel = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        if (flatVel.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatVel.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }

        if (model != null)
        {
            model.localRotation = Quaternion.Euler(modelRotationOffset);
            
        }

            // -------- vertical tilt (visual only) --------
        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.LeftControl)) verticalInput = 1f;
        else if (Input.GetKey(KeyCode.Space)) verticalInput = -1f;


        float targetTilt = verticalInput * maxVerticalTilt;


        float currentX = transform.localEulerAngles.x;
        if (currentX > 180f) currentX -= 360f; 

        float newX = Mathf.Lerp(currentX, targetTilt, Time.deltaTime * tiltSmooth);


        Vector3 euler = transform.localEulerAngles;
        euler.x = newX;
        transform.localEulerAngles = euler;

    }
        void OnGUI()
        {
        if (Event.current.type != EventType.Repaint) return;

         // texture for drawing lines
        if (_lineTex == null)
        {
            _lineTex = new Texture2D(1, 1);
            _lineTex.SetPixel(0, 0, Color.white);
            _lineTex.Apply();
        }

    // center of the screen
    float cx = Screen.width * 0.5f;
    float cy = Screen.height * 0.5f;

    float size = crosshairSize;
    float thick = crosshairThickness;

    Color oldColor = GUI.color;
    GUI.color = crosshairColor;

    // gap
    float gap = 2f; // size\

    //left horizontal line
    GUI.DrawTexture(
        new Rect(cx - gap - size, cy - thick * 0.5f, size, thick),
        _lineTex);

    // right horizontal line
    GUI.DrawTexture(
        new Rect(cx + gap, cy - thick * 0.5f, size, thick),
        _lineTex);

    // upper vertical line
    GUI.DrawTexture(
        new Rect(cx - thick * 0.5f, cy - gap - size, thick, size),
        _lineTex);

    // lower vertical line
    GUI.DrawTexture(
        new Rect(cx - thick * 0.5f, cy + gap, thick, size),
        _lineTex);

    // colors reset
    GUI.color = oldColor;
    }


}
