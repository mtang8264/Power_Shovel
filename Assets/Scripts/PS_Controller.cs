using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PS_Controller : MonoBehaviour
{
    Transform cab_transform;
    Transform arm0_transform;
    Transform arm1_transform;
    Transform shovel_transform;

    [Header("Cab")]
    [SerializeField]
    [Range(-1,1)]
    float cab_rotation;
    float cab_rotation_speed = 1;
    float current_cab_speed = 0;
    
    [Header("Arm Segment 0")]
    [SerializeField]
    [Range(0,1)]
    float arm0_extension;
    float arm0_extension_speed = .5f;
    Quaternion arm0_extension_min = new Quaternion((float)0.15053, (float)-0.00020, (float)0.00009, (float)0.98861);
    Quaternion arm0_extension_max = new Quaternion((float)0.69937, (float)-0.00011, (float)0.00019, (float)0.71476);
    float current_arm0_speed = 0;

    [Header("Arm Segment 1")]
    [SerializeField]
    [Range(0,1)]
    float arm1_extension;
    float arm1_extension_speed = .75f;
    Quaternion arm1_extension_min = new Quaternion((float)-.91203, (float)0.00018, (float)0.00012, (float)0.41012);
    Quaternion arm1_extension_max = new Quaternion((float)-.22124, (float)0.00021, (float)-0.00008, (float)0.97522);
    float current_arm1_speed = 0;

    [Header("Shovel")]
    [SerializeField]
    [Range(0,1)]
    float shovel_extension;
    float shovel_extension_speed = 1.3f;
    Quaternion shovel_extension_min = new Quaternion((float).78137, (float)0, (float)0, (float)0.62406);
    Quaternion shovel_extension_max = new Quaternion((float).18199, (float)0, (float)0, (float)0.98330);
    float current_shovel_speed = 0;

    [Header("Movement")]
    float move_speed = 2;

    // Start is called before the first frame update
    void Start() {
        cab_transform = GameObject.Find("Cab").transform;
        arm0_transform = GameObject.Find("Arm_0").transform;
        arm1_transform = GameObject.Find("Arm_1").transform;
        shovel_transform = GameObject.Find("Shovel").transform;
    }

    // Update is called once per frame
    void Update() {
        UpdateCabRotation();
        UpdateArm0Extension();
        UpdateArm1Extension();
        UpdateShovelExtension();

        Movement();
    }

    #region Input Actions
    public void InputAction_CabRotate(InputAction.CallbackContext context) {
        current_cab_speed = context.action.ReadValue<float>();
    }

    public void InputAction_Arm0Extension(InputAction.CallbackContext context) {
        current_arm0_speed = context.action.ReadValue<float>();
    }

    public void InputAction_Arm1Extension(InputAction.CallbackContext context) {
        current_arm1_speed = context.action.ReadValue<float>();
    }

    public void InputAction_ShovelExtension(InputAction.CallbackContext context) {
        current_shovel_speed = context.action.ReadValue<float>();
    }
    #endregion

    void Movement() {
        float l = Gamepad.current.leftStick.ReadValue().y;
        float r = Gamepad.current.rightStick.ReadValue().y;
        float dl = l * move_speed * Time.deltaTime;
        float dr = r * move_speed * Time.deltaTime;

        Vector3 center = transform.position;
        Vector3 leftPoint = transform.position - transform.right * 1.2f;
        Vector3 rightPoint = transform.position + transform.right * 1.2f;

        Vector3 targetLeftPoint = leftPoint + transform.forward * dl;
        Vector3 targetRightPoint = rightPoint + transform.forward * dr;
        Vector3 targetCenter = Vector3.Lerp(targetLeftPoint, targetRightPoint, 0.5f);

        Vector3 newRight = targetRightPoint - targetCenter;
        newRight = Vector3.Normalize(newRight);

        Vector3 cross = Vector3.Cross(newRight, transform.up);

        transform.position = targetCenter;
        transform.LookAt(transform.position + cross, transform.up);
    }

    void UpdateCabRotation() {
        cab_rotation += cab_rotation_speed * Time.deltaTime * current_cab_speed;
        
        // Clamp cab rotation
        while (cab_rotation > 1) cab_rotation -= 2;
        while (cab_rotation < -1) cab_rotation += 2;
        // Determine angle
        float a = Mathf.Lerp(-180,180,(cab_rotation+1)/2);
        // Move model
        Vector3 r = cab_transform.localEulerAngles;
        r.y = a;
        cab_transform.localRotation = Quaternion.Euler(r);
    }

    void UpdateArm0Extension() {
        arm0_extension += current_arm0_speed * arm0_extension_speed * Time.deltaTime;

        // Clamp
        arm0_extension = Mathf.Clamp(arm0_extension,0,1);
        // Determine rotation
        Quaternion r = Quaternion.Lerp(arm0_extension_min, arm0_extension_max, arm0_extension);
        // Apply rotation
        arm0_transform.localRotation = r;
    }

    void UpdateArm1Extension() {
        arm1_extension += current_arm1_speed * arm1_extension_speed * Time.deltaTime;
        
        // Clamp
        arm1_extension = Mathf.Clamp(arm1_extension,0,1);
        // Determine rotation
        Quaternion r = Quaternion.Lerp(arm1_extension_min, arm1_extension_max, arm1_extension);
        // Apply rotation
        arm1_transform.localRotation = r;
    }

    void UpdateShovelExtension() {
        shovel_extension += current_shovel_speed * shovel_extension_speed * Time.deltaTime;

        // Clamp
        shovel_extension = Mathf.Clamp(shovel_extension,0,1);
        // Determine rotation
        Quaternion r = Quaternion.Lerp(shovel_extension_min, shovel_extension_max, shovel_extension);
        // Apply rotation
        shovel_transform.localRotation = r;
    }
}
