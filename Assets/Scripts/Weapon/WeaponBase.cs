using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CrosshairType { CROSS, CIRCLE };
public enum WeaponType { PISTOL, SHOTGUN, SMG, ASSAULT_RIFLE, SNIPER_RIFLE };
public enum ReloadType { MAGAZINE, INSERTION };
public enum FireMode { AUTO, SEMI };

public class WeaponBase : MonoBehaviour {
	[Header("Gun Attributes")]
	public string weaponName;
	public WeaponType weaponType;
	public ReloadType reloadType;
	public CrosshairType crosshairType;
	public FireMode fireMode;
	public float damage = 17f;
	public float fireRate = 0.1f;
	public float range = 100f;
	public int bulletsPerMag = 30;
	public int bulletsLeft = 45;
	public int startBullets = 45;
	public int loadedBullets;
	public int pellets = 1;
	public float spread;
	public float bulletEjectingSpeed = 5.0f;
	public bool hasLastFire = false;
	public float recoil = 0f;

	[Header("UI Refs")]
	public RewardText rewardText;
	public Text weaponNameText;
	public Text weaponAmmoText;
	

	[Header("Animation Attributes")]
	public Vector3 aimPos;
	public float aimingSpeed = 8f;

	[Header("Game System Attributes")]
	public FundSystem fundSystem;
	public LevelSystem levelSystem;

	[Header("Sounds")]
	public AudioClip drySound;
	public AudioClip gunFireSound;


	[Header("External Refs")]
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;
	public GameObject hitParticle;
	public GameObject bulletImpact;
	public GameObject gunSmoke;
	public GameObject emptyCase;
	public SoundManager soundManager;
	
	[Header("Internal Refs")]
	public Transform shootPoint;
	public Transform muzzlePoint;
	public Transform caseSpawnPoint;
	public ParticleSystem muzzleflash;

	[Header("Weapon Upgrades")]
	public float upgradeDamageFactor = 1;
	public int upgradeDamage = 0;

	public float upgradeMagFactor = 1;
	public int upgradeMag = 0;

	public float upgradeMaxAmmoFactor = 1;
	public int upgradeMaxAmmo = 0;

	public float upgradeRangeFactor = 1;
	public int upgradeRange = 0;

	public float upgradeRecoilFactor = 1;
	public int upgradeRecoil = 0;

	public float upgradeReloadFactor = 1;
	public int upgradeReload = 0;

	public float upgradeSteadyFactor = 1;
	public int upgradeSteady = 0;

	private Animator animator;
	private float fireTimer;
	private bool isReloading = false;
	private bool isEnabled = true;
	private Vector3 originalPos;
	
	// UI Variables
	private Image crosshair;

	private PrefabManager prefabManager;
	private SpriteManager spriteManager;
	private Sprite crosshairCross;
	private Sprite crosshairCrossAim;
	private Sprite crosshairCircle;
	private Sprite crosshairCircleAim;

	private WeaponBob weaponBob;

	public bool IsEnabled {
		get {
			return isEnabled;
		}
		set {
			isEnabled = value;
		}
	}

	public void RecalculateMagSize() {
		float extraLoads = upgradeMag * upgradeMagFactor;
		bulletsPerMag = bulletsPerMag + (int) extraLoads;
	}

	public void RecalculateMaxAmmo() {
		float extraAmmo = upgradeMaxAmmo * upgradeMaxAmmoFactor;
		startBullets = startBullets + (int) extraAmmo;
	}


	void Start() {
		animator = GetComponent<Animator>();
		weaponBob = GetComponent<WeaponBob>();

		InitAmmo();

		originalPos = transform.localPosition;
		crosshair = GameObject.Find("Crosshair").GetComponent<Image>();

		prefabManager = PrefabManager.GetInstance();
		spriteManager = SpriteManager.GetInstance();

		crosshairCross = spriteManager.GetSprite("Crosshair_Default");
		crosshairCrossAim = spriteManager.GetSprite("Crosshair_Default_Aim");
		crosshairCircle = spriteManager.GetSprite("Crosshair_Circle");
		crosshairCircleAim = spriteManager.GetSprite("Crosshair_Circle_Aim");

		weaponNameText = GameObject.Find("UI/InGameUI/WeaponStatus/WeaponNameText").GetComponent<Text>();
		weaponAmmoText = GameObject.Find("UI/InGameUI/WeaponStatus/AmmoText").GetComponent<Text>();
	}

