using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowController : MonoBehaviourPun
{
    [TabGroup("Arrow"), SerializeField]
    private int maxArrows;
    [TabGroup("Arrow"), SerializeField, ProgressBar(0, "maxArrows", 255, 255, 255, Segmented = true)]
    private int currArrows;

    [TabGroup("Setup")]
    public float DrawTime
    {
        get { return drawTime; }
        set
        {
            drawTime = value;
            if (.2f > drawTime)
            {
                drawTime = .2f;
            }
        }
    }


    [TabGroup("Setup"), SerializeField]
    private float drawTime;
    [TabGroup("Setup")]
    public float timeBetweenReload;
    [TabGroup("Setup"), SerializeField]
    private Transform castPoint;
    [TabGroup("Arrow")]
    public bool isDrawing;
    [TabGroup("Arrow")]
    public bool isReloading;
    [TabGroup("Setup")]
    public Sprite bombArrowIcon;
    [TabGroup("Arrow")]
    public Projectile currentArrow;
    [TabGroup("Setup"), SerializeField]
    private float bombCooldown;
    [TabGroup("Setup"), SerializeField]
    private Projectile bombArrow;
    [TabGroup("Setup"), SerializeField]
    private Animator animator;
    private float drawStartTime;
    private EffectVisualController effectVisualController;
    private PlayerManger manger;
    private PlayerInput playerInput;
    private bool stop;
    private float currentBombTimer;
    private bool bombOnCooldown;
    private Projectile arrowCache;
    private bool bombEquiped;

    [TabGroup("Audio"), Required, SerializeField]
    private SFX arrowDraw;
    [TabGroup("Audio"), Required, SerializeField]
    private WeightedRandomList<SFX> FireSounds;
    [TabGroup("Audio"), Required, SerializeField]
    private WeightedRandomList<SFX> ReloadSounds;


    private string currentState;
    public int MaxArrows
    {
        get { return maxArrows; }
        set
        {
            maxArrows = value;
            if (currArrows > maxArrows)
            {
                CurrArrows = MaxArrows;
            }
        }
    }
    public int CurrArrows
    {
        get { return currArrows; }
        private set
        {
            currArrows = value;
            if (currArrows > maxArrows)
            {
                currArrows = maxArrows;
            }
        }
    }
    private void Awake()
    {
        effectVisualController = GetComponent<EffectVisualController>();
        manger = GetComponent<PlayerManger>();
        currArrows = maxArrows;
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Attack"].performed += OnShootPerformed;
        playerInput.actions["Attack"].canceled += OnShootCanceled;
    }
    private void Update()
    {
        if (isDrawing && Input.GetKeyDown(KeyCode.C))
        {
            UpdateDraw(false);
        }

        if (manger.isRollExecuting)
        {
            UpdateDraw(false);
        }

        if (currentBombTimer > 0)
        {
            bombOnCooldown = true;
            currentBombTimer -= Time.deltaTime;
        }
        else if (currentBombTimer < 0)
        {
            bombOnCooldown = false;
            currentBombTimer = 0;
        }

        if (readyToFire() && isDrawing)
        {
            animator.SetBool("holding", true);
        }
        else
        {
            animator.SetBool("holding", false);
        }
        if (!animator.GetBool("drawing"))
        {
            animator.ResetTrigger("shoot");
            animator.ResetTrigger("Attack 0");
        }
    }
    private void OnShootPerformed(InputAction.CallbackContext ctx)
    {
        if (currArrows > 0 && !isDrawing)
        {
            if (isReloading)
            {
                StopCoroutine(ReloadArrowsCoroutine());
                isReloading = false;
                stop = true;
            }
            manger.photonView.RPC("UpdateAttack", RpcTarget.All);
            UpdateDraw(true);
            drawStartTime = Time.time;

        }
    }
    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {

        if (readyToFire())
        {
            UpdateDraw(false);
            animator.SetTrigger("shoot");
            FireArrow();
        }
        else
        {
            UpdateDraw(false);
        }

    }
    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed && photonView.IsMine && currArrows < maxArrows && !isReloading && !isDrawing)
            StartCoroutine(ReloadArrowsCoroutine());
        else if (context.performed && photonView.IsMine && isReloading)
        {
            StopCoroutine(ReloadArrowsCoroutine());
            isReloading = false;
            stop = true;
        }

    }
    public void BombArrow(InputAction.CallbackContext context)
    {
        if (context.performed && photonView.IsMine && currArrows > 0 && !bombOnCooldown && !bombEquiped)
        {
            bombEquiped = true;
            arrowCache = currentArrow;
            currentArrow = bombArrow;
            PlayerUi.Instance.SecondaryEquipped(true);
        }
        else if ((context.performed && photonView.IsMine && bombEquiped))
        {
            bombEquiped = false;
            currentArrow = arrowCache;
            PlayerUi.Instance.SecondaryEquipped(false);
        }
    }
    private IEnumerator ReloadArrowsCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(timeBetweenReload);
        CurrArrows++;
        photonView.RPC("ReloadSFX", RpcTarget.All);
        if (currArrows != maxArrows && !stop)
            StartCoroutine(ReloadArrowsCoroutine());
        else if (currArrows == maxArrows && !stop)
            isReloading = false;
        if (stop)
        {
            isReloading = false;
            stop = false;
        }
    }
    private void FireArrow()
    {
        currArrows--;
        photonView.RPC("FireSFX", RpcTarget.All);
        if (!bombEquiped)
        {
            GameObject arrow = PhotonNetwork.Instantiate(currentArrow.name, castPoint.position, castPoint.rotation);
            arrow.GetComponent<Projectile>().Setup(manger.character, manger, null);

        }
        else if (bombEquiped)
        {
            GameObject arrow = PhotonNetwork.Instantiate(bombArrow.name, castPoint.position, castPoint.rotation);
            arrow.GetComponent<Projectile>().Setup(manger.character, manger, hasEffects());
            currentArrow = arrowCache;
            PlayerUi.Instance.SecondaryEquipped(false);
            bombEquiped = false;
            bombOnCooldown = true;
            currentBombTimer = bombCooldown;
            PlayerUi.Instance.SecondaryCooldownGFX(bombCooldown);
        }
    }
    private StatusEffectSO hasEffects()
    {
        if (arrowCache.statusEffects.Count > 0)
            return arrowCache.statusEffects[0];
        else
            return null;
    }

    public void UpdateDraw(bool draw)
    {

        isDrawing = draw;
        animator.SetFloat("DrawSpeed", DrawSpeedScale());
        animator.SetBool("drawing", draw);
        photonView.RPC("DrawSFX", RpcTarget.All, draw);
    }
    public bool readyToFire()
    {
        if (isDrawing)
        {
            float drawDuration = Time.time - drawStartTime;
            if (drawDuration >= drawTime)
            {
                return true;
            }
            else if (drawDuration < drawTime)
            {
                return false;
            }
        }
        return false;
    }

    private float DrawSpeedScale()
    {
        switch (drawTime)
        {
            case 1:
                return 1;
            case .9f:
                return 1.1f;
            case .8f:
                return 1.2f;
            case .7f:
                return 1.3f;
            case .6f:
                return 1.4f;
            case .5f:
                return 1.5f;
            case .4f:
                return 1.6f;
            case .3f:
                return 1.7f;
            case .2f:
                return 1.8f;
            default: return 1;
        }
    }
    [PunRPC]
    private void FireSFX()
    {
        SFX soundToPlay = FireSounds.GetRandom();
        soundToPlay.PlaySFX();
    }
    [PunRPC]
    private void DrawSFX(bool state)
    {
        if (state)
            arrowDraw.PlaySFX();
        else if (!state)
            arrowDraw.StopSFX();
    }

    [PunRPC]
    private void ReloadSFX()
    {
        SFX soundToPlay = ReloadSounds.GetRandom();
        soundToPlay.PlaySFX();
    }
}





