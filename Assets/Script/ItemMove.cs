using UnityEngine;

public class ItemMove : MonoBehaviour
{
    public enum MoveMode
    {
        Horizontal = 1, // ì¢Œìš° ?´ë™
        Vertical = 2,   // ?„ì•„???´ë™
        Circle   = 3,   // ???´ë™
        Teleport = 4    // ?œê°„?´ë™
    }

    [Header("ê¸°ë³¸ ?¤ì •")]
    public MoveMode mode = MoveMode.Horizontal; // ?¸ìŠ¤?™í„°?ì„œ ëª¨ë“œ ? íƒ
    public float distance = 2f;                  // ì¢Œìš°/?„ì•„???´ë™ ê±°ë¦¬
    public float speed = 2f;                     // ?´ë™/?Œì „ ?ë„

    [Header("???´ë™ ?¤ì • (Circle ëª¨ë“œ ?¬ìš©)")]
    public float radius = 2f;                    // ??ë°˜ì?ë¦?

    [Header("?œê°„?´ë™ ?¤ì • (Teleport ëª¨ë“œ ?¬ìš©)")]
    public Transform point1;                     // 1ë²??„ì¹˜
    public Transform point2;                     // 2ë²??„ì¹˜
    public Transform point3;                     // 3ë²??„ì¹˜
    public float teleportInterval = 1.0f;        // ëª?ì´ˆë§ˆ???œê°„?´ë™? ì?

        Horizontal = 1, // ÁÂ¿ì ÀÌµ¿
        Vertical = 2,   // À§¾Æ·¡ ÀÌµ¿
        Circle = 3,   // ¿ø ¿îµ¿
        Teleport = 4    // ¼ø°£ÀÌµ¿
    }

    [Header("±âº» ¼³Á¤")]
    public MoveMode mode = MoveMode.Horizontal; // ÀÎ½ºÆåÅÍ¿¡¼­ ¸ðµå ¼±ÅÃ
    public float distance = 2f;                  // ÁÂ¿ì/À§¾Æ·¡ ÀÌµ¿ °Å¸®
    public float speed = 2f;                     // ÀÌµ¿/È¸Àü ¼Óµµ

    [Header("¿ø ¿îµ¿ ¼³Á¤ (Circle ¸ðµå »ç¿ë)")]
    public float radius = 2f;                    // ¿ø ¹ÝÁö¸§

    [Header("¼ø°£ÀÌµ¿ ¼³Á¤ (Teleport ¸ðµå »ç¿ë)")]
    public Transform point1;                     // 1¹ø À§Ä¡
    public Transform point2;                     // 2¹ø À§Ä¡
    public Transform point3;                     // 3¹ø À§Ä¡
    public float teleportInterval = 1.0f;        // ¸î ÃÊ¸¶´Ù ¼ø°£ÀÌµ¿ÇÒÁö


    private Vector3 startPos;
    private float teleportTimer = 0f;
    private int teleportIndex = 0;

    void Start()
    {
        startPos = transform.position;

        if (mode == MoveMode.Teleport)
        {
            Transform first = GetTeleportTarget(0);
            if (first != null)
                transform.position = first.position;
        }
    }

    void Update()
    {
        switch (mode)
        {
            case MoveMode.Horizontal:
                MoveHorizontal();
                break;

            case MoveMode.Vertical:
                MoveVertical();
                break;

            case MoveMode.Circle:
                MoveCircle();
                break;

            case MoveMode.Teleport:
                TeleportMove();
                break;
        }
    }

    void MoveHorizontal()
    {
        float newX = startPos.x + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(newX, startPos.y, startPos.z);
    }

>>>>>>> Stashed changes
    void MoveVertical()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }


>>>>>>> Stashed changes
    void MoveCircle()
    {
        float t = Time.time * speed;
        float newX = startPos.x + Mathf.Cos(t) * radius;
        float newY = startPos.y + Mathf.Sin(t) * radius;
        transform.position = new Vector3(newX, newY, startPos.z);
    }


    void TeleportMove()
    {
        teleportTimer += Time.deltaTime;
        if (teleportTimer >= teleportInterval)
        {
            teleportTimer = 0f;


            teleportIndex = (teleportIndex + 1) % 3; // 0 ??1 ??2 ??0 ??

            teleportIndex = (teleportIndex + 1) % 3; // 0 ¡æ 1 ¡æ 2 ¡æ 0 ¡¦

            Transform target = GetTeleportTarget(teleportIndex);
            if (target != null)
            {
                transform.position = target.position;
            }
        }
    }

    Transform GetTeleportTarget(int index)
    {
        switch (index)
        {
            case 0: return point1;
            case 1: return point2;
            case 2: return point3;
            default: return null;
        }
    }
