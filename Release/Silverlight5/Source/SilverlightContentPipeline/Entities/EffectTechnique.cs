using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SilverlightContentPipeline
{
    public class EffectTechnique
    {
        string _sourceCode;

        #region Properties

        public string Name { get; set; }
        public string SourceCode
        {
            get { return _sourceCode; }
            set 
            { 
                _sourceCode = value;
                ExtractPasses();
            }
        }

        public EffectPass[] Passes { get; private set; }

        #endregion

        #region Private methods

        /// <summary>
        /// Find all passes
        /// </summary>
        void ExtractPasses()
        {
            Regex regex = new Regex("pass(?<name>.*?){(?<code>.*?)}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection matches = regex.Matches(SourceCode);

            Passes =(
                    from Match match in matches
                    select new EffectPass
                    {
                        Name = match.Groups["name"].Value.Trim(),
                        SourceCode = match.Groups["code"].Value
                    }
                   ).ToArray();
        }

        #endregion
    }
}
