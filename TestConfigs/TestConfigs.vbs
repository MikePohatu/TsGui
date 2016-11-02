option explicit

Dim newExe: newExe = "TsGui.exe"
Dim referenceExe: referenceExe = "TsGui-0.9.4.1.exe"

Dim objShell: Set objShell = CreateObject("Wscript.Shell")
Dim objFSO: Set objFSO = CreateObject("Scripting.FileSystemObject")
Dim objScriptFile: Set objScriptFile = objFSO.GetFile(Wscript.ScriptFullName)
Dim strScriptParentFolder: strScriptParentFolder = objScriptFile.ParentFolder
Dim objScriptParentFolder: set objScriptParentFolder = objFSO.GetFolder(strScriptParentFolder)

Dim intFileNumber
Dim file, currentfile

'objShell.Run strPath
WScript.Echo "Which number config do you want to start with: "
' Read dummy input. This call will not return until [ENTER] is pressed.
intFileNumber = WScript.StdIn.ReadLine()
		
For Each file In objScriptParentFolder.Files
	currentfile = "Config." & intFileNumber & ".xml"
	
	if (file.Type = "XML Document") AND (file.Name = currentfile) then
		wscript.echo file.name
		objFSO.CopyFile file.path,file.ParentFolder & "\\Config.xml"
		objShell.Run strScriptParentFolder & "\\" & newExe
		objShell.Run strScriptParentFolder & "\\" & referenceExe
		
		WScript.Echo "Press [ENTER] to continue..."
		' Read dummy input. This call will not return until [ENTER] is pressed.
		WScript.StdIn.ReadLine
		intFileNumber = intFileNumber + 1
	end if
Next