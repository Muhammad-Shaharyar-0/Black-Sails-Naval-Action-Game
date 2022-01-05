﻿using UnityEngine;

[AddComponentMenu("Exploration/Ship Orbit")]
public class ShipOrbit : MonoBehaviour
{
	public ShipController control;
	public float sensitivity = 1f;
	public Vector2 horizontalTiltRange = new Vector2(-20f, 20f);

	Transform mTrans;
	Vector2 mInput;
	Vector2 mOffset;

	void Start ()
	{
		mTrans = transform;
	}

	void Update ()
	{
		if (control != null)
		{
			float multiplier = Time.deltaTime * sensitivity;
			bool mouseInput = Input.GetMouseButton(0);

			// Automatically show the cursor
			if (!Application.isEditor && Input.GetMouseButtonUp(0)) Cursor.visible = true;

			if (mouseInput)
			{
				// Mouse input
				mInput.x = Input.GetAxis("Mouse X");
				mInput.y = Input.GetAxis("Mouse Y");
				multiplier *= 300f;
			}
			else
			{
				// Joystick input
			//	mInput.x = Input.GetAxis("View X");
				//mInput.y = Input.GetAxis("View Y");
				multiplier *= 75f;
			}

			if (mouseInput || mInput.sqrMagnitude > 0.001f)
			{
				mOffset.x += mInput.x * multiplier;
				mOffset.y += mInput.y * multiplier;

				// Limit the angles
				mOffset.x = Tools.WrapAngle(mOffset.x);
				mOffset.y = Mathf.Clamp(mOffset.y, horizontalTiltRange.x, horizontalTiltRange.y);

				// Automatically hide the cursor
				if (mouseInput && !Application.isEditor && mOffset.magnitude > 10f) Cursor.visible = false;
			}
			else if (Mathf.Abs(mOffset.x) < 35f)
			{
				// No key pressed and the camera has not been moved much -- slowly interpolate the offset back to 0
				float factor = Time.deltaTime * control.speed * 4f;
				mOffset.x = Mathf.Lerp(mOffset.x, 0f, factor);
				mOffset.y = Mathf.Lerp(mOffset.y, 0f, factor);
			}

			// Calculate the rotation and wrap it around
			Quaternion targetRot = Quaternion.Euler(-mOffset.y, mOffset.x, 0f);

			// Interpolate the rotation for smoother results
			mTrans.localRotation = Quaternion.Slerp(mTrans.localRotation,
				targetRot, Mathf.Clamp01(Time.deltaTime * 10f));
		}
	}
}