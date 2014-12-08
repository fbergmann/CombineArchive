; Script generated by the HM NIS Edit Script Wizard.

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "COMBINE archive"
!define PRODUCT_VERSION "1.9"
!define PRODUCT_PUBLISHER "Frank T. Bergmann"
!define PRODUCT_WEB_SITE "http://fbergmann.github.io/CombineArchive/"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\FormsCombineArchive.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
!include "FileAssociation.nsh"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\FormsCombineArchive.exe"
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\README.txt"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "SetupCOMBINE_${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES\COMBINE archive"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite try
  File "bin\CombineCLI.exe"
  File "bin\FormsCombineArchive.exe"
  CreateDirectory "$SMPROGRAMS\COMBINE archive"
  CreateShortCut "$SMPROGRAMS\COMBINE archive\COMBINE archive.lnk" "$INSTDIR\FormsCombineArchive.exe"
  CreateShortCut "$DESKTOP\COMBINE archive.lnk" "$INSTDIR\FormsCombineArchive.exe"
  File "bin\FormsCombineArchive.exe.config"
  File "bin\FormsCombineArchive.pdb"
  File "bin\ICSharpCode.SharpZipLib.dll"
  File "bin\LibCombine.dll"
  File "bin\LibCombine.pdb"
  File "bin\SBWCSharp.dll"
  SetOverwrite ifnewer
  File /oname=README.txt "README.md"
  ${registerExtension} "$INSTDIR\FormsCombineArchive.exe" ".omex" "COMBINE archive"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\COMBINE archive\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\COMBINE archive\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\FormsCombineArchive.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\FormsCombineArchive.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\README.txt"
  Delete "$INSTDIR\SBWCSharp.dll"
  Delete "$INSTDIR\LibCombine.pdb"
  Delete "$INSTDIR\LibCombine.dll"
  Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"
  Delete "$INSTDIR\FormsCombineArchive.pdb"
  Delete "$INSTDIR\FormsCombineArchive.exe.config"

  Delete "$INSTDIR\CombineCLI.exe"
  Delete "$INSTDIR\FormsCombineArchive.exe"

  Delete "$SMPROGRAMS\COMBINE archive\Uninstall.lnk"
  Delete "$SMPROGRAMS\COMBINE archive\Website.lnk"
  Delete "$DESKTOP\COMBINE archive.lnk"
  Delete "$SMPROGRAMS\COMBINE archive\COMBINE archive.lnk"

  RMDir "$SMPROGRAMS\COMBINE archive"
  RMDir "$INSTDIR"

  ${unregisterExtension} ".omex" "COMBINE archive"


  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd