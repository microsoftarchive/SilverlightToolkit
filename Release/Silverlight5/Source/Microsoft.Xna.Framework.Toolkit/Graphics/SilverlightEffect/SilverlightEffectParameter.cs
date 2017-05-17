using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Parameter class for SilverlightEffect
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class SilverlightEffectParameter
    {
        bool isDirty = true;
        Vector4[] data;

        #region Creation

        /// <summary>Creates a new effect parameter</summary>
        internal SilverlightEffectParameter(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the name of the parameter
        /// </summary>
        public string Name { get; internal set; }

        internal bool IsDirty
        {
            get { return isDirty; }
        }

        Vector4[] Data
        {
            set
            {
                isDirty = true;
                data = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Affect a single value to the parameter
        /// </summary>
        public void SetValue(float single)
        {
            Data = new[] { new Vector4(single,0, 0, 0) };            
        }

        /// <summary>
        /// Affect a Vector2 value to the parameter
        /// </summary>
        public void SetValue(Vector2 vector2)
        {
            Data = new[]{new Vector4(vector2, 0, 0)};
        }

        /// <summary>
        /// Affect a Vector3 value to the parameter
        /// </summary>
        public void SetValue(Vector3 vector3)
        {
            Data = new[] { new Vector4(vector3, 0) };
        }

        /// <summary>
        /// Affect a Vector4 value to the parameter
        /// </summary>
        public void SetValue(Vector4 vector4)
        {
            Data = new[] {vector4};
        }

        /// <summary>
        /// Affect a Matrix value to the parameter
        /// </summary>
        public void SetValue(Matrix matrix)
        {
            var result = new Vector4[4];

            result[0] = new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
            result[1] = new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
            result[2] = new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
            result[3] = new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44);

            Data = result;
        }

        /// <summary>
        /// Affect an array of Matrix value to the parameter
        /// </summary>
        public void SetValue(Matrix[] matrices)
        {
            var tempData = new Vector4[4 * matrices.Length];
            for (int i = 0; i < matrices.Length; i++)
            {
                tempData[4 * i] = new Vector4(matrices[i].M11, matrices[i].M12, matrices[i].M13, matrices[i].M14);
                tempData[1 + 4 * i] = new Vector4(matrices[i].M21, matrices[i].M22, matrices[i].M23, matrices[i].M24);
                tempData[2 + 4 * i] = new Vector4(matrices[i].M31, matrices[i].M32, matrices[i].M33, matrices[i].M34);
                tempData[3 + 4 * i] = new Vector4(matrices[i].M41, matrices[i].M42, matrices[i].M43, matrices[i].M44);
            }
            Data = tempData;
        }

        /// <summary>
        /// Affect a 4 floats to the parameter
        /// </summary>
        public void SetValue(float float01, float float02, float float03, float float04)
        {
            Data = new[] {new Vector4(float01, float02, float03, float04)};
        }

        /// <summary>
        /// Affect 4 booleans value to the parameter
        /// </summary>
        public void SetValue(bool bool01, bool bool02, bool bool03, bool bool04)
        {
            Data = new[] {new Vector4(bool01 ? 1 : 0, bool02 ? 1 : 0, bool03 ? 1 : 0, bool04 ? 1 : 0)};
        }

        /// <summary>
        /// Affect a color (with alpha value) to the parameter
        /// </summary>
        public void SetValue(Color color, float alpha)
        {
            Data = new[]{new Vector4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, alpha)};
        }

        /// <summary>
        /// Affect a color value to the parameter
        /// </summary>
        public void SetValue(Color color)
        {
            Data = new[] { new Vector4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f) };
        }

        #endregion

        #region Internal methods

        internal void Apply(List<SilverlightEffectInternalParameter> parameters)
        {
            isDirty = false;
            foreach (SilverlightEffectInternalParameter parameter in parameters)
            {
                if (parameter.Name == Name)
                {
                    parameter.Data = data;
                    break;
                }
            }
        }

        #endregion        
    }
}
