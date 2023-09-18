using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Game.SystemsManagement
{
    public class SystemsManager : MonoBehaviour
    {
        private static SystemsManager _instance;
        [SerializeField] private GameSystem[] _gameSystems;
        private static Dictionary<System.Type, GameSystem> _systems;
        private bool _isInitialized = false;
        private int _currentSystem = -1;
        private static bool _allSystemsInitialized = false;

        public static bool AllSystemsInitialized
        {
            get { return _allSystemsInitialized; }
        }

        private void Awake()
        {
            _instance = this;
            Initialize();
            ContinueSystemsInitialization();
        }
        private static void FindInstance()
        {
            _instance = FindFirstObjectByType<SystemsManager>();
            _instance.Initialize();
        }
        private void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _systems = new Dictionary<System.Type, GameSystem>();

            System.Type systemType;
            foreach (var system in _gameSystems)
            {
                systemType = system.GetType();
                _systems.Add(systemType, system);
            }
        }
        public static T GetSystemOfType<T>() where T : GameSystem
        {
            if (_instance == null)
                FindInstance();

            System.Type systemType = typeof(T);

            if (_systems.TryGetValue(systemType, out GameSystem systemInstance))
            {
                return (T)systemInstance;
            }
            else
            {
                Debug.LogError(string.Format("System of type {0} is not exist on current scene!", systemType.Name));
                return default;
            }
        }
        private void Update()
        {
            if (!_allSystemsInitialized) return;

            foreach (var system in _gameSystems)
            {
                system.OnUpdate();
            }
        }

        private void ContinueSystemsInitialization()
        {
            for (int i = _currentSystem + 1; i < _gameSystems.Length; i++)
            {
                _currentSystem = i;
                if (!_gameSystems[i].AsyncInitialization)
                    _gameSystems[i].Initialize(null);
                else
                {
                    _gameSystems[i].Initialize(ContinueSystemsInitialization);
                    break;
                }
                if (i == _gameSystems.Length - 1) _allSystemsInitialized = true;
            }
        }
    }
}