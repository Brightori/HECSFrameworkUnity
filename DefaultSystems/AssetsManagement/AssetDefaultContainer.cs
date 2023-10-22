using AssetsManagement.Containers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HECSFramework.HECS.Unity.DefaultSystems.AssetsManagement
{
    public class AssetDefaultContainer : IAssetGameObjectContainer
    {
        private readonly GameObject asset;
        
        public GameObject Asset => asset;

        public AssetDefaultContainer(GameObject asset)
        {
            this.asset = asset;
        }
        
        public UniTask<GameObject> CreateInstance(Vector3 pos, Quaternion rot, Transform parent = null)
        {
            return UniTask.FromResult(Object.Instantiate(asset, pos, rot, parent));
        }

        public UniTask<TComponent> CreateInstanceForComponent<TComponent>(Vector3 pos = default, Quaternion rot = default,
            Transform parent = null) where TComponent : Component
        {
            var obj = Object.Instantiate(asset, pos, rot, parent);
            return UniTask.FromResult(obj.GetComponent<TComponent>());
        }

        public void ReleaseInstance(GameObject instance)
        {
            Object.Destroy(instance);
        }
    }
}