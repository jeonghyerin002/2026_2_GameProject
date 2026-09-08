using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("건물 정보")]
    public BuildingType buildingType;
    public string buildingName = "건물";

    [System.Serializable]
    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExited;
        public UnityEvent<BuildingType> OnServiceUsed;
    }

    public BuildingEvents buildingEvents;

    public void Start()
    {
        SetupBuilding();
    }
    void HandleDriverService(DeliveryDriver driver)
    {
        switch (buildingType)
        {
            case BuildingType.Restautant:
                Debug.Log($"{buildingName} 에서 음식을 픽업했습니다.");
                break;
            case BuildingType.Customer:
                Debug.Log($"{buildingName} 배달 완료");
                driver.CompleteDelivery();
                break;
            case BuildingType.ChangingStation:
                Debug.Log($"{buildingName} 에서 배터리를 충전 했습니다.");
                driver.ChargeBattery();
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if (driver != null)
        {
            buildingEvents.OnDriverEntered?.Invoke(buildingName);
            HandleDriverService(driver);
        }
    }
    void OnTriggerExit(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if (driver != null)
        {
            buildingEvents.OnDriverExited?.Invoke(buildingName);
            Debug.Log($"{buildingName} 을 떠났습니다. ");
        }
    }
    void SetupBuilding()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material mat = renderer.material;

            switch (buildingType)
            {
                case BuildingType.Restautant:
                    mat.color = Color.red;
                    buildingName = "음식점";
                    break;
                case BuildingType.Customer:
                    mat.color = Color.green;
                    buildingName = "배달지";
                    break;
                case BuildingType.ChangingStation:
                    mat.color = Color.yellow;
                    buildingName = "충전소";
                    break;
            }
        }
        Collider col = GetComponent<Collider>();
        if (col != null) { col.isTrigger = true; }
    }
}
