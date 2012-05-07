// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using PhoneToolkitSample.Data;

namespace PhoneToolkitSample.Samples
{
    public partial class ListPickerSample : PhoneApplicationPage
    {
        #region Sample System Locales
        private List<string> _regions = new List<string>
        {
            "Afrikaans",
            "Albanian",
            "Azeri (Azerbaijan, Cyrillic)",
            "Azeri (Azerbaijan, Latin)",
            "Basque",
            "Belarusian",
            "Bulgarian",
            "Catalan",
            "Chinese (Hong Kong S.A.R.)",
            "Chinese (Macao S.A.R.)",
            "Chinese (PRC)",
            "Chinese (Singapore)",
            "Chinese (Taiwan)",
            "Croatian",
            "Czech",
            "Danish",
            "Dutch (Belgium)",
            "Dutch (Netherlands)",
            "English (Australia)",
            "English (Belize)",
            "English (Canada)",
            "English (Caribbean)",
            "English (India)",
            "English (Ireland)",
            "English (Jamaica)",
            "English (New Zealand)",
            "English (Republic of the Philippines)",
            "English (Singapore)",
            "English (South Africa)",
            "English (Trinidad and Tobago)",
            "English (United Kingdom)",
            "English (United States)",
            "English (Zimbabwe)",
            "Estonian",
            "Faroese",
            "Finnish",
            "French (Belgium)",
            "French (Canada)",
            "French (France)",
            "French (Luxembourg)",
            "French (Principality of Monaco)",
            "French (Switzerland)",
            "Galician",
            "German (Austria)",
            "German (Germany)",
            "German (Liechtenstein)",
            "German (Luxembourg)",
            "German (Switzerland)",
            "Greek",
            "Hungarian",
            "Icelandic",
            "Indonesian",
            "Italian (Italy)",
            "Italian (Switzerland)",
            "Japanese",
            "Kiswahili",
            "Korean",
            "Kyrgyz",
            "Latvian",
            "Lithuanian",
            "Macedonian (FYROM)",
            "Malay (Brunei Darussalam)",
            "Malay (Malaysia)",
            "Mongolian (Cyrillic)",
            "Norwegian (Bokmål)",
            "Norwegian (Nynorsk)",
            "Polish",
            "Portuguese (Brazil)",
            "Portuguese (Portugal)",
            "Romanian",
            "Russian",
            "Serbian (Cyrillic, Montenegro)",
            "Serbian (Cyrillic, Serbia and Montenegro (Former))",
            "Serbian (Cyrillic, Serbia)",
            "Serbian (Latin, Montenegro)",
            "Serbian (Latin, Serbia and Montenegro (Former))",
            "Serbian (Latin, Serbia)",
            "Slovak",
            "Slovenian",
            "Spanish (Argentina)",
            "Spanish (Boliviarian Republic of Venezuela)",
            "Spanish (Bolivia)",
            "Spanish (Chile)",
            "Spanish (Colombia)",
            "Spanish (Costa Rica)",
            "Spanish (Dominican Republic)",
            "Spanish (Ecuador)",
            "Spanish (El Salvador)",
            "Spanish (Guatemala)",
            "Spanish (Honduras)",
            "Spanish (Mexico)",
            "Spanish (Nicaragua)",
            "Spanish (Panama)",
            "Spanish (Paraguay)",
            "Spanish (Peru)",
            "Spanish (Puerto Rico)",
            "Spanish (Spain - International Sort)",
            "Spanish (Unites States)",
            "Spanish (Uruguay)",
            "Swedish (Finland)",
            "Swedish (Sweden)",
            "Tatar",
            "Turkish",
            "Ukrainian",
            "Uzbek (Cyrillic)",
            "Uzbek (Latin)"
        };
        #endregion

        public ListPickerSample()
        {
            InitializeComponent();

            DataContext = ColorExtensions.AccentColors();

            PrintInColors.SummaryForSelectedItemsDelegate = Summarize;

            RegionList.ItemsSource = _regions;
        }

        private string Summarize(IList items)
        {
            string str = "";
            if (null != items)
            {
                if (items.Contains("Cyan"))
                {
                    str += "C";
                }
                if (items.Contains("Majenta"))
                {
                    str += "M";
                }
                if (items.Contains("Yellow"))
                {
                    str += "Y";
                }
                if (items.Contains("Black"))
                {
                    str += "K";
                }
            }

            return str;
        }
    }
}
