//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Defines a URI to URI mapping.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class UriMapping
    {
        #region Fields

        private Uri _uri;
        private Uri _mappedUri;
        private Regex _uriRegex;
        private bool _uriRegexIdentifierUsedTwice;
        private bool _uriHasQueryString;
        private bool _uriHasFragment;
        private List<string> _uriIdentifiers;
        private List<string> _mappedUriIdentifiers;
        private static readonly Regex _conversionRegex = new Regex("(?<ConversionCapture>{.*?})", RegexOptions.ExplicitCapture);
        private bool _initialized;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the original URI value or pattern.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this._uri;
            }

            set
            {
                this._uri = value;
                this._initialized = false;
            }
        }

        /// <summary>
        /// Gets or sets the mapped URI value or pattern.
        /// </summary>
        public Uri MappedUri
        {
            get
            {
                return this._mappedUri;
            }

            set
            {
                this._mappedUri = value;
                this._initialized = false;
            }
        }

        #endregion Properties

        #region Methods

        private bool UriTemplateContainsSameIdentifierTwice(Uri uri, out Regex uriRegex)
        {
            if (uri == null)
            {
                uriRegex = null;
                return false;
            }

            string origString = uri.OriginalString;
            MatchCollection matches = _conversionRegex.Matches(origString);
            this._uriIdentifiers = new List<string>();

            foreach (Match m in matches)
            {
                string valWithoutBraces = m.Value.Replace("{", String.Empty).Replace("}", String.Empty);

                // We've hit the same identifier being used twice.  This isn't valid, so return false.
                if (this._uriIdentifiers.Contains(valWithoutBraces))
                {
                    uriRegex = null;
                    return true;
                }

                this._uriIdentifiers.Add(valWithoutBraces);
            }

            string convertedValue = _conversionRegex.Replace(origString, "(?<$1>.*?)").Replace("{", String.Empty).Replace("}", String.Empty);
            uriRegex = new Regex("^" + convertedValue + "$");
            return false;
        }

        private void GetIdentifiersForMappedUri(Uri mappedUri)
        {
            string origString = mappedUri.OriginalString;
            MatchCollection matches = _conversionRegex.Matches(origString);
            this._mappedUriIdentifiers = new List<string>();

            foreach (Match m in matches)
            {
                string valWithoutBraces = m.Value.Replace("{", String.Empty).Replace("}", String.Empty);
                if (!this._mappedUriIdentifiers.Contains(valWithoutBraces))
                {
                    this._mappedUriIdentifiers.Add(valWithoutBraces);
                }
            }
        }

        private void Initialize()
        {
            // Initialize stuff for the Uri template
            Regex newFromRegex = null;
            this._uriRegexIdentifierUsedTwice = this.UriTemplateContainsSameIdentifierTwice(this._uri, out newFromRegex);
            this._uriHasQueryString = !String.IsNullOrEmpty(UriParsingHelper.InternalUriGetQueryString(this._uri));
            this._uriHasFragment = !String.IsNullOrEmpty(UriParsingHelper.InternalUriGetFragment(this._uri));
            this._uriRegex = newFromRegex;
            
            // Initialize stuff for the mapped Uri template
            this.GetIdentifiersForMappedUri(this._mappedUri);

            this._initialized = true;
        }

        /// <summary>
        /// Attempts to process a Uri, if it matches the Uri template
        /// </summary>
        /// <param name="uri">The Uri to map</param>
        /// <returns>The Uri after mapping, or null if mapping did not succeed</returns>
        public Uri MapUri(Uri uri)
        {
            this.CheckPreconditions();

            string originalUriWithoutQueryString = UriParsingHelper.InternalUriGetBaseValue(uri);

            Match m = this._uriRegex.Match(originalUriWithoutQueryString);

            if (!m.Success)
            {
                return null;
            }

            string uriAfterMappingBase = UriParsingHelper.InternalUriGetBaseValue(this._mappedUri);
            IDictionary<string, string> uriAfterMappingQueryString = UriParsingHelper.InternalUriParseQueryStringToDictionary(this._mappedUri);
            IDictionary<string, string> originalQueryString = UriParsingHelper.InternalUriParseQueryStringToDictionary(uri);
            string originalFragment = UriParsingHelper.InternalUriGetFragment(uri);
            string uriAfterMappingFragment = UriParsingHelper.InternalUriGetFragment(this._mappedUri);

            // 'uriValues' is the values of the identifiers from the 'Uri' template, as they appear in the Uri
            // being processed
            IDictionary<string, string> uriValues = new Dictionary<string, string>();

            // i begins at 1 because the group at index 0 is always equal to the parent's Match,
            // which we do not want.  We only want explicitly-named groups.
            int groupCount = m.Groups.Count;
            for (int i = 1; i < groupCount; i++)
            {
                uriValues.Add(this._uriRegex.GroupNameFromNumber(i), m.Groups[i].Value);
            }

            foreach (string identifier in this._mappedUriIdentifiers)
            {
                string identifierWithBraces = "{" + identifier + "}";
                string replacementValue = (uriValues.ContainsKey(identifier) ? uriValues[identifier] : String.Empty);

                // First check for identifiers in the base Uri, and replace them as appropriate
                uriAfterMappingBase = uriAfterMappingBase.Replace(identifierWithBraces, replacementValue);

                // Then, look through the query string (both the key and the value) and replace as appropriate
                string[] keys = new string[uriAfterMappingQueryString.Keys.Count];
                uriAfterMappingQueryString.Keys.CopyTo(keys, 0);
                foreach (string key in keys)
                {
                    // First check if the value contains it, as this is an easy replacement
                    if (uriAfterMappingQueryString[key].Contains(identifierWithBraces))
                    {
                        if (uriValues.ContainsKey(identifier))
                        {
                            uriAfterMappingQueryString[key] = uriAfterMappingQueryString[key].Replace(identifierWithBraces, replacementValue);
                        }
                    }

                    // If the key itself contains the identifier, then we need to remove the existing item with the key that
                    // contains the identifier, and re-add to the dictionary with the new key and the pre-existing value
                    if (key.Contains(identifierWithBraces))
                    {
                        string existingVal = uriAfterMappingQueryString[key];
                        uriAfterMappingQueryString.Remove(key);
                        uriAfterMappingQueryString.Add(key.Replace(identifierWithBraces, replacementValue), existingVal);
                    }
                }

                // If there's an original fragment already present, it will always win, so don't bother doing replacements
                if (String.IsNullOrEmpty(originalFragment) &&
                    !String.IsNullOrEmpty(uriAfterMappingFragment))
                {
                    if (uriAfterMappingFragment.Contains(identifierWithBraces))
                    {
                        uriAfterMappingFragment = uriAfterMappingFragment.Replace(identifierWithBraces, replacementValue);
                    }
                }
            }

            foreach (string key in originalQueryString.Keys)
            {
                if (!uriAfterMappingQueryString.ContainsKey(key))
                {
                    uriAfterMappingQueryString.Add(key, originalQueryString[key]);
                }
                else
                {
                    // If a value is present in the originally-navigated-to query string, it
                    // takes precedence over anything in the aliased query string by default.
                    uriAfterMappingQueryString[key] = originalQueryString[key];
                }
            }

            if (!String.IsNullOrEmpty(originalFragment))
            {
                uriAfterMappingFragment = originalFragment;
            }

            return UriParsingHelper.InternalUriCreateWithQueryStringValues(uriAfterMappingBase, uriAfterMappingQueryString, uriAfterMappingFragment);
        }

        private void CheckPreconditions()
        {
            if (this._uri == null)
            {
                throw new InvalidOperationException(Resource.UriMapping_UriTemplateMustBeSpecified);
            }

            if (this._mappedUri == null)
            {
                throw new InvalidOperationException(Resource.UriMapping_MappedUriTemplateMustBeSpecified);
            }

            if (this._initialized == false)
            {
                this.Initialize();
            }

            if (this._uriHasQueryString)
            {
                throw new InvalidOperationException(Resource.UriMapping_UriTemplateCannotHaveAQueryString);
            }

            if (this._uriHasFragment)
            {
                throw new InvalidOperationException(Resource.UriMapping_UriTemplateCannotHaveAFragment);
            }

            if (this._uriRegexIdentifierUsedTwice)
            {
                throw new InvalidOperationException(Resource.UriMapping_UriTemplateCannotContainTheSameIdentifierMoreThanOnce);
            }
        }

        #endregion
    }
}
