using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUIManager : MonoBehaviour
{
    [Header("UI 요소")]
    public Text statusText;
    public Text messageText;
    public Slider batterySlider;
    public Image batteryFill;
    public DeliveryDriver driver;

    void Start()
    {
        if (driver !=  null)
        {
            driver.driverEvents.OnMoneyChanged.AddListener(UpdateMoney);
            driver.driverEvents.OnBatteryCharged.AddListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountCharged.AddListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.AddListener(OnMoveStarted);
            driver.driverEvents.OnmoveStopped.AddListener(OnMoveStopped);
            driver.driverEvents.OnLowBattery.AddListener(OnLowBattery);
            driver.driverEvents.OnLowBatteryEmpty.AddListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);


        }
    }


    void Update()
    {
        
    }
    void ShowMessage(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            StartCoroutine(ClearMessageAfterDelay(2f));
        }
    }
    IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    void UpdateMoney(float money)
    {
        ShowMessage($"돈 : {money} 원", Color.green);
    }
    void UpdateBattery(float battery)
    {
        if (batterySlider != null)
        {
            batterySlider.value = battery / 100f;
        }

        if (batteryFill != null)
        {
            if (battery > 50f)
            {
                batteryFill.color = Color.green;
            }
            else if (battery > 20f)
            {
                batteryFill.color = Color.yellow;
            }
            else if (battery > 10f)
            {
                batteryFill.color = Color.red;
            }
        }
    }
    void UpdateDeliveryCount(int count)
    {
        ShowMessage($"배달 완료 : {count}건", Color.blue);
    }
    void OnMoveStarted()
    {
        ShowMessage("이동 시작", Color.cyan);
    }
    void OnMoveStopped()
    {
        ShowMessage("이동 정지", Color.gray);
    }
    void OnLowBattery()
    {
        ShowMessage("배터리 부족", Color.red);
    }
    void OnBatteryEmpty()
    {
        ShowMessage("배터리 방전", Color.red);
    }
    void OnDeliveryCompleted()
    {
        ShowMessage("배달 완료", Color.green);
    }
    void UpdateUI()
    {
        if (driver != null)
        {
            UpdateMoney(driver.currentMoney);
            UpdateBattery(driver.batteryLevel);
            UpdateDeliveryCount(driver.deliveryCount);
        }
    }
}
