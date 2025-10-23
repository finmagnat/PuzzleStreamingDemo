using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Profiling;

public class ZoneStreamer : MonoBehaviour
{
    [System.Serializable]
    public class Zone
    {
        public string addressableSceneName;
        public Vector3 center;
        public float enterRadius = 10f;
        public float exitRadius = 12f;

        [HideInInspector] public bool isLoaded;
        [HideInInspector] public bool isLoading;
    }

    [Header("References")]
    public Transform player;
    public LoadingIndicator loadingIndicator;

    [Header("Zones")]
    public Zone[] zones;

    [Header("Options")]
    public bool simulateSlowLoad = false;
    [Range(0f, 5f)] public float loadDelay = 1.5f;

    private void Update()
    {
        if (player == null || zones == null) return;

        foreach (var zone in zones)
        {
            float distance = Vector3.Distance(player.position, zone.center);

            if (!zone.isLoaded && !zone.isLoading && distance <= zone.enterRadius)
                StartCoroutine(LoadZone(zone));
            else if (zone.isLoaded && distance > zone.exitRadius)
                StartCoroutine(UnloadZone(zone));
        }
    }

    private IEnumerator LoadZone(Zone zone)
    {
        if (zone.isLoaded) yield break;
        zone.isLoading = true;

        Debug.Log($"▶ Loading {zone.addressableSceneName}...");
        loadingIndicator?.ShowLoading(zone.addressableSceneName);

        if (simulateSlowLoad && loadDelay > 0f)
            yield return new WaitForSeconds(loadDelay);

        var handle = Addressables.LoadSceneAsync(zone.addressableSceneName, LoadSceneMode.Additive);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            zone.isLoaded = true;
            Debug.Log($"✅ Zone loaded: {zone.addressableSceneName}");
            LogMem($"After LOAD {zone.addressableSceneName}");
        }
        else
        {
            Debug.LogError($"❌ Failed to load zone: {zone.addressableSceneName}");
        }

        loadingIndicator?.HideLoading();
        zone.isLoading = false;
    }

    private IEnumerator UnloadZone(Zone zone)
    {
        if (zone.isLoading || !zone.isLoaded)
            yield break;

        Debug.Log($"⏹ Unloading {zone.addressableSceneName}...");
        loadingIndicator?.ShowLoading(zone.addressableSceneName);

        if (simulateSlowLoad && loadDelay > 0f)
            yield return new WaitForSeconds(loadDelay);

        Scene scene = SceneManager.GetSceneByName(zone.addressableSceneName);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.Log($"⚠️ Scene {zone.addressableSceneName} already unloaded or invalid — skipping.");
            zone.isLoaded = false;
            loadingIndicator?.HideLoading();
            yield break;
        }

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene);
        yield return unloadOp;

        if (unloadOp.isDone)
        {
            zone.isLoaded = false;
            Debug.Log($"🧹 Zone unloaded: {zone.addressableSceneName}");
            LogMem($"After UNLOAD {zone.addressableSceneName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Unload may not have completed properly: {zone.addressableSceneName}");
        }

        loadingIndicator?.HideLoading();
    }

    // --- Memory diagnostics ---
    private static void LogMem(string tag)
    {
        long mem = Profiler.GetTotalAllocatedMemoryLong(); // bytes
        int goCount = Resources.FindObjectsOfTypeAll<GameObject>().Length;
        Debug.Log($"[MEM] {tag} | Alloc: {(mem / 1048576f):0.0} MB | GameObjects: {goCount}");
    }

    private void OnDrawGizmos()
    {
        if (zones == null) return;

        foreach (var z in zones)
        {
            Color activeColor = z.isLoaded ? Color.cyan : Color.green;
            Gizmos.color = activeColor;
            Gizmos.DrawWireSphere(z.center, z.enterRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(z.center, z.exitRadius);
        }
    }
}
