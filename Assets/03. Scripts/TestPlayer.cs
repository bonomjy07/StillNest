using System;
using KinematicCharacterController.Examples;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
	public ExampleCharacterController character;

	private const string MouseXInput = "Mouse X";
	private const string MouseYInput = "Mouse Y";
	private const string MouseScrollInput = "Mouse ScrollWheel";
	private const string HorizontalInput = "Horizontal";
	private const string VerticalInput = "Vertical";

	private void Start()
	{
		// not gonna use camera yet
		if (character)
		{
			character.OrientationMethod = OrientationMethod.TowardsMovement;
		}
	}

	private void Update()
	{
		HandleCharacterInput();
	}

	private void HandleCharacterInput()
	{
		PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

		// Build the CharacterInputs struct
		characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
		characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
		// characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
		characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
		characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
		characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

		// Apply inputs to character
		character.SetInputs(ref characterInputs);
	}
}
