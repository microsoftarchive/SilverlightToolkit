// SilverlightShaderBuildHelper.h
#pragma once

#include "d3dx9.h"
#include "stdafx.h"
#include <atlstr.h>
#include <msclr/marshal.h>
#include "CompilerResult.h"

using namespace System;
using namespace System::Text;
using namespace System::IO;
using namespace System::Diagnostics;
using namespace System::Collections::Generic;
using namespace System::Xml;
using namespace System::Runtime::InteropServices;
using namespace System::Text::RegularExpressions;
using namespace msclr::interop; 


namespace SilverlightShaderCompiler 
{
    public ref class Compiler
    {
        String^ _dxLibraryToUse;
        bool _calculatedDxLibraryToUse;

        typedef HRESULT (WINAPI *ShaderCompilerType)
        (
            LPCSTR                          pSrcData,
            UINT                            SrcDataLen,
            CONST D3DXMACRO*                pDefines,
            LPD3DXINCLUDE                   pInclude,
            LPCSTR                          pFunctionName,
            LPCSTR                          pProfile,
            DWORD                           Flags,
            LPD3DXBUFFER*                   ppShader,
            LPD3DXBUFFER*                   ppErrorMsgs,
            LPD3DXCONSTANTTABLE*            ppConstantTable);
        
        String^ GetDxLibraryToUse()
        {
            if (!_calculatedDxLibraryToUse)
            {		
                const int maxIndex = 60;
                const int minIndex = 36;
                bool gotOne = false;
                int index = maxIndex;

                // DirectX SDK's install new D3DX libraries by appending a version number.  Here, we start from a high
                // number and count down until we find a library that contains the function we're looking for.  This
                // thus gets us the most recently installed SDK.  If there are no SDKs installed, we _dxLibraryToUse remains
                // NULL and we'll use the statically linked one.
                for (int index = maxIndex; !gotOne && index >= minIndex; index--)
                {
                    String^ libName = "d3dx9_" + index.ToString() + ".dll";
                    CString libNameAsCString(libName);

                    HMODULE dxLibrary = ::LoadLibrary((LPCWSTR)libNameAsCString);
                    if (dxLibrary != NULL)
                    {
                        FARPROC sc = ::GetProcAddress(dxLibrary, "D3DXCompileShader");
                        if (sc != NULL)
                        {
                            gotOne = true;
                            _dxLibraryToUse = libName;
                        }
                        ::FreeLibrary(dxLibrary);
                    }
                }

                _calculatedDxLibraryToUse = true;
            }

            return _dxLibraryToUse;
        }

        String^ GetConstants(LPD3DXCONSTANTTABLE constantTable, List<String^>^ errors)
        {	
            // Get constant table description
            D3DXCONSTANTTABLE_DESC desc;
            HRESULT hr = constantTable->GetDesc(&desc);

            // Create XML writer
            StringBuilder^ output = gcnew StringBuilder();
            XmlWriterSettings^ settings = gcnew XmlWriterSettings();
            settings->Indent = true;
            XmlWriter^ writer = XmlWriter::Create(output, settings);

            // Write root element
            writer->WriteStartElement("ShaderConstants");
            Version^ version = gcnew Version(D3DSHADER_VERSION_MAJOR(desc.Version), D3DSHADER_VERSION_MINOR(desc.Version));
            writer->WriteAttributeString("FileFormatVersion", "1.0");
            writer->WriteAttributeString("Version", version->ToString());
            writer->WriteAttributeString("Constants", (gcnew UInt32(desc.Constants))->ToString());	
            writer->WriteAttributeString("Creator", gcnew String(desc.Creator));

            for(unsigned int c = 0; c < desc.Constants; c++)
            {
                // get constant
                D3DXHANDLE constant = constantTable->GetConstant(NULL, c);
                if(constant == NULL)
                {
                    errors->Add(String::Format("Unable to get constant: {0}", c));
                    continue;
                }

                // get constant desc count		
                UINT descCount = 0;
                hr = constantTable->GetConstantDesc(constant, NULL, &descCount);
                if (!SUCCEEDED(hr) || descCount == 0)
                {
                    errors->Add(String::Format("Unable to get description count for constant: {0}", c));
                    continue;
                }

                // get constant desc
                D3DXCONSTANT_DESC *constantDesc = new D3DXCONSTANT_DESC[descCount];		
                hr = constantTable->GetConstantDesc(constant, &constantDesc[0], &descCount);
                if (!SUCCEEDED(hr))
                {
                    delete [] constantDesc;
                    errors->Add(String::Format("Unable to get description for constant: {0}", c));
                    continue;
                }

                writer->WriteStartElement("Constant");
                writer->WriteAttributeString("Index", (gcnew UInt32(c))->ToString());
                writer->WriteAttributeString("Descriptions", (gcnew UInt32(descCount))->ToString());

                for(unsigned int d = 0; d < descCount; d++)
                {
                    writer->WriteStartElement("Description");

                    writer->WriteElementString("Name", gcnew String(constantDesc[d].Name));
            
                    switch(constantDesc[d].RegisterSet)
                    {
                    case D3DXRS_BOOL:
                        writer->WriteElementString("RegisterSet", "Bool");
                        break;
                    case D3DXRS_INT4:
                        writer->WriteElementString("RegisterSet", "Int4");
                        break;
                    case D3DXRS_FLOAT4:
                        writer->WriteElementString("RegisterSet", "Float4");
                        break;
                    case D3DXRS_SAMPLER:
                        writer->WriteElementString("RegisterSet", "Sampler");
                        break;
                    }
                    writer->WriteElementString("RegisterIndex", (gcnew UInt32(constantDesc[d].RegisterIndex))->ToString());
                    writer->WriteElementString("RegisterCount", (gcnew UInt32(constantDesc[d].RegisterCount))->ToString());

                    writer->WriteElementString("Rows", (gcnew UInt32(constantDesc[d].Rows))->ToString());
                    writer->WriteElementString("Columns", (gcnew UInt32(constantDesc[d].Columns))->ToString());
                    writer->WriteElementString("Elements", (gcnew UInt32(constantDesc[d].Elements))->ToString());
                    writer->WriteElementString("StructMembers", (gcnew UInt32(constantDesc[d].StructMembers))->ToString());

                    writer->WriteElementString("Bytes", (gcnew UInt32(constantDesc[d].Bytes))->ToString());

                    switch(constantDesc[d].Class)
                    {
                    case D3DXPC_SCALAR:
                        writer->WriteElementString("Class", "Scalar");
                        break;
                    case D3DXPC_VECTOR:
                        writer->WriteElementString("Class", "Vector");
                        break;
                    case D3DXPC_MATRIX_ROWS:
                        writer->WriteElementString("Class", "Rows");
                        break;
                    case D3DXPC_MATRIX_COLUMNS:
                        writer->WriteElementString("Class", "Columns");
                        break;
                    case D3DXPC_OBJECT:
                        writer->WriteElementString("Class", "Object");
                        break;
                    case D3DXPC_STRUCT:
                        writer->WriteElementString("Class", "Struct");
                        break;
                    }

                    switch(constantDesc[d].Type)
                    {
                    case D3DXPT_VOID:
                        writer->WriteElementString("Type", "Void");
                        break;
                    case D3DXPT_BOOL:
                        writer->WriteElementString("Type", "Bool");
                        break;
                    case D3DXPT_INT:
                        writer->WriteElementString("Type", "Int");
                        break;
                    case D3DXPT_FLOAT:
                        writer->WriteElementString("Type", "Float");
                        break;
                    case D3DXPT_STRING:
                        writer->WriteElementString("Type", "String");
                        break;
                    case D3DXPT_TEXTURE:
                        writer->WriteElementString("Type", "Texture");
                        break;
                    case D3DXPT_TEXTURE1D:
                        writer->WriteElementString("Type", "Texture1D");
                        break;
                    case D3DXPT_TEXTURE2D:
                        writer->WriteElementString("Type", "Texture2D");
                        break;
                    case D3DXPT_TEXTURE3D:
                        writer->WriteElementString("Type", "Texture3D");
                        break;
                    case D3DXPT_TEXTURECUBE:
                        writer->WriteElementString("Type", "TextureCube");
                        break;
                    case D3DXPT_SAMPLER:
                        writer->WriteElementString("Type", "Sampler");
                        break;
                    case D3DXPT_SAMPLER1D:
                        writer->WriteElementString("Type", "Sampler1D");
                        break;
                    case D3DXPT_SAMPLER2D:
                        writer->WriteElementString("Type", "Sampler2D");
                        break;
                    case D3DXPT_SAMPLER3D:
                        writer->WriteElementString("Type", "Sampler3D");
                        break;
                    case D3DXPT_SAMPLERCUBE:
                        writer->WriteElementString("Type", "SamplerCube");
                        break;
                    case D3DXPT_PIXELSHADER:
                        writer->WriteElementString("Type", "PixelShader");
                        break;
                    case D3DXPT_VERTEXSHADER:
                        writer->WriteElementString("Type", "VertexShader");
                        break;
                    case D3DXPT_PIXELFRAGMENT:
                        writer->WriteElementString("Type", "PixelFragment");
                        break;
                    case D3DXPT_VERTEXFRAGMENT:
                        writer->WriteElementString("Type", "VertexFragment");
                        break;
                    case D3DXPT_UNSUPPORTED:
                        writer->WriteElementString("Type", "Unsupported");
                        break;
                    }

                    writer->WriteEndElement(); // Description
                }

                writer->WriteEndElement(); // Constant
            }

            writer->WriteEndElement(); // ShaderConstants
            writer->Flush();
            writer->Close();

            return output->ToString();
        }
    public:
        Compiler()
        {
            _calculatedDxLibraryToUse = false;
        }

        CompilerResult^ Process(String^ shaderSourceCode, List<String^>^ errors, String^ shaderProfile, String^ entryPoint, int optimizationLevel, bool debug, bool packMatrixRowMajor)
        {
            CompilerResult^ result;
            marshal_context^ context = gcnew marshal_context();
        
            LPCSTR lpShaderSourceCode = context->marshal_as<LPCSTR>(shaderSourceCode);
            LPD3DXBUFFER compiledShader;
            LPD3DXBUFFER errorMessages;
            LPD3DXCONSTANTTABLE constantTable;

            ShaderCompilerType shaderCompiler = ::D3DXCompileShader;

            // Try to get the latest if the DX SDK is installed.  Otherwise, back up to the statically linked version.
            String^ libraryToLoad = GetDxLibraryToUse();
            CString libraryToLoadAsCString(libraryToLoad);

            HMODULE dxLibrary = ::LoadLibrary((LPCWSTR)libraryToLoadAsCString); 
            bool gotDynamicOne = false;
            if (dxLibrary != NULL)
            {
                FARPROC sc = ::GetProcAddress(dxLibrary, "D3DXCompileShader");
                shaderCompiler = (ShaderCompilerType)sc;
                gotDynamicOne = true;
            }

            LPCSTR lpEntryPoint = context->marshal_as<LPCSTR>(entryPoint);
            LPCSTR lpShaderProfile = context->marshal_as<LPCSTR>(shaderProfile);

            // initialize flags
            DWORD compilerFlags = 0;

            if (packMatrixRowMajor)
                compilerFlags |= D3DXSHADER_PACKMATRIX_ROWMAJOR;
            else
                compilerFlags |= D3DXSHADER_PACKMATRIX_COLUMNMAJOR;

            if (debug)
                compilerFlags |= D3DXSHADER_DEBUG;

            // Not supported on original DX9 compiler
            switch (optimizationLevel)
            {
            case 0:
                compilerFlags |= D3DXSHADER_OPTIMIZATION_LEVEL0;
                break;
            case 1:
                compilerFlags |= D3DXSHADER_OPTIMIZATION_LEVEL1;
                break;
            case 2:
                compilerFlags |= D3DXSHADER_OPTIMIZATION_LEVEL2;
                break;
            case 3:
                compilerFlags |= D3DXSHADER_OPTIMIZATION_LEVEL3;
                break;
            }

            HRESULT compileResult = shaderCompiler(
                    lpShaderSourceCode,
                    shaderSourceCode->Length,
                    NULL, // pDefines
                    NULL, // pIncludes
                    lpEntryPoint, // entrypoint
                    lpShaderProfile, // "ps_2_0", "vs_2_0", etc.
                    compilerFlags, // compiler flags
                    &compiledShader,
                    &errorMessages,
                    &constantTable   // constant table output
                    );

            if (!SUCCEEDED(compileResult))
            {
                errors->Add(String::Format("Compile error {0}", compileResult));
                
                char *nativeErrorString = NULL;
                if(errorMessages != NULL)
                    nativeErrorString = (char *)(errorMessages->GetBufferPointer());

                String^ managedErrorString = context->marshal_as<String^>(nativeErrorString == NULL ? "Unknown compile error (check flags against DX version)" : nativeErrorString);

                // Need to build up our own error information, since error string from the compiler
                // doesn't identify the source file.

                // Pull the error string from the shader compiler apart.
                // Note that the backslashes are escaped, since C++ needs an escaping of them.  
                String^ subcategory = "Shader";
                String^ dir;
                String^ line;
                String^ col;
                String^ descrip;
                String^ errorCode = "";
                String^ helpKeyword = "";
                int     lineNum = 0;
                int     colNum = 0;
                bool    parsedLineNum = false;

                if (gotDynamicOne)
                {
                    String^ regexString = "(?<directory>[^@]+)memory\\((?<line>[^@]+),(?<col>[^@]+)\\): (?<descrip>[^@]+)";
                    Regex^ errorRegex = gcnew Regex(regexString);
                    Match^ m = errorRegex->Match(managedErrorString);

                    dir     = m->Groups["directory"]->Value;
                    line    = m->Groups["line"]->Value;
                    col     = m->Groups["col"]->Value;
                    descrip = m->Groups["descrip"]->Value;

                    parsedLineNum = Int32::TryParse(line, lineNum);
                    Int32::TryParse(col, colNum);
                }
                else
                {
                    // Statically linked d3dx9.lib's error string is a different format, need to parse that.

                    // Example string: (16): error X3018: invalid subscript 'U'
                    String^ regexString = "\\((?<line>[^@]+)\\): (?<descrip>[^@]+)";
                    Regex^ errorRegex = gcnew Regex(regexString);
                    Match^ m = errorRegex->Match(managedErrorString);

                    line    = m->Groups["line"]->Value;
                    descrip = m->Groups["descrip"]->Value;

                    parsedLineNum = Int32::TryParse(line, lineNum);

                    int colNum = 0;  // no column information supplied
                }

                if (!parsedLineNum)
                {
                    // Just use the whole string as the description.
                    descrip = managedErrorString;
                }
                errors->Add(String::Format("({0}, {1}): error {2} : {3}", lineNum, colNum, errorCode, descrip));

                result = nullptr;
            }
            else
            {
                char *nativeBytestream = (char *)(compiledShader->GetBufferPointer());
                result = gcnew CompilerResult();
                result->ShaderCode = gcnew array<unsigned char>(compiledShader->GetBufferSize());

                // TODO: Really ugly way to copy from a uchar* to a managed array, but I can't easily figure out the
                // "right" way to do it.
                for (unsigned int i = 0; i < compiledShader->GetBufferSize(); i++)
                {
                    result->ShaderCode[i] = nativeBytestream[i];
                }
                
                // Constants
                result->ConstantsDefinition = GetConstants(constantTable, errors);
            }

            if (dxLibrary != NULL)
            {
                ::FreeLibrary(dxLibrary);
            }
            return result;			
        }
    };
}
