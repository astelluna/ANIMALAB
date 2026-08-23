using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : Singleton<IAPManager>
{
    private const string REMOVE_ADS_ID = "remove_ads";

    private StoreController storeController;

    public bool IsAdRemoved { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        InitializeIAP();
    }

    private async void InitializeIAP()
    {
        storeController = UnityIAPServices.StoreController();

        // 이벤트 등록
        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnProductsFetchFailed += OnProductsFetchFailed;

        storeController.OnPurchasesFetched += OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

        storeController.OnPurchasePending += OnPurchasePending;

        storeController.OnStoreDisconnected += OnStoreDisconnected;

        // Google Play 연결
        await storeController.Connect();

        // 판매할 상품 등록
        var products = new List<ProductDefinition>
        {
            new ProductDefinition(
                REMOVE_ADS_ID,
                ProductType.NonConsumable
            )
        };

        // 상품 정보 가져오기
        storeController.FetchProducts(products);
    }
    //상품을 정상적으로 가져왔을 때
    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log("상품 불러오기 성공");

        // 기존 구매 내역 확인
        storeController.FetchPurchases();
    }
    //기존에 광고 제거를 샀는지 확인
    private void OnPurchasesFetched(Orders orders)
    {
        IsAdRemoved = false;

        foreach (var order in orders.ConfirmedOrders)
        {
            foreach (var item in order.CartOrdered.Items())
            {
                if (item.Product.definition.id == REMOVE_ADS_ID)
                {
                    IsAdRemoved = true;

                    Debug.Log("광고 제거 구매 상태 확인");
                    return;
                }
            }
        }

        Debug.Log("광고 제거 미구매");
    }
    //구매 버튼을 눌렀을 때
    public void BuyRemoveAds()
    {
        if (storeController == null)
        {
            Debug.Log("IAP 초기화 중");
            return;
        }

        if (IsAdRemoved)
        {
            Debug.Log("이미 광고 제거를 구매했습니다.");
            return;
        }

        storeController.PurchaseProduct(REMOVE_ADS_ID);
    }
    private void OnPurchasePending(PendingOrder order)
    {
        foreach (var item in order.CartOrdered.Items())
        {
            if (item.Product.definition.id == REMOVE_ADS_ID)
            {
                IsAdRemoved = true;

                Debug.Log("광고 제거 구매 완료");
            }
        }

        // 구매 완료 처리
        storeController.ConfirmPurchase(order);
    }
    //실패 처리
    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.LogError(
            $"상품 가져오기 실패 : {failure}"
        );
    }

    private void OnPurchasesFetchFailed(
        PurchasesFetchFailureDescription failure)
    {
        Debug.LogError(
            $"구매 내역 가져오기 실패 : {failure}"
        );
    }

    private void OnStoreDisconnected(
        StoreConnectionFailureDescription failure)
    {
        Debug.LogError(
            $"스토어 연결 실패 : {failure}"
        );
    }
}