	void Update() {
		if(fireMode == FireMode.SEMI && Input.GetButtonDown("Fire1") && !isReloading) {
			Fire();
		}
		else if(fireMode == FireMode.AUTO && Input.GetButton("Fire1") && !isReloading) {
			Fire();
		}
		else if(Input.GetButtonDown("Reload")) {
			StartReload();
		}
		
		if(fireTimer < fireRate) {
			fireTimer += Time.deltaTime;
		}

		AdjustAimingSights();

		float reloadSpeed = 1 + (upgradeReload * upgradeReloadFactor);
		animator.SetFloat("ReloadSpeed", reloadSpeed);
	}

	void DrawHitRay() {
		Debug.DrawRay(shootPoint.position, CalculateSpread(spread, shootPoint), Color.green, 10.0f);
	}

	Vector3 CalculateSpread(float inaccuracy, Transform trans) {
		if(Input.GetButton("Fire2")) inaccuracy /= 2;

		return Vector3.Lerp(trans.TransformDirection(Vector3.forward * range), Random.onUnitSphere, inaccuracy);
	}

	IEnumerator DisableFire(float time = 0.3f) {
		isEnabled = false;

		yield return new WaitForSeconds(time);
		isEnabled = true;

		yield break;
	}

	float GetWeaponDamage() {
		float extraDamage = upgradeDamageFactor * (float) upgradeDamage;
		return extraDamage + damage;
	}

	void Fire() {
		if(fireTimer < fireRate || !isEnabled) return;

		if(loadedBullets <= 0) {
			// When Ammo is out, make fire is not working for a moment
			StartCoroutine(DisableFire());
			soundManager.Play(drySound);
			return;
		}
		
		RaycastHit hit;

		for(int i = 0; i < pellets; i++) {
			if(Physics.Raycast(shootPoint.position, CalculateSpread(spread, shootPoint), out hit, range)) {
				HealthManager health = hit.transform.GetComponent<HealthManager>();

				if(health) {
					health.ApplyDamage(GetWeaponDamage());

					if(health.IsDead) {
						KillReward killReward;

						// Check it was head
						if(health.referer) {
							killReward = health.referer.transform.GetComponent<KillReward>();
						}
						else {
							killReward = hit.transform.GetComponent<KillReward>();
						}

						if(killReward) {
							int exp = killReward.exp;
							int fund = killReward.fund;

							levelSystem.GiveExp(exp);
							fundSystem.AddFund(fund);

							rewardText.Show(exp, fund);
						}
					}

					EnemyType enemyType = hit.transform.GetComponent<EnemyType>();

					if(enemyType) {
						if(enemyType.type == Type.BIO) {
							CreateBlood(hit.point);
						}
					}
					else {
						CreateRicochet(hit.point, hit.normal);
					}
				}
				else {
					CreateRicochet(hit.point, hit.normal);

					GameObject bulletHole = Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
					bulletHole.transform.SetParent(hit.transform);
					Destroy(bulletHole, 10f);
				}
			}
		}

		if(hasLastFire && loadedBullets <= 1) {
			animator.CrossFadeInFixedTime("FPSHand|FireLast", 0.01f);
		}
		else {
			animator.CrossFadeInFixedTime("FPSHand|Fire", 0.01f);
		}
		
		GameObject gunSmokeEffect = Instantiate(gunSmoke, muzzlePoint.position, muzzlePoint.rotation);
		Destroy(gunSmokeEffect, 5f);

		GiveRecoil();

		muzzleflash.Play();
		soundManager.Play(gunFireSound);

		loadedBullets--;
		UpdateAmmoText();

		fireTimer = 0.0f;
	}

	void GiveRecoil() {
		float baseRecoil = recoil;
		float calcRecoil = baseRecoil - (upgradeRecoil * upgradeRecoilFactor);

		if(calcRecoil <= 0) {
			calcRecoil = 0f;
		}

		controller.mouseLook.StartRecoil(calcRecoil);
	}

	void CreateRicochet(Vector3 pos, Vector3 normal) {
		GameObject hitParticleEffect = Instantiate(hitParticle, pos, Quaternion.FromToRotation(Vector3.up, normal));
		Destroy(hitParticleEffect, 1f);
	}

	void CreateBlood(Vector3 pos) {		
		GameObject blood = prefabManager.GetPrefab("BloodEffect");
		GameObject bloodEffect = Instantiate(blood, pos, new Quaternion(0, 0, 0, 0));
		Destroy(bloodEffect, 3f);
	}

	void UpdateCrosshair(bool isAiming) {
		Sprite crosshairSprite = null;

		if(crosshairType == CrosshairType.CROSS) {
			crosshairSprite = isAiming ? crosshairCrossAim : crosshairCross;
		}
		else if(crosshairType == CrosshairType.CIRCLE) {
			crosshairSprite = isAiming ? crosshairCircleAim : crosshairCircle;
		}

		crosshair.sprite = crosshairSprite;
	}

