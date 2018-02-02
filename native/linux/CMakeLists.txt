cmake_minimum_required(VERSION 3.1)
project(MemoryObserver)

#Unfortunately Mono is not supported yet...
# enable_language(CSharp)

#options
option(SINGLEUSER "Install only for single user" 0)
option(FIREFOX "Install manifest for Firefox" 1)
option(CHROME "Install manifest for Chrome" 1)
option(CHROMIUM "Install manifest for Chromium" 1)

#check install options
if( NOT (FIREFOX OR CHROME OR CHROMIUM) )
    message(FATAL_ERROR, "Please set at least one target browser. use -DFIREFOX=[ON|OFF] or -DCHROME=[ON|OFF] or -DCHROMIUM=[ON|OFF]")
endif()

set(APPNAME chromia.ext.memoryobserver)
set(DSTMANIFESTNAME ${APPNAME}.json)
set(SRCDIR ${CMAKE_SOURCE_DIR})
set(WORKDIR ${PROJECT_BINARY_DIR})
set(INSTALLDIR ${CMAKE_INSTALL_PREFIX}/bin)

if(SINGLEUSER)
    message(STATUS "Configuration: For single user")
    if(FIREFOX)
        message(STATUS "Configuration: Target Firefox is enabled")
        set(MANIFESTINSTALLDIR_FIREFOX ~/.mozilla/native-messaging-hosts)
        set(SRCMANIFESTNAME_FIREFOX manifest.firefox)
    endif()
    if(CHROME)
        message(STATUS "Configuration: Target Chrome is enabled")
        set(MANIFESTINSTALLDIR_CHROME ~/.config/google-chrome/NativeMessagingHosts)
        set(SRCMANIFESTNAME_CHROMXXX manifest.chrome) #common with chromium
    endif()
    if(CHROMIUM)
        message(STATUS "Configuration: Target Chromium is enabled")
        set(MANIFESTINSTALLDIR_CHROMIUM ~/.config/chromium/NativeMessagingHosts)
        set(SRCMANIFESTNAME_CHROMXXX manifest.chrome) #common with chrome
    endif()
else()
    message(STATUS "Configuration: For all users")
    if(FIREFOX)
        message(STATUS "Configuration: Target Firefox is enabled")
        set(MANIFESTINSTALLDIR_FIREFOX /usr/lib/mozilla/native-messaging-hosts)
        set(SRCMANIFESTNAME_FIREFOX manifest.firefox)
    endif()
    if(CHROME)
        message(STATUS "Configuration: Target Chrome is enabled")
        set(MANIFESTINSTALLDIR_CHROME /etc/opt/chrome/native-messaging-hosts)
        set(SRCMANIFESTNAME_CHROMXXX manifest.chrome) #common with chromium
    endif()
    if(CHROMIUM)
        message(STATUS "Configuration: Target Chromium is enabled")
        set(MANIFESTINSTALLDIR_CHROMIUM /etc/chromium/native-messaging-hosts)
        set(SRCMANIFESTNAME_CHROMXXX manifest.chrome) #common with chrome
    endif()
endif()

#build target - execute
set(EXENAME memoryobserver.exe)
set(EXEPATH ${WORKDIR}/${EXENAME})
set(CS_COMPILER mcs)
set(BASEDIR ${SRCDIR}/..)
set(SOURCES
    ${BASEDIR}/src/memoryobserver.cs
    ${BASEDIR}/windows/other/Properties/AssemblyInfo.cs
)
add_custom_target(output_exec ALL
    COMMAND ${CS_COMPILER} /debug- /optimize+ /out:${EXENAME} ${SOURCES}
    COMMENT "Building: ${EXENAME}"
)

#build target - manifest
if(FIREFOX)
    add_custom_target(output_manifest_firefox ALL
        COMMAND sed -e \"s|_REPLACE_THIS_|${INSTALLDIR}|\" ${SRCDIR}/${SRCMANIFESTNAME_FIREFOX} > ${WORKDIR}/${SRCMANIFESTNAME_FIREFOX}
        COMMENT "Generating manifest: ${SRCMANIFESTNAME_FIREFOX}")
endif()
if(CHROME OR CHROMIUM)
    add_custom_target(output_manifest_chrome ALL
        COMMAND sed -e \"s|_REPLACE_THIS_|${INSTALLDIR}|\" ${SRCDIR}/${SRCMANIFESTNAME_CHROMXXX} > ${WORKDIR}/${SRCMANIFESTNAME_CHROMXXX}
        COMMENT "Generating manifest: ${SRCMANIFESTNAME_CHROMXXX}")
endif()

#build target - packaging script
add_custom_target(pkgscript
    cp -f ${SRCDIR}/description-pak ${SRCDIR}/makepkg.sh ${WORKDIR}
    COMMAND chmod +x ${WORKDIR}/makepkg.sh)

#install target
install(PROGRAMS ${EXEPATH} DESTINATION ${INSTALLDIR})
if(FIREFOX)
    install(FILES ${WORKDIR}/${SRCMANIFESTNAME_FIREFOX} DESTINATION ${MANIFESTINSTALLDIR_FIREFOX} RENAME ${DSTMANIFESTNAME})
endif()
if(CHROME)
    install(FILES ${WORKDIR}/${SRCMANIFESTNAME_CHROMXXX} DESTINATION ${MANIFESTINSTALLDIR_CHROME} RENAME ${DSTMANIFESTNAME})
endif()
if(CHROMIUM)
    install(FILES ${WORKDIR}/${SRCMANIFESTNAME_CHROMXXX} DESTINATION ${MANIFESTINSTALLDIR_CHROMIUM} RENAME ${DSTMANIFESTNAME})
endif()
