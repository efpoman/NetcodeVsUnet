using UnityEngine;

//[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    Player player;
 

    private PlayerMotor motor;
    
    void Start()
    {
        player = GetComponent<Player>();
        motor = GetComponent<PlayerMotor>();
     
    }
    //Recoge el input y cambia el valor de posicion y rotacion para ejecutar el movimiento en PlayerMotor
    private void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;
        //Almacena el valor de velocidad
        if(player.GetHealth()>0)
        motor.Move(velocity);


        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
        //Almacena el valor de la rotacion del cliente
        if (player.GetHealth() > 0)
            motor.Rotate(rotation);

        
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * lookSensitivity;
        //Almacena el valor de la rotacion de la camara
        motor.RotateCamera(cameraRotationX);

     
    }
   
}
