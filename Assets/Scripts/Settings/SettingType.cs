public class SettingType
{
	public string Title;
	public SettingObjectType ObjectType;
	public bool ShowOnMobile;

	public SettingType (string title, SettingObjectType type, bool mobile)
	{
		Title = title;
		ObjectType = type;
		ShowOnMobile = mobile;
	}
}
