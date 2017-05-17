using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SilverlightContentPipeline
{
    public class EffectPass
    {
        string _sourceCode;
        readonly Dictionary<string, string> renderStates = new Dictionary<string, string>();

        #region Properties

        public string Name { get; set; }
        public string SourceCode
        {
            get { return _sourceCode; }
            set 
            { 
                _sourceCode = value;

                ExtractRenderStates();
            }
        }

        public string VertexShaderEntryPoint { get; private set; }
        public string PixelShaderEntryPoint { get; private set; }

        public Dictionary<string, string> RenderStates
        {
            get { return renderStates; }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Extract all the render states and launch the extraction of shaders
        /// </summary>
        private void ExtractRenderStates()
        {
            Regex regex = new Regex("(?<name>.*?)=(?<value>.*?);");

            foreach (Match match in regex.Matches(SourceCode))
            {
                RenderStates.Add(match.Groups["name"].Value.Trim().ToLower(), match.Groups["value"].Value.Trim());
            }

            if (!RenderStates.ContainsKey("vertexshader"))
                ExceptionHelper.RaiseException(String.Format("Invalid effect file. Unable to find vertex shader in pass \"{0}\"", Name));
            if (!RenderStates.ContainsKey("pixelshader"))
                ExceptionHelper.RaiseException(String.Format("Invalid effect file. Unable to find pixel shader in pass \"{0}\"", Name));

            string vertexShaderLine = RenderStates["vertexshader"];
            string pixelShaderLine = RenderStates["pixelshader"];

            VertexShaderEntryPoint = ExtractEntryPoint(vertexShaderLine);
            RenderStates.Remove("vertexshader");

            PixelShaderEntryPoint = ExtractEntryPoint(pixelShaderLine);
            RenderStates.Remove("pixelshader");
        }

        /// <summary>
        /// Find the entry point of a compilation order
        /// </summary>
        string ExtractEntryPoint(string codeLine)
        {
            Regex regex = new Regex(@".*\s(?<entryPoint>.*?)\(\)", RegexOptions.IgnoreCase);
            Match match = regex.Match(codeLine);

            if (!match.Success)
                ExceptionHelper.RaiseException(String.Format("Invalid effect file. Unable to find shader entry point in pass \"{0}\"", Name));

            return match.Groups["entryPoint"].Value.Trim();
        }

        #endregion
    }
}
