mkdir vendors
cd vendors
git clone https://github.com/microsoft/vcpkg
cd vcpkg
call "bootstrap-vcpkg.bat"
.\vcpkg integrate install
.\vcpkg install cpprestsdk cpprestsdk:x64-windows
:: cd ..\..\
::set SOL_DIR=%~dp0
::mkdir build
::cmake -B .\build -S . -DVCPKG_TARGET_TRIPLET=x64-windows -DCMAKE_TOOLCHAIN_FILE=%SOL_DIR%\vendors\vcpkg\scripts\buildsystems\vcpkg.cmake