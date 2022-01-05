using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



static class WeaponSpaceTooltip
{
    public const string
        m_inputFire = "Key or button used to fire weapon.",
        m_inputAutoFire = "Key or button that is held down for automatic firing. Often the same as Input Auto Fire.",
        m_inputReload = "Key or button used to reload weapon.",
        m_inputSwitch = "Key or button used to switch weapon.",
        m_inputAim = "Key or button used to aim weapon.",
        m_inputRun = "Key or button that is held down to run.",

        m_MouseXInfluenceName = "Name of axis specified in Input Manager for left and right mouse movements.",
        m_MouseYInfluenceName = "Name of axis specified in Input Manager for up and down mouse movements.",

        m_WeaponCollection = "WeaponCollection to use on this character.",

        m_CameraRaySpawn = "Camera used to project firing ray via its Z axis.",
        m_FPSPlayer = "FPS player character in the scene.",

        m_UICrosshairSpace = "UI image GameObject used to display crosshair sprites.",
        m_UIMagAmmoCount = "UI text GameObject used to display weapon ammo count.",
        m_UITotalAmmoCount = "UI text GameObject used to display total ammo count.",
        m_UIAmmoIconSpace = "UI image GameObject used to display ammo icon sprites.";
}

public class WeaponSpace : MonoBehaviour
{
    [SerializeField] CharacterController m_CharacterController;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_WeaponCollection)] WeaponCollection m_WeaponCollection;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_CameraRaySpawn)] Transform m_CameraRaySpawn;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_FPSPlayer)] Transform m_FPSPlayer;

    [Header("Input Keys")]
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputFire)] KeyCode m_inputFire;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputAutoFire)] KeyCode m_inputAutoFire;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputReload)] KeyCode m_inputReload;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputSwitch)] KeyCode m_inputSwitch;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputAim)] KeyCode m_inputAim;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputRun)] KeyCode m_inputRun;

    [Header("Mouse Influence Axes")]
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_MouseXInfluenceName)] string m_MouseXInfluenceName;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_MouseYInfluenceName)] string m_MouseYInfluenceName;

    [Header("UI Objects")]
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UICrosshairSpace)] Image m_UICrosshairSpace;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UIMagAmmoCount)] Text m_UIMagAmmoCount;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UITotalAmmoCount)] Text m_UITotalAmmoCount;
    [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UIAmmoIconSpace)] Image m_UIAmmoIconSpace;

    [Header("Layer Names")]
    [SerializeField] string m_FirstPersonLayerName = "FirstPerson";
    [SerializeField] string m_ThirdPersonLayerName = "ThirdPerson";
    [SerializeField] string m_ProjectileLayerName = "Projectile";


    Transform m_ProjectileSpawn;
    Transform m_BarrelFlashSpawn;
    Transform m_EjectedCartridgeSpawn;

    Animator m_Animator;

    int m_SelectedWeapon;
    int m_LastSelectedWeapon;

    // Updated during runtime, thus should not belong in Weapon scriptable object (for initialisation only)
    bool[] m_IsWeaponEnabled;
    int[] m_CurrentCapacity;
    float[] m_TimeUntillNextRound;

    Dictionary<string, int> m_AmmoAmounts = new Dictionary<string, int>();

    int m_BurstRoundsCount;

    int m_WeaponsLength;  

    // Ad-hoc transition timings to ensure certain actions are only avaliable after a defined duration
    // Useful for syncing mechanics with animations
    float m_CurrentAimingTime;
    float m_CurrentRunningRecoveryTime;
    float m_CurrentPReloadInterruptionTime;

    bool m_IsReloading;
    bool m_IsSwitching;
    bool m_IsCurrentWeaponDisabled;

    bool m_IsJumping;
    bool m_IsRunning;
    bool m_IsWalking;
        
    Coroutine m_ReloadWeaponCoroutine;

    GameObject m_InstantiatedWeaponObject;

    Vector2 m_CurrentMouseInfluance;

    // ?  for resolving wanring
    bool m_IsWeaponLoadedAtStart = false;      

    // Controls
    bool m_IsInputFire;
    bool m_IsInputAutoFire;
    bool m_IsInputReload;
    bool m_IsInputSwitch;
    bool m_IsInputAim;
    bool m_IsInputRun;

    private bool GunFired = false;

    
    void Awake()
    {
        // ?    need component checks? 
        m_Animator = GetComponent<Animator>();

        m_WeaponsLength = m_WeaponCollection.weapons.Count;

        m_IsWeaponEnabled = new bool[m_WeaponsLength];
        m_CurrentCapacity = new int[m_WeaponsLength];
        m_TimeUntillNextRound = new float[m_WeaponsLength];

        for (int i = 0; i < m_WeaponsLength; i++)
        {
            if (!m_AmmoAmounts.ContainsKey(m_WeaponCollection.weapons[i].ammo.name))
            {
                m_AmmoAmounts.Add(m_WeaponCollection.weapons[i].ammo.name, m_WeaponCollection.weapons[i].ammo.startAmount);
            }
        }

        for (int i = 0; i < m_WeaponsLength; i++)
        {
            if (m_WeaponCollection.weapons[i].enableOnStart)
            {
                if (m_WeaponCollection.weapons[i].loadedAtStart)
                {
                    EnableWeapon(i, true, m_WeaponCollection.weapons[i].capacity);
                }
                else
                {
                    EnableWeapon(i, true, 0);
                }
            }
        }

    }

    void Start()
    {
        GunFired = false;
        for (int i = 0; i < m_WeaponsLength; i++)
        {
            if (m_WeaponCollection.weapons[i].enableOnStart)
            {
                m_SelectedWeapon = i;
                i = m_WeaponsLength;
            }
        }

        // Removes all child object of FPSWeaponSystems
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

    }
    public bool GetGunFired()
    {
        return GunFired;
    }
    void Update()
    {
        CheckInputs();

        // Prepares defalt weapon on start          // ? put this in start()?
        if (!m_IsWeaponLoadedAtStart)
        {
            LoadSelectedWeapon();
            m_IsWeaponLoadedAtStart = true;
        }

        CheckCharacterMovement();
        CheckMouseMovement();

        CheckWeaponFire();
        CheckWeaponAim();
        CheckWeaponSwitch();
        CheckWeaponReload();
        UpdateHUD();
    }

    void CheckWeaponFire()
    {
        // Decrement time untill next round for all weapons
        for (int i = 0; i < m_TimeUntillNextRound.Length; i++)
        {
            if (m_TimeUntillNextRound[i] > 0f)
            {
                m_TimeUntillNextRound[i] -= 1f * Time.deltaTime;
            }
            else
            {
                m_TimeUntillNextRound[i] = 0f;
            }  
        }

        // Return firing animation
        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].fireAnimationVarName, false);

        // Check trigger press
        if (m_WeaponCollection.weapons[m_SelectedWeapon].firingType == Weapon.FiringType.Semi && m_IsInputFire || m_BurstRoundsCount < m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst)
        { }
        else if (m_WeaponCollection.weapons[m_SelectedWeapon].firingType == Weapon.FiringType.Auto && m_IsInputAutoFire || m_BurstRoundsCount < m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst)
        { }
        else
        {
            return;
        }

        // Check firing restrictions
        if (m_TimeUntillNextRound[m_SelectedWeapon] <= 0f && m_CurrentCapacity[m_SelectedWeapon] >= m_WeaponCollection.weapons[m_SelectedWeapon].ammoLossPerRound && !m_IsReloading && !m_IsSwitching && !m_IsRunning && m_CurrentRunningRecoveryTime <= 0f && m_CurrentPReloadInterruptionTime <= 0f) // ? isRunning needed with currentRunningTransition? & currentPReloadInter..?
        { }
        else
        {
            return;
        }

        Vector3 randomisedSpread;

        // The firing
        for (int i = 1; i <= m_WeaponCollection.weapons[m_SelectedWeapon].outputPerRound; i++)
        {
            // Spread
            if (m_CurrentAimingTime >= 1f)
            {
                randomisedSpread = new Vector3(Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].aimingSpread, m_WeaponCollection.weapons[m_SelectedWeapon].aimingSpread), Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].aimingSpread, m_WeaponCollection.weapons[m_SelectedWeapon].aimingSpread), Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].aimingSpread, m_WeaponCollection.weapons[m_SelectedWeapon].aimingSpread));
            }
            else if (m_IsWalking && m_IsInputAutoFire)
            {
                randomisedSpread = new Vector3(Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].movementSpread, m_WeaponCollection.weapons[m_SelectedWeapon].movementSpread), Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].movementSpread, m_WeaponCollection.weapons[m_SelectedWeapon].movementSpread), Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].movementSpread, m_WeaponCollection.weapons[m_SelectedWeapon].movementSpread));
            }
            else
            {
                randomisedSpread = new Vector3(Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].spread, m_WeaponCollection.weapons[m_SelectedWeapon].spread), Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].spread, m_WeaponCollection.weapons[m_SelectedWeapon].spread), Random.Range(-m_WeaponCollection.weapons[m_SelectedWeapon].spread, m_WeaponCollection.weapons[m_SelectedWeapon].spread));
            }

            // Output type
            if (m_WeaponCollection.weapons[m_SelectedWeapon].outputType == Weapon.OutputType.Ray)
            {
                RaycastHit hit;
                // Needed to ignore multiple layers (FirstPerson, ThirdPerson & Projectile)
                // Raycast's integer parameter behaves like bool array at bit level
                // '~' converts integer to negitive spectrum, thus defines listed layers will be ignored
                int layersToIgnore = ~LayerMask.GetMask(m_FirstPersonLayerName, m_ThirdPersonLayerName, m_ProjectileLayerName,"Water","Ignore");
                GunFired = true;
                // ?    need raycast to ignore collider of FPSController, without ignoring other player's FPSController, may involve RaycastAll
                if (Physics.Raycast(m_CameraRaySpawn.position, m_CameraRaySpawn.forward, out hit, 750f, layersToIgnore))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    Debug.Log(hit.collider.gameObject.tag);
                    if (m_WeaponCollection.weapons[m_SelectedWeapon].rayMode.rayImpact != null)
                    {
                        GameObject hitInstance = Instantiate(m_WeaponCollection.weapons[m_SelectedWeapon].rayMode.rayImpact, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(hitInstance, 0.5f); // ? should be user controllable?
                    }           
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        hit.collider.gameObject.GetComponent<Enemy_health>().Damage(1);
                    }
                    if (hit.collider.gameObject.CompareTag("Enemy2"))
                    {
                        hit.collider.gameObject.GetComponent<Enemy_health>().Damage(1);
                    }
                    if (hit.collider.gameObject.CompareTag("Boss"))
                    {
                        hit.collider.gameObject.GetComponent<Enemy_health>().Damage(1);
                    }
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * m_WeaponCollection.weapons[m_SelectedWeapon].rayMode.impactForce);
                    }
                }
            }
            else if (m_WeaponCollection.weapons[m_SelectedWeapon].outputType == Weapon.OutputType.Projectile)
            {
               
                if (m_WeaponCollection.weapons[m_SelectedWeapon].projectileMode.projectileObject != null)
                {
                    GameObject grenadeInstance = Instantiate(m_WeaponCollection.weapons[m_SelectedWeapon].projectileMode.projectileObject, m_ProjectileSpawn.position, m_ProjectileSpawn.rotation);

                    grenadeInstance.transform.Rotate(90f, 0f, 0f);
                    grenadeInstance.GetComponent<Rigidbody>().AddForce((transform.forward + randomisedSpread) * m_WeaponCollection.weapons[m_SelectedWeapon].projectileMode.launchForce);
                    Destroy(grenadeInstance, 5f); // ? should be user controllable?
                }
            }
        }

        // Barrel flash
        if (m_WeaponCollection.weapons[m_SelectedWeapon].barrelFlash != null)
        {
            GameObject flashInstance = Instantiate(m_WeaponCollection.weapons[m_SelectedWeapon].barrelFlash, m_BarrelFlashSpawn.position, m_BarrelFlashSpawn.rotation);
            Destroy(flashInstance, 0.5f);
        }

        // Sound
        m_BarrelFlashSpawn.GetComponent<AudioSource>().Play();

        // Animation
        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].fireAnimationVarName, true);

        // Carrage ejection
        if (m_WeaponCollection.weapons[m_SelectedWeapon].ejectedCartridge.ejectedObject != null)
        {
            GameObject cartridgeInstance = Instantiate(m_WeaponCollection.weapons[m_SelectedWeapon].ejectedCartridge.ejectedObject, m_EjectedCartridgeSpawn.position, m_EjectedCartridgeSpawn.rotation);
            Destroy(cartridgeInstance, 2.0f);

           // Physics.IgnoreCollision(cartridgeInstance.GetComponent<Collider>(), m_FPSPlayer.GetComponent<Collider>());

            Vector3 ejectionTrajectory = m_EjectedCartridgeSpawn.rotation * m_WeaponCollection.weapons[m_SelectedWeapon].ejectedCartridge.ejectionTrajectory.normalized;

           // cartridgeInstance.GetComponent<Rigidbody>().AddForce((ejectionTrajectory * m_WeaponCollection.weapons[m_SelectedWeapon].ejectedCartridge.ejectionForce) + (m_CharacterController.velocity * 3f));
        }

        // Mange burst count
        m_BurstRoundsCount--;
        if (m_BurstRoundsCount <= 0)
        {
            m_BurstRoundsCount = m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst;
        }

        // After calculations
        m_TimeUntillNextRound[m_SelectedWeapon] = 1f / m_WeaponCollection.weapons[m_SelectedWeapon].fireRate;
        m_CurrentCapacity[m_SelectedWeapon] -= m_WeaponCollection.weapons[m_SelectedWeapon].ammoLossPerRound;
    }

    void CheckWeaponSwitch()
    {
        // Prevent switching while switching & allow switching if current weapon is disabled
        if (!((m_IsInputSwitch || m_IsCurrentWeaponDisabled) && !m_IsSwitching))
        {
            return;
        }

        m_LastSelectedWeapon = m_SelectedWeapon;
        m_SelectedWeapon++;

        if (m_SelectedWeapon >= m_WeaponsLength)
        {
            m_SelectedWeapon = 0;
        }

        // Find next enabled weapon
        while (!m_IsWeaponEnabled[m_SelectedWeapon])
        {
            m_SelectedWeapon++;

            if (m_SelectedWeapon >= m_WeaponsLength)
            {
                m_SelectedWeapon = 0;
            }
        }

        // Abort if no other weapon if found
        if (m_LastSelectedWeapon == m_SelectedWeapon)
        {
            return;
        }

        // End weapon reloading process
        if (m_ReloadWeaponCoroutine != null)
        {
            StopCoroutine(m_ReloadWeaponCoroutine);
        }

        StartCoroutine(WaitSwitchWeapon());

        m_IsReloading = false;
    }

    IEnumerator WaitSwitchWeapon()
    {     
        m_IsSwitching = true;

        GetComponent<AudioSource>().clip = m_WeaponCollection.weapons[m_LastSelectedWeapon].switchOutSound;
        GetComponent<AudioSource>().Play();

        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].switchAnimationVarName, true);

        // Switching out last weapon
        yield return new WaitForSeconds(m_WeaponCollection.weapons[m_SelectedWeapon].switchingTime);

        GetComponent<AudioSource>().clip = m_WeaponCollection.weapons[m_SelectedWeapon].switchInSound;
        GetComponent<AudioSource>().Play();

        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].switchAnimationVarName, false);

        LoadSelectedWeapon();

        // Switching in next weapon
        yield return new WaitForSeconds(m_WeaponCollection.weapons[m_SelectedWeapon].switchingTime);

        m_IsSwitching = false;
        m_IsCurrentWeaponDisabled = false;

    }

    void CheckWeaponReload()
    {

        // Decrement interruption time
        if (m_CurrentPReloadInterruptionTime > 0f)
        {
            m_CurrentPReloadInterruptionTime -= 1f * Time.deltaTime;
        }
        else
        {
            m_CurrentPReloadInterruptionTime = 0f;
        }

        // Stop repeated partical reload on fire input
        if (
            m_IsReloading &&
            (m_WeaponCollection.weapons[m_SelectedWeapon].firingType == Weapon.FiringType.Semi && m_IsInputFire || m_WeaponCollection.weapons[m_SelectedWeapon].firingType == Weapon.FiringType.Auto && m_IsInputAutoFire) &&
            m_WeaponCollection.weapons[m_SelectedWeapon].reloadingType == Weapon.ReloadingType.PartialRepeat && m_CurrentCapacity[m_SelectedWeapon] > 0 // #
            )
        {
            if (m_ReloadWeaponCoroutine != null)
            {
                StopCoroutine(m_ReloadWeaponCoroutine);
            }

            m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].reloadAniamtionVarName, false);
            m_BurstRoundsCount = m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst;

            if (GetComponent<AudioSource>().clip == m_WeaponCollection.weapons[m_SelectedWeapon].reloadSound)
            {
                GetComponent<AudioSource>().Stop();
            }

            m_CurrentPReloadInterruptionTime = m_WeaponCollection.weapons[m_SelectedWeapon].partialReloadInterruptionTime;
            m_IsReloading = false;
        }

        // Start reload coroutines
        if (
            (m_CurrentCapacity[m_SelectedWeapon] < m_WeaponCollection.weapons[m_SelectedWeapon].ammoLossPerRound || (m_CurrentCapacity[m_SelectedWeapon] < m_WeaponCollection.weapons[m_SelectedWeapon].capacity && m_IsInputReload)) &&
            m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] > 0 &&
            !m_IsReloading && !m_IsSwitching && !m_IsRunning
            )
        {
            if (m_WeaponCollection.weapons[m_SelectedWeapon].reloadingType == Weapon.ReloadingType.PartialRepeat)
            {
                m_ReloadWeaponCoroutine = StartCoroutine(WaitReloadWeaponRepeat());
            }
            else
            {
                m_ReloadWeaponCoroutine = StartCoroutine(WaitReloadWeapon());
            } 
        }
    }

    IEnumerator WaitReloadWeapon()
    {
        m_IsReloading = true;

        GetComponent<AudioSource>().clip = m_WeaponCollection.weapons[m_SelectedWeapon].reloadSound;
        GetComponent<AudioSource>().Play();

        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].reloadAniamtionVarName, true);

        // Reloading weapon
        yield return new WaitForSeconds(m_WeaponCollection.weapons[m_SelectedWeapon].reloadingTime);

        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].reloadAniamtionVarName, false);

        m_BurstRoundsCount = m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst;


        // Will reload fill weapon capacity completely
        if (
            m_WeaponCollection.weapons[m_SelectedWeapon].reloadingType == Weapon.ReloadingType.Full ||  
            (m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload > (m_WeaponCollection.weapons[m_SelectedWeapon].capacity - m_CurrentCapacity[m_SelectedWeapon]))
            ) 
        {
            // Is there enough ammo for reload
            if (m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] < (m_WeaponCollection.weapons[m_SelectedWeapon].capacity - m_CurrentCapacity[m_SelectedWeapon]))
            {
                // Reload weapon with remaining total ammo
                m_CurrentCapacity[m_SelectedWeapon] += m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name];
                m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] = 0;
            }
            else
            {
                // Reload weapon fully
                m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] -= (m_WeaponCollection.weapons[m_SelectedWeapon].capacity - m_CurrentCapacity[m_SelectedWeapon]);
                m_CurrentCapacity[m_SelectedWeapon] = m_WeaponCollection.weapons[m_SelectedWeapon].capacity;
            }
        }
        else
        {
            // Is there enough ammo for reload
            if (m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] < m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload)
            {
                // Reload weapon with remaining total ammo
                m_CurrentCapacity[m_SelectedWeapon] += m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name];
                m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] = 0;
            }
            else
            {
                // Reload weapon with expected partial amount
                m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] -= m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload;
                m_CurrentCapacity[m_SelectedWeapon] += m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload;
                    
            }
        }

        // Prevents firing when weapon is transitioning from reloading to idle animations
        if (m_WeaponCollection.weapons[m_SelectedWeapon].reloadingType == Weapon.ReloadingType.Partial)
        {
            m_CurrentPReloadInterruptionTime = m_WeaponCollection.weapons[m_SelectedWeapon].partialReloadInterruptionTime;
        }

        m_IsReloading = false;
    }

    IEnumerator WaitReloadWeaponRepeat()
    {
        m_IsReloading = true;

        GetComponent<AudioSource>().clip = m_WeaponCollection.weapons[m_SelectedWeapon].reloadSound;

        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].reloadAniamtionVarName, true);

        // Repeat reloading process
        for (int i = m_CurrentCapacity[m_SelectedWeapon]; i < m_WeaponCollection.weapons[m_SelectedWeapon].capacity; i += m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload)
        {
            GetComponent<AudioSource>().Play();

            // Reloading weapon
            yield return new WaitForSeconds(m_WeaponCollection.weapons[m_SelectedWeapon].reloadingTime);

            // Will reload fill weapon capacity completely
            if (m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload > (m_WeaponCollection.weapons[m_SelectedWeapon].capacity - m_CurrentCapacity[m_SelectedWeapon]))
            {
                // Is there enough ammo for reload
                if (m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] < (m_WeaponCollection.weapons[m_SelectedWeapon].capacity - m_CurrentCapacity[m_SelectedWeapon]))
                {
                    // Reload weapon with remaining total ammo
                    m_CurrentCapacity[m_SelectedWeapon] += m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name];
                    m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] = 0;
                }
                else
                {
                    // Reload weapon fully
                    m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] -= (m_WeaponCollection.weapons[m_SelectedWeapon].capacity - m_CurrentCapacity[m_SelectedWeapon]);
                    m_CurrentCapacity[m_SelectedWeapon] = m_WeaponCollection.weapons[m_SelectedWeapon].capacity;
                }
            }
            else
            {
                // Is there enough ammo for reload
                if (m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] < m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload)
                {
                    // Reload weapon with remaining total ammo
                    m_CurrentCapacity[m_SelectedWeapon] += m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name];
                    m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] = 0;
                }
                else
                {
                    // Reload weapon with expected partial amount
                    m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] -= m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload;
                    m_CurrentCapacity[m_SelectedWeapon] += m_WeaponCollection.weapons[m_SelectedWeapon].ammoAddedPerReload;

                }
            }

            // Ensures reloading stops when total ammo runs out
            if (m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name] <= 0)
            {
                i = m_WeaponCollection.weapons[m_SelectedWeapon].capacity;
            }
        }

        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].reloadAniamtionVarName, false);

        m_BurstRoundsCount = m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst;

        // Prevents firing when weapon is transitioning from reloading to idle animations
        m_CurrentPReloadInterruptionTime = m_WeaponCollection.weapons[m_SelectedWeapon].partialReloadInterruptionTime;

        m_IsReloading = false;
    }

    void CheckWeaponAim()
    {
        if (m_IsInputAim && !m_IsReloading && !m_IsSwitching && !m_IsRunning)
        {
            m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].aimAnimationVarName, true);

            // Increase aiming effect
            if (m_CurrentAimingTime < 1f)
            {
                m_CurrentAimingTime += (1f / m_WeaponCollection.weapons[m_SelectedWeapon].aimingTime) * Time.deltaTime;

                if (m_CurrentAimingTime > 1f)
                {
                    m_CurrentAimingTime = 1f;
                }
            }
        }
        else
        {
            m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].aimAnimationVarName, false);

            // Decrease aiming effect
            if (m_CurrentAimingTime > 0f)
            {
                m_CurrentAimingTime -= (1f / m_WeaponCollection.weapons[m_SelectedWeapon].aimingTime) * Time.deltaTime;

                if (m_CurrentAimingTime < 0f)
                {
                    m_CurrentAimingTime = 0f;
                }
            }
        }
    }

    void LoadSelectedWeapon()
    {
        // Remove old weapon instance
        if (m_InstantiatedWeaponObject != null)
        {
            Destroy(m_InstantiatedWeaponObject);
        }

        // Spawn new weapon instance
        if (m_WeaponCollection.weapons[m_SelectedWeapon].weaponPrefab != null)
        {
            m_InstantiatedWeaponObject = Instantiate(m_WeaponCollection.weapons[m_SelectedWeapon].weaponPrefab, transform);  //GameObject.Find("FPSWeaponSystem").transform);

            // Removes "(clone)" from name of spawned weapon object, to prevent animations from disconnecting with object
            const int numberOfCharacterToRemove = 7;

            // ? remove gameObject
            m_InstantiatedWeaponObject.gameObject.name = m_InstantiatedWeaponObject.gameObject.name.Remove(m_InstantiatedWeaponObject.gameObject.name.Length - numberOfCharacterToRemove);
        }

        // Find points for weapon functionality
        try
        {
            m_BarrelFlashSpawn = GameObject.Find(m_WeaponCollection.weapons[m_SelectedWeapon].barrelFlashSpawnName).transform;
        }
        catch
        {
            // ?    Change text when field name changes
            throw new System.Exception("Cannot find Barrel Flash Spawn object, ensure Barrel Flash Spawn Name field matches object's name.");
        }

        try
        {
            m_ProjectileSpawn = GameObject.Find(m_WeaponCollection.weapons[m_SelectedWeapon].projectileSpawnName).transform;
        }
        catch
        {
            // ?    Change text when field name changes
            throw new System.Exception("Cannot find Projectile Spawn object, ensure Projectile Spawn Name field matches object's name.");
        }

        try
        {
            m_EjectedCartridgeSpawn = GameObject.Find(m_WeaponCollection.weapons[m_SelectedWeapon].cartridgeSpawnName).transform;
        }
        catch
        {
            // ?    Change text when field name changes
            throw new System.Exception("Cannot find Cartridge Spawn object, ensure Cartridge Spawn Name field matches object's name.");
        }

        m_BarrelFlashSpawn.GetComponent<AudioSource>().clip = m_WeaponCollection.weapons[m_SelectedWeapon].barrelSound;
        m_BurstRoundsCount = m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst;

        // Prevents animation disconnection issue (if first weapon controller is pre-applied to animator)
        m_Animator.runtimeAnimatorController = null;

      //  m_Animator.runtimeAnimatorController = m_WeaponCollection.weapons[m_SelectedWeapon].animatorController;

    }

    public void EnableWeapon(Weapon weapon, bool state, int ammoInWeapon)
    {
        for (int i = 0; i < m_WeaponsLength; i++)
        {
            if (m_WeaponCollection.weapons[i].name == weapon.name)
            {
                if (state && !m_IsWeaponEnabled[i])
                {
                    m_IsWeaponEnabled[i] = true;
                    m_TimeUntillNextRound[i] = 1f / m_WeaponCollection.weapons[i].fireRate;

                    // Prevents current ammo being negitive or over weapon capacity
                    if (ammoInWeapon > m_WeaponCollection.weapons[i].capacity)
                    {
                        m_CurrentCapacity[i] = m_WeaponCollection.weapons[i].capacity;

                    }
                    else if (ammoInWeapon < 0)
                    {
                        m_CurrentCapacity[i] = 0;
                    }
                    else
                    {
                        m_CurrentCapacity[i] = ammoInWeapon;
                    }
                }
                else if (!state && m_IsWeaponEnabled[i])
                {
                    m_IsWeaponEnabled[i] = false;

                    // ? should switch out weapon is currently weilded 
                    if (i == m_SelectedWeapon)
                    {
                        m_IsCurrentWeaponDisabled = true;
                    }

                }

                i = m_WeaponsLength;
            }
        }
    }
    void EnableWeapon(int index, bool state, int ammoInWeapon)
    {
        if (state && !m_IsWeaponEnabled[index])
        {
            m_IsWeaponEnabled[index] = true;
            m_TimeUntillNextRound[index] = 1f / m_WeaponCollection.weapons[index].fireRate;

            // Prevents current ammo being negitive or over weapon capacity
            if (ammoInWeapon > m_WeaponCollection.weapons[index].capacity)
            {
                m_CurrentCapacity[index] = m_WeaponCollection.weapons[index].capacity;
            }
            else if (ammoInWeapon < 0)
            {
                m_CurrentCapacity[index] = 0;
            }
            else
            {
                m_CurrentCapacity[index] = ammoInWeapon;
            }
        }
        else if (!state && m_IsWeaponEnabled[index])
        {
            m_IsWeaponEnabled[index] = false;

            // ? should switch out weapon is currently weilded
            if (index == m_SelectedWeapon)
            {
                m_IsCurrentWeaponDisabled = true;
            }
        }
    }

    public void IncreaseAmmoCount(Ammo ammo, int amount)
    {
        for (int i = 0; i < m_WeaponsLength; i++)
        {
            if (m_WeaponCollection.weapons[i].ammo.name == ammo.name)
            {
                m_AmmoAmounts[m_WeaponCollection.weapons[i].ammo.name] += amount;

                i = m_WeaponsLength;
            }
        }
    }

    void CheckCharacterMovement()
    {
        // Decrement running transition
        if (m_CurrentRunningRecoveryTime > 0f)
        {
            m_CurrentRunningRecoveryTime -= 1f * Time.deltaTime;
        }
        else
        {
            m_CurrentRunningRecoveryTime = 0f;
        }

        // State conditions, used across script
        m_IsJumping = false;
        m_IsRunning = false;
        m_IsWalking = false;

      /*  if (!m_CharacterController.isGrounded) // Jumping
        {
            m_IsJumping = true;
        }
        if (m_IsInputRun && new Vector2(m_CharacterController.velocity.x, m_CharacterController.velocity.z).magnitude > 0f) // Running
        {
            m_IsRunning = true;
        }
        else if (new Vector2(m_CharacterController.velocity.x, m_CharacterController.velocity.z).magnitude > 0f) // Walking
        {
            m_IsWalking = true;
        }
      */
        // Animation effected by the states
        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].jumpAnimationVarName, false);
        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].runAnimationVarName, false);
        m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].walkAnimationVarName, false);

        if (m_IsJumping)
        {
            m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].jumpAnimationVarName, true);
        }
        if (m_IsRunning)
        {
            m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].runAnimationVarName, true);
            m_CurrentRunningRecoveryTime = m_WeaponCollection.weapons[m_SelectedWeapon].runningRecoveryTime;

            // Abort weapon reloading
            if (m_IsReloading)
            {
                if (m_ReloadWeaponCoroutine != null)
                {
                    // Prevents continuing burst-fire after partial repeat reloading is aborted
                    m_BurstRoundsCount = m_WeaponCollection.weapons[m_SelectedWeapon].roundsPerBurst;

                    StopCoroutine(m_ReloadWeaponCoroutine);
                }

                m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].reloadAniamtionVarName, false);

                m_IsReloading = false;
            }
        }
        if (m_IsWalking)
        {
            m_Animator.SetBool(m_WeaponCollection.weapons[m_SelectedWeapon].walkAnimationVarName, true);
        }
    }

    void CheckMouseMovement()
    {
        const float mouseMovementRate = 0.1f;             // ? should be public property, for users to access without inspector
        const float mouseMovementRounding = 0.001f;       // ? should be public property, for users to access without inspector

        // Allows mouse movement influance to adjust gradully, relying on mouse GetAxis alone causes jittering 
        if (m_CurrentMouseInfluance.x < Input.GetAxis(m_MouseXInfluenceName) - mouseMovementRounding || m_CurrentMouseInfluance.x > Input.GetAxis(m_MouseXInfluenceName) + mouseMovementRounding)
        {
            m_CurrentMouseInfluance.x += (Input.GetAxis(m_MouseXInfluenceName) - m_CurrentMouseInfluance.x) * mouseMovementRate;
        }
        else
        {
            m_CurrentMouseInfluance.x = Input.GetAxis(m_MouseXInfluenceName);
        }
        
        // Allows mouse movement influance to adjust gradully, relying on mouse GetAxis alone causes jittering 
        if (m_CurrentMouseInfluance.y < Input.GetAxis(m_MouseYInfluenceName) - mouseMovementRounding || m_CurrentMouseInfluance.y > Input.GetAxis(m_MouseYInfluenceName) + mouseMovementRounding)
        {
            m_CurrentMouseInfluance.y += (Input.GetAxis(m_MouseYInfluenceName) - m_CurrentMouseInfluance.y) * mouseMovementRate;
        }
        else
        {
            m_CurrentMouseInfluance.y = Input.GetAxis(m_MouseYInfluenceName);
        }


        m_Animator.SetFloat(m_WeaponCollection.weapons[m_SelectedWeapon].mouseXAnimationVarName, m_CurrentMouseInfluance.x);
        m_Animator.SetFloat(m_WeaponCollection.weapons[m_SelectedWeapon].mouseYAnimationVarName, m_CurrentMouseInfluance.y);
    }

    void UpdateHUD()
    {
        if (!m_IsSwitching)
        {

            m_UICrosshairSpace.sprite = m_WeaponCollection.weapons[m_SelectedWeapon].crosshairSprite;
                
            if (m_UICrosshairSpace.sprite != null && m_CurrentAimingTime == 0f)
            {
                m_UICrosshairSpace.color = m_WeaponCollection.weapons[m_SelectedWeapon].crosshairColour;
            }
            else
            {
                Color c = m_WeaponCollection.weapons[m_SelectedWeapon].crosshairColour;
                m_UICrosshairSpace.color = new Color(c.r, c.g, c.b, 0f);
            }
                

            m_UIMagAmmoCount.text = m_CurrentCapacity[m_SelectedWeapon].ToString();
            m_UITotalAmmoCount.text = m_AmmoAmounts[m_WeaponCollection.weapons[m_SelectedWeapon].ammo.name].ToString();
            m_UIAmmoIconSpace.sprite = m_WeaponCollection.weapons[m_SelectedWeapon].ammo.ammoIcon;
        }
    }

    void CheckInputs()
    {
        if (Input.GetKeyDown(m_inputFire)) 
        {
            m_IsInputFire = true;
        }
        else
        {
            m_IsInputFire = false;
        }

        if (Input.GetKey(m_inputAutoFire))
        {
            m_IsInputAutoFire = true;
        }
        else
        {
            m_IsInputAutoFire = false;
        }

        if (Input.GetKeyDown(m_inputReload))
        {
            m_IsInputReload = true;
        }
        else
        {
            m_IsInputReload = false;
        }

        if (Input.GetKeyDown(m_inputSwitch))
        {
            m_IsInputSwitch = true;
        }
        else
        {
            m_IsInputSwitch = false;
        }

        if (Input.GetKey(m_inputAim))
        {
            m_IsInputAim = true;
        }
        else
        {
            m_IsInputAim = false;
        }

        if (Input.GetKey(m_inputRun))
        {
            m_IsInputRun = true;
        }
        else
        {
            m_IsInputRun = false;
        }
    }

}
