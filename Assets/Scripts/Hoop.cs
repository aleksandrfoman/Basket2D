using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    [SerializeField]
    private Transform hoopNet, netPoint, hoopPointParent;
    public Transform ParentPoint => hoopPointParent;
    [SerializeField]
    private Vector3 startPosParent;
    [SerializeField]
    private float minScale, maxScale;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private Transform spawnStarPos;
    [SerializeField]
    private GameObject starPrefab;
    [SerializeField]
    private float chanceSpawnStar;

    private void Start()
    {
        SpawnStar();
    }

    public void SpawnStar()
    {
        float rndValue = Random.Range(0f, 1f);
        if (rndValue<=chanceSpawnStar)
            Instantiate(starPrefab, spawnStarPos.position, Quaternion.identity);
    }
    public void UpdateScaleHoopNet(float modif)
    {
        float scaleFactor = Mathf.Clamp(modif, minScale, maxScale);
        hoopNet.localScale = new Vector3(1f,scaleFactor*1f, 1f);
        hoopPointParent.transform.position = netPoint.transform.position;
    }

    public void RotateHoop(Vector2 dir)
    {
        var angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0f, 0f, angle-90f);
    }

    public void ResetScaleHoopNet()
    {
        hoopNet.localScale = Vector3.one;
        hoopPointParent.localPosition = startPosParent;
        LevelController.Instance.DeacitaveteHoop();
    }
    
    public void SetParent()
    {
        LevelController.Instance.SetHoop(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            SetParent();
            if (LevelController.Instance.GetFirstHope().name != this.name)
            {
                LevelController.Instance.RemoveHoop();
                LevelController.Instance.AddHoop();
                LevelController.Instance.AddScore();
                audioSource.Play();
            }
        }
    }
}
