# CMake generated Testfile for 
# Source directory: /home/parrot/Desktop/skillbuddy./skillbuddy/cmake-4.0.3
# Build directory: /home/parrot/Desktop/skillbuddy./skillbuddy/cmake-4.0.3
# 
# This file includes the relevant testing commands required for 
# testing this directory and lists subdirectories to be tested as well.
include("/home/parrot/Desktop/skillbuddy./skillbuddy/cmake-4.0.3/Tests/EnforceConfig.cmake")
add_test([=[SystemInformationNew]=] "/home/parrot/Desktop/skillbuddy./skillbuddy/cmake-4.0.3/bin/cmake" "--system-information" "-G" "Unix Makefiles")
set_tests_properties([=[SystemInformationNew]=] PROPERTIES  _BACKTRACE_TRIPLES "/home/parrot/Desktop/skillbuddy./skillbuddy/cmake-4.0.3/CMakeLists.txt;530;add_test;/home/parrot/Desktop/skillbuddy./skillbuddy/cmake-4.0.3/CMakeLists.txt;0;")
subdirs("Source/kwsys")
subdirs("Utilities/std")
subdirs("Utilities/KWIML")
subdirs("Utilities/cmlibrhash")
subdirs("Utilities/cmzlib")
subdirs("Utilities/cmcurl")
subdirs("Utilities/cmnghttp2")
subdirs("Utilities/cmexpat")
subdirs("Utilities/cmbzip2")
subdirs("Utilities/cmzstd")
subdirs("Utilities/cmliblzma")
subdirs("Utilities/cmlibarchive")
subdirs("Utilities/cmjsoncpp")
subdirs("Utilities/cmlibuv")
subdirs("Utilities/cmcppdap")
subdirs("Utilities/cmllpkgc")
subdirs("Source")
subdirs("Utilities")
subdirs("Tests")
subdirs("Auxiliary")
