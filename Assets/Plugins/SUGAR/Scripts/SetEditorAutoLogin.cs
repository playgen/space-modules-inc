#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace PlayGen.SUGAR.Unity
{
	public static class SetEditorAutoLogin
	{
		[MenuItem("Tools/SUGAR/Set Auto Log-in Values")]
		public static void SeedAchivements()
		{
			AutoLogIn window = ScriptableObject.CreateInstance<AutoLogIn>();
			window.titleContent.text = "Set Auto Log-in Values";
			window.Show();
		}
	}

	public class AutoLogIn : EditorWindow
	{
		private string _username;
		private string _password;
		private string _group;
		private string _source;
		private bool _auto;
		private bool _passRequired;
		private string _customArgs;

		void OnEnable()
		{
			_passRequired = !EditorPrefs.HasKey("AutoLoginSourcePassRequired") || EditorPrefs.GetBool("AutoLoginSourcePassRequired");
			_auto = !EditorPrefs.HasKey("AutoLoginAuto") || EditorPrefs.GetBool("AutoLoginAuto");
			_username = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginUsername") : string.Empty;
			_password = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginPassword") : string.Empty;
			_group = EditorPrefs.HasKey("AutoLoginGroup") ? EditorPrefs.GetString("AutoLoginGroup") : string.Empty;
			_source = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginSourceToken") : string.Empty;
			_customArgs = EditorPrefs.HasKey("AutoLoginCustomArgs") ? EditorPrefs.GetString("AutoLoginCustomArgs") : string.Empty;
		}

		void OnGUI()
		{
			_username = EditorGUILayout.TextField("Username", _username, EditorStyles.textField);
			_passRequired = EditorGUILayout.Toggle("Password Required", _passRequired, EditorStyles.toggle);
			if (_passRequired) {
				_password = EditorGUILayout.PasswordField("Password", _password, EditorStyles.textField);
			}
			_group = EditorGUILayout.TextField("Group Id", _group, EditorStyles.textField);
			_source = EditorGUILayout.TextField("Account Source", _source, EditorStyles.textField);
			_auto = EditorGUILayout.Toggle("Auto Log-in", _auto, EditorStyles.toggle);
			_customArgs = EditorGUILayout.TextField("Custom Args. key=value key=value etc.", _customArgs, EditorStyles.textField);
			if (GUILayout.Button("Save"))
			{
				EditorPrefs.SetBool("AutoLoginSourcePassRequired", _passRequired);
				EditorPrefs.SetBool("AutoLoginAuto", _auto);
				EditorPrefs.SetString("AutoLoginUsername", _username);
				EditorPrefs.SetString("AutoLoginPassword", _password);
				EditorPrefs.SetString("AutoLoginGroup", _group);
				EditorPrefs.SetString("AutoLoginSourceToken", _source);
				EditorPrefs.SetString("AutoLoginCustomArgs", _customArgs);
				Close();
			}
		}
	}
}
#endif
