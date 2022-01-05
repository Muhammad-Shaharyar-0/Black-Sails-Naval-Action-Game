using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Game/Cannonball")]
public class Cannonball : MonoBehaviour
{
	// How much damage is applied on hit
	public float damage = 5f;

	// Particle emitter that will be creating a smoke trail behind the cannonball
	public GameObject Smoke;
	ParticleSystem[] Smokes;
	// Cannonballs will be destroyed after this much time passes
	public float maxLifetime = 4f;

	// Smoke will stop being produced by the smoke emitter after this amount of time
	public float smokeCutoffTime = 1f;

	// Object (ship, tower) that fired this cannon ball
	[HideInInspector] public GameObject owner;

	// Cache some values
	Rigidbody mRb;
	float mSpawnTime = 0f;

	void Start ()
	{

        mRb = gameObject.GetComponent<Rigidbody>();
        //mSpawnTime = Time.time;
        ////Smokes = Smoke.GetComponentsInChildren<ParticleSystem>();
        ////foreach (ParticleSystem smokeEmitter in Smokes)
        ////{
        ////	smokeEmitter.Play();
        ////}
        damage =damage * PlayerPrefs.GetFloat("Damage", 1);
	}

    /// <summary>
    /// Smoke should start at 100% and taper off to nothing over the course of 'smokeCutoffTime'.
    /// </summary>

    //void Update ()
    //{
    //	float lifetime = Time.time - mSpawnTime;

    //       // Destroy the cannonballs once their lifetime expires
    //       if (lifetime > maxLifetime) Destroy(gameObject);
    //}

    /// <summary>
    /// Going below water should increase drag significantly.
    /// </summary>

    //void fixedupdate()
    //{
    //    Vector3 pos = mRb.position;
    //    if (pos.y < 0f) mRb.drag = 7f;
    //}
}