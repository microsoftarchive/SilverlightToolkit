@rem Compiles the three .fx shader source files, recreating the .bin compiled shader blobs.
@rem Must be run from a DXSDK command prompt so the FXC shader compiler is in the path.

fxc /nologo /T:ps_2_0 /Zpr BloomExtract.fx /E:PixelShaderFunction /FoBloomExtract.bin
fxc /nologo /T:ps_2_0 /Zpr BloomCombine.fx /E:PixelShaderFunction /FoBloomCombine.bin
fxc /nologo /T:ps_2_0 /Zpr GaussianBlur.fx /E:PixelShaderFunction /FoGaussianBlur.bin
