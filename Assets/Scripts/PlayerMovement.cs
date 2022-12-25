using System;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(CharacterController), typeof(Player))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform cameraProxy;

        [SerializeField] private float gravity;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float jumpHeight;

        private float gravityAcceleration;
        private float moveSpeed;
        private float jumpSpeed;

        private bool[] inputs;
        private float yVelocity;

        private void OnValidate()
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            if (player == null) GetComponent<Player>();
            Initialize();
        }

        private void Start()
        {
            Initialize();
            inputs = new bool[6];
        }

        private void Initialize()
        {
            gravityAcceleration = gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            moveSpeed = movementSpeed * Time.fixedDeltaTime;
            jumpSpeed = Mathf.Sqrt(jumpHeight * -2f * gravityAcceleration);
        }

        private void Move(Vector2 inputDirection, bool jump, bool sprint)
        {
            var moveDirection = Vector3
                .Normalize(cameraProxy.right * inputDirection.x + 
                           FlattenVector3(Vector3.Normalize(cameraProxy.forward)) * inputDirection.y);

            if (sprint) moveDirection *= 2f;

            if (controller.isGrounded)
            {
                yVelocity = 0f;
                if (jump)
                    yVelocity = jumpSpeed;
            }

            yVelocity += gravityAcceleration;
            moveDirection.y = yVelocity;
            controller.Move(moveDirection);
        }

        private Vector3 FlattenVector3(Vector3 vector3)
        {
            vector3.y = 0;
            return vector3;
        }
    }
}