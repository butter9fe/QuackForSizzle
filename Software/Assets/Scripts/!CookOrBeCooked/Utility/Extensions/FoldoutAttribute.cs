using UnityEngine;

public class FoldoutAttribute : PropertyAttribute
{
	public string name;
	public bool foldEverything;

	/// <summary>Adds the property to the specified foldout group.</summary>
	/// <param name="name">Name of the foldout group.</param>
	/// <param name="foldEverything">Toggle to put all properties to the specified group. If false, need to attach to every property individually</param>
	public FoldoutAttribute(string name, bool foldEverything = true)
	{
		this.foldEverything = foldEverything;
		this.name = name;
	}
}