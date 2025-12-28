using UnityEditor;
using UnityEngine;
using THEBADDEST.VirtualCurrencySystem;

namespace THEBADDEST.VirtualCurrencySystem.Editor
{
	[CustomPropertyDrawer(typeof(CurrencyConfig))]
	public class CurrencyConfigDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Calculate rects
			float lineHeight = EditorGUIUtility.singleLineHeight;
			float spacing = EditorGUIUtility.standardVerticalSpacing;
			float yPos = position.y;

			// Draw foldout
			property.isExpanded = EditorGUI.Foldout(
				new Rect(position.x, yPos, position.width, lineHeight),
				property.isExpanded,
				label,
				true
			);

			if (property.isExpanded)
			{
				EditorGUI.indentLevel++;

				yPos += lineHeight + spacing;

				// Currency Name
				SerializedProperty currencyNameProp = property.FindPropertyRelative("currencyName");
				Rect currencyNameRect = new Rect(position.x, yPos, position.width, lineHeight);
				EditorGUI.PropertyField(currencyNameRect, currencyNameProp, new GUIContent("Currency Name"), false);

				yPos += lineHeight + spacing;

				// Value Type
				SerializedProperty valueTypeProp = property.FindPropertyRelative("valueType");
				Rect valueTypeRect = new Rect(position.x, yPos, position.width, lineHeight);
				EditorGUI.PropertyField(valueTypeRect, valueTypeProp, new GUIContent("Value Type"), false);

				yPos += lineHeight + spacing;

				// Conditionally show initial value field based on valueType
				CurrencyValueType valueType = (CurrencyValueType)valueTypeProp.enumValueIndex;
				Rect valueRect = new Rect(position.x, yPos, position.width, lineHeight);
				
				switch (valueType)
				{
					case CurrencyValueType.Int:
						SerializedProperty intValueProp = property.FindPropertyRelative("initialValueInt");
						EditorGUI.PropertyField(valueRect, intValueProp, new GUIContent("Initial Value"), false);
						break;

					case CurrencyValueType.Float:
						SerializedProperty floatValueProp = property.FindPropertyRelative("initialValueFloat");
						EditorGUI.PropertyField(valueRect, floatValueProp, new GUIContent("Initial Value"), false);
						break;

					case CurrencyValueType.BigNumber:
						SerializedProperty bigNumberValueProp = property.FindPropertyRelative("initialValueBigNumber");
						EditorGUI.PropertyField(valueRect, bigNumberValueProp, new GUIContent("Initial Value"), false);
						break;
				}

				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.isExpanded)
			{
				return EditorGUIUtility.singleLineHeight;
			}

			float lineHeight = EditorGUIUtility.singleLineHeight;
			float spacing = EditorGUIUtility.standardVerticalSpacing;
			float height = EditorGUIUtility.singleLineHeight; // Foldout

			// Currency Name
			height += lineHeight + spacing;

			// Value Type
			height += lineHeight + spacing;

			// Initial Value (one of the three fields)
			height += lineHeight;

			return height;
		}
	}
}

