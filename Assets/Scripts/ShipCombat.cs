using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShipCombat : MonoBehaviour
{
	//General Variables
	[SerializeField] float Distance;
	Vector2 mInput = Vector2.zero;
	Transform mTrans;
	GameShip mStats;
	public Cannon[] mCannons;
	public Mortar[] motars;
	public Camera camera;
	[SerializeField]
	MSCameraController CameraController;
	public Vector2 input { get { return mInput; } set { mInput = value; } }
	ShipMovement ship;
	WaitForSeconds waitfor1second = new WaitForSeconds(1f);

	int WeaponSelected = 1;
	[SerializeField] TextMeshProUGUI WeaponSelectedText;

	//Varialbes for Cannos
	[SerializeField] public GameObject rightCanonsParticle;
	[SerializeField] public GameObject leftCanonsParticle;
	[SerializeField] GameObject[] rightCanons;
	[SerializeField] GameObject[] leftCanons;
	float RightCannonmRechargeTime = 0f;
	float LeftCannonmRechargeTime = 0f;
	public float RightCannonrechargeTime = 3f;
	public float LeftCannonrechargeTime = 3f;

	public bool cannonsFired = false;
	public bool RightcannonsFired = false;
	public bool LeftcannonsFired = false;
	public bool MortorFired = false;
	public bool OilBarrelsFired = false;
	//Varialbes for FireBarrels
	public float FireBarrelrechargeTime = 3f;
	float FireBarrelmRechargeTime = 0f;
	public GameObject FireBarrelPrefab;
	[SerializeField] Transform[] FireBarrelLocations;

	//Varialbes for Guns
	[SerializeField] GameObject GunHUD;

	//Varialbes for Mortar
	public Camera Mortarcamera;
	[SerializeField] public GameObject MortarParticles;
	public float MortarrechargeTime = 3f;
	float MortarmRechargeTime = 0f;

	[SerializeField] GameObject MiniMap;
	public int GetWeaponSelected()
    {
		return WeaponSelected;

	}
	void Start()
	{
		cannonsFired = false;
		WeaponSelected = 1;
		mTrans = gameObject.transform.GetChild(0).transform;
		mStats = GetComponent<GameShip>();
		//rightCanonsParticles = GameObject.FindGameObjectsWithTag("RightCannonParticle");
		//leftCanonsParticles = GameObject.FindGameObjectsWithTag("LeftCannonParticle");
		mCannons = GetComponentsInChildren<Cannon>();
		motars = GetComponentsInChildren<Mortar>();
		rightCanons = GameObject.FindGameObjectsWithTag("RightCannon");
		leftCanons = GameObject.FindGameObjectsWithTag("LeftCannon");
		camera = Camera.main;
		ship = gameObject.GetComponentInChildren<ShipMovement>();
		WeaponSelectedText.text = "Cannonballs";

	}

	// Update is called once per frame
	void Update()
	{
		
		if (ship.Playerdead == false)
		{

			WeaponSelection();
			CanUseWeaopn();
			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (WeaponSelected == 1)
				{
					FireCannons();
				}
				else if (WeaponSelected == 2)
				{
					FireCannons();

				}
				else if (WeaponSelected == 3)
				{
					LuanchFireBarrels();
				}
				else if (WeaponSelected == 5)
				{
					StartCoroutine("FireMotar");
				}

			}
			CameraController.weaponselected = WeaponSelected;
			if(CameraController.index!=0)
            {
				MiniMap.SetActive(false);

			}
			else
            {

				MiniMap.SetActive(true);
			}
		}
		else
		{
			CameraController.weaponselected = 0;

		}


	}
	bool CanUseWeaopn()
    {
		float time = Time.time;
		if (WeaponSelected == 2 || WeaponSelected == 1)
		{
			time = Time.time;
			if (RightCannonmRechargeTime < time || LeftCannonmRechargeTime < time)
			{
				if(WeaponSelected==1)
					WeaponSelectedText.text = "Cannonballs";
				else
					WeaponSelectedText.text = "Chain-Cannonballs";
				foreach (Cannon cannon in mCannons)
				{
					cannon.gameObject.SetActive(true);
				}
				return true;
			}
		}
		else if (WeaponSelected == 3)
		{
			if (FireBarrelmRechargeTime < time)
			{
				WeaponSelectedText.text = "FireBarrels";
				return true;
			}
		}
		else if(WeaponSelected==4)
        {
			return true;
        }
		else if (WeaponSelected == 5)
		{
			time = Time.time;
			if (MortarmRechargeTime < time)
			{
				WeaponSelectedText.text = "Mortar";
				return true;
			}
		}
		if(!WeaponSelectedText.text.Contains("Loading.."))
			WeaponSelectedText.text += "\n Loading..";
		return false;
	}
	IEnumerator FireMotar()
	{
		if (motars != null)
		{
			float time = Time.time;
			if (MortarmRechargeTime < time)
			{
				MortorFired = true;
				MortarmRechargeTime = time + MortarrechargeTime;
				Ray r = Mortarcamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				int layerMask = 1 << 8;
				layerMask = ~layerMask;
				//bool first = true;
				if (Physics.Raycast(r, out hit, 200f, layerMask))
				{
					Debug.Log("Ray" + hit.point);

				}
				MortarParticles.SetActive(true);
				if (motars != null)
				{

					foreach (Mortar mortar in motars)
					{
						mortar.Fire(hit.point);
					}
				}
				yield return waitfor1second;
				MortarParticles.SetActive(false);
				
			}
		}
	}
	void WeaponSelection()
    {
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			WeaponSelected = 1;
			WeaponSelectedText.text = "Cannonballs";
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{

			WeaponSelected = 2;
			WeaponSelectedText.text = "Chain-Cannonballs";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			
			WeaponSelected = 3;
			WeaponSelectedText.text = "FireBarrels";
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{

			WeaponSelected = 4;
			WeaponSelectedText.text = "Gun";
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{

			WeaponSelected = 5;
			WeaponSelectedText.text = "Mortar";
		}
	}
	void LuanchFireBarrels()
    {
		if (FireBarrelPrefab != null)
		{
			// Instantiate the prefab
			float time = Time.time;
			if (FireBarrelmRechargeTime < time)
			{
				OilBarrelsFired = true;
				FireBarrelmRechargeTime = time + FireBarrelrechargeTime;
				Instantiate(FireBarrelPrefab, FireBarrelLocations[0].position, FireBarrelLocations[0].rotation);
				Instantiate(FireBarrelPrefab, FireBarrelLocations[1].position, FireBarrelLocations[1].rotation);
			}
		}
	}

	IEnumerator RightCanonsFire(Vector3 dir, float distance)
	{

		rightCanonsParticle.SetActive(true);
		if (rightCanons != null)
        {
			RightcannonsFired = true;
			foreach (GameObject c in rightCanons)
            {
                Cannon cannon = c.GetComponent<Cannon>();
                cannon.Fire(dir, distance, WeaponSelected);
            }
        }
		yield return waitfor1second;
		rightCanonsParticle.SetActive(false);	

	}
	IEnumerator LeftCanonsFire(Vector3 dir, float distance)
	{
		
		leftCanonsParticle.SetActive(true);

		if (leftCanons != null)
        {
			LeftcannonsFired = true;
			foreach (GameObject c in leftCanons)
            {
                Cannon cannon = c.GetComponent<Cannon>();
                cannon.Fire(dir, distance,WeaponSelected);
            }
        }
		yield return waitfor1second;
		leftCanonsParticle.SetActive(false);
		
	}
	public int GetCameraIndex()
    {
		return CameraController.index;

	}
	void FireCannons()
    {
		if (mCannons != null )
		{
			
			Vector3 left = Vector3.left;
			Vector3 right = Vector3.right;
            left.y = 0f;
            right.y = 0f;
            left.Normalize();
            right.Normalize();
			float maxRange = 1f;
            if (CameraController.index==1)
            {
				//foreach (GameObject c in rightCanons)
				//{
				//    Cannon cannon = c.GetComponent<Cannon>();
				//    float range = cannon.maxRange;
				//    if (range > maxRange) maxRange = range;
				//}
				//Distance = maxRange * 0.25f;
				float time = Time.time;
				if (RightCannonmRechargeTime < time)
				{
					RightCannonmRechargeTime = time + RightCannonrechargeTime;
					StartCoroutine(RightCanonsFire(right, Distance));
				}
			}
			else if(CameraController.index == 2)
			{
				//foreach (GameObject c in leftCanons)
				//{
				//    Cannon cannon = c.GetComponent<Cannon>();
				//    float range = cannon.maxRange;
				//    if (range > maxRange) maxRange = range;
				//}
				//Distance = maxRange * 0.25f;
				float time = Time.time;
				if (LeftCannonmRechargeTime < time)
				{
					LeftCannonmRechargeTime = time + LeftCannonrechargeTime;
					StartCoroutine(LeftCanonsFire(left, Distance));
				}
			}
			else
            {
				//foreach (Cannon cannon in mCannons)
				//{
				//    float range = cannon.maxRange;
				//    if (range > maxRange) maxRange = range;
				//}
				//Distance = maxRange * 0.25f;
				//if (target != null)
				//{
				//	right = target.transform.position - mTrans.position;
				//	distance = right.magnitude;
				//	if (distance > 0f) dir *= 1.0f / distance;
				//	else distance = maxRange;
				//	Debug.Log(right);
				//}
				float time = Time.time;
				if (LeftCannonmRechargeTime < time)
				{
					LeftCannonmRechargeTime = time + LeftCannonrechargeTime;
					StartCoroutine(LeftCanonsFire(left, Distance));
				}
				if (RightCannonmRechargeTime < time)
				{
					RightCannonmRechargeTime = time + RightCannonrechargeTime;
					StartCoroutine(RightCanonsFire(right, Distance));
				}
				cannonsFired = true;
			}
			
		}
	}
	
}
