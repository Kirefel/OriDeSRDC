msbuild /t:Build /restore /p:Configuration=Release /p:OutDir=..\build

rm -R .\release -ErrorAction SilentlyContinue
mkdir .\release\

cp .\build\OriDeSRDC.dll .\release\
cp .\mod.json .\release\

rm .\OriDeSRDC.zip -ErrorAction SilentlyContinue
powershell Compress-Archive .\release\* .\OriDeSRDC.zip
