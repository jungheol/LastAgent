using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    //무기 타입을 상수로 지정
    public enum WeaponType
    {
        RIFLE = 0, KNIFE, GRENADE
    }
    //주인공이 현재 들고 있는 무기를 저장할 변수
    public WeaponType currWeapon = WeaponType.RIFLE;

    public GameObject bullet;
    public Transform firePos;
    public ParticleSystem cartridge;
    ParticleSystem muzzleFlash;
    //오디오 클립을 저장할 변수
    public PlayerSfx playerSfx;
    AudioSource _audio;
    Shake shake;

    //변경할 무기 이미지
    public Sprite[] weaponIcons;
    //교체할 무기 이미지
    public Image weaponImage;

    //탄창 이미지 UI
    public Image magazineImg;
    //남은 총알 수 Text UI
    public Text magazineTxt;
    //최대 총알 수
    public int maxBullet = 10;
    //남은 총알 수
    public int remainBullet = 10;
    //재장전 시간
    public float reloadTime = 2.0f;
    //재장전 여부를 판단할 변수
    bool isReloading = false;
    void Start()
    {
        //탄창 이미지 초기화
        magazineImg.fillAmount = 1;
        //남은 총알 수 갱신
        UpdateBulletText();
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        //shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            //총알 수를 하나 감소
            --remainBullet;
            Fire();
            //남은 총알이 없을 경우 재장전 코루틴 호출
            if(remainBullet == 0)
            {
                Debug.Log("재장전");
                StartCoroutine(Reloading());
            }
        }
        if (!isReloading && Input.GetKey("r") && remainBullet < maxBullet)
        {
            StartCoroutine(Reloading());
        }
    }

    public void Fire()
    {
        //StartCoroutine(shake.ShakeCamera());

        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        cartridge.Play();
        muzzleFlash.Play();
        FireSfx();
        //재장전 이미지의 fillAmount 속성값 지정
        magazineImg.fillAmount = (float)remainBullet / (float)maxBullet;
        UpdateBulletText();
    }

    void FireSfx()
    {
        //현재 들고 있는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        _audio.PlayOneShot(_sfx, 1.0f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1.0f);
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainBullet = maxBullet;
        //남은 총알 수 갱신
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        magazineTxt.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainBullet, maxBullet);
    }

    public void OnChangeWeapon()
    {
        currWeapon = (WeaponType)((int)++currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
}
