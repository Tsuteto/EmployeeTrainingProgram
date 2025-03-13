using System.Collections.Generic;
using System.Linq;

namespace EmployeeTraining.Localization
{
    public class StringLocalizer
    {
        public string Lang { get; set; } = "en";

		private readonly List<Localizations> localizedTexts;

        public StringLocalizer(List<Localizations> texts)
        {
            this.localizedTexts = texts;
        }

		public Texts Get(string text)
		{
			return this.Get(text, this.Lang);
		}

		private Texts Get(string text, string language)
		{
			Localizations localizations = (from x in this.localizedTexts where x.Language == language select x).FirstOrDefault();
			if (localizations == null)
			{
				localizations = (from x in this.localizedTexts where x.Language == "en" select x).FirstOrDefault();
				return localizations.GetText(text);
			}
			if (localizations.GetText(text) != null)
			{
				return localizations.GetText(text);
			}
			localizations = (from x in this.localizedTexts where x.Language == "en" select x).FirstOrDefault();
			return localizations.GetText(text);
		}

    }
}
