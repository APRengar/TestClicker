using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "IdleClicker/GameSetings")]
public class GameSetings : ScriptableObject
{
    [Header("Energy Settings")]
    public int maxEnergy = 1000;
    public int energyPerClick = 1;
    public int autoClickCost = 1;
    public int energyRechargeAmount = 10;
    public float energyRechargeInterval = 10f;

    [Header("Currency Settings")]
    public GameObject currencyPrefab;
    public GameObject clickVFX;
    public AudioClip coinSound;
    public int currencyPerClick = 1;

    [Header("Auto Click Settings")]
    public float autoClickInterval = 3f;
}