	void AdjustAimingSights() {
		if(Input.GetButton("Fire2") && !isReloading) {
			UpdateCrosshair(true);
			transform.localPosition = Vector3.Lerp(transform.localPosition, aimPos, Time.deltaTime * aimingSpeed);

			weaponBob.xOffset = aimPos.x;
			weaponBob.yOffset = aimPos.y;

			controller.IsAiming = true;
		}
		else {
			UpdateCrosshair(false);
			transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * aimingSpeed);

			weaponBob.Reset();

			controller.IsAiming = false;
		}
	}

	void StartReload() {
		if(isReloading ||
			loadedBullets >= bulletsPerMag || 
			bulletsLeft <= 0) return;
		
		isReloading = true;

		if(reloadType == ReloadType.MAGAZINE) {
			animator.CrossFadeInFixedTime("FPSHand|Reload", 0.01f);
		}
		else if(reloadType == ReloadType.INSERTION) {
			if(loadedBullets <= 0) animator.CrossFadeInFixedTime("FPSHand|ReloadStartEmpty", 0.01f);
			else animator.CrossFadeInFixedTime("FPSHand|ReloadStart", 0.01f);
		}
	}

	void RefillAmmunitions() {
		int bulletsToLoad = bulletsPerMag - loadedBullets;
		int bulletsToDeduct = bulletsLeft >= bulletsToLoad ? bulletsToLoad : bulletsLeft;

		bulletsLeft -= bulletsToDeduct;
		loadedBullets += bulletsToDeduct;

		if(hasLastFire) {
			animator.SetBool("IsEmpty", false);	
		}

		UpdateAmmoText();
	}

	public void Draw() {
		StartCoroutine(PrepareWeapon());
	}

	public void InitAmmo() {
		bulletsLeft = startBullets;
		loadedBullets = bulletsPerMag;
	}

	IEnumerator PrepareWeapon() {
		yield return new WaitForEndOfFrame();
		weaponNameText.text = weaponName + "(" + weaponType.ToString() + ")";
		
		UpdateAmmoText();

		isEnabled = true;
		animator.Play("FPSHand|Draw");

		if(hasLastFire && loadedBullets <= 0) {
			animator.SetBool("IsEmpty", true);
		}

		yield break;
	}

	public void UpdateAmmoText() {
		weaponAmmoText.text = loadedBullets + " / " + bulletsLeft;
	}

	public void Unload() {
		isReloading = false;
		isEnabled = false;
		
		gameObject.SetActive(false);
	}

	void OnCaseOut() {
		GameObject ejectedCase = Instantiate(emptyCase, caseSpawnPoint.position, caseSpawnPoint.rotation);
		Rigidbody caseRigidbody = ejectedCase.GetComponent<Rigidbody>();
		caseRigidbody.velocity = caseSpawnPoint.TransformDirection(-Vector3.left * bulletEjectingSpeed);
		caseRigidbody.AddTorque(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, 0.2f), Random.Range(-0.2f, 0.2f));
		caseRigidbody.AddForce(0, Random.Range(2.0f, 4.0f), 0, ForceMode.Impulse);

		Destroy(ejectedCase, 10f);
	}

	void OnMagIn() {
		RefillAmmunitions();
	}

	void OnAmmoInsertion() {
		isReloading = false;	// Make gun fire is possible
		bulletsLeft--;
		loadedBullets++;

		if(hasLastFire) {
			animator.SetBool("IsEmpty", false);	
		}

		UpdateAmmoText();
	}

	void OnFirstAmmoInsert() {
		if(bulletsLeft <= 0) {
			animator.CrossFadeInFixedTime("FPSHand|Stand", 0.01f);
		}
		else {
			animator.CrossFadeInFixedTime("FPSHand|ReloadStart", 0.01f);
		}
	}

	void OnBeforeInsert() {
		isReloading = true;
	}

	void OnAfterInsert() {
		if(loadedBullets >= bulletsPerMag) {
			animator.CrossFadeInFixedTime("FPSHand|ReloadEnd", 0.01f);
		}
		else if(bulletsLeft <= 0) {
			animator.CrossFadeInFixedTime("FPSHand|ReloadEnd", 0.01f);
		}
		else {
			animator.CrossFadeInFixedTime("FPSHand|ReloadInsert", 0.01f);
		}
	}

	void OnReloadEnd() {
		isReloading = false;
	}
}
