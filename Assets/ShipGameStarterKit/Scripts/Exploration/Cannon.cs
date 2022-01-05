using UnityEngine;

[AddComponentMenu("Exploration/Cannon")]
public class Cannon : MonoBehaviour
{
	// Cannonball prefab
	public GameObject cannonballPrefab;
	public GameObject chain_cannonballPrefab;
	// Initial velocity applied to the cannon ball's rigidbody
	public float initialVelocity = 250f;
	public float ChainCannoninitialVelocity = 1750f;
	public float CannoninitialVelocity = 500f;
	// Maximum pitch that can be applied to each shot
	public float maxPitch = 25f;

	// Maximum angle at which the cannon is able to fire
	public float maxYaw = 45f;

	// The firing direction will have this much deviation in degrees
	public float maxAimDeviationAngle = 5f;

	// Maximum possible delay that the cannon will fire after pressing the 'fire' button
	public float reactionTime = 0.2f;

	// How long it takes for the cannon to recharge
	public float rechargeTime = 2f;

	Transform mTrans;
	GameShip mStats;
	float mFireTime = 0f;
	float mRechargeTime = 0f;
	Collider[] mColliders;
	Vector3 mFiringDir;
	float mFiringPitch = 0f;
	float mMaxRange = 1f;
	int WeaponSelected = 1;

	GameObject CannonBall;
	GameObject Chain_CannonBall;
	/// <summary>
	/// Calculated maximum range of the cannon based on max pitch and initial velocity.
	/// </summary>

	public float maxRange { get { return mMaxRange; } }

	/// <summary>
	/// Helper function that calculates the cannon's maximum firing range.
	/// </summary>

	float CalculateMaxRange ()
	{
		// Vertical velocity can be calculated using the pitch and initial full velocity:
		float velocity = Mathf.Sin(Mathf.Deg2Rad * maxPitch) * initialVelocity;

		// This is how long it will take the fired cannon ball to reach the sea level
		float time = -velocity / (0.5f * Physics.gravity.y);

		// Now let's calculate the distance traveled horizontally in the same amount of time
		return Mathf.Cos(Mathf.Deg2Rad * maxPitch) * initialVelocity * time;
	}

	/// <summary>
	/// Cache the transform and the ship controlling this cannon.
	/// </summary>

	void Start ()
	{
		mTrans = transform;
		mStats = GameShip.Find(mTrans);

		// Calculate the cannon's maximum range
		mMaxRange = CalculateMaxRange();

		if (mStats != null)
		{
			// Ship stats found -- use it as root node
			mColliders = mStats.GetComponentsInChildren<Collider>();
		}
		else
		{
			// No ship stats present -- see if there is a rigidbody that can be used as root
			Rigidbody rb = Tools.GetRigidbody(mTrans);
			mColliders = (rb != null) ? rb.GetComponentsInChildren<Collider>() : GetComponentsInChildren<Collider>();
		}
		WeaponSelected = 1;
		//CannonBall= Instantiate(cannonballPrefab, mTrans.position, mTrans.rotation) as GameObject;
		//CannonBall.SetActive(false);
		//Chain_CannonBall = Instantiate(chain_cannonballPrefab, mTrans.position, mTrans.rotation) as GameObject;
		//Chain_CannonBall.SetActive(false);
	}

	/// <summary>
	/// Fire the cannon when ready.
	/// </summary>

	void Update()
	{
		if(WeaponSelected==1)
        {
			initialVelocity = CannoninitialVelocity;
			maxYaw = 5;
        }
		else if(WeaponSelected==2)
        {
			initialVelocity = ChainCannoninitialVelocity;
			maxYaw = 60;
		}
		float time = Time.time;

		// We're ready to fire -- fire the cannon
		if (mFireTime != 0f && mFireTime <= time)
		{
			// Recharge time varies from 80% to 120% of the intended duration just to add variety
			mRechargeTime = time + rechargeTime;//* Mathf.Lerp(0.8f, 1.2f, Random.value);
			mFireTime = 0f;

			// Create the cannon ball
			if (cannonballPrefab != null)
			{
				// Instantiate the prefab
				GameObject go = null;
				if (WeaponSelected==1)
                {
					//CannonBall.SetActive(true);
					go = Instantiate(cannonballPrefab, mTrans.position, mTrans.rotation) as GameObject;
				}
				else
                {
					//Chain_CannonBall.SetActive(true);
					go= Instantiate(chain_cannonballPrefab, mTrans.position, mTrans.rotation) as GameObject;
				}
				

				// Ensure that the newly instantiated object's collider won't collide with our colliders
				if (mColliders != null)
				{
					Collider col = go.GetComponent<Collider>();

					if (col != null)
					{
						foreach (Collider c in mColliders)
						{
							Physics.IgnoreCollision(c, col);
						}
					}
				}



				// Rigidbody is generally expected to be present
				Rigidbody rb = go.GetComponent<Rigidbody>();

				if (rb != null)
				{
					Vector2 deviation = Vector2.zero;

					// Deviate the aim a little
					if (maxAimDeviationAngle > 0f)
					{
						deviation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
						deviation.Normalize();
						deviation *= maxAimDeviationAngle;
					}

					// Calculate the initial velocity
					Quaternion firingDir = Quaternion.LookRotation(mFiringDir);
					Vector3 vel = (firingDir * Quaternion.Euler(-mFiringPitch + deviation.x, deviation.y, 0f)) *
						Vector3.forward * initialVelocity;

					// NOTE: For physics-accurate results we should append the ship's velocity as well,
					// but this makes the auto-aiming logic fail unless the ship isn't moving.
					//rb.velocity = vel;
					rb.AddForce(vel, ForceMode.Impulse);
					gameObject.SetActive(false);
				}
				else
				{
					Debug.LogWarning("The cannon ball is missing its rigidbody");
				}
			}
		}
	}

	/// <summary>
	/// Start the firing process.
	/// </summary>

	public void Fire (Vector3 dir, float distance,int weapon)
	{
		float time = Time.time;
		WeaponSelected = weapon;
		//if (mRechargeTime < time && mFireTime == 0f)
		if (mFireTime == 0f)
		{
			Vector3 cannonDir = mTrans.rotation * Vector3.forward;
			mFireTime = time + Random.value * reactionTime;
			mFiringDir = cannonDir;
			mFiringPitch = Mathf.Clamp01(distance / mMaxRange) * maxPitch;
			
		}
	}
}