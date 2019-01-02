using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Weapon {
	None,
	Python,
	Glock,
	AKM,
	MP5K,
	UMP45,
	M870
};

public class WeaponManager : MonoBehaviour {
	[SerializeField]
	public Weapon primaryWeapon;

	[SerializeField]
	public Weapon secondaryWeapon;

	public Weapon currentWeapon;

	public GameObject primaryWeaponGO;
	public GameObject secondaryWeaponGO;
	public GameObject currentWeaponGO;

	void Start() {
		primaryWeapon = Weapon.None;
		secondaryWeapon = Weapon.Glock;
		currentWeapon = secondaryWeapon;

		primaryWeaponGO = null;
		secondaryWeaponGO = transform.Find(secondaryWeapon.ToString()).gameObject;
		currentWeaponGO = secondaryWeaponGO;

		StartCoroutine(Init());
	}

	IEnumerator Init() {
		currentWeaponGO.SetActive(true);
		
		yield return new WaitForSeconds(0.1f);
		currentWeaponGO.GetComponent<WeaponBase>().Draw();

		yield break;
	}

	void Update() {
		if(primaryWeapon != Weapon.None && Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon != primaryWeapon) {
			currentWeaponGO.GetComponent<WeaponBase>().Unload();

			currentWeapon = primaryWeapon;
			currentWeaponGO = primaryWeaponGO;

			currentWeaponGO.SetActive(true);
			currentWeaponGO.GetComponent<WeaponBase>().Draw();
		}
		else if(secondaryWeapon != Weapon.None && Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon != secondaryWeapon) {
			currentWeaponGO.GetComponent<WeaponBase>().Unload();

			currentWeapon = secondaryWeapon;
			currentWeaponGO = secondaryWeaponGO;

			currentWeaponGO.SetActive(true);
			currentWeaponGO.GetComponent<WeaponBase>().Draw();
		}
	}

	public bool HasWeapon(Weapon weapon) {
		if(primaryWeapon == weapon || secondaryWeapon == weapon) return true;
		
		return false;
	}
}
