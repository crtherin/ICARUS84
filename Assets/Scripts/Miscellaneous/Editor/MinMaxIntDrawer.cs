using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Miscellaneous.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxInt))]
    public class MinMaxIntDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            container.style.marginLeft = 3;
            container.style.marginRight = 3;
            container.style.marginTop = 1;
            container.style.marginBottom = 1;
            
            container.style.flexDirection = FlexDirection.Row;

            var label = new Label(property.displayName);
            
            var minField = new IntegerField(string.Empty);
            var maxField = new IntegerField(string.Empty);

            label.style.minWidth = 150;
            label.style.paddingLeft = 0;
            label.style.paddingTop = 2;
            label.style.paddingRight = 2;

            minField.style.flexShrink = 1;
            minField.style.flexGrow = 1;
            maxField.style.flexShrink = 1;
            maxField.style.flexGrow = 1;

            minField.style.marginTop = 0;
            minField.style.marginLeft = 0;
            minField.style.marginRight = 0;
            minField.style.marginBottom = 0;
            
            maxField.style.marginTop = 0;
            maxField.style.marginLeft = 0;
            maxField.style.marginRight = 0;
            maxField.style.marginBottom = 0;
            
            minField.bindingPath = "min";
            maxField.bindingPath = "max";
            
            container.Add(label);
            container.Add(minField);
            container.Add(new Label("to") { style = { minWidth = 20, unityTextAlign = TextAnchor.MiddleCenter } });
            container.Add(maxField);
            
            container.Bind(property.serializedObject);

            return container;
        }
    }
}