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
    DeliveryOrderSystem orderSystem;

    public void Start()
    {
        SetupBuilding();
        orderSystem = FindFirstObjectByType<DeliveryOrderSystem>();
        CreateNameTag();
    }
    void HandleDriverService(DeliveryDriver driver)
    {
        switch (buildingType)
        {
            case BuildingType.Restautant:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredRestaurant(this);
                }
                break;

            case BuildingType.Customer:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredCustomer(this);
                }
                else
                {
                    driver.CompleteDelivery();
                }
                break;

            case BuildingType.ChangingStation:
                driver.ChargeBattery();
                break;
        }
        buildingEvents.OnServiceUsed?.Invoke(buildingType);
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

                    break;
                case BuildingType.Customer:
                    mat.color = Color.green;

                    break;
                case BuildingType.ChangingStation:
                    mat.color = Color.yellow;
                    break;
            }
        }
        Collider col = GetComponent<Collider>();
        if (col != null) { col.isTrigger = true; }
    }

    void CreateNameTag()
    {
        //건물 위에 이름표 생성
        GameObject nameTag = new GameObject("NameTag");
        nameTag.transform.SetParent(transform);
        nameTag.transform.localPosition = Vector3.up * 1.5f;

        TextMesh textMesh = nameTag.AddComponent<TextMesh>();
        textMesh.text = buildingName;
        textMesh.characterSize = 0.2f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.fontSize = 20;

        nameTag.AddComponent<BillBoard>();
    }
}
