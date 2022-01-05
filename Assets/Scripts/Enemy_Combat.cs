using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy_Combat : MonoBehaviour
{       //General Variables
    public GameObject Player;
    [SerializeField] float Distance;
    [SerializeField] GameObject RightAim;
    [SerializeField] GameObject LeftAim;
    Vector2 mInput = Vector2.zero;
    Transform mTrans;
    GameShip mStats;
    public Cannon[] mCannons;
    public Mortar[] motars;
    public Vector2 input { get { return mInput; } set { mInput = value; } }

    Enemy_health ship_health;
    int WeaponSelected = 1;
    WaitForSeconds waitfor1second = new WaitForSeconds(1f);
    //Varialbes for Cannos
    [SerializeField] public GameObject rightCanonsParticles;
    [SerializeField] public GameObject leftCanonsParticles;
    [SerializeField] List<GameObject> rightCanons;
    [SerializeField] List<GameObject> leftCanons;
    float RightCannonmRechargeTime = 0f;
    float LeftCannonmRechargeTime = 0f;
    public float RightCannonrechargeTime = 3f;
    public float LeftCannonrechargeTime = 3f;


    //Varialbes for FireBarrels
    public float FireBarrelrechargeTime = 3f;
    float FireBarrelmRechargeTime = 0f;
    public GameObject FireBarrelPrefab;
    [SerializeField] Transform[] FireBarrelLocations;


    //Varialbes for Mortar
    public Camera Mortarcamera;
    [SerializeField] public GameObject MortarParticles;
    public float MortarrechargeTime = 3f;
    float MortarmRechargeTime = 0f;

    void Start()
    {
        WeaponSelected = 1;
        //Player = GameObject.FindGameObjectWithTag("Player");
        mTrans = gameObject.transform.GetChild(0).transform;
        mStats = GetComponent<GameShip>();
        //rightCanonsParticles = GameObject.FindGameObjectsWithTag("RightCannonParticle");
        //leftCanonsParticles = GameObject.FindGameObjectsWithTag("LeftCannonParticle");
        mCannons = GetComponentsInChildren<Cannon>();
        motars = GetComponentsInChildren<Mortar>();
        rightCanons = new List<GameObject>(GameObject.FindGameObjectsWithTag("RightCannonE")).FindAll(g => g.transform.IsChildOf(this.transform));
        leftCanons = new List<GameObject>(GameObject.FindGameObjectsWithTag("LeftCannonE")).FindAll(g => g.transform.IsChildOf(this.transform));
        ship_health = gameObject.GetComponent<Enemy_health>();
        NavMeshAgent agent = gameObject.GetComponentInParent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ship_health.EnemyDead())
        {

            //WeaponSelection();
            if (CanUseWeaopn() || PlayerInRange())
            {
                if (WeaponSelected == 1)
                {

                    //FireCannons();
                }
                else if (WeaponSelected == 2)
                {
                    FireCannons();

                }
                else if (WeaponSelected == 3)
                {
                    LuanchFireBarrels();
                }
                else if (WeaponSelected == 4)
                {

                }
                else if (WeaponSelected == 5)
                {
                    FireMotar();
                }

            }
        }
    }
    bool PlayerInRange()
    {
        return true;
    }
    bool CanUseWeaopn()
    {
        float time = Time.time;
        if (WeaponSelected == 2 || WeaponSelected == 1)
        {
            time = Time.time;
            if (RightCannonmRechargeTime < time || LeftCannonmRechargeTime < time)
            {
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
                return true;
            }
        }
        else if (WeaponSelected == 4)
        {
            return true;
        }
        else if (WeaponSelected == 5)
        {
            time = Time.time;
            if (MortarmRechargeTime < time)
            {

                return true;
            }
        }
        return false;
    }
    public void FireMotar1()
    {
        StartCoroutine("FireMotar");
    }
    IEnumerator FireMotar()
    {
        if (motars != null)
        {
            float time = Time.time;
            if (MortarmRechargeTime < time)
            {
                MortarmRechargeTime = time + MortarrechargeTime;

                MortarParticles.SetActive(true);
                if (motars != null)
                {

                    foreach (Mortar mortar in motars)
                    {
                        mortar.Fire(Player.transform.position);
                    }
                }

                yield return waitfor1second;
                MortarParticles.SetActive(false);
            }
        }
    }
    public void LuanchFireBarrels()
    {
        if (FireBarrelPrefab != null)
        {
            // Instantiate the prefab
            float time = Time.time;
            if (FireBarrelmRechargeTime < time)
            {
                FireBarrelmRechargeTime = time + FireBarrelrechargeTime;
                Instantiate(FireBarrelPrefab, FireBarrelLocations[0].position, FireBarrelLocations[0].rotation);
                Instantiate(FireBarrelPrefab, FireBarrelLocations[1].position, FireBarrelLocations[1].rotation);
            }
        }
    }

    public IEnumerator RightCanonsFire(Vector3 dir, float distance)
    {

        float time = Time.time;
        if (RightCannonmRechargeTime < time)
        {
            RightCannonmRechargeTime = time + RightCannonrechargeTime;
            rightCanonsParticles.SetActive(true);
            if (rightCanons != null)
            {

                foreach (GameObject c in rightCanons)
                {
                    Cannon cannon = c.GetComponent<Cannon>();
                    cannon.Fire(dir, distance, WeaponSelected);
                }
            }
            yield return waitfor1second;
            rightCanonsParticles.SetActive(false);
        }

    }
    public IEnumerator LeftCanonsFire(Vector3 dir, float distance)
    {

        float time = Time.time;
        if (LeftCannonmRechargeTime < time)
        {
            LeftCannonmRechargeTime = time + LeftCannonrechargeTime;
            leftCanonsParticles.SetActive(true);
            if (leftCanons != null)
            {

                foreach (GameObject c in leftCanons)
                {
                    Cannon cannon = c.GetComponent<Cannon>();
                    cannon.Fire(dir, distance, WeaponSelected);
                }
            }
            yield return waitfor1second;
            leftCanonsParticles.SetActive(false);
        }
    }
    public void FireCannons()
    {
        if (mCannons != null)
        {
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            left.y = 0f;
            right.y = 0f;
            left.Normalize();
            right.Normalize();

            float maxRange = 1f;
            RaycastHit hit;


            int layersToIgnore = ~LayerMask.GetMask("Ignore", "Projectile");
            if (Physics.Raycast(RightAim.transform.position, RightAim.transform.forward, out hit, 700f, layersToIgnore))
            {

                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    //foreach (GameObject c in rightCanons)
                    //{
                    //    Cannon cannon = c.GetComponent<Cannon>();
                    //    float range = cannon.maxRange;
                    //    if (range > maxRange) maxRange = range;
                    //}
                    //float distance = maxRange * 0.25f;

                    // If a unit was found, override the direction and angle

                    StartCoroutine(RightCanonsFire(right, Distance));
                }

            }

            if (Physics.Raycast(LeftAim.transform.position, LeftAim.transform.forward, out hit, 700f, layersToIgnore))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    //foreach (GameObject c in leftCanons)
                    //{
                    //    Cannon cannon = c.GetComponent<Cannon>();
                    //    float range = cannon.maxRange;
                    //    if (range > maxRange) maxRange = range;
                    //}
                    //float distance = maxRange * 0.25f;
                    StartCoroutine(LeftCanonsFire(left, Distance));

                  
                }

            }


        }
    }
}
