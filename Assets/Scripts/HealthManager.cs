using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
	Animator animator;

	[SerializeField] public float Health = 100.0f;
	public float MaxHealth = 100.0f;
	public bool isPlayer = false;
	public bool removeColliderOnDeath = false;
	public HealthManager referer;	// Special prorperty for create multiple hit system, if it sets on GameObject that has same HealthManager, Apply Damage to it.
	public float damageFactor = 1.0f;
	public Text healthText;

	void Start() {
		animator = GetComponent<Animator>();

		if(isPlayer) healthText = GameObject.Find("UI/InGameUI/CharacterStatus/HealthText").GetComponent<Text>();
	}

	void Update() {
		if(healthText) {
			healthText.text = "HP: " + Health.ToString();
		}
	}
	
	public void ApplyDamage(float damage) {
		if(IsDead) return;

		damage *= damageFactor;

		if(referer) {
			referer.ApplyDamage(damage);
		}
		else {
			Health -= damage;

			if(Health <= 0) {
				Health = 0;
				
				if(animator) {
					animator.SetTrigger("Dead");
				}
				
				if(removeColliderOnDeath) {
					RemoveColliders(GetComponents<Collider>());
					RemoveColliders(GetComponentsInChildren<Collider>());
				}
			}
		}
	}

	public void SetHealth(float newHealth) {
		Health = newHealth;
	}

	public bool IsDead {
		get {
			if(!referer) {
				return Health <= 0;
			}
			else {
				return referer.IsDead;
			}
		}
	}

	void RemoveColliders(Collider[] colliders) {
		foreach(Collider collider in colliders) {
			collider.enabled = false;
		}
	}

	public void Heal() {
		Health = MaxHealth;
	}

}
