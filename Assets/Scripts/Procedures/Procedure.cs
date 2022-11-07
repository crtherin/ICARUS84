#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Procedures
{
    public class Procedure : MonoBehaviour
    {
        public event EventHandler OnCallback;

        private int? lastHash;
        private bool isRunning;
        private bool shouldStop;
        private Callbacks callbacks;
        private List<Process> processes;
        private ProcedureHandler procedureHandler;
        [SerializeField] private VariationTree tree;
        [SerializeField] [ReadOnlyPlayMode] private SerializedProcedure serializedProcedure;

        private List<Process> enable;
        private List<Process> disable;

        public Callbacks Callbacks => callbacks;

        private string PrefsKey
        {
            get { return "Procedure " + serializedProcedure.name; }
        }

        public bool IsRunning => isRunning;

#if UNITY_EDITOR
        private bool areProcessesFolded;
#endif

        public void Save()
        {
            PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(tree));
        }

        protected void Awake()
        {
            callbacks = new Callbacks();
            processes = new List<Process>();

            enable = new List<Process>();
            disable = new List<Process>();

#if UNITY_EDITOR
            serializedProcedure = serializedProcedure.GetAssetReference();
#endif
            tree = serializedProcedure.GetTree().Copy();

            procedureHandler = GetComponentInParent<ProcedureHandler>();

            if (procedureHandler != null)
                procedureHandler.AddProcedure(serializedProcedure.GetName(), this);

            serializedProcedure.AddProcessesTo(this);
            callbacks.Initialize();

            if (tree != null)
                ApplyVariation();
        }

        protected void Start()
        {
            if (PlayerPrefs.HasKey(PrefsKey))
            {
                string json = PlayerPrefs.GetString(PrefsKey);
                var loadedTree = JsonUtility.FromJson<VariationTree>(json);

                if (loadedTree != null)
                {
                    tree = loadedTree;
                    ApplyVariation();
                }
            }
        }

        protected void OnEnable()
        {
            if (procedureHandler == null)
                return;

            procedureHandler.AddProcedure(serializedProcedure.GetName(), this);
        }

        protected void OnDisable()
        {
            if (procedureHandler == null)
                return;

            procedureHandler.RemoveProcedure(serializedProcedure.GetName());
        }

        protected void Update()
        {
            if (processes == null || lastHash == null)
                return;

            callbacks.PassiveUpdate();

            if (!isRunning)
            {
                if (tree != null && lastHash != tree.GetHash())
                    ApplyVariation();

                WaitForCanRun();
                return;
            }

            callbacks.Update();

            if (shouldStop)
            {
                shouldStop = false;

                if (isRunning)
                {
                    isRunning = false;
                    callbacks.Stop();
                }
            }
        }

        private void WaitForCanRun()
        {
            if (!callbacks.CanRun())
                return;

            isRunning = true;
            callbacks.Start();

            if (procedureHandler != null)
                procedureHandler.AddActive(serializedProcedure.GetName());
        }

        public void Stop()
        {
            if (!isRunning)
                return;

            shouldStop = true;

            if (procedureHandler != null)
                procedureHandler.RemoveActive(serializedProcedure.GetName());
        }

        public void Callback()
        {
            OnCallback.SafeInvoke(this, null);
        }

        public int GetIndex(Process process)
        {
            for (int i = 0; i < processes.Count; i++)
            {
                if (processes[i] == process)
                    return i;
            }

            return -1;
        }

        public T GetProcess<T>() where T : Process
        {
            return processes.FirstOrDefault(p => p.IsEnabled() && p is T) as T;
        }

        public T GetProcessUpwards<T>(Process process) where T : Process
        {
            T lastFound = null;

            for (int i = 0; i < processes.Count; i++)
            {
                if (processes[i] == process)
                    return lastFound;

                if (processes[i].IsEnabled() && processes[i] is T)
                    lastFound = processes[i] as T;
            }

            return null;
        }

        public T AddProcess<T>(int hash = default(int), bool initialize = true) where T : Process
        {
            return (T) AddProcess(typeof(T), hash);
        }

        public Process AddProcess(Type type, int hash = default(int), bool doSetup = true)
        {
            return AddProcess((Process) Activator.CreateInstance(type), hash);
        }

        private T AddProcess<T>(T process, int hash = default(int), bool doSetup = true) where T : Process
        {
            process.SetHash(hash);
            process.SetProcedure(this);
            processes.Add(process);
            callbacks.AddAll(process);

            if (doSetup)
            {
                IInitialize initialize = process as IInitialize;

                if (initialize != null)
                    initialize.Initialize();

                IRefresh refresh = process as IRefresh;

                if (refresh != null)
                    refresh.Refresh();

                if (serializedProcedure.GetTree() != null)
                    process.SetEnabledFrom(serializedProcedure.GetTree());
            }

            return process;
        }

        private void ApplyVariation()
        {
            if (lastHash != null && lastHash == tree.GetHash())
                return;

            lastHash = tree.GetHash();

            enable.Clear();
            disable.Clear();

            lastHash = tree.GetHash();

            foreach (Process process in processes)
            {
                process.SetEnabledFrom(tree, enable, disable);
            }

            foreach (Process process in disable)
            {
                if (process is IDisabled)
                    process.Cast<IDisabled>().Disabled();
            }

            callbacks.Refresh();

            foreach (Process process in enable)
            {
                if (process is IEnabled)
                    process.Cast<IEnabled>().Enabled();
            }
        }

        public bool RemoveProcess<T>() where T : Process
        {
            for (int i = 0; i < processes.Count; i++)
            {
                if (processes[i] is T)
                {
                    if (processes[i].IsEnabled())
                        if (processes[i] is IDisabled)
                            processes[i].Cast<IDisabled>().Disabled();

                    processes.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public VariationTree GetTree()
        {
            return tree;
        }

        public List<Process> GetProcesses()
        {
            return processes;
        }

#if UNITY_EDITOR

        [ContextMenu("New Procedure")]
        public void NewProcedureAsset()
        {
            string path = EditorUtility.SaveFilePanel("Create New Procedure", "Assets", "New Procedure", "asset");

            if (path.Length != 0)
            {
                SerializedProcedure newProcedure = ScriptableObjectUtility.CreateAsset<SerializedProcedure>(path);

                if (newProcedure != null)
                {
                    newProcedure.SetName(Path.GetFileName(path));

                    if (serializedProcedure != null)
                        newProcedure.SetContainer(serializedProcedure.GetContainer());

                    serializedProcedure = newProcedure;
                }
            }

            Selection.activeGameObject = gameObject;
        }
#endif
    }
}