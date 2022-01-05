using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

public class ShipMovement : MonoBehaviour
{
    [Tooltip("In m/s")] [SerializeField] float xSpeed = 50f;
    [Tooltip("In m/s")] [SerializeField] float ySpeed = 50f;
    [Tooltip("In m/s")] [SerializeField] float SpeedforTranslation = 35f;
    public bool Playerdead = false;
    int sails=0;
    float xThrow;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform trans;

    [SerializeField] float upperlimit= -35.8f;
    [SerializeField] float lowerlimit= -38.5f;
    [SerializeField] float xboundry = -38.5f;
    [SerializeField] float zboundry = -38.5f;

    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trans = GetComponent<Transform>();
        Playerdead = false;
        velocity = rb.velocity;
        
    }
    public float ReturnSpeed()
    {
        float factor = SpeedforTranslation * sails;
        if (sails == 2)
        {
            factor = 55f;
        }
        return factor;
    }
    // Update is called once per frame
    void Update()
    {
        if (Playerdead == false)
        {
            if (trans.localPosition.y > upperlimit)
            {
                trans.localPosition = new Vector3(trans.localPosition.x, upperlimit, trans.localPosition.z);
            }
            if (trans.localPosition.y < lowerlimit)
            {
                trans.localPosition = new Vector3(trans.localPosition.x, lowerlimit, trans.localPosition.z);
            }
            if (trans.localPosition.x > xboundry)
            {
                trans.localPosition = new Vector3(xboundry, trans.localPosition.y, trans.localPosition.z);
            }
            if (trans.localPosition.z> zboundry)
            {
                trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, zboundry);
            }
            if (trans.localPosition.x < -xboundry)
            {
                trans.localPosition = new Vector3(-xboundry, trans.localPosition.y, trans.localPosition.z);
            }
            if (trans.localPosition.z < -zboundry)
            {
                trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, -zboundry);
            }
            UpdateInput();
        }
        
    }
    void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            sails++;
            if (sails == 3)
                sails = 2;

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            sails--;
            if (sails < 0)
                sails = 0;
        }

        xThrow = Input.GetAxis("Horizontal");
    }
    void MoveForward()
    {
        float factor = SpeedforTranslation * sails;
        if (sails == 2)
        {
            factor = 55f;
        }
        transform.Translate(Vector3.forward * factor * Time.deltaTime);
      
    }
    private void FixedUpdate()
    {
        MoveForward();
        Rotation();
    }
    public float ReturnRotationSpeed()
    {
        float rotationspeed = xSpeed;
        if (sails == 0)
        {
            rotationspeed = xSpeed * 1.55f;
        }
        else if (sails == 1)
        {
            rotationspeed = xSpeed * 1.3f;
        }
        return rotationspeed;
    }
    public float Getsails()
    {
  
        return sails;
    }
    void Rotation()
    {
        //if (velocity != rb.velocity)
        //{
        //     rb.velocity = (transform.forward * sails) * ySpeed * Time.fixedDeltaTime;
        //}

        float rotationspeed = xSpeed;
        if(sails==0)
        {
            rotationspeed = xSpeed * 1.55f;
        }
        else if(sails==1)
        {
            rotationspeed = xSpeed * 1.3f;
        }
        rb.angularVelocity = Vector3.zero;
        rb.AddTorque(transform.up * rotationspeed * xThrow * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    

}
