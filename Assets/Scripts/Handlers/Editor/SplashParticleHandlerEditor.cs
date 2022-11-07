using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Handlers.Editor
{
    [CustomEditor(typeof(SplashParticleHandler))]
    public class SplashParticleHandlerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            
            container.Add(new PropertyField(serializedObject.FindProperty("particlePrefab")));
            container.Add(new PropertyField(serializedObject.FindProperty("coneAngle")));
            container.Add(new PropertyField(serializedObject.FindProperty("velocity")));
            container.Add(new PropertyField(serializedObject.FindProperty("drag")));
            container.Add(new PropertyField(serializedObject.FindProperty("lifetime")));
            container.Add(new PropertyField(serializedObject.FindProperty("stayForever")));
            container.Add(new PropertyField(serializedObject.FindProperty("count")));
            container.Add(new PropertyField(serializedObject.FindProperty("scale")));

            return container;
        }
    }
}