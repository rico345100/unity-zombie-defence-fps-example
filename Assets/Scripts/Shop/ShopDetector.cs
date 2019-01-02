using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopDetector : MonoBehaviour {
	
	int healUsed = 0;
	public Transform shootPoint;
	public float detectRange;
	public Text shopText;
	public Text warningText;
	public AudioClip purchasedSound;

	IEnumerator warningTextCo = null;

	IEnumerator HideWarningText() {
		yield return new WaitForSeconds(3f);

		warningText.text = "";
		yield break;
	}
	
	void Start() {
		shopText = GameObject.Find("UI/InGameUI/Info/ShopText").GetComponent<Text>();
		warningText = GameObject.Find("UI/InGameUI/Info/WarningText").GetComponent<Text>();
	}

	void PrintWarning(string text) {
		if(warningTextCo != null) StopCoroutine(warningTextCo);

		warningTextCo = HideWarningText();
		warningText.text = text;

		StartCoroutine(warningTextCo);
	}

	void BuyWeapon(Weapon weapon) {
		string weaponName = weapon.ToString();
		WeaponManager weaponManager = transform.Find("WeaponHolder").GetComponent<WeaponManager>();
		GameObject weaponGO = transform.Find("WeaponHolder/" + weaponName).gameObject;
		weaponManager.currentWeaponGO.GetComponent<WeaponBase>().Unload();

		weaponManager.currentWeapon = weapon;
		weaponManager.currentWeaponGO = weaponGO;

		weaponManager.primaryWeapon = weapon;
		weaponManager.primaryWeaponGO = weaponGO;

		WeaponBase weaponBase = weaponManager.currentWeaponGO.GetComponent<WeaponBase>();

		weaponManager.currentWeaponGO.SetActive(true);
		weaponBase.InitAmmo();
		weaponBase.Draw();
	}

	void UpgradeWeapon(WeaponBase weaponBase, ShopType upgradeType) {
		switch(upgradeType) {
			case ShopType.UPGRADE_DAMAGE:
				weaponBase.upgradeDamage++;
				break;
			case ShopType.UPGRADE_RELOAD:
				weaponBase.upgradeReload++;
				break;
			case ShopType.UPGRADE_RECOIL:
				weaponBase.upgradeRecoil++;
				break;
			case ShopType.UPGRADE_MAGAZINE:
				weaponBase.upgradeMag++;
				weaponBase.RecalculateMagSize();
				break;
			case ShopType.UPGRADE_MAX_AMMO:
				weaponBase.upgradeMaxAmmo++;
				weaponBase.RecalculateMaxAmmo();
				break;
		}
	}


	int GetAmmoPrice(Weapon weapon) {
		int price = 0;

		switch(weapon) {
			case Weapon.AKM:
				price = 250;
				break;
			case Weapon.M870:
				price = 200;
				break;
			case Weapon.MP5K:
				price = 150;
				break;
			case Weapon.Glock:
				price = 100;
				break;
			case Weapon.Python:
				price = 100;
				break;
			default:
				price = 100;
				break;
		}

		return price;
	}

	int GetUpgradePrice(Weapon weapon, int upgraded) {
		int basePrice = 100;

		switch(weapon) {
			case Weapon.AKM:
				basePrice = 150;
				break;
			case Weapon.M870:
				basePrice = 100;
				break;
			case Weapon.MP5K:
				basePrice = 75;
				break;
			case Weapon.Glock:
				basePrice = 50;
				break;
			case Weapon.Python:
				basePrice = 50;
				break;
		}

		return basePrice * (upgraded + 1);
	}


	void Update() {
		RaycastHit hit;
		Vector3 position = shootPoint.position;
		position.y += 1;	// Adjust height differences

		// Debug.DrawRay(position, transform.TransformDirection(Vector3.forward * detectRange), Color.red);
		if(Physics.Raycast(position, transform.TransformDirection(Vector3.forward * detectRange), out hit, detectRange)) {
			if(hit.transform.tag == "Shop") {
				Shop shop = hit.transform.GetComponent<Shop>();
				ShopType shopType = shop.shopType;
				string shopTitle = shop.title;
				string shopDesc = shop.description;
				int shopPrice = shop.price;
				bool isPurchasable = true;

				WeaponManager weaponManager = transform.Find("WeaponHolder").GetComponent<WeaponManager>();
				WeaponBase weaponBase = weaponManager.currentWeaponGO.GetComponent<WeaponBase>();
				Weapon weapon = weaponManager.currentWeapon;

				if(shopType == ShopType.AMMO) {
					shopPrice = GetAmmoPrice(weapon);
					shopText.text = shopTitle + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
				}
				else if(shopType == ShopType.HEAL) {
					shopPrice = 100 + (75 * healUsed);
					shopText.text = shopTitle + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
				}
				else if(shopType == ShopType.UPGRADE_DAMAGE) {
					int upgraded = weaponBase.upgradeDamage;

					if(upgraded < 10) {
						shopPrice = GetUpgradePrice(weaponManager.currentWeapon, upgraded);
						shopText.text = shopTitle + " Lv" + (upgraded + 1) + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
					}
					else {
						isPurchasable = false;
						shopText.text = "Your weapon is fully upgraded.";
					}
				}
				else if(shopType == ShopType.UPGRADE_RELOAD) {
					int upgraded = weaponBase.upgradeReload;

					if(upgraded < 10) {
						shopPrice = GetUpgradePrice(weaponManager.currentWeapon, upgraded);
						shopText.text = shopTitle + " Lv" + (upgraded + 1) + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
					}
					else {
						isPurchasable = false;
						shopText.text = "Your weapon is fully upgraded.";
					}
				}
				else if(shopType == ShopType.UPGRADE_RECOIL) {
					int upgraded = weaponBase.upgradeRecoil;

					if(upgraded < 10) {
						shopPrice = GetUpgradePrice(weaponManager.currentWeapon, upgraded);
						shopText.text = shopTitle + " Lv" + (upgraded + 1) + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
					}
					else {
						isPurchasable = false;
						shopText.text = "Your weapon is fully upgraded.";
					}
				}
				else if(shopType == ShopType.UPGRADE_MAGAZINE) {
					int upgraded = weaponBase.upgradeMag;

					if(upgraded < 10) {
						shopPrice = GetUpgradePrice(weaponManager.currentWeapon, upgraded);
						shopText.text = shopTitle + " Lv" + (upgraded + 1) + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
					}
					else {
						isPurchasable = false;
						shopText.text = "Your weapon is fully upgraded.";
					}
				}
				else if(shopType == ShopType.UPGRADE_MAX_AMMO) {
					int upgraded = weaponBase.upgradeMaxAmmo;

					if(upgraded < 10) {
						shopPrice = GetUpgradePrice(weaponManager.currentWeapon, upgraded);
						shopText.text = shopTitle + " Lv" + (upgraded + 1) + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
					}
					else {
						isPurchasable = false;
						shopText.text = "Your weapon is fully upgraded.";
					}
				}
				else {
					shopText.text = shopTitle + "\n(" + shopPrice + "$)\n\n" +  shopDesc + "\n\n";
				}

				if(isPurchasable && Input.GetKeyDown(KeyCode.F)) {
					FundSystem fundSystem = transform.parent.GetComponent<FundSystem>();
					int fund = fundSystem.GetFund();

					if(fund < shopPrice) {
						PrintWarning("Not enough money!");
					}
					else {
						bool wasPurchased = true;

						if(shopType == ShopType.AMMO) {
							weaponBase.bulletsLeft = weaponBase.startBullets + weaponBase.bulletsPerMag;
							weaponBase.UpdateAmmoText();
						}
						else if(shopType == ShopType.HEAL) {
							HealthManager healthManager = transform.parent.GetComponent<HealthManager>();

							if(healthManager.Health >= healthManager.MaxHealth) {
								wasPurchased = false;
								PrintWarning("You have full health.");
							}
							else {
								healthManager.Heal();
								healUsed++;
							}
						}
						else if(shopType == ShopType.WEAPON_PYTHON) {
							Debug.Log("Try buy;");
							if(!weaponManager.HasWeapon(Weapon.Python)) {
								BuyWeapon(Weapon.Python);
							}
							else {
								wasPurchased = false;
								PrintWarning("You already have weapon.");
							}
						}
						else if(shopType == ShopType.WEAPON_MP5K) {
							if(!weaponManager.HasWeapon(Weapon.MP5K)) {
								BuyWeapon(Weapon.MP5K);
							}
							else {
								wasPurchased = false;
								PrintWarning("You already have weapon.");
							}
						}
						else if(shopType == ShopType.WEAPON_UMP45) {
							if(!weaponManager.HasWeapon(Weapon.UMP45)) {
								BuyWeapon(Weapon.UMP45);
							}
							else {
								wasPurchased = false;
								PrintWarning("You already have weapon.");
							}
						}
						else if(shopType == ShopType.WEAPON_AKM) {
							if(!weaponManager.HasWeapon(Weapon.AKM)) {
								BuyWeapon(Weapon.AKM);
							}
							else {
								wasPurchased = false;
								PrintWarning("You already have weapon.");
							}
						}
						else if(shopType == ShopType.WEAPON_M870) {
							if(!weaponManager.HasWeapon(Weapon.M870)) {
								BuyWeapon(Weapon.M870);
							}
							else {
								wasPurchased = false;
								PrintWarning("You already have weapon.");
							}
						}
						else if(shopType == ShopType.UPGRADE_DAMAGE) {
							if(weaponBase.upgradeDamage >= 10) {
								wasPurchased = false;
								PrintWarning("Your weapon is fully upgraded.");
							}
							else {
								UpgradeWeapon(weaponBase, ShopType.UPGRADE_DAMAGE);								
							}
						}
						else if(shopType == ShopType.UPGRADE_RELOAD) {
							if(weaponBase.upgradeReload >= 10) {
								wasPurchased = false;
								PrintWarning("Your weapon is fully upgraded.");
							}
							else {
								UpgradeWeapon(weaponBase, ShopType.UPGRADE_RELOAD);								
							}
						}
						else if(shopType == ShopType.UPGRADE_RECOIL) {
							if(weaponBase.upgradeRecoil >= 10) {
								wasPurchased = false;
								PrintWarning("Your weapon is fully upgraded.");
							}
							else {
								UpgradeWeapon(weaponBase, ShopType.UPGRADE_RECOIL);								
							}
						}
						else if(shopType == ShopType.UPGRADE_MAGAZINE) {
							if(weaponBase.upgradeMag >= 10) {
								wasPurchased = false;
								PrintWarning("Your weapon is fully upgraded.");
							}
							else {
								UpgradeWeapon(weaponBase, ShopType.UPGRADE_MAGAZINE);
							}
						}
						else if(shopType == ShopType.UPGRADE_MAX_AMMO) {
							if(weaponBase.upgradeMaxAmmo >= 10) {
								wasPurchased = false;
								PrintWarning("Your weapon is fully upgraded.");
							}
							else {
								UpgradeWeapon(weaponBase, ShopType.UPGRADE_MAX_AMMO);
							}
						}
						else {
							wasPurchased = false;
						}

						if(wasPurchased) {
							fundSystem.TakeFund(shopPrice);
							SoundManager soundManager = transform.Find("SoundManager").GetComponent<SoundManager>();
							soundManager.Play(purchasedSound);
						}
					}
				}
			}
		}
		else {
			shopText.text = "";
		}
	}
}
