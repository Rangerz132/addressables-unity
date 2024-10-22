using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    [SerializeField] private AssetReference assetReference; // Unique GameObject via an AssetReference
    [SerializeField] private List<AssetReference> assetReferences; // List of unique GameObject
    [SerializeField] private AssetLabelReference assetLabelReference; // Multiple GameObject via an AssetLabelReference
    
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadAssetReference();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadAssetLabelReference();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadAssetLabelReferences();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(LoadAssetReferencesOneByOne());
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(LoadAssetLabelReferencesOneByOne());
        }
    }

    private void LoadAssetReference() {
        assetReference.LoadAssetAsync<GameObject>().Completed += (asyncOperationHandle) =>
        {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Instantiate(asyncOperationHandle.Result);
            }
            else
            {
                Debug.Log("Failed to load...");
            }
        };
    }

    private void LoadAssetLabelReference()
    {
        Addressables.LoadAssetAsync<GameObject>(assetLabelReference).Completed += (asyncOperationHandle) =>
        {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Instantiate(asyncOperationHandle.Result);
            }
            else
            {
                Debug.Log("Failed to load...");
            }
        };
    }

    private void LoadAssetLabelReferences()
    {
        Addressables.LoadAssetsAsync<GameObject>(assetLabelReference, (gameObject => {
            Instantiate(gameObject);
        })).Completed += (asyncOperationHandle) =>
        {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("All Assets loaded...");
            }
            else
            {
                Debug.Log("Failed to load...");
            }
        };
    }

    private IEnumerator LoadAssetReferencesOneByOne()
    {
        foreach (AssetReference assetReference in assetReferences)
        {

            AsyncOperationHandle<GameObject> handle = assetReference.LoadAssetAsync<GameObject>();

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Instantiate(handle.Result);
                Debug.Log("Loaded asset: " + assetReference.RuntimeKey.ToString());
            }
            else
            {
                Debug.LogError("Failed to load... " + assetReference.RuntimeKey.ToString());
            }

            // Additional delay
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator LoadAssetLabelReferencesOneByOne()
    {
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(assetLabelReference, null);

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (GameObject asset in handle.Result)
            {
                Instantiate(asset);
                Debug.Log("Loaded and instantiated asset: " + asset.name);

                //yield return null;

                // Additional delay
                yield return new WaitForSeconds(1f);
            }

            Debug.Log("All assets loaded and instantiated successfully.");
        }
        else
        {
            Debug.LogError("Failed to load assets for label: " + assetLabelReference.labelString);
        }
    }
}
