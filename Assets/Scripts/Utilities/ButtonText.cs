using UnityEngine;
using UnityEngine.UI;

///<summary>
/// Sets the color of text to be the same as the color of the button, useful for disabling buttons 
/// </summary>
public class ButtonText : MonoBehaviour
{
	private Button _button;
	private Text _text;
	private Color _color;

	// Use this for initialization
	private void Start ()
	{
		_button = GetComponentInParent<Button>();
		_text = GetComponent<Text>();
		_color = _text.color;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		_color.a = _button.interactable ? _button.colors.normalColor.a : _button.colors.disabledColor.a;
		_text.color = _color;
	}
}
