#pragma once

using namespace System;

namespace SilverlightShaderCompiler 
{
	public ref class CompilerResult
	{
	public:
		property array<unsigned char>^ ShaderCode;
		property String^ ConstantsDefinition;
	};
}

