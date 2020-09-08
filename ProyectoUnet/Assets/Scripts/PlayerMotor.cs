using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    public bool auto;
    public bool autoSpawn;
    Vector3 dest = new Vector3(0f, 0.5f, -30f);
    Vector3 dest2 = new Vector3(0f, 0.5f, 30f);
    PlayerShoot shoot;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        shoot = GetComponent<PlayerShoot>();
        autoSpawn = true;
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    public void RotateCamera(float _cameraRotation)
    {
        cameraRotationX = _cameraRotation;
    }

    private void FixedUpdate()
    {
        if (auto) { 
            PerformAutoMovement();
        }
        else {
            PerformMovement();
            PerformRotation();
        }
    }


    //Ejecuta el movimiento en funcion de los valores de velocidad y posicion obtenidos de PlayerController
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }
    //Ejecuta la rotacion en funcion del valor de rotacion obtenido de PlayerController
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        //Si existe una camara anclada rota la camara en funcion del valor de rotacion de esta obtenido de PlayerController
        if (cam != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
    void PerformAutoMovement()
    {
        float step = 4 * Time.deltaTime;


        if (Vector3.Distance(rb.position, dest) < 0.1f)
            if (Vector3.Distance(dest, dest2) != 0)
            {
                dest = dest2;
                shoot.spawnBallsAuto = false;
            }
            else
            {
                dest = new Vector3(0f, 0.5f, -30f);
                if(autoSpawn)
                    shoot.spawnBallsAuto = true;
                if (GetComponent<NetworkIdentity>().netId.ToString() != "2")
                    shoot.CmdPlayerShot("Player 2", 10);
            }

        rb.position = Vector3.MoveTowards(rb.position, dest, step);
        cam.transform.LookAt(dest);
    }


}
