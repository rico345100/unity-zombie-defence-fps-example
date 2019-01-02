using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopType {
	AMMO,
	HEAL,
	RESURRECTION,
	WEAPON_MP5K,
	WEAPON_UMP45,
	WEAPON_PYTHON,
	WEAPON_AKM,
	WEAPON_M870,
	UPGRADE_DAMAGE,
	UPGRADE_MAGAZINE,
	UPGRADE_MAX_AMMO,
	UPGRADE_RANGE,
	UPGRADE_RECOIL,
	UPGRADE_RELOAD,
	UPGRADE_STEADY,
	UPGRADE_HEALTH,
	UPGRADE_REGENERATION
};

public class Shop : MonoBehaviour {
	public ShopType shopType;
	public string title;
	public string description;
	public int price;
}
