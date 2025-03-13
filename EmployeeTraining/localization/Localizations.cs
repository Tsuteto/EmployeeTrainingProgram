using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UIElements.Collections;

namespace EmployeeTraining.Localization
{
    [Serializable]
    public class Localizations : Dictionary<string, Texts>
	{
		private string _language;

		public Localizations(string Language)
		{
			this._language = Language;
		}

		public new string this[string id]
		{
			set {
				this.Add(id, new Texts(value));
			}
		}

		public Texts GetText(string text)
		{
			if (this.ContainsKey(text))
			{
				return this.Get(text);
			}
			Plugin.LogWarn($"Translation not found: {text}");
			return new Texts(text);
		}

		public string Language
		{
			get
			{
				return this._language;
			}
		}
	}

	[Serializable]
	public class Texts
	{
		[JsonProperty("Text")]
		private string text;

		public Texts(string text)
		{
			this.text = text;
		}

		public string Translate(params object[] args)
		{
			try
			{
				return String.Format(this.text, args);
			}
			catch (FormatException)
			{
				Plugin.LogWarn($"Invalid format: {this.text} with args=[{String.Join(", ", args)}]");
				return "*TranslateError*";
			}
		}
	}

